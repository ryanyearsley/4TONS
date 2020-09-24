using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]
public class CursorController : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private Transform player;

	private void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	private void LateUpdate () {
		if (player != null) {
			Vector4 position = new Vector4 (player.position.x, player.position.y, 0f, 0f);
			spriteRenderer.material.SetVector ("PlayerPosition", position);
		}
	}

	public void SetPlayerTransform (Transform player) {
		this.player = player;
	}
}