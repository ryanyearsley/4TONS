using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticProjectileSpell : ProjectileSpell
{
    public override void SpellButtonHold () {
        if (isCastEligible ()) {
            CastSpell ();
        }
    }

}
