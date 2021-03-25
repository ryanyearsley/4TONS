using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnController : PoolObject {

	private Animator animator;

	public override void SetupObject () {
		animator = GetComponentInChildren<Animator> ();
	}

	public override void ReuseObject () {
		GameManager.instance.beginLevelEvent += OnBeginLevel;
	}
	public override void TerminateObjectFunctions () {
		GameManager.instance.beginLevelEvent -= OnBeginLevel;
	}

	public void OnBeginLevel (int levelIndex) {
		animator.SetTrigger ("open");
	}
}