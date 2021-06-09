using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer), typeof (Animator))]
public class AnimationComponent : CreatureComponent {

    protected SpriteRenderer sprite;
    protected Animator animator;

    readonly AnimationHashID attackAnimID = new AnimationHashID("Attack");
    readonly AnimationHashID hitAnimID = new AnimationHashID("Hit");
    readonly AnimationHashID deathAnimID = new AnimationHashID("Death");
    readonly AnimationHashID snapOutAnimID = new AnimationHashID("SnapOut");

    #region Unity Callbacks
    protected void OnDisable () {
        InterruptCurrentAnimation ();
    }
    #endregion
    #region PlayerComponent Callback/Overrides
    public override void SetUpComponent (GameObject rootObject) {
        base.SetUpComponent (rootObject);
        sprite = GetComponent<SpriteRenderer> ();
        animator = GetComponent<Animator> ();
    }
    public override void SubscribeToCreatureEvents () {
        Debug.Log ("Animation Controller subscribing to events");
        base.SubscribeToCreatureEvents ();
        creatureObject.OnSetFaceDirEvent += OnSetFaceDirection;
        creatureObject.OnSetVelocityEvent += OnSetVelocity;

    }
    public override void UnsubscribeFromCreatureEvents () {
        base.UnsubscribeFromCreatureEvents ();
        creatureObject.OnSetFaceDirEvent -= OnSetFaceDirection;
        creatureObject.OnSetVelocityEvent -= OnSetVelocity;

    }
    public override void OnSpawn (Vector3 spawnPosition) {
        base.OnSpawn (spawnPosition);
        StopCurrentAnimation ();
    }
    public override void OnDeath () {
        Debug.Log("animationComponent onDeath subscriber occurred");
        InterruptCurrentAnimation ();
        animator.Play (deathAnimID.hashId);
        base.OnDeath ();//unsubs from events.
    }

    public override void OnAttack (AttackInfo attackInfo) {
        if (!creatureObject.isDead) {
            PlayTimedAnimation (attackAnimID, attackInfo.attackTime);
        }
    }
    public override void OnHit (HitInfo hitInfo) {
        if (!creatureObject.isDead) {
            PlayTimedAnimation (hitAnimID, hitInfo.damage / 80);
        }
    }
    public void OnSetVelocity (Vector2 velocity) {
        //for animation purposes, we want to undo the
        //isometric scaling applied to the velocity from MovementController
        Vector2 isoVelocity = new Vector2 (velocity.x, velocity.y * 2);
        float speed = isoVelocity.magnitude;
        animator.SetFloat ("speed", speed);//returns to idle/walk
    }
    public void OnSetFaceDirection (int faceDirection) {
        sprite.flipX = (faceDirection < 0);
    }
    #endregion

    #region public methods

    //returns to walk or idle
    public void StopCurrentAnimation () {
        StopAllCoroutines ();
        sprite.color = new Color (1, 1, 1, 1);
        animator.Play (snapOutAnimID.hashId);

    }
    //interrupt to display another animation.
    public void InterruptCurrentAnimation () {
        StopAllCoroutines ();
        sprite.color = new Color (1, 1, 1, 1);
    }
    //For everything outside of idle and walking
    public void PlayTimedAnimation (AnimationHashID animID, float animationTime) {
        InterruptCurrentAnimation ();//
        StartCoroutine (PlayTimedAnimationRoutine (animID, animationTime));
	}

    public IEnumerator PlayTimedAnimationRoutine (AnimationHashID animID, float animationLength) {
        animator.Play (animID.hashId);
        Debug.Log ("playing animation " + animID + ". waiting for " + animationLength + " seconds.");
        yield return new WaitForSeconds (animationLength);
        Debug.Log ("snapping out of animation " + animID);
        StopCurrentAnimation ();//returns to walk or idle
    }
 
	#endregion



}