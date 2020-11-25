using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer), typeof (Animator))]
public class AnimationController : MonoBehaviour {

    protected AbstractStateController stateController;

    protected SpriteRenderer sprite;
    protected Animator animator;

    private void Awake () {
        sprite = GetComponent<SpriteRenderer> ();
        animator = GetComponent<Animator> ();
    }
    public void InitializeComponent (Player player) {
        animator.runtimeAnimatorController = player.currentWizard.spellSchoolData.animatorController;
    }

    public void OnDeath () {
        animator.SetBool("isDead", true);
        //animator.SetTrigger ("die");
    }

    public void OnRespawn (Vector3 spawnPosition) {
        animator.Rebind ();
    }

    public void OnSetFaceDirection (int faceDirection) {
        sprite.flipX = (faceDirection < 0);
    }

    public void OnSetVelocity (Vector2 velocity) {
        if (velocity == Vector2.zero)
            animator.SetBool ("isWalking", false);
        else
            animator.SetBool ("isWalking", true);
    }

    public void OnHit (OnHitInfo onHitInfo) {
        if (!stateController.isDead)
        {
            sprite.flipX = (Mathf.Sign(onHitInfo.direction.x) > 0);
            animator.SetTrigger("hit");
        }
    }


    protected virtual void OnEnable () {
        stateController = transform.parent.GetComponent<AbstractStateController> ();
        stateController.OnDeathEvent += OnDeath;
        stateController.OnRespawnEvent += OnRespawn;
        stateController.OnSetFaceDirEvent += OnSetFaceDirection;
        stateController.OnSetVelocityEvent += OnSetVelocity;
        stateController.OnHitEvent += OnHit;
    }

    private void OnDisable () {
        stateController.OnDeathEvent -= OnDeath;
        stateController.OnRespawnEvent -= OnRespawn;
        stateController.OnSetFaceDirEvent -= OnSetFaceDirection;
        stateController.OnSetVelocityEvent -= OnSetVelocity;
        stateController.OnHitEvent -= OnHit;
    }
}