using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    private SpriteRenderer playerSprite;

    private PlayerStateController stateController;
    private Animator animator;

    private void Start () {
        playerSprite = GetComponent<SpriteRenderer> ();
        animator = GetComponent<Animator> ();
    }

    public void OnDeath () {
        animator.SetBool("isDead", true);
        //animator.SetTrigger ("die");
    }

    public void OnRespawn () {
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

    public void OnHit (Vector2 direction) {
        if (!stateController.isDead)
        {
            playerSprite.flipX = (Mathf.Sign(direction.x) > 0);
            animator.SetTrigger("hit");
        }
    }

    private void OnEnable () {
        stateController = GetComponent<PlayerStateController> ();
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