using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnObject : PoolObject {



	private Animator animator;

	public override void SetupObject () {
		animator = GetComponentInChildren<Animator> ();
	}

	public override void ReuseObject () {
		base.ReuseObject ();
	}
	public override void TerminateObjectFunctions () {
	}

	public void OnBeginLevel (int levelIndex) {
		animator.SetTrigger ("open");
	}
}