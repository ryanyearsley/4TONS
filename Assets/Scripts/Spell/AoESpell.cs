using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoESpell : Spell
{
    public override void CastSpell () {
        base.CastSpell ();

        PoolManager.instance.ReuseObject (spellData.spellObject, new Vector3(spellCastTransform.position.x, spellCastTransform.position.y, 0f), Quaternion.identity, this.tag);

    }
}
