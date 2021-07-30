using System;
using UnityEngine;
public class BabyBrainsObject : CreatureObject {

	public ThinkComponent thinkComponent;
	BabyBrainsComponent[] babyBrainsComponents;
	BabyBrainsBehaviour[] babyBrainsBehaviours;

	public BabyBrainsState currentEnemyState; 

	public event Action<BabyBrainsState> OnChangeBabyBrainsEvent;

	public override void SetupObject () {
		base.SetupObject ();
		thinkComponent = GetComponent<ThinkComponent> ();
		babyBrainsComponents = GetComponentsInChildren<BabyBrainsComponent> ();
		babyBrainsBehaviours = GetComponentsInChildren<BabyBrainsBehaviour> ();
	}

	public void OnChangeBabyBrainsState (BabyBrainsState enemyState) {
		currentEnemyState = enemyState;
		OnChangeBabyBrainsEvent?.Invoke (currentEnemyState);
	}

	public override void OnSpawn (Vector3 spawnPosition) {
		Debug.Log ("Baby Brains object OnSpawn event");
		OnChangeBabyBrainsState (BabyBrainsState.IDLE);
		base.OnSpawn (spawnPosition);
	}
	public override void OnDeath () {
		base.OnDeath ();
		OnChangeBabyBrainsState (BabyBrainsState.DEAD);
	}
}

public enum BabyBrainsState {	
	DISABLED, IDLE, PURSUIT, ATTACK, DEAD
}
