using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpell : Spell
{


    public override void CastSpell()
    {
        base.CastSpell();

        PoolManager.instance.ReuseObject (spellData.spellObject, spellCastTransform.position, spellCastTransform.rotation, this.tag);
        
    }
}
