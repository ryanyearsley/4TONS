using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour {

	private PlayerStateController stateController;
    private ManaController manaController;
	private bool canCast;

    AbstractSpell[] spells;

    public void OnSpellButtonDown(int spellIndex)
    {
        if (!canCast)
            return;

        PlayerPositions positions = stateController.playerPositions;
        AbstractSpell spell = spells[spellIndex];
        if (spell.onCooldown && 
            manaController.SubtractManaCost(spell.manaCost))
        {
            spell.CastSpell();
            stateController.AddDebuff(new DebuffInfo(0.5f, 0.5f, false));
        }
    }

    public void OnSpellButton (int spellIndex) {
        //channel spell
		if (!canCast)
			return;

		PlayerPositions positions = stateController.playerPositions;
	}

	public void OnSpellButtonUp (int spellIndex) {
		if (!canCast)
			return;

		PlayerPositions positions = stateController.playerPositions;
	}

	public void SetCanCast (bool canCast) {
		this.canCast = canCast;
	}

	private void OnEnable () {
		stateController = GetComponent<PlayerStateController> ();
		stateController.OnSetCanCastEvent += SetCanCast;
	}

	private void OnDisable () {
		stateController.OnSetCanCastEvent -= SetCanCast;
	}
}