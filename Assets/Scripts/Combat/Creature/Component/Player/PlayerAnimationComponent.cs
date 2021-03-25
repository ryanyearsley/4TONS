using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationComponent : PlayerComponent {

    private AnimationComponent animationComponent;
    private Animator animator;
    private SpriteRenderer sprite;

    readonly AnimationHashID dashAnimID = new AnimationHashID("Dash");



	public override void SetUpComponent (GameObject rootObject) {
        base.SetUpComponent (rootObject);
        animationComponent = GetComponent<AnimationComponent> ();
        animator = GetComponent<Animator> ();
        sprite = GetComponent<SpriteRenderer> ();

    }
	public override void ReusePlayerComponent (Player player) {
        animator.runtimeAnimatorController = player.wizardSaveData.spellSchoolData.animatorController;

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
