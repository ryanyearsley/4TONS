using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTutorialPortalComponent : MonoBehaviour
{

	private void OnCollisionEnter2D (Collision2D collision) {
		if (collision.collider.tag == "Player1") {
			TutorialManager.instance.SetTaskComplete (TutorialTask.ENTER_PORTAL);
		}
	}
}
