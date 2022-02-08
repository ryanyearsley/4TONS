using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Listens to game events and acts accordingly.
[RequireComponent (typeof (CreatureObject))]
public class GauntletObjectiveComponent : CreatureComponent {

	public override void OnSpawn (Vector3 spawnpoint) {
		if (GauntletGameManager.instance != null) {
			GauntletGameManager.instance.RegisterEnemy (this);
		}
	}
	public override void OnDeath () {
		if (GauntletGameManager.instance != null) {
			GauntletGameManager.instance.ReportEnemyDeath (this);
		}
		if (TutorialManager.instance != null) {
			TutorialManager.instance.SetTaskComplete (TutorialTask.KILL_ENEMY);
		}
		base.OnDeath ();
	}
}
