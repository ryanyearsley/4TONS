using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritWalkSpell : CastOnReleaseSpell
{
	public override void ConfigureSpellToPlayer (PlayerObject playerObject) {
		base.ConfigureSpellToPlayer (playerObject);
		previewObjectSprite.sprite = playerObject.wizardGameData.spellSchoolData.wizardData.spectralPreview;
	}

	public override void CastSpell () {
		base.CastSpell ();
		playerObject.transform.position = new Vector3(spellCastTransform.position.x, spellCastTransform.position.y, 0);
	}
}