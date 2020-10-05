using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement {
	[RequireComponent (typeof (MovementController))]

	public class Player : MonoBehaviour {
		public float moveSpeed = 6f;
		[Tooltip ("Scaling amount used to adjust the speeds on different axis.")]
		public Vector2 velocityScaling = new Vector2 (1f, 1f);
		[Range (0.01f, 0.9f)]
		public float acceleration = 0.1f;
		public float dashSpeedMultiplier = 4f;
		[Tooltip ("In seconds.")]
		[Range (0.01f, 1f)] public float dashDuration = 0.2f;

		public SpriteRenderer sprite;

		private Vector2 velocity;
		private Vector2 velocitySmoothing;
		private float speedMultiplier = 1f;

        private MovementController movementController;
        private AnimationController animationController;
        private Animator animator;

		private Vector2 directionalInput;

		private bool isDead;
		private bool canWalk = true;
		private bool canCast = true;
		private bool isDashing;

		private List<DebuffInfo> debuffs;

		private void Start () {
			movementController = GetComponent<MovementController> ();
			animator = GetComponent<Animator> ();
			debuffs = new List<DebuffInfo> ();
		}

		private void FixedUpdate () {
			if (isDead)
				return;

			CalculateSpeedMultipliers ();

			if (!canWalk)
				return;

			CalculateVelocity ();

			movementController.Move (velocity * Time.fixedDeltaTime, directionalInput);

			if (animator != null) {
				if (directionalInput != Vector2.zero && movementController.collisions.moveAmountOld != Vector2.zero) {
					animator.SetBool ("isWalking", true);
				} else {
					animator.SetBool ("isWalking", false);
				}
			}
		}

		public void SetDirectionalInput (Vector2 input, Vector2 cursorDirection) {
			directionalInput = input;
			if (sprite != null && !isDashing)
				sprite.flipX = (Mathf.Sign (cursorDirection.x) == -1);
		}

		public void OnDashInputDown () {
			if (!isDashing) {
				isDashing = true;
				StartCoroutine (ResetIsDashing (dashDuration));

				if (sprite != null)
					sprite.flipX = (Mathf.Sign (movementController.collisions.faceDirectionX) == -1);

				if (animator != null)
					animator.SetTrigger ("rollDodge");
			}
		}

		public void AddImpulseForce (Vector2 direction, float force) {
			velocity = direction.normalized * force;
			if (animator != null) {
				animator.SetTrigger ("hit");
				if (sprite != null)
					sprite.flipX = (Mathf.Sign (direction.x) == 1);
			}
		}

		public void AddDebuff (DebuffInfo debuffInfo) {
			debuffs.Add (debuffInfo);
		}

		private void CalculateSpeedMultipliers () {
			canWalk = true;
			canCast = true;
			speedMultiplier = 1f;
			for (int i = 0; i < debuffs.Count; i++) {
				if (debuffs[i].speedMultiplier == 0)
					canWalk = false;
				if (!debuffs[i].canCast)
					canCast = false;

				speedMultiplier += debuffs[i].speedMultiplier - 1f;
				debuffs[i].timeRemaining -= Time.fixedDeltaTime;
			}

			debuffs.RemoveAll (debuff => debuff.timeRemaining <= 0f);
		}

		private void CalculateVelocity () {
			Vector2 normalizedInput = directionalInput;
			if (normalizedInput.sqrMagnitude > 1f)
				normalizedInput = normalizedInput.normalized;

			Vector2 targetVelocity = normalizedInput * (moveSpeed * speedMultiplier * velocityScaling);

			if (isDashing) {
				targetVelocity *= dashSpeedMultiplier;
			}

			velocity = Vector2.SmoothDamp (velocity, targetVelocity, ref velocitySmoothing, acceleration);
		}

		private IEnumerator ResetIsDashing (float duration) {
			yield return new WaitForSeconds (duration);
			isDashing = false;
		}
	}
}

public class DebuffInfo {

	public float timeRemaining;
	public float speedMultiplier;
	public bool canCast;

	public DebuffInfo (float timeRemaining, float speedMultiplier, bool canCast) {
		this.timeRemaining = timeRemaining;
		this.speedMultiplier = speedMultiplier;
		this.canCast = canCast;
	}
}