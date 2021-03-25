using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    FIRE, ICE, LIGHT, DARK
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell Object Data", order = 1)]
public class SpellObjectData : ScriptableObject
{
    public float lifeTime;
    public float damage;
    public float manaDamage;
    public float enemySpeedReduction;
    public float casterHealthHealAmount;
    public float casterManaRegenAmount;
    public float casterSpeedIncrease;
    public DamageType damageType;
}