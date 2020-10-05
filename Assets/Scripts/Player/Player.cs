﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement {
	[RequireComponent (typeof (MovementController), typeof (PlayerStateController))]

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
		private PlayerStateController stateController;

		private Vector2 directionalInput;

		private bool isDashing;

		private List<DebuffInfo> debuffs;

		private void Start () {
			movementController = GetComponent<MovementController> ();
			debuffs = new List<DebuffInfo> ();
		}

		private void Update () {
			if (stateController.isDead)
				return;

			CalculateSpeedMultipliers ();

			if (Input.GetKeyDown (KeyCode.E))
				stateController.AddDebuff (new DebuffInfo (5f, 0.5f, false));

			if (!stateController.canWalk)
				directionalInput = Vector2.zero;

			CalculateVelocity ();

			movementController.Move (velocity);

			stateController.SetFeetPosition (transform.position);
			stateController.SetVelocity (
				(!stateController.canWalk)
				? Vector2.zero
				: directionalInput * speedMultiplier);
		}

		public void SetDirectionalInput (Vector2 input, Vector2 cursorDirection) {
			directionalInput = input;
			if (!isDashing)
				stateController.SetFaceDirection ((int)Mathf.Sign (cursorDirection.x));
		}

		public void OnDashInputDown () {
			if (!isDashing) {
				isDashing = true;
				StartCoroutine (ResetIsDashing (dashDuration));
				stateController.SetFaceDirection ((int)Mathf.Sign (movementController.faceDirection));
				stateController.OnDash ();
			}
		}

		public void AddImpulseForce (Vector2 direction, float force) {
			velocity = direction.normalized * force;
			stateController.SetFaceDirection ((int)Mathf.Sign (direction.x));
			stateController.OnHit (direction);
		}
		

		public void AddDebuff (DebuffInfo debuffInfo) {
			if (!stateController.isDead)
				debuffs.Add (debuffInfo);
		}

		private void CalculateSpeedMultipliers () {
			bool canWalk = true;
			bool canCast = true;
			speedMultiplier = 1f;
			for (int i = 0; i < debuffs.Count; i++) {
				if (debuffs[i].speedMultiplier == 0)
					canWalk = false;
				if (!debuffs[i].canCast)
					canCast = false;

				speedMultiplier += debuffs[i].speedMultiplier - 1f;
				speedMultiplier = Mathf.Clamp (speedMultiplier, 0f, float.MaxValue);
				debuffs[i].timeRemaining -= Time.deltaTime;
			}
			stateController.SetCanWalk (canWalk);
			stateController.SetCanCast (canCast);
			debuffs.RemoveAll (debuff => debuff.timeRemaining <= 0f);
		}

		private void CalculateVelocity () {
			Vector2 normalizedInput = (directionalInput.sqrMagnitude > 1f)
				? directionalInput.normalized
				: directionalInput;

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

		private void OnEnable () {
			stateController = GetComponent<PlayerStateController> ();
			stateController.OnAddDebuffEvent += AddDebuff;
		}

		private void OnDisable () {
			stateController.OnAddDebuffEvent -= AddDebuff;
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