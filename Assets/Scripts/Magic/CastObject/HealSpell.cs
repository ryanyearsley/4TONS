using UnityEngine;

public class HealSpell : Spell
{
    public float healAmount;
    public float manaRegenAmount;
    public override void CastSpell () {
        base.CastSpell ();
        playerObject.vitalsEntity.health.Heal (healAmount);
        playerObject.vitalsEntity.resource.RegenerateMana (manaRegenAmount);
        //PoolManager.instance.ReuseSpellObject (spellData.spellObject, spellCastTransform.position, Quaternion.identity, playerObject.vitalsEntity);
    }
}
