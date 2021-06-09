using System;
using UnityEngine;
public class BabyBrainsObject : CreatureObject {
	/*Basic state machine management/flow for enemy AI
	1. Polls for enemy targets within x range (IDLE)
	2. Once within pursuit range, pursues target on foot (PURSUIT)
	3. Once iwthin attack range, attacks target at Y interval (ATTACK)
	4. Dies (DEAD)
	 */

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
