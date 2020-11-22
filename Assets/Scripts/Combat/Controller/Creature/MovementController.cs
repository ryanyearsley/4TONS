using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement {
	[RequireComponent (typeof (Rigidbody2D), typeof (BoxCollider2D))]
	public class MovementController : MonoBehaviour {

		[SerializeField]
		protected float moveSpeed = 4f;
		[Tooltip ("Scaling amount used to adjust the speeds on different axis.")]
		public Vector2 isometricScaling = new Vector2 (1f, 0.5f);
		[Range (0.01f, 0.9f)]
		public float acceleration = 0.1f;
		private Vector2 velocitySmoothing;

		protected Vector2 velocity;
		private Rigidbody2D rb;
		protected float debuffSpeedMultiplier = 1f;

		public new BoxCollider2D collider { get; private set; }

		public int faceDirection { get; private set; }


		protected AbstractStateController stateController;
		private List<DebuffInfo> debuffs = new List<DebuffInfo>();

		protected virtual void Awake () {
			rb = GetComponent<Rigidbody2D> ();
			collider = GetComponent<BoxCollider2D> ();
			stateController = GetComponent<AbstractStateController> ();
		}

		private void OnEnable () {
			stateController.OnAddDebuffEvent += AddDebuff;
		}

		private void OnDisable () {
			stateController.OnAddDebuffEvent -= AddDebuff;
		}

		public void MoveWithDebuffCalculate (Vector2 velocity) {
			this.velocity = CalculateVelocity (velocity);


		}

		public void Move (Vector2 velocity) {
			this.velocity = velocity;
		}
		public void AddDebuff (DebuffInfo debuffInfo) {
			if (!stateController.isDead)
				debuffs.Add (debuffInfo);
		}

		protected virtual Vector2 CalculateVelocity (Vector2 direction) {
			debuffSpeedMultiplier = CalculateSpeedMultipliers ();
			Vector2 normalizedInput = (direction.sqrMagnitude > 1f)
				? direction.normalized
				: direction;

			Vector2 targetVelocity = normalizedInput * AggregateMovementModifiers();
			Vector2 smoothedVelocity = Vector2.SmoothDamp (velocity, targetVelocity, ref velocitySmoothing, acceleration);

			stateController.SetVelocity (
				(!stateController.canWalk)
				? Vector2.zero
				: smoothedVelocity * debuffSpeedMultiplier);
			return smoothedVelocity;
		}

		protected virtual Vector2 AggregateMovementModifiers() {
			return moveSpeed * debuffSpeedMultiplier * isometricScaling;
		}

		protected float CalculateSpeedMultipliers () {
			bool canWalk = true;
			bool canCast = true;
			debuffSpeedMultiplier = 1f;
			for (int i = 0; i < debuffs.Count; i++) {
				if (debuffs [i].speedMultiplier == 0)
					canWalk = false;
				if (!debuffs [i].canCast)
					canCast = false;

				debuffSpeedMultiplier += debuffs [i].speedMultiplier - 1f;
				debuffSpeedMultiplier = Mathf.Clamp (debuffSpeedMultiplier, 0f, float.MaxValue);
				debuffs [i].timeRemaining -= Time.deltaTime;
			}
			stateController.SetCanWalk (canWalk);
			stateController.SetCanAttack (canCast);
			debuffs.RemoveAll (debuff => debuff.timeRemaining <= 0f);
			return debuffSpeedMultiplier;
		}

		void FixedUpdate () {
			if (velocity == Vector2.zero)
				return;

			faceDirection = (int)Mathf.Sign (velocity.x);
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}

	}
}