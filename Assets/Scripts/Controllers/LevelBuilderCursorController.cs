using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilderCursorController : MonoBehaviour {

	private void OnTriggerEnter2D (Collider2D collision) {
		TileObject tileObject = collision.GetComponent<TileObject> ();
		if (tileObject != null) {
			tileObject.EnableOutline ();
		}
	}

	private void OnTriggerExit2D (Collider2D collision) {
		TileObject tileObject = collision.GetComponent<TileObject> ();
		if (tileObject != null) {
			tileObject.DisableOutline ();
		}
	}
}