using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour {

	public SpriteRenderer spriteRenderer;

	private Material material;

	private void Start () {
		material = spriteRenderer.material;
	}

	public void EnableOutline () {
		material.SetFloat ("_OutlineThickness", 2);
	}

	public void DisableOutline () {
		material.SetFloat ("_OutlineThickness", 0);
	}
}