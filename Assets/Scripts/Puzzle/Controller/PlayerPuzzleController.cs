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

public enum PuzzleCursorRegion {
	DISABLED, INVENTORY, STAFF, OUTSIDE_BOUNDS
}
public class PlayerPuzzleController : MonoBehaviour {
	private PlayerStateController playerStateController;

	[SerializeField]
	private Vector2Int defaultInventorySize = new Vector2Int(9, 6);
	[SerializeField]
	private Vector2Int inventoryOrigin = new Vector2Int(-10, 0);
	[SerializeField]
	private Vector2Int staffOrigin = Vector2Int.zero;

	[SerializeField]
	private PuzzleCursorRegion currentPuzzleCursorRegion;

	private PuzzleCursorRegion revertRegion;

	private PuzzleUI puzzleUI;
	private GameObject interactButton;


	private Dictionary<PuzzleCursorRegion, PuzzleGroupingDetails> puzzleGroupingDictionary = new Dictionary<PuzzleCursorRegion, PuzzleGroupingDetails>();

	[SerializeField]
	public PuzzleGroupingDetails inventoryDetails;
	[SerializeField]
	public PuzzleGroupingDetails staffDetails;


	[SerializeField]
	private Vector2Int highlightedGridCell;
	[SerializeField]
	private PuzzleTileInfo highlightedTileInfo;

	private SpellSaveData highlightedSpellSaveData;

	private SpellSaveData movingSpellSaveData;


	//active list of spell gems within pick-up distance.
	private List<SpellGemPickup> interactableSpellGemPickups = new List<SpellGemPickup>();

	#region initialization
	private void OnEnable () {
		playerStateController = GetComponent<PlayerStateController> ();
		playerStateController.OnChangeStateEvent += OnChangeState;
		playerStateController.OnBindSpellGemEvent += OnBindSpellGem;
		playerStateController.OnUnbindSpellGemEvent += OnUnbindSpellGem;
		playerStateController.OnUpdateSpellBindingEvent += OnUpdateSpellBinding;
	}

	private void OnDisable () {
		playerStateController.OnChangeStateEvent -= OnChangeState;
		playerStateController.OnBindSpellGemEvent -= OnBindSpellGem;
		playerStateController.OnUnbindSpellGemEvent -= OnUnbindSpellGem;
		playerStateController.OnUpdateSpellBindingEvent -= OnUpdateSpellBinding;
	}


	public void InitializeComponent (Player player) {
		//player component setup
		playerStateController = GetComponent<PlayerStateController> ();
		//puzzle UI setup
		interactButton = Instantiate (ConstantsManager.instance.InteractButtonPrefab);
		interactButton.transform.parent = this.transform;
		interactButton.transform.localPosition = Vector2.up * 1.5f;
		interactButton.SetActive (false);

		GameObject puzzleUIGo = Instantiate (ConstantsManager.instance.puzzleUIPrefab);
		puzzleUIGo.transform.parent = this.transform.root;
		puzzleUI = puzzleUIGo.GetComponent<PuzzleUI> ();
		puzzleUI.ClearHighlightedSpellGemInformation ();

		//inventory setup
		inventoryDetails = new PuzzleGroupingDetails
			(PuzzleFactory.GenerateEmptyInventory (defaultInventorySize),
			inventoryOrigin,
			PuzzleGroupingType.INVENTORY,
			puzzleUI);
		puzzleGroupingDictionary.Add (PuzzleCursorRegion.INVENTORY, inventoryDetails);

		//staff setup
		staffDetails = new PuzzleGroupingDetails
			(PuzzleFactory.DeserializeStaffFile (player.currentWizard.primaryStaffSaveData.staffData.staffFile),
			staffOrigin,
			PuzzleGroupingType.EQUIPPED_STAFF,
			puzzleUI);
		puzzleGroupingDictionary.Add (PuzzleCursorRegion.STAFF, staffDetails);
		//Validate 

		puzzleUI.InitializePuzzleUI (player.currentWizard, inventoryDetails, staffDetails);
		LoadPlayerSpellGemEntities (player.currentWizard.primaryStaffSaveData.puzzleSaveDataDictionary, staffDetails);
		LoadPlayerSpellGemEntities (player.currentWizard.inventorySaveDataDictionary, inventoryDetails);
	}

	private void LoadPlayerSpellGemEntities (PuzzleSaveDataDictionary puzzleSaveDataDictionary, PuzzleGroupingDetails details) {
		details.puzzleSaveDataDictionary = puzzleSaveDataDictionary;
		foreach (SpellSaveData spellSaveData in puzzleSaveDataDictionary.Values) {
			if (PuzzleUtility.CheckSpellFitmentEligibility (details, spellSaveData)) {
				playerStateController.OnBindSpellGem (details, spellSaveData);
			}
		}
	}

	#endregion
	#region Player Events
	public void OnChangeState (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.DEAD):
			case (PlayerState.COMBAT): {
					puzzleUI.gameObject.SetActive (false);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					puzzleUI.gameObject.SetActive (true);
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					puzzleUI.gameObject.SetActive (true);
					break;
				}
		}
	}

	public void OnBindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		PuzzleUtility.AddSpellGemToPuzzle (details, spellSaveData);
		SpellGemEntity spellGemEntity = puzzleUI.AddSpellGemToPuzzleUI (details, spellSaveData);
		spellSaveData.spellGemEntity = spellGemEntity;
		movingSpellSaveData = null;
		playerStateController.OnChangeState (PlayerState.PUZZLE_BROWSING);
	}
	public void OnUnbindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		movingSpellSaveData = spellSaveData;
		revertRegion = currentPuzzleCursorRegion;
		PuzzleUtility.RemoveSpellGemFromPuzzle (details, spellSaveData);
		SpellGemEntity spellGemEntity = puzzleUI.AddSpellGemToPuzzleUI (details, spellSaveData);
		spellSaveData.spellGemEntity = spellGemEntity;
		puzzleUI.MoveSpellGemToUncommited (movingSpellSaveData);
		playerStateController.OnChangeState (PlayerState.PUZZLE_MOVING_SPELLGEM);
	}
	public void OnUpdateSpellBinding (int spellIndex, Spell spell) {
		if (spell != null) {
			puzzleUI.UpdateBindingUI (spellIndex, true);
		} else {
			puzzleUI.UpdateBindingUI (spellIndex, false);
		}
	}
	#endregion
	#region Puzzle Input Events
	public void OnTogglePuzzleMenuButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					playerStateController.OnChangeState (PlayerState.PUZZLE_BROWSING);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					playerStateController.OnChangeState (PlayerState.COMBAT);
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					//return spellgem to previous position
					PuzzleGroupingDetails details = puzzleGroupingDictionary[revertRegion];
					if (revertRegion == PuzzleCursorRegion.OUTSIDE_BOUNDS) {

					}
					if (movingSpellSaveData != null && PuzzleUtility.CheckSpellFitmentEligibility (details, movingSpellSaveData)) {
						playerStateController.OnBindSpellGem (details, movingSpellSaveData);
					}
					playerStateController.OnChangeState (PlayerState.COMBAT);
					break;
				}
		}
	}
	public void OnGrabSpellGemButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.PUZZLE_BROWSING): {
					Debug.Log ("move spellgem button down while browsing puzzle.");
					if (highlightedSpellSaveData != null && puzzleGroupingDictionary.ContainsKey (currentPuzzleCursorRegion)) {
						playerStateController.OnUnbindSpellGem (puzzleGroupingDictionary [currentPuzzleCursorRegion], highlightedSpellSaveData);
						puzzleUI.MoveSpellGemToUncommited (movingSpellSaveData);
						playerStateController.OnChangeState (PlayerState.PUZZLE_MOVING_SPELLGEM);
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					if (movingSpellSaveData != null && puzzleGroupingDictionary.ContainsKey (currentPuzzleCursorRegion)) {
						PuzzleGroupingDetails details = puzzleGroupingDictionary[currentPuzzleCursorRegion];
						movingSpellSaveData.spellGemOriginCoordinate = PuzzleUtility.CheckMapValue (details, highlightedGridCell).mapCoordinate;
						if (PuzzleUtility.CheckSpellFitmentEligibility(details, movingSpellSaveData)) {
							playerStateController.OnBindSpellGem (details, movingSpellSaveData);
						} else {
							puzzleUI.ErrorFlashSpellGemEntity (movingSpellSaveData.spellGemEntity);
						}
					}
					break;
				}
		}
	}
	public void OnRotateSpellGemButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					if (movingSpellSaveData.spellGemRotation >= 3) {
						movingSpellSaveData.spellGemRotation = 0;
					} else {
						movingSpellSaveData.spellGemRotation++;
					}
					puzzleUI.RotateSpellGemEntity (movingSpellSaveData.spellGemEntity, movingSpellSaveData.spellGemRotation * 90);
					break;
				}
		}
	}

	public void OnDropSpellGemButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.PUZZLE_BROWSING): {
					if (highlightedSpellSaveData != null) {
						//Save Data is assigned to a puzzle grouping.
						playerStateController.OnUnbindSpellGem(puzzleGroupingDictionary[currentPuzzleCursorRegion], highlightedSpellSaveData);
						DropSpellGemToWorld (highlightedSpellSaveData);
						playerStateController.OnChangeState (PlayerState.PUZZLE_BROWSING);
						//Unbind from that. This removes it from the dictionary.
						//dele
					}
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					//Save Data is not assigned to any puzzle grouping in this state.
					if (movingSpellSaveData != null) {
						DropSpellGemToWorld (movingSpellSaveData);
						playerStateController.OnChangeState (PlayerState.PUZZLE_BROWSING);
					}
					break;
			}
		}
	}
	private void DropSpellGemToWorld (SpellSaveData spellSaveData) {
		if (spellSaveData.spellCast != null)
			Destroy (highlightedSpellSaveData.spellCast.gameObject);
		if (spellSaveData.spellGemEntity != null)
			Destroy (spellSaveData.spellGemEntity.gameObject);
		GameObject pickupGo = Instantiate(ConstantsManager.instance.spellGemPickupPrefab);
		pickupGo.transform.position = this.transform.position;
		SpellGemPickup spellGemPickUp = pickupGo.GetComponent<SpellGemPickup> ();
		spellGemPickUp.InitializeSpellGemPickUp (spellSaveData.spellData);
		highlightedSpellSaveData = null;
		movingSpellSaveData = null;
	}
	public void OnSpellBindingButtonDown (PlayerState playerState, int spellIndex) {
		switch (playerState) {
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					if (movingSpellSaveData != null && puzzleGroupingDictionary.ContainsKey (currentPuzzleCursorRegion)) {
						PuzzleGroupingDetails details = puzzleGroupingDictionary [currentPuzzleCursorRegion];
						movingSpellSaveData.spellGemOriginCoordinate = PuzzleUtility.CheckMapValue (details, highlightedGridCell).mapCoordinate;
						movingSpellSaveData.spellIndex = spellIndex;
						if (PuzzleUtility.CheckSpellFitmentEligibility (details, movingSpellSaveData)) {
							playerStateController.OnBindSpellGem (details, movingSpellSaveData);
						} else {
							puzzleUI.ErrorFlashSpellGemEntity (movingSpellSaveData.spellGemEntity);
						}
					}
					break;
				}
		}
	}

	public void OnConfirmButtonDown () {
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
					puzzleUI.RoundSpellGemEntityLocationToNearestTile (movingSpellSaveData.spellGemEntity, highlightedGridCell);
					break;
				}
			default:
				//shouldn't hit this, but it's a catch-all.
				return;
		}
	}

	public void PuzzleCursorUpdate () {
		Vector3 cursorPosition = playerStateController.creaturePositions.targetTransform.position;
		highlightedGridCell = puzzleUI.RoundCursorLocationToNearestPuzzleSlot (cursorPosition);
		currentPuzzleCursorRegion = CalculatePuzzleCursorRegion (highlightedGridCell);
	}
	private PuzzleCursorRegion CalculatePuzzleCursorRegion (Vector2Int gridCell) {
		if (staffDetails.gridBounds.isWithinBounds (gridCell))
			return PuzzleCursorRegion.STAFF;
		else if (inventoryDetails.gridBounds.isWithinBounds (gridCell))
			return PuzzleCursorRegion.INVENTORY;
		else
			return PuzzleCursorRegion.OUTSIDE_BOUNDS;
	}
	private void UpdateTileCheck () {
		if (puzzleGroupingDictionary.ContainsKey (currentPuzzleCursorRegion)) {
			UpdateHighlightedTileInfo (PuzzleUtility.CheckMapValue (puzzleGroupingDictionary [currentPuzzleCursorRegion], highlightedGridCell));
		}
	}
	#endregion


	private void UpdateHighlightedTileInfo (PuzzleTileInfo newPuzzleTileInfo) {
		if (newPuzzleTileInfo != highlightedTileInfo) {
			highlightedTileInfo = newPuzzleTileInfo;
			if (highlightedTileInfo == null) {
				puzzleUI.ClearHighlightedSpellGemInformation ();
			} else if (highlightedTileInfo.spellSaveData == null) {
				highlightedSpellSaveData = null;
				puzzleUI.ClearHighlightedSpellGemInformation ();
			} else if (highlightedSpellSaveData != highlightedTileInfo.spellSaveData) {
				highlightedSpellSaveData = highlightedTileInfo.spellSaveData;
				puzzleUI.UpdateHighlightedSpellGemInformation (highlightedTileInfo.spellSaveData.spellData);
			}
		}
	}
	public void OnChangePuzzleCursorRegion (PuzzleCursorRegion puzzleCursorRegion) {
		currentPuzzleCursorRegion = puzzleCursorRegion;
	}
	#region spellgem pickup
	public void AddSpellGemToInteractable (SpellGemPickup spellGemPickup) {
		interactableSpellGemPickups.Add (spellGemPickup);
		if (interactableSpellGemPickups.Count >= 1) {
			interactButton.SetActive (true);
		}

	}
	public void RemoveSpellGemFromInteractable (SpellGemPickup spellGemPickup) {
		interactableSpellGemPickups.Remove (spellGemPickup);

		if (interactableSpellGemPickups.Count == 0) {
			interactButton.SetActive (false);
		}
	}
	public void OnPickUpSpellGemButtonDown (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					if (interactableSpellGemPickups.Count == 1) {
						PickUpSpellGem (interactableSpellGemPickups [0]);
					} else if (interactableSpellGemPickups.Count > 1) {
						PickUpSpellGem (CalculateClosestSpellGem ());
					}
					break;
				}
		}
	}

	private void PickUpSpellGem (SpellGemPickup spellGemPickUp) {
		SpellSaveData pickUpSpellSaveData = new SpellSaveData();
		pickUpSpellSaveData.spellData = spellGemPickUp.spellData;
		Destroy (spellGemPickUp.gameObject);
		pickUpSpellSaveData.spellGemEntity = puzzleUI.AddSpellGemToPuzzleUI (inventoryDetails, pickUpSpellSaveData);
		puzzleUI.MoveSpellGemToUncommited (pickUpSpellSaveData);
		movingSpellSaveData = pickUpSpellSaveData;
		revertRegion = PuzzleCursorRegion.OUTSIDE_BOUNDS;
		playerStateController.OnChangeState (PlayerState.PUZZLE_MOVING_SPELLGEM);
	}
	private SpellGemPickup CalculateClosestSpellGem () {
		SpellGemPickup closestSpellGemPickup = null;
		float closestDistance = 5;
		foreach (SpellGemPickup spellGemPickup in interactableSpellGemPickups) {
			float distance = Vector2.Distance(spellGemPickup.transform.position, transform.position);
			if (closestSpellGemPickup == null || distance < closestDistance) {
				closestSpellGemPickup = spellGemPickup;
				closestDistance = distance;
			}
		}
		return closestSpellGemPickup;
	}
	#endregion


	private void OnDrawGizmosSelected () {
		if (staffDetails != null) {
			for (int x = 0; x <= staffDetails.mapBounds.maxCoord.x; x++) {
				for (int y = 0; y <= staffDetails.mapBounds.maxCoord.y; y++) {
					Vector3 position = new Vector3 (-staffDetails.mapBounds.maxCoord.x / 2 + x + 0.5f, -staffDetails.mapBounds.maxCoord.y / 2 + y + 0.5f, 0);
					PuzzleTileInfo tile = staffDetails.map[x, y];
					Gizmos.color = (staffDetails.map [x, y].value != 1) ? Color.black : Color.white;
					Gizmos.DrawCube (position, Vector3.one);
				}
			}
		}
	}
}
