using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAFLTempleObject : InteractableObject {

	public override void InteractWithObject () {
		GameManager.instance.MakeDecision (0);
	}

	public override void UnhighlightInteractable () {
		GameManager.instance.ResumeCombat ();
	}
}
