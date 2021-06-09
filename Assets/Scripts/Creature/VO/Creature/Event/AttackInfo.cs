using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInfo
{
	public float attackTime;
	public float attackSpeedModifier;
	public SpellData spellData;
	public AttackInfo(float attackTime, float attackSpeedModifier) {
		this.attackTime = attackTime;
		this.attackSpeedModifier = attackSpeedModifier;
	}
	public AttackInfo (float attackTime, float attackSpeedModifier, SpellData spellData) {
		this.attackTime = attackTime;
		this.attackSpeedModifier = attackSpeedModifier;
		this.spellData = spellData;
	}
}
