using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpell : Spell
{
    [SerializeField]
    private bool isAutomatic = false;


    public override void CastSpell()
    {
        base.CastSpell ();
        PoolManager.instance.ReuseSpellObject (spellData.spellObject, spellCastTransform.position, spellCastTransform.rotation, playerObject.vitalsEntity);
        
    }
}
