using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelSpell : Spell
{
	public override void SpellButtonHold () {
		if (isCastEligible ()) {
			CastSpell ();
		}
	}
	public override void SpellButtonUp () {

	}

	public override void EndSpell () {
		playerObject.OnEndSpell (this);
	}
}
