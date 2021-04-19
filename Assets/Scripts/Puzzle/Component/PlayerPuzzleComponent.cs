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
	AUTOMATIC, MANUAL
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
			playerObject.EquipStaff (PuzzleKey.PRIMARY_STAFF, primaryStaffGameData);
		}
		else if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			PuzzleGameData secondaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.SECONDARY_STAFF];
			playerObject.EquipStaff (PuzzleKey.SECONDARY_STAFF, secondaryStaffGameData);
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

	public override void OnEquipStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {
	}

	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		DropStaffToWorld (puzzleGameData);

		PuzzleGameData otherStaffPuzzleData = null;
		foreach (KeyValuePair<PuzzleKey, PuzzleGameData> kvp in wizardGameData.puzzleGameDataDictionary) {
			if (kvp.Value.puzzleData.puzzleType != PuzzleType.INVENTORY) {
				otherStaffPuzzleData = kvp.Value;
			}
			// do something with entry.Value or entry.Key
		}

		if (otherStaffPuzzleData != null) {
			playerObject.EquipStaff (otherStaffPuzzleData.puzzleKey, otherStaffPuzzleData);
		} else {
			wizardGameData.currentStaffKey = PuzzleKey.NO_WEAPON;
		}

	}

	public override void OnPickUpSpellGem (SpellGemGameData spellGemGameData) {
		movingSpellGemGameData = spellGemGameData;
		revertInfo = new SpellGemRevertInfo (PuzzleKey.OUTSIDE_BOUNDS, movingSpellGemGameData);
		playerObject.OnChangePlayerState (PlayerState.PUZZLE_MOVING_SPELLGEM);
	}

	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {
		Debug.Log ("player puzzle OnBindSpellGem event");
		//update model
		PuzzleUtility.AddSpellGemToPuzzle (puzzleGameData, spellGemGameData);
		//update state
		movingSpellGemGameData = null;
		revertInfo = null;
	}
	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {
		//update model
		PuzzleUtility.RemoveSpellGemFromPuzzle (puzzleGameData, spellGemGameData);
		//update state
		movingSpellGemGameData = spellGemGameData;
		revertInfo = new SpellGemRevertInfo (puzzleGameData.puzzleKey, spellGemGameData);

	}
	#endregion
	#region Puzzle Input Events
	public void OnTogglePuzzleMenuButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					playerObject.OnChangePlayerState (PlayerState.PUZZLE_BROWSING);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					playerObject.OnChangePlayerState (PlayerState.COMBAT);
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					//return spellgem to previous position

					RevertCurrentSpellGemMovement ();
					playerObject.OnChangePlayerState (PlayerState.COMBAT);
					break;
				}
		}
	}
	private void RevertCurrentSpellGemMovement () {
		if (movingSpellGemGameData != null) {
			movingSpellGemGameData.spellGemOriginCoordinate = revertInfo.spellGemOriginCoordinate;
			movingSpellGemGameData.spellGemRotation = revertInfo.spellGemRotation;
			movingSpellGemGameData.spellBindIndex = revertInfo.spellBindIndex;

			if (revertInfo.puzzleKey == PuzzleKey.OUTSIDE_BOUNDS) {
				DropSpellGemToWorld (movingSpellGemGameData);
			} else {
				PuzzleGameData puzzleGameData = playerObject.wizardGameData.puzzleGameDataDictionary[revertInfo.puzzleKey];
				if (movingSpellGemGameData != null && PuzzleUtility.CheckSpellFitmentEligibility (puzzleGameData, movingSpellGemGameData)) {
					playerObject.BindSpellGem (puzzleGameData, movingSpellGemGameData);
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
						playerObject.OnUnbindSpellGem (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleKey], highlightedSpellGemGameData);
						playerObject.OnChangePlayerState (PlayerState.PUZZLE_MOVING_SPELLGEM);
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {

					Debug.Log ("PlayerPuzzleComponent: OnGrabButtonDown, and Moving SpellGem.");
					if (movingSpellGemGameData != null && wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleKey)) {

						Debug.Log ("PlayerPuzzleComponent: OnGrabButtonDown, GameData contains cursor key.");
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
			playerObject.BindSpellGem (puzzleGameData, spellGemGameData);
		} else {
			playerPuzzleUI.ErrorFlashSpellGemEntity (spellGemGameData.spellGemEntity);
		}
	}
	public void OnRotateSpellGemButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					if (movingSpellGemGameData.spellGemRotation >= 3) {
						movingSpellGemGameData.spellGemRotation = 0;
					} else {
						movingSpellGemGameData.spellGemRotation++;
					}
					movingSpellGemGameData.spellGemEntity.Rotate(movingSpellGemGameData.spellGemRotation * 90);
					break;
				}
		}
	}

	public void OnDropButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					playerObject.DropStaff (wizardGameData.currentStaffKey);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					if (highlightedSpellGemGameData != null) {
						//Save Data is assigned to a puzzle grouping.
						playerObject.OnUnbindSpellGem (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleKey], highlightedSpellGemGameData);
						DropSpellGemToWorld (highlightedSpellGemGameData);
						playerObject.OnChangePlayerState (PlayerState.PUZZLE_BROWSING);
					} else if (highlightedCursorLocation.puzzleKey != PuzzleKey.OUTSIDE_BOUNDS
						  && wizardGameData.puzzleGameDataDictionary.ContainsKey (wizardGameData.currentStaffKey)) {
						playerObject.DropStaff (wizardGameData.currentStaffKey);
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					//Save Data is not assigned to any puzzle grouping in this state.
					if (movingSpellGemGameData != null) {
						playerObject.DropSpellGem (movingSpellGemGameData);
						DropSpellGemToWorld (movingSpellGemGameData);
						playerObject.OnChangePlayerState (PlayerState.PUZZLE_BROWSING);
					}
					break;
				}
		}
	}

	public void OnSwitchToPrimaryStaffButtonDown () {
		if (wizardGameData.currentStaffKey != PuzzleKey.PRIMARY_STAFF && wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
			playerObject.EquipStaff (PuzzleKey.PRIMARY_STAFF, wizardGameData.puzzleGameDataDictionary [PuzzleKey.PRIMARY_STAFF]);
		}
	}
	public void OnSwitchToSecondaryStaffButtonDown () {
		if (wizardGameData.currentStaffKey != PuzzleKey.SECONDARY_STAFF && wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			playerObject.EquipStaff (PuzzleKey.SECONDARY_STAFF, wizardGameData.puzzleGameDataDictionary [PuzzleKey.SECONDARY_STAFF]);
		}
	}

	public void OnSwitchToOtherStaffButtonDown () {

	}
	private void DropStaffToWorld (PuzzleGameData puzzleGameData) {
		GameObject pickupGo = PoolManager.instance.ReuseObject(ConstantsManager.instance.staffPickupPrefab, transform.position, Quaternion.identity);
		StaffPickUp staffPickUp = pickupGo.GetComponent<StaffPickUp>();
		wizardGameData.puzzleGameDataDictionary.Remove (puzzleGameData.puzzleKey);
		if (puzzleGameData.puzzleEntity != null) {
			staffPickUp.ReuseStaffPickUpPlayerDrop (puzzleGameData);
		}
	}

	private void DropSpellGemToWorld (SpellGemGameData spellGemGameData) {
		if (spellGemGameData.spellGemEntity != null)
			Destroy (spellGemGameData.spellGemEntity.gameObject);
		GameObject pickupGo = PoolManager.instance.ReuseObject(ConstantsManager.instance.spellGemPickupPrefab, transform.position, Quaternion.identity);
		pickupGo.transform.position = this.transform.position;
		SpellGemPickUp spellGemPickUp = pickupGo.GetComponent<SpellGemPickUp> ();
		spellGemPickUp.ReuseSpellGemPickUp (spellGemGameData.spellData);
		highlightedSpellGemGameData = null;
		movingSpellGemGameData = null;
	}
	public void OnSpellBindingButtonDown (PlayerState playerState, int spellIndex) {
		switch (playerState) {
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
	private void UpdateBrowsingTileCheck () {
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleKey)) {
			UpdateHighlightedTileInfo (PuzzleUtility.CheckMapValue (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleKey], highlightedCursorLocation.coordinate.XY ()));
		} else {
			highlightedTileInfo = null;
			highlightedSpellGemGameData = null;
			playerPuzzleUI.ClearHighlightedSpellGemInformation ();
		}
	}
	#endregion


	private void UpdateHighlightedTileInfo (PuzzleTileInfo newPuzzleTileInfo) {
		if (newPuzzleTileInfo != highlightedTileInfo) {
			highlightedTileInfo = newPuzzleTileInfo;
			if (highlightedTileInfo == null) {
				playerPuzzleUI.ClearHighlightedSpellGemInformation ();
			} else if (highlightedTileInfo.spellGemGameData == null) {
				highlightedSpellGemGameData = null;
				playerPuzzleUI.ClearHighlightedSpellGemInformation ();
			} else if (highlightedSpellGemGameData != highlightedTileInfo.spellGemGameData) {
				highlightedSpellGemGameData = highlightedTileInfo.spellGemGameData;
				playerPuzzleUI.UpdateHighlightedSpellGemInformation(highlightedSpellGemGameData.spellData);
			}
		}
	}
}
