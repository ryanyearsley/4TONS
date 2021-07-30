using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class for every create/player that's going to participate in combat.
//Manages player events (Actions) 
//Tracks player state
public class CreatureObject : PoolObject {

	public CreatureData creatureData;

	public VitalsEntity vitalsEntity;

	
	public CreatureComponent[] creatureComponents;
	public CreaturePositions creaturePositions { get; private set; }
	public void SetCreaturePositions (Transform targetTransform, Transform feetTransform, Transform staffTransform) {
		creaturePositions = new CreaturePositions (targetTransform, feetTransform, staffTransform);
	}

	//Life Cycle
	public event Action<Vector3> OnSpawnEvent;

	public bool isDead { get; private set; }
	public event Action OnDeathEvent;

	//Game event

	//Setters
	public bool canWalk { get; private set; }
	public event Action<bool> OnSetCanWalkEvent;

	public bool canAttack { get; private set; }
	public event Action<bool> OnSetCanAttackEvent;

	public int faceDirection { get; private set; }
	public event Action<int> OnSetFaceDirEvent;

	public Vector2 velocity { get; private set; }
	public event Action<Vector2> OnSetVelocityEvent;

	//Combat
	public event Action<AttackInfo> OnAttackEvent;
	public event Action<HitInfo> OnHitEvent;
	public event Action<SpeedAlteringEffect> OnAddDebuffEvent;

	#region PoolObject Callbacks
	public override void SetupObject () {
		vitalsEntity = new VitalsEntity (this.gameObject);
		vitalsEntity.creatureData = this.creatureData;
		SetUpComponents ();
	}
	protected virtual void SetUpComponents() {
		creatureComponents = GetComponentsInChildren<CreatureComponent> ();
		for (int i = 0; i < creatureComponents.Length; i++) {
			creatureComponents [i].SetUpComponent (this.gameObject);
		}
	}
	public override void ReuseObject () {
		SubscribeToEvents ();
		for (int i = 0; i < creatureComponents.Length; i++) {
			creatureComponents [i].ReuseComponent ();//subscribe to events
		}

		OnSpawn (transform.position);//perform any init that requires event subs
	}
	public override void TerminateObjectFunctions() {
		UnsubscribeFromEvents ();
	}
	#endregion
	public virtual void SubscribeToEvents() {
		GameManager.instance.levelEndEvent += OnLevelEnd;
	}
	public virtual void UnsubscribeFromEvents() {
		GameManager.instance.levelEndEvent -= OnLevelEnd;
	}

	public virtual void OnLevelEnd (int levelIndex) {
		Destroy ();
	}

	#region CreatureObject Callbacks
	public virtual void OnSpawn (Vector3 spawnPosition) {
		isDead = false;
		vitalsEntity.EnableVitals ();
		OnSpawnEvent?.Invoke (spawnPosition);
	}
	public virtual void OnDeath () {
		if (isDead)
			return;

		isDead = true;
		vitalsEntity.DisableVitals ();
		OnDeathEvent?.Invoke ();
		Destroy (30);
	}

	public virtual void OnAttack (AttackInfo attackInfo) {
		OnAttackEvent?.Invoke (attackInfo);
		StartCoroutine (AttackRoutine (attackInfo));
	}

	public IEnumerator AttackRoutine(AttackInfo attackInfo) {
		SetCanAttack (false);
		yield return new WaitForSeconds (attackInfo.attackTime);
		SetCanAttack (true);
	}
	public virtual void OnHit (HitInfo onHitInfo) {
		OnHitEvent?.Invoke (onHitInfo);
	}
	public virtual void AddSpeedEffect (SpeedAlteringEffect speedEffect) {
		OnAddDebuffEvent?.Invoke (speedEffect);
	}
	#endregion
	#region Setters
	public virtual void SetCanWalk (bool canWalk) {
		this.canWalk = canWalk;
		OnSetCanWalkEvent?.Invoke (canWalk);
	}

	public virtual void SetCanAttack (bool canAttack) {
		this.canAttack = canAttack;
		OnSetCanAttackEvent?.Invoke (canAttack);
	}

	public virtual void SetFaceDirection (int faceDirection) {
		if (this.faceDirection == faceDirection) {
			return;//cancel event, this is old news.
		}
		Debug.Log (GetInstanceID() + "Flipping face direction");
		this.faceDirection = faceDirection;
		OnSetFaceDirEvent?.Invoke (faceDirection);
	}

	public virtual void SetVelocity (Vector2 velocity) {
		this.velocity = velocity;
		OnSetVelocityEvent?.Invoke (velocity);
	}

	#endregion
}
