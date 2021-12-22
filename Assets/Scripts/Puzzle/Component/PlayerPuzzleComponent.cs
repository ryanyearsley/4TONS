using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/*
This class...
-Initializes player's spellgem system
    
    
1. Reads player input
2. Moves and binds spellgems accordingly
*/

public enum PuzzleBindType {
	AUTOMATIC, MANUAL, COLD_SWAP, HOT_SWAP
}
public enum PuzzleUnbindType {
	TO_HAND, TO_FLOOR, COLD_SWAP, HOT_SWAP
}

public enum StaffEquipType {
	AUTO_EQUIP, PICK_UP, MANUAL_SWAP, QUICK_SWAP, DROPPED_OTHER
}
public enum StaffDropType {
	MANUAL_DROP, HOT_SWAP
}
public enum PuzzleKey {
	DISABLED, INVENTORY, PRIMARY_STAFF, SECONDARY_STAFF, OUTSIDE_BOUNDS, PICK_UP, NO_WEAPON
}
public class PlayerPuzzleComponent : PlayerComponent {

	//References

	private WizardGameData wizardGameData;

	[SerializeField]
	private PlayerPuzzleUIComponent playerPuzzleUI;

	//currents
	[SerializeField]
	private PuzzleCursorLocation highlightedCursorLocation;
	[SerializeField]
	private PuzzleTileInfo highlightedTileInfo;
	private SpellGemGameData highlightedSpellGemGameData;
	private Vector2Int currentGridCoord;

	private SpellGemRevertInfo revertInfo;

	[SerializeField]
	private SpellGemGameData movingSpellGemGameData;//MovingSpellGemGameData is always in limbo (not assigned on player GameData)


	#region initialization

	//called on player object re-use to inject player save data.
	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
	}


	public override void ReusePlayerComponent (Player player) {
		base.ReusePlayerComponent (player);
		this.wizardGameData = playerObject.wizardGameData;
		playerPuzzleUI = GetComponentInChildren<PlayerPuzzleUIComponent> ();
	}

	public override void OnSpawn (Vector3 spawnPosition) {
		Player player = playerObject.player;

		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
			PuzzleGameData primaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.PRIMARY_STAFF];
			playerObject.EquipStaff (PuzzleKey.PRIMARY_STAFF, primaryStaffGameData, StaffEquipType.AUTO_EQUIP);
		} else if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			PuzzleGameData secondaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.SECONDARY_STAFF];
			playerObject.EquipStaff (PuzzleKey.SECONDARY_STAFF, secondaryStaffGameData, StaffEquipType.AUTO_EQUIP);
		}
	}
	#endregion
	#region PlayerComponent Events
	public override void OnChangePlayerState (PlayerState playerState) {
		highlightedTileInfo = null;
		highlightedSpellGemGameData = null;
	}
	public override void OnPickUpStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {

		puzzleGameData.puzzleKey = key;
		wizardGameData.puzzleGameDataDictionary.Add (key, puzzleGameData);
	}
	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		DropStaffToWorld (puzzleGameData);
	}

	public override void OnPickUpSpellGem (SpellGemGameData spellGemGameData) {
		movingSpellGemGameData = spellGemGameData;
		highlightedSpellGemGameData = spellGemGameData;
		playerObject.movingSpellGemData = movingSpellGemGameData;
		playerObject.highlightedSpellGemData = highlightedSpellGemGameData;
		revertInfo = new SpellGemRevertInfo (PuzzleKey.OUTSIDE_BOUNDS, movingSpellGemGameData);
		playerObject.ChangePlayerState (PlayerState.PUZZLE_MOVING_SPELLGEM);
	}

	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData, PuzzleBindType bindType) {
		Debug.Log ("player puzzle OnBindSpellGem event");
		//update model
		PuzzleUtility.AddSpellGemToPuzzle (puzzleGameData, spellGemGameData);
		//update state
		movingSpellGemGameData = null;
		revertInfo = null;
		playerObject.highlightedSpellGemData = null;
		playerObject.movingSpellGemData = null;
	}
	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData, PuzzleUnbindType unbindType) {
		//update model
		PuzzleUtility.RemoveSpellGemFromPuzzle (puzzleGameData, spellGemGameData);
		//update state
		highlightedSpellGemGameData = spellGemGameData;
		movingSpellGemGameData = spellGemGameData;
		playerObject.highlightedSpellGemData = highlightedSpellGemGameData;
		playerObject.movingSpellGemData = movingSpellGemGameData;
		revertInfo = new SpellGemRevertInfo (puzzleGameData.puzzleKey, spellGemGameData);
	}

	public override void OnRotateSpellGem (SpellGemGameData spellGemGameData, int rotateIndex) {
		movingSpellGemGameData.spellGemRotation = rotateIndex;
		spellGemGameData.spellGemEntity.Rotate (movingSpellGemGameData.spellGemRotation * 90);
	}

	public override void OnDropSpellGem (SpellGemGameData spellGemGameData) {
		DropSpellGemToWorld (spellGemGameData);
		highlightedSpellGemGameData = null;
		movingSpellGemGameData = null;
	}
	#endregion
	#region Puzzle Input Events
	public void OnTogglePuzzleMenuButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					playerObject.ChangePlayerState (PlayerState.PUZZLE_BROWSING);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					playerObject.ChangePlayerState (PlayerState.COMBAT);
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					//return spellgem to previous position

					RevertCurrentSpellGemMovement ();
					playerObject.ChangePlayerState (PlayerState.COMBAT);
					break;
				}
		}
	}
	private void RevertCurrentSpellGemMovement () {
		playerPuzzleUI.InterruptFlashRoutine ();
		if (movingSpellGemGameData != null) {
			movingSpellGemGameData.spellGemOriginCoordinate = revertInfo.spellGemOriginCoordinate;
			movingSpellGemGameData.spellGemRotation = revertInfo.spellGemRotation;
			movingSpellGemGameData.spellBindIndex = revertInfo.spellBindIndex;
			movingSpellGemGameData.spellGemEntity.SetNormalColor ();

			if (revertInfo.puzzleKey == PuzzleKey.OUTSIDE_BOUNDS) {
				DropSpellGemToWorld (movingSpellGemGameData);
			} else {
				PuzzleGameData puzzleGameData = playerObject.wizardGameData.puzzleGameDataDictionary[revertInfo.puzzleKey];
				if (movingSpellGemGameData != null && PuzzleUtility.CheckSpellFitmentEligibility (puzzleGameData, movingSpellGemGameData)) {
					playerObject.BindSpellGem (puzzleGameData, movingSpellGemGameData, PuzzleBindType.MANUAL);
				}
			}
		}
		revertInfo = null;
	}

	public void OnGrabButtonDown (PlayerState playerState) {
		Debug.Log ("PlayerPuzzleComponent: OnGrabButtonDown.");
		switch (playerState) {
			case (PlayerState.PUZZLE_BROWSING): {
					if (highlightedSpellGemGameData != null && wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleKey)) {
						playerObject.UnbindSpellGem (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleKey], highlightedSpellGemGameData, PuzzleUnbindType.TO_HAND);
						playerObject.ChangePlayerState (PlayerState.PUZZLE_MOVING_SPELLGEM);
					}
					break;
				}
		}
	}

	public void OnBindButtonDown (PlayerState playerState) {
		Debug.Log ("PlayerPuzzleComponent: OnBindButtonDown.");
		switch (playerState) {
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {

					if (movingSpellGemGameData != null && wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleKey)) {

						PuzzleGameData puzzleGameData = wizardGameData.puzzleGameDataDictionary[highlightedCursorLocation.puzzleKey];
						movingSpellGemGameData.spellGemOriginCoordinate = (Vector2Int)highlightedCursorLocation.coordinate;

						AttemptBindSpellGem (puzzleGameData, movingSpellGemGameData, PuzzleBindType.AUTOMATIC);

					}
					break;
				}
		}
	}
	//
	private void AttemptBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData, PuzzleBindType bindingType) {
		Debug.Log ("PlayerPuzzleComponent: AttemptBindSpellGem");
		if (bindingType == PuzzleBindType.AUTOMATIC) {
			if (puzzleGameData.puzzleData.puzzleType != PuzzleType.INVENTORY) {
				int spellBindIndex = PuzzleUtility.CalculateSpellBind (puzzleGameData.spellBindingDictionary);
				if (puzzleGameData.spellBindingDictionary.ContainsKey (spellBindIndex)) {
					spellGemGameData.spellBindIndex = spellBindIndex;
				} else return;//auto-bind fail (all slots full)
			}
		} else if (bindingType == PuzzleBindType.MANUAL) {
			if (!puzzleGameData.spellBindingDictionary.ContainsKey (spellGemGameData.spellBindIndex) || puzzleGameData.spellBindingDictionary [spellGemGameData.spellBindIndex] != null) {
				return;//manual spell binding fail.
			}
		}
		if (PuzzleUtility.CheckSpellFitmentEligibility (puzzleGameData, spellGemGameData)) {
			playerObject.BindSpellGem (puzzleGameData, spellGemGameData, bindingType);
		} else {
			playerPuzzleUI.ErrorFlashSpellGemEntity (spellGemGameData.spellGemEntity);
		}
	}
	public void OnRotateSpellGemCCWButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					int rotateIndex = 0;
					if (movingSpellGemGameData.spellGemRotation >= 3) {
						rotateIndex = 0;
					} else {
						rotateIndex = movingSpellGemGameData.spellGemRotation + 1;
					}
					playerObject.RotateSpellGem (movingSpellGemGameData, rotateIndex);
					break;
				}
		}
	}

	public void OnRotateSpellGemCWButtonDown (PlayerState playerState) {
		Debug.Log ("PlayerPuzzleComponent: SpellGem Rotate button pressed");
		switch (playerState) {
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					int rotateIndex = 0;
					if (movingSpellGemGameData.spellGemRotation <= 0) {
						rotateIndex = 3;
					} else {
						rotateIndex = movingSpellGemGameData.spellGemRotation - 1;
					}
					playerObject.RotateSpellGem (movingSpellGemGameData, rotateIndex);
					break;
				}
		}
	}

	public void OnDropButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					playerObject.DropStaff (wizardGameData.currentStaffKey, StaffDropType.MANUAL_DROP);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					if (highlightedSpellGemGameData != null) {
						SpellGemGameData droppingGemGameData = highlightedSpellGemGameData;
						//Save Data is assigned to a puzzle grouping.
						playerObject.UnbindSpellGem (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleKey], droppingGemGameData, PuzzleUnbindType.TO_FLOOR);
						playerObject.DropSpellGem (droppingGemGameData);
						playerObject.ChangePlayerState (PlayerState.PUZZLE_BROWSING);
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					//Save Data is not assigned to any puzzle grouping in this state.
					if (movingSpellGemGameData != null) {
						playerObject.DropSpellGem (movingSpellGemGameData);
						playerObject.ChangePlayerState (PlayerState.PUZZLE_BROWSING);
					}
					break;
				}
		}
	}

	public void OnSwitchToPrimaryStaffButtonDown () {
		if (wizardGameData.currentStaffKey != PuzzleKey.PRIMARY_STAFF && wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
			playerObject.EquipStaff (PuzzleKey.PRIMARY_STAFF, wizardGameData.puzzleGameDataDictionary [PuzzleKey.PRIMARY_STAFF], StaffEquipType.MANUAL_SWAP);
		}
	}
	public void OnSwitchToSecondaryStaffButtonDown () {
		if (wizardGameData.currentStaffKey != PuzzleKey.SECONDARY_STAFF && wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			playerObject.EquipStaff (PuzzleKey.SECONDARY_STAFF, wizardGameData.puzzleGameDataDictionary [PuzzleKey.SECONDARY_STAFF], StaffEquipType.MANUAL_SWAP);
		}
	}

	public void OnSwitchToAlternateStaffButtonDown () {
		if (wizardGameData.currentStaffKey == PuzzleKey.SECONDARY_STAFF) {
			if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
				playerObject.EquipStaff (PuzzleKey.PRIMARY_STAFF, wizardGameData.puzzleGameDataDictionary [PuzzleKey.PRIMARY_STAFF], StaffEquipType.QUICK_SWAP);
			}
		} else if (wizardGameData.currentStaffKey == PuzzleKey.PRIMARY_STAFF) {
			if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
				playerObject.EquipStaff (PuzzleKey.SECONDARY_STAFF, wizardGameData.puzzleGameDataDictionary [PuzzleKey.SECONDARY_STAFF], StaffEquipType.QUICK_SWAP);
			}
		}
	}
	private void DropStaffToWorld (PuzzleGameData puzzleGameData) {
		GameObject pickupGo = PoolManager.instance.ReuseObject(ConstantsManager.instance.staffPickUpData.spawnObjectPrefab, transform.position, Quaternion.identity);
		StaffPickUpObject staffPickUp = pickupGo.GetComponent<StaffPickUpObject>();
		wizardGameData.puzzleGameDataDictionary.Remove (puzzleGameData.puzzleKey);
		if (puzzleGameData.puzzleEntity != null) {
			staffPickUp.ReuseStaffPickUpPlayerDrop (puzzleGameData);
		}
	}

	private void DropSpellGemToWorld (SpellGemGameData spellGemGameData) {
		playerPuzzleUI.InterruptFlashRoutine ();
		if (spellGemGameData.spellGemEntity != null)
			Destroy (spellGemGameData.spellGemEntity.gameObject);
		GameObject pickupGo = PoolManager.instance.ReuseObject(ConstantsManager.instance.spellGemPickUpData.spawnObjectPrefab, transform.position, Quaternion.identity);
		pickupGo.transform.position = this.transform.position;
		SpellGemPickUpObject spellGemPickUp = pickupGo.GetComponent<SpellGemPickUpObject> ();
		spellGemPickUp.ReuseSpellGemPickUp (spellGemGameData.spellData);
	}
	public void OnSpellBindingButtonDown (PlayerState playerState, int spellIndex) {
		switch (playerState) {
			
			case (PlayerState.PUZZLE_BROWSING): {
					Debug.Log ("PlayerPuzzleComponent: SpellBindingDown on PUZZLE_BROWSING.");
					if (highlightedSpellGemGameData != null && highlightedSpellGemGameData.spellBindIndex != spellIndex && wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleKey)) {
						PuzzleGameData highlightedPuzzleGameData = wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleKey];
						if (highlightedPuzzleGameData.spellBindingDictionary.ContainsKey(spellIndex)) {

							SpellGemGameData potentialSwappingSpellGameData = null;
							foreach (SpellGemGameData gemGameData in highlightedPuzzleGameData.spellGemGameDataDictionary.Values) {
								if (gemGameData.spellBindIndex == spellIndex)
									potentialSwappingSpellGameData = gemGameData;
							}
							
							if (potentialSwappingSpellGameData != null) {
								HotSwapSpellBinds (highlightedPuzzleGameData, highlightedSpellGemGameData, potentialSwappingSpellGameData);
							} else {
								ColdSwapSpellBind (highlightedPuzzleGameData, highlightedSpellGemGameData, spellIndex);
							}
						}
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					if (movingSpellGemGameData != null && wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleKey)) {
						PuzzleGameData puzzleGameData = wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleKey];
						movingSpellGemGameData.spellGemOriginCoordinate = highlightedCursorLocation.coordinate.XY ();
						movingSpellGemGameData.spellBindIndex = spellIndex;
						AttemptBindSpellGem (puzzleGameData, movingSpellGemGameData, PuzzleBindType.MANUAL);
					}
					break;
				}
		}
	}
	private void ColdSwapSpellBind (PuzzleGameData puzzleGameData, SpellGemGameData highlightedSpellGemGameData, int newBindIndex) {
		playerObject.UnbindSpellGem (puzzleGameData, highlightedSpellGemGameData, PuzzleUnbindType.COLD_SWAP);
		highlightedSpellGemGameData.spellBindIndex = newBindIndex;
		playerObject.BindSpellGem (puzzleGameData, highlightedSpellGemGameData, PuzzleBindType.COLD_SWAP);

	}
	private void HotSwapSpellBinds(PuzzleGameData puzzleGameData, SpellGemGameData highlightedSpellGemGameData, SpellGemGameData swapBindGemGameData) {
		int highlightedIndex = highlightedSpellGemGameData.spellBindIndex;
		int swapBindIndex = swapBindGemGameData.spellBindIndex;
		playerObject.UnbindSpellGem (puzzleGameData, highlightedSpellGemGameData, PuzzleUnbindType.HOT_SWAP);
		playerObject.UnbindSpellGem (puzzleGameData, swapBindGemGameData, PuzzleUnbindType.HOT_SWAP);

		highlightedSpellGemGameData.spellBindIndex = swapBindIndex;
		swapBindGemGameData.spellBindIndex = highlightedIndex;
		playerObject.BindSpellGem(puzzleGameData, highlightedSpellGemGameData, PuzzleBindType.HOT_SWAP);
		playerObject.BindSpellGem (puzzleGameData, swapBindGemGameData, PuzzleBindType.HOT_SWAP);

	}
	#endregion

	#region update methods

	public void PuzzleUpdate (PlayerState playerState) {
		PuzzleCursorUpdate ();
		switch (playerState) {
			case (PlayerState.PUZZLE_BROWSING): {
					UpdateBrowsingTileCheck ();
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					playerPuzzleUI.RoundSpellGemEntityLocationToNearestTile (movingSpellGemGameData.spellGemEntity, currentGridCoord);
					break;
				}
			default:
				//shouldn't hit this, but it's a catch-all.
				return;
		}
	}

	public void PuzzleCursorUpdate () {
		Vector3 cursorPosition = playerObject.creaturePositions.targetTransform.position;
		PuzzleCursorLocation newCursorLocation = playerPuzzleUI.CalculatePuzzleCursorLocation (cursorPosition);
		if (highlightedCursorLocation != newCursorLocation) {
			highlightedCursorLocation = newCursorLocation;
			currentGridCoord = playerPuzzleUI.RoundCursorLocationToNearestPuzzleSlot (cursorPosition);
		}
	}

	public void OnUpdateCursorLocation () {
	}
	private void UpdateBrowsingTileCheck () {
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleKey)) {
			UpdateHighlightedTileInfo (PuzzleUtility.CheckMapValue (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleKey], highlightedCursorLocation.coordinate.XY ()));
		} else {
			highlightedTileInfo = null;
			highlightedSpellGemGameData = null;
			playerObject.highlightedSpellGemData = null;
			playerPuzzleUI.ClearToolTip ();
		}
	}

	#endregion


	private void UpdateHighlightedTileInfo (PuzzleTileInfo newPuzzleTileInfo) {
		if (newPuzzleTileInfo != highlightedTileInfo) {
			highlightedTileInfo = newPuzzleTileInfo;
			if (highlightedTileInfo == null) {
				playerPuzzleUI.ClearToolTip ();
			} else if (highlightedTileInfo.spellGemGameData == null) {
				highlightedSpellGemGameData = null;
				playerPuzzleUI.ClearToolTip ();
			} else if (highlightedSpellGemGameData != highlightedTileInfo.spellGemGameData) {
				highlightedSpellGemGameData = highlightedTileInfo.spellGemGameData;
				playerPuzzleUI.UpdateToolTip (highlightedSpellGemGameData.spellGemEntity, highlightedSpellGemGameData.spellData.spellName);
			}
		}
	}
}
