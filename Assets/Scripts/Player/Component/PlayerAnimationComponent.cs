using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationComponent : PlayerComponent {

    private AnimationComponent animationComponent;
    private Animator animator;
    private SpriteRenderer sprite;

    readonly AnimationHashID dashAnimID = new AnimationHashID("Dash");

    readonly AnimationHashID attackAnimID = new AnimationHashID("Attack");
    readonly AnimationHashID channelAnimID = new AnimationHashID("Channel");



    public override void SetUpComponent (GameObject rootObject) {
        base.SetUpComponent (rootObject);
        animationComponent = GetComponent<AnimationComponent> ();
        animator = GetComponent<Animator> ();
        sprite = GetComponent<SpriteRenderer> ();

    }
	public override void ReusePlayerComponent (Player player) {
        animator.runtimeAnimatorController = player.wizardSaveData.spellSchoolData.wizardData.animatorController;

    }

    public override void OnCastSpell (Spell spell, SpellCastType spellCastType) {
        if (spellCastType == SpellCastType.CAST)
            animationComponent.PlayTimedAnimation (attackAnimID, spell.spellData.castTime);
        else if (spellCastType == SpellCastType.CHANNEL) {
            animationComponent.PlayLoopingAnimation (channelAnimID);
        }
    }

    public override void OnEndSpell(Spell spell) {
        animationComponent.StopCurrentAnimation ();
    }
    public override void OnDash (DashInfo dashInfo) {
        animationComponent.PlayTimedAnimation (dashAnimID, dashInfo.animationTime);
        StartCoroutine (DashIFrameRoutine (dashInfo));
    }
    private IEnumerator DashIFrameRoutine (DashInfo dashInfo) {
        if (!creatureObject.isDead) {
            sprite.color = new Color (1, 1, 1, 0.5f);
            yield return new WaitForSeconds (dashInfo.invulnerableTime);
            sprite.color = new Color (1, 1, 1, 1);
        }
    }
}
