using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Base class for all components used on both players and AI.
public class CreatureComponent : MonoBehaviour, ICreatureComponent
{
	protected CreatureObject creatureObject;

	#region CreatureComponent Callbacks
	//called during pool creation. 
	public virtual void SetUpComponent (GameObject rootObject) {
		creatureObject = rootObject.GetComponent<CreatureObject> ();
	}

	//Called when object is reused via the pooling system (before spawn is called).
	public virtual void ReuseComponent() {
		SubscribeToCreatureEvents ();
		SubscribeToGameEvents ();
	}
	//life cycle
	public virtual void OnSpawn (Vector3 spawnPosition) {
	}

	public virtual void OnDeath () {
		UnsubscribeFromCreatureEvents ();
	}

	//game events

	public virtual void OnBeginLevel (int levelIndex) {

	}
	public virtual void OnLevelEnd (int levelIndex) {
		UnsubscribeFromGameEvents ();
	}

	//Creature events

	public virtual void OnAttack (AttackInfo attackInfo) {

	}
	public virtual void OnAttackSpecial (AttackInfo attackInfo) {
	
	}
	public virtual void OnHit (HitInfo hitInfo) {

	}
	public virtual void OnAddDebuff (SpeedAlteringEffect sae) {

	}


	//subscriptions
	public virtual void SubscribeToCreatureEvents () {
		creatureObject.OnSpawnEvent += OnSpawn;
		creatureObject.OnDeathEvent += OnDeath;
		creatureObject.OnAttackEvent += OnAttack;
		creatureObject.OnAttackSpecialEvent += OnAttackSpecial;
		creatureObject.OnHitEvent += OnHit;
		creatureObject.OnAddDebuffEvent += OnAddDebuff;
	}

	public virtual void UnsubscribeFromCreatureEvents () {
		creatureObject.OnSpawnEvent -= OnSpawn;
		creatureObject.OnDeathEvent -= OnDeath;
		creatureObject.OnAttackEvent -= OnAttack;
		creatureObject.OnAttackSpecialEvent -= OnAttackSpecial;
		creatureObject.OnHitEvent -= OnHit;
		creatureObject.OnAddDebuffEvent -= OnAddDebuff;
	}
	public virtual void SubscribeToGameEvents() {
		GameManager.instance.beginLevelEvent += OnBeginLevel;
		GameManager.instance.levelEndEvent += OnLevelEnd;
	}
	public virtual void UnsubscribeFromGameEvents () {
		GameManager.instance.beginLevelEvent -= OnBeginLevel;
		GameManager.instance.levelEndEvent -= OnLevelEnd;

	}


	#endregion
}
