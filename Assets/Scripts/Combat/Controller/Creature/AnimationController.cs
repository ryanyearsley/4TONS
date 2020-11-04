using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer), typeof (Animator))]
public class AnimationController : MonoBehaviour {

    private PlayerStateController stateController;

    private SpriteRenderer playerSprite;
    private Animator animator;

    private void Awake () {
        playerSprite = GetComponent<SpriteRenderer> ();
        animator = GetComponent<Animator> ();
    }
    public void InitializeComponent (Player player) {
        Debug.Log ("Initializing Animation Controller");
        animator.runtimeAnimatorController = player.currentWizard.spellSchoolData.animatorController;
    }

    public void OnDeath () {
        animator.SetBool("isDead", true);
        //animator.SetTrigger ("die");
    }

    public void OnRespawn (Vector3 spawnPosition) {
        animator.Rebind ();
    }

    public void OnDash () {
        if (!stateController.isDead)
        animator.SetTrigger ("rollDodge");
    }

    public void OnSetFaceDirection (int faceDirection) {
        playerSprite.flipX = (faceDirection < 0);
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
            playerSprite.flipX = (Mathf.Sign(onHitInfo.direction.x) > 0);
            animator.SetTrigger("hit");
        }
    }

    private void OnEnable () {
        stateController = transform.parent.GetComponent<PlayerStateController> ();
        stateController.OnDeathEvent += OnDeath;
        stateController.OnRespawnEvent += OnRespawn;
        stateController.OnDashEvent += OnDash;
        stateController.OnSetFaceDirEvent += OnSetFaceDirection;
        stateController.OnSetVelocityEvent += OnSetVelocity;
        stateController.OnHitEvent += OnHit;
    }

    private void OnDisable () {
        stateController.OnDeathEvent -= OnDeath;
        stateController.OnRespawnEvent -= OnRespawn;
        stateController.OnDashEvent -= OnDash;
        stateController.OnSetFaceDirEvent -= OnSetFaceDirection;
        stateController.OnSetVelocityEvent -= OnSetVelocity;
        stateController.OnHitEvent -= OnHit;
    }
}