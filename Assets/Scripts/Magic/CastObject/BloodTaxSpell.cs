using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodTaxSpell : Spell
{
    public float healthCost;
    public float manaRegenAmount;
    public override void CastSpell () {
        base.CastSpell ();
        playerObject.vitalsEntity.health.ApplyDamage (healthCost);
        playerObject.vitalsEntity.resource.RegenerateMana (manaRegenAmount);
        if (spellData.spellObject != null) {
            PoolManager.instance.ReuseSpellObject (spellData.spellObject, playerObject.vitalsEntity.creatureObject.transform.position, Quaternion.identity, playerObject.vitalsEntity);
        }
    }
}
