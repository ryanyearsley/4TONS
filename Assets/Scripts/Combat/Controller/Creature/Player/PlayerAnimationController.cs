using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : AnimationController {

    PlayerStateController playerStateController;
    public void OnDash (DashInfo dashInfo) {
        StartCoroutine (DashRoutine (dashInfo));
    }


    public void OnCastSpell (SpellData spellData) {
        if (!stateController.isDead) {
            //cast animation trigger
        }
    }

    private IEnumerator DashRoutine (DashInfo dashInfo) {
        if (!stateController.isDead)
            animator.SetTrigger ("rollDodge");
        sprite.color = new Color (1, 1, 1, 0.5f);
        yield return new WaitForSeconds (dashInfo.invulnerableTime);
        sprite.color = new Color (1, 1, 1, 1);
    }

	protected override void OnEnable () {
		base.OnEnable ();
        playerStateController = GetComponentInParent<PlayerStateController> ();
        playerStateController.OnDashEvent += OnDash;
        playerStateController.OnCastSpellEvent += OnCastSpell;
    }
}
