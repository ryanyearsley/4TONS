using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCollider : MonoBehaviour
{

	private InteractableObject interactableComponent;
	private Collider2D coll;

	void Awake () {
		interactableComponent = GetComponentInParent<InteractableObject> ();
		coll = GetComponent<Collider2D> ();
	}

	public void SetInteractable() {
		coll.enabled = true;
	}

	public void SetNonInteractable() {
		coll.enabled = false;
	}

	private void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player1") {
			interactableComponent.HighlightInteractable ();
			other.transform.GetComponentInParent<PlayerInteractComponent> ().AddItemToInteractable (interactableComponent);
		}
	}
	private void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player1") {
			interactableComponent.UnhighlightInteractable ();
			other.transform.GetComponentInParent<PlayerInteractComponent> ().RemoveItemFromInteractable (interactableComponent);
		}
	}
}
