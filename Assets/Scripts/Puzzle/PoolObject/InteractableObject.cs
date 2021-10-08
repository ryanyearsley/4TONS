using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : PoolObject
{
	public Transform trans;

	private void Awake () {
		trans = GetComponent<Transform> ();
	}

	protected virtual void HighlightInteractable() {

	}
	protected virtual void UnhighlightInteractable () {
	}

	public virtual void InteractWithObject () {

	}

	private void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player1") {
			HighlightInteractable ();
			other.transform.GetComponentInParent<PlayerInteractComponent> ().AddItemToInteractable (this);
			}
	}
	private void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player1") {
			UnhighlightInteractable ();
			other.transform.GetComponentInParent<PlayerInteractComponent> ().RemoveItemFromInteractable (this);
		}
	}
}
