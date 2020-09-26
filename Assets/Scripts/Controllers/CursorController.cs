using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]
public class CursorController : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private Transform cursorCenter;

	private void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	private void LateUpdate () {
		if (cursorCenter != null) {
			Vector4 position = new Vector4 (cursorCenter.position.x, cursorCenter.position.y, 0f, 0f);
			spriteRenderer.material.SetVector ("CursorCenterPosition", position);
		}
	}

	public void SetCursorCenter (Transform cursorCenter) {
		this.cursorCenter = cursorCenter;
	}
}