using System;
using UnityEngine;

public class SpellObject : PoolObject, ISpellObject {

	[SerializeField]
	protected float lifeTime = 5f;
	[SerializeField]
	protected float lifeTimer;
	[SerializeField]
	protected SpellEffect[] spellEffects;

	[NonSerialized]
	public VitalsEntity casterVitalsEntity;
	protected bool isAlive;



	protected Transform trans;
	private Sound spellObjectSound;
	private Sound spellObjectDestroySound;

	public override void SetupObject () {
		base.SetupObject ();

		trans = this.transform;
		spellEffects = trans.GetComponentsInChildren<SpellEffect> ();
		if (spellObjectSound != null)
			AudioManager.instance.RegisterSound (spellObjectSound);
		if (spellObjectDestroySound != null)
			AudioManager.instance.RegisterSound (spellObjectDestroySound);
	}

	public virtual void SetSpellObjectTag (VitalsEntity vitalsEntity) {
		casterVitalsEntity = vitalsEntity;
		if (vitalsEntity.tag != this.tag) {
			this.tag = vitalsEntity.tag;
		}
	}


	public virtual void ReuseSpellObject (VitalsEntity vitalsEntity) {
		casterVitalsEntity = vitalsEntity;
		if (spellObjectSound != null)
			AudioManager.instance.PlaySound (spellObjectSound.clipName);
		lifeTimer = 0;
		isAlive = true;
	}

	public virtual void OnWallHit () {
	}
	public virtual void OnEnemyHit (VitalsEntity enemyVitals) {
		foreach (SpellEffect spellEffect in spellEffects) {
			spellEffect.OnEnemyHit (casterVitalsEntity, enemyVitals);
		}
	}
	public virtual void OnAllyHit (VitalsEntity ally) {
		foreach (SpellEffect spellEffect in spellEffects) {
			spellEffect.OnAllyHit (casterVitalsEntity, ally);
		}
	}

	public virtual void OnBarrierHit(BarrierObject barrierObject) {

	}

	public override void TerminateObjectFunctions () {
		isAlive = false;
		this.tag = "Untagged";

		//source.Stop ();
		if (spellObjectDestroySound != null) {
			AudioManager.instance.PlaySound (spellObjectDestroySound.clipName);
		}

	}
	public virtual void Update () {
		if (isAlive) {
			lifeTimer += Time.deltaTime;
			if (lifeTimer >= lifeTime) {
				print ("lifetime exceeded. object destroyed");
				Destroy ();
			}
		}
	}
}
