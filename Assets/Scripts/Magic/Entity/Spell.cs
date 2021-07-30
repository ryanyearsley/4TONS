﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellCastLocation {
	Staff, Cursor, Player, Melee
}

public abstract class Spell : MonoBehaviour {

	protected PlayerObject playerObject;

	//static
	public SpellData spellData;

	//dynamic
	public bool onCooldown;

	protected Transform spellCastTransform;
	protected float cdTimer;
	public SpellUI spellUI;

	void Start () {
		SetUpSpell ();
	}


	public virtual void SetUpSpell () {
		PoolManager.instance.CreateSpellObjectPool (spellData.spellObject, spellData.poolSize);
	}

	public void ConfigureSpellToPlayer (PlayerObject playerObject) {
		this.playerObject = playerObject;
		transform.parent = playerObject.transform;
		switch (spellData.spellCastLocation) {
			case SpellCastLocation.Staff:
				spellCastTransform = playerObject.creaturePositions.staffAimTransform;
				break;
			case SpellCastLocation.Cursor:
				spellCastTransform = playerObject.creaturePositions.targetTransform;
				break;
			case SpellCastLocation.Player:
				spellCastTransform = playerObject.creaturePositions.feetTransform;
				break;
		}
	}

	private void Update () {
		if (onCooldown) {
			cdTimer -= Time.deltaTime;

			if (cdTimer <= 0) {
				onCooldown = false;
			}

			if (spellUI != null) {
				float fillPercentage = 1 - cdTimer/spellData.coolDown;
				spellUI.UpdateSpellUICooldown (fillPercentage, cdTimer);
			}
		}
	}

	public void OnBindSpell () {

	}

	public virtual bool isCastEligible () {
		if (onCooldown
			|| !playerObject.canAttack
			|| !playerObject.vitalsEntity.resource.HasEnoughMana (spellData.manaCost)
			)
			return false;
		return true;
	}

	public virtual void SpellButtonDown () {
		if (isCastEligible ()) {
			CastSpell ();
		}
	}
	public virtual void SpellButtonHold () {
	}
	public virtual void SpellButtonUp () {
	}


	public virtual void CastSpell () {
		playerObject.vitalsEntity.resource.SubtractResourceCost (spellData.manaCost);
		cdTimer = spellData.coolDown;
		spellUI.GreyOutSpellUI ();
		onCooldown = true;
		playerObject.OnAttack (new AttackInfo (spellData.castTime, spellData.castSpeedReduction, spellData));
		playerObject.AddSpeedEffect (new SpeedAlteringEffect (spellData.castSpeedReduction, spellData.castTime, false));

	}
	public virtual void ChannelSpell () {

	}
	public virtual void EndSpell () {
	}

}
