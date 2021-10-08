using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractComponent : PlayerComponent {
	[SerializeField]
	private GameObject interactButtonPrefab;

	private GameObject interactButton;

	//active list of spell gems within pick-up distance.
	[SerializeField]
	private List<InteractableObject> interactablePickUps = new List<InteractableObject>();

	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		interactButton = Instantiate (interactButtonPrefab);
		interactButton.transform.parent = rootObject.transform;
		interactButton.transform.localPosition = Vector2.up * 1.75f;
		interactButton.SetActive (false);
	}

	public void AddItemToInteractable (InteractableObject pickUp) {
		interactablePickUps.Add (pickUp);
		if (interactablePickUps.Count >= 1) {
			interactButton.SetActive (true);
		}
	}
	public void RemoveItemFromInteractable (InteractableObject pickUp) {
		interactablePickUps.Remove (pickUp);
		if (interactablePickUps.Count == 0) {
			interactButton.SetActive (false);
		}
	}

	public void OnGrabButtonDown () {
		if (playerObject.currentPlayerState == PlayerState.COMBAT || playerObject.currentPlayerState == PlayerState.PUZZLE_BROWSING) {
			if (interactablePickUps.Count == 0) {
				return;//No pick ups in range.
			} else {
				InteractableObject closestObject = null;
				if (interactablePickUps.Count == 1) {
					closestObject = interactablePickUps [0];
				} else if (interactablePickUps.Count > 1) {
					closestObject = CalculateClosestPickUp (interactablePickUps);
				}

				if (closestObject is SpellGemPickUp) {
					Debug.Log ("PlayerInteractComponent: Picking up SG");
					PickUpSpellGem (closestObject.GetComponent<SpellGemPickUp> ());
				} else if (closestObject is StaffPickUp) {
					PickUpStaff (closestObject.GetComponent<StaffPickUp> ());
				} else {
					closestObject.InteractWithObject ();
				}
			}
		} else if (playerObject.currentPlayerState == PlayerState.PUZZLE_BROWSING) {

		}
	}

	private void PickUpSpellGem (SpellGemPickUp spellGemPickUp) {
		SpellGemGameData spellGemGameData = new SpellGemGameData();
		spellGemGameData.spellData = spellGemPickUp.spellData;
		spellGemGameData.spellCast = spellGemPickUp.spellCast;
		spellGemGameData.spellCast.transform.parent = this.transform.parent;
		playerObject.PickUpSpellGem (spellGemGameData);
		Destroy (spellGemPickUp.gameObject);
	}
	private void PickUpStaff (StaffPickUp staffPickUp) {

		PuzzleGameData staffPuzzleGameData = staffPickUp.puzzleGameData;
		if (!playerObject.wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
			playerObject.PickUpStaff (PuzzleKey.PRIMARY_STAFF, staffPuzzleGameData);
		} else if (!playerObject.wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			playerObject.PickUpStaff (PuzzleKey.SECONDARY_STAFF, staffPuzzleGameData);
		} else {
			playerObject.DropStaff (playerObject.wizardGameData.currentStaffKey);
			playerObject.PickUpStaff (playerObject.wizardGameData.currentStaffKey, staffPuzzleGameData);
		}
		Destroy (staffPickUp.gameObject);
		Debug.Log ("PlayerInteractComponent: staff pickup destroyed");
	}

	private InteractableObject CalculateClosestPickUp (List<InteractableObject> objectList) {
		InteractableObject closestInteractable = null;
		float closestDistance = 5;
		foreach (InteractableObject interactableObject in objectList) {
			float distance = Vector2.Distance(interactableObject.trans.position, transform.position);
			if (closestInteractable == null || distance < closestDistance) {
				closestInteractable = interactableObject;
				closestDistance = distance;
			}
		}
		return closestInteractable;
	}
}
