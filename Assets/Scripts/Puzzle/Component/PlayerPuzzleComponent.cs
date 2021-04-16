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
	private PlayerPuzzleUIComponent puzzleUI;

	private WizardGameData wizardGameData;

	//currents
	[SerializeField]
	private PuzzleCursorLocation highlightedCursorLocation;
	[SerializeField]
	private PuzzleTileInfo highlightedTileInfo;
	private SpellGemGameData highlightedSpellGemGameData;
	private Vector2Int currentGridCell;

	private SpellGemRevertInfo revertInfo;

	[SerializeField]
	private SpellGemGameData movingSpellGemGameData;//MovingSpellGemGameData is always in limbo (not assigned on player GameData)


	#region initialization

	//called on player object re-use to inject player save data.
	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		puzzleUI = GetComponentInChildren<PlayerPuzzleUIComponent> ();
	}


	public override void ReusePlayerComponent (Player player) {
		base.ReusePlayerComponent (player);
		this.wizardGameData = playerObject.wizardGameData;
		puzzleUI.ClearGridChildren ();


	}
	public override void OnSpawn (Vector3 spawnPosition) {
		Player player = playerObject.player;
		PuzzleGameData equipStaffPuzzleGameData = null;
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.INVENTORY)) {
			PuzzleGameData inventoryGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.INVENTORY];
			Debug.Log ("PuzzleComponent: Creating Staff PuzzleEntity.");
			inventoryGameData.puzzleEntity = puzzleUI.AddPuzzleEntityToPuzzleUI (PuzzleKey.INVENTORY, inventoryGameData);
			LoadSpellGemGameDataDictionaries (inventoryGameData, player.wizardSaveData.inventorySaveData.spellGemSaveDataDictionary);
		}

		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
			PuzzleGameData primaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.PRIMARY_STAFF];
			equipStaffPuzzleGameData = primaryStaffGameData;
			primaryStaffGameData.puzzleEntity = puzzleUI.AddPuzzleEntityToPuzzleUI (PuzzleKey.PRIMARY_STAFF, primaryStaffGameData);
			LoadSpellGemGameDataDictionaries (primaryStaffGameData, player.wizardSaveData.primaryStaffSaveData.spellGemSaveDataDictionary);
		}

		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			PuzzleGameData secondaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.SECONDARY_STAFF];
			secondaryStaffGameData.puzzleEntity = puzzleUI.AddPuzzleEntityToPuzzleUI (PuzzleKey.SECONDARY_STAFF, secondaryStaffGameData);
			LoadSpellGemGameDataDictionaries (secondaryStaffGameData, player.wizardSaveData.secondaryStaffSaveData.spellGemSaveDataDictionary);
		}

		playerObject.EquipStaff (PuzzleKey.PRIMARY_STAFF, equipStaffPuzzleGameData);
	}


	//only done on player reuse
	private void LoadSpellGemGameDataDictionaries (PuzzleGameData puzzleGameData, SpellGemSaveDataDictionary spellGemSaveDataDictionary) {
		Debug.Log ("Loading player spellgem entities");
		puzzleGameData.spellGemGameDataDictionary = new SpellGemGameDataDictionary ();
		foreach (SpellGemSaveData spellSaveData in spellGemSaveDataDictionary.Values) {
			SpellGemGameData spellGemGameData = WizardGameDataMapper.MapSpellGemSaveToGameData(spellSaveData);
			if (PuzzleUtility.CheckSpellFitmentEligibility (puzzleGameData, spellGemGameData)) {
				Debug.Log ("PlayerPuzzleComponent: spellgem save data loaded successfully for spell: " + spellGemGameData.spellData.spellName);
				playerObject.BindSpellGem (puzzleGameData, spellGemGameData);
			} else {
				Debug.Log ("PlayerPuzzleComponent: Cannot convert SpellGemSaveData -> Game data. SpellGem does not fit in alleged spot.");
			}
		}
	}

	#endregion
	#region PlayerComponent Events
	public override void OnChangePlayerState (PlayerState playerState) {
		Debug.Log ("player puzzle OnChangePlayerState event");
		switch (playerState) {
			case (PlayerState.DEAD): {
					puzzleUI.DisablePuzzleUI ();
					break;
				}
			case (PlayerState.COMBAT): {
					puzzleUI.DisablePuzzleUI ();
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					puzzleUI.EnablePuzzleUI ();
					if (wizardGameData.currentStaffKey != PuzzleKey.NO_WEAPON) {
						wizardGameData.puzzleGameDataDictionary [wizardGameData.currentStaffKey].puzzleEntity.gameObject.SetActive (true);
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					puzzleUI.EnablePuzzleUI ();
					break;
				}
		}
	}
	public override void OnPickUpStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {

		puzzleGameData.puzzleKey = key;
		wizardGameData.puzzleGameDataDictionary.Add (key, puzzleGameData);
		puzzleUI.AddPuzzleEntityToPuzzleUI (key, puzzleGameData);
	}

	public override void OnEquipStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {

		foreach (PuzzleGameData value in wizardGameData.puzzleGameDataDictionary.Values) {
			if (value.puzzleData.puzzleType != PuzzleType.INVENTORY)
				value.puzzleEntity.gameObject.SetActive (false);
		}
		puzzleGameData.puzzleEntity.gameObject.SetActive (true);
		puzzleUI.UpdateStaffInfo (puzzleGameData.puzzleData);
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
			puzzleUI.ClearStaffInfo ();
		}

	}

	public override void OnPickUpSpellGem (SpellGemGameData spellGemGameData) {

		spellGemGameData.spellGemEntity = puzzleUI.AddSpellGemToPuzzleUIUncommited (spellGemGameData);
		puzzleUI.MoveSpellGemToUncommited (spellGemGameData);
		puzzleUI.UpdateHighlightedSpellGemInformation (spellGemGameData.spellData);
		movingSpellGemGameData = spellGemGameData;
		revertInfo = new SpellGemRevertInfo (PuzzleKey.OUTSIDE_BOUNDS, movingSpellGemGameData);
		playerObject.OnChangePlayerState (PlayerState.PUZZLE_MOVING_SPELLGEM);
	}

	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {
		Debug.Log ("player puzzle OnBindSpellGem event");
		//update puzzle view
		SpellGemEntity spellGemEntity = puzzleUI.AddSpellGemToPuzzleUI (puzzleGameData.puzzleEntity, spellGemGameData);

		//update model
		PuzzleUtility.AddSpellGemToPuzzle (puzzleGameData, spellGemGameData);
		spellGemGameData.spellGemEntity = spellGemEntity;
		//update state
		movingSpellGemGameData = null;
		revertInfo = null;
	}
	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {

		//update state
		movingSpellGemGameData = spellGemGameData;
		revertInfo = new SpellGemRevertInfo (puzzleGameData.puzzleKey, spellGemGameData);
		//update model
		PuzzleUtility.RemoveSpellGemFromPuzzle (puzzleGameData, spellGemGameData);
		//update view
		puzzleUI.MoveSpellGemToUncommited (movingSpellGemGameData);
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
					if (highlightedSpellGemGameData != null && wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleRegion)) {
						playerObject.OnUnbindSpellGem (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleRegion], highlightedSpellGemGameData);
						puzzleUI.MoveSpellGemToUncommited (movingSpellGemGameData);
						playerObject.OnChangePlayerState (PlayerState.PUZZLE_MOVING_SPELLGEM);
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {

					Debug.Log ("PlayerPuzzleComponent: OnGrabButtonDown, and Moving SpellGem.");
					if (movingSpellGemGameData != null && wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleRegion)) {

						Debug.Log ("PlayerPuzzleComponent: OnGrabButtonDown, GameData contains cursor key.");
						PuzzleGameData puzzleGameData = wizardGameData.puzzleGameDataDictionary[highlightedCursorLocation.puzzleRegion];
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
			puzzleUI.ErrorFlashSpellGemEntity (spellGemGameData.spellGemEntity);
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
					puzzleUI.RotateSpellGemEntity (movingSpellGemGameData.spellGemEntity, movingSpellGemGameData.spellGemRotation * 90);
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
						playerObject.OnUnbindSpellGem (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleRegion], highlightedSpellGemGameData);
						DropSpellGemToWorld (highlightedSpellGemGameData);
						playerObject.OnChangePlayerState (PlayerState.PUZZLE_BROWSING);
					} else if (highlightedCursorLocation.puzzleRegion != PuzzleKey.OUTSIDE_BOUNDS
						  && wizardGameData.puzzleGameDataDictionary.ContainsKey (wizardGameData.currentStaffKey)) {
						playerObject.DropStaff (wizardGameData.currentStaffKey);
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					//Save Data is not assigned to any puzzle grouping in this state.
					if (movingSpellGemGameData != null) {
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
		staffPickUp.SetupObject ();
		wizardGameData.puzzleGameDataDictionary.Remove (puzzleGameData.puzzleKey);
		if (puzzleGameData.puzzleEntity != null) {
			staffPickUp.ReuseStaffPickUpPlayerDrop (puzzleGameData);
		}
		staffPickUp.AddPuzzleEntityToPickUp (puzzleGameData);
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
					if (movingSpellGemGameData != null && wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleRegion)) {
						PuzzleGameData puzzleGameData = wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleRegion];
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
					UpdateTileCheck ();
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					puzzleUI.RoundSpellGemEntityLocationToNearestTile (movingSpellGemGameData.spellGemEntity, currentGridCell);
					break;
				}
			default:
				//shouldn't hit this, but it's a catch-all.
				return;
		}
	}

	public void PuzzleCursorUpdate () {
		Vector3 cursorPosition = playerObject.creaturePositions.targetTransform.position;
		PuzzleCursorLocation newCursorLocation = puzzleUI.CalculatePuzzleCursorLocation (cursorPosition);
		if (highlightedCursorLocation != newCursorLocation) {
			highlightedCursorLocation = newCursorLocation;
			currentGridCell = puzzleUI.RoundCursorLocationToNearestPuzzleSlot (cursorPosition);
		}
	}
	private void UpdateTileCheck () {
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (highlightedCursorLocation.puzzleRegion)) {
			UpdateHighlightedTileInfo (PuzzleUtility.CheckMapValue (wizardGameData.puzzleGameDataDictionary [highlightedCursorLocation.puzzleRegion], highlightedCursorLocation.coordinate.XY ()));
		}
	}
	#endregion


	private void UpdateHighlightedTileInfo (PuzzleTileInfo newPuzzleTileInfo) {
		if (newPuzzleTileInfo != highlightedTileInfo) {
			highlightedTileInfo = newPuzzleTileInfo;
			if (highlightedTileInfo == null) {
				puzzleUI.ClearHighlightedSpellGemInformation ();
			} else if (highlightedTileInfo.spellGemGameData == null) {
				highlightedSpellGemGameData = null;
				playerObject.OnChangePlayerState (PlayerState.PUZZLE_BROWSING);
				puzzleUI.ClearHighlightedSpellGemInformation ();
			} else if (highlightedSpellGemGameData != highlightedTileInfo.spellGemGameData) {
				highlightedSpellGemGameData = highlightedTileInfo.spellGemGameData;
				puzzleUI.UpdateHighlightedSpellGemInformation (highlightedSpellGemGameData.spellData);
			}
		}
	}

	#region item pickup
	#endregion

}
