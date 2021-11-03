using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseCollider : MonoBehaviour {
	private Collider2D coll;

	void Awake () {
		coll = GetComponent<Collider2D> ();
	}

	public void SetInteractable () {
		coll.enabled = true;
	}

	public void SetNonInteractable () {
		coll.enabled = false;
	}
}
