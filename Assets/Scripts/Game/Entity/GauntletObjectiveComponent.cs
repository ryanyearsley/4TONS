using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Listens to game events and acts accordingly.
[RequireComponent (typeof (CreatureObject))]
public class GauntletObjectiveComponent : CreatureComponent {

	public override void OnSpawn (Vector3 spawnpoint) {
		if (GauntletGameManager.instance != null) {
			GauntletGameManager.instance.RegisterEnemy (this);
			Debug.Log ("GauntletObjectiveComponent: Registering object " + gameObject.GetInstanceID ());
		}
	}
	public override void OnDeath () {
		Debug.Log ("Gauntlet enemy death reported.");
		if (GauntletGameManager.instance != null) {

			GauntletGameManager.instance.ReportEnemyDeath (this);
			Debug.Log ("GauntletObjectiveComponent: Reporting death. object " + gameObject.GetInstanceID ());
		}
		else if (TutorialManager.instance != null) {
			TutorialManager.instance.SetTaskComplete (TutorialTask.KILL_ENEMY);
		}
		base.OnDeath ();
	}
}
