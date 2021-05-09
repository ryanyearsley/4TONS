using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Listens to game events and acts accordingly.
[RequireComponent (typeof (CreatureObject))]
public class GauntletObjectiveComponent : CreatureComponent {

	public override void OnDeath () {
		base.OnDeath ();
		Debug.Log ("Gauntlet enemy death reported.");
		if (GauntletGameManager.instance != null)
			GauntletGameManager.instance.ReportEnemyDeath (this);
		else if (TutorialManager.instance != null) {
			TutorialManager.instance.SetTaskComplete (TutorialTask.KILL_ENEMY);
		}
	}
}
