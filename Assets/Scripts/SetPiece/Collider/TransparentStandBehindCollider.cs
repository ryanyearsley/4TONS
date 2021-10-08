using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentStandBehindCollider : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;



	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player1") {
			spriteRenderer.color = new Color (1, 1, 1, 0.5f);
		}
	}
	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player1") { 
			spriteRenderer.color = new Color (1, 1, 1, 1f);
		}
	}
}
