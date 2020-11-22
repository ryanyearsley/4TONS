using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement {
	[RequireComponent (typeof (MovementController), typeof (PlayerStateController))]

	public class PlayerController : MonoBehaviour {

		public float moveSpeed = 6f;
		[Tooltip ("Scaling amount used to adjust the speeds on different axis.")]
		public Vector2 velocityScaling = new Vector2 (1f, 0.5f);
		[Range (0.01f, 0.9f)]
		public float acceleration = 0.1f;
		public float dashSpeedMultiplier = 4f;
		[Tooltip ("In seconds.")]
		[Range (0.01f, 1f)] public float dashDuration = 0.2f;

		private Vector3 spawnPosition;
		private Vector2 velocity;
		private Vector2 velocitySmoothing;
		private float speedMultiplier = 1f;

		private MovementController movementController;
		private PlayerStateController stateController;

		private Vector2 directionalInput;

		private bool isDashing;

		private List<DebuffInfo> debuffs;

		private void Start () {
			spawnPosition = transform.position;
			movementController = GetComponent<MovementController> ();
			debuffs = new List<DebuffInfo> ();
		}

		private void Update () {
			if (stateController.isDead)
				return;

			CalculateSpeedMultipliers ();

			if (!stateController.canWalk)
				directionalInput = Vector2.zero;

			CalculateVelocity ();
			movementController.Move (velocity);

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
				//stateController.OnDash ();
			}
		}

		public void AddImpulseForce (Vector2 direction, float force) {
			velocity = direction.normalized * force;
			stateController.SetFaceDirection ((int)Mathf.Sign (direction.x));
			stateController.OnHit (new OnHitInfo(stateController.Health, direction, stateController.Health));
		}


		public void AddDebuff (DebuffInfo debuffInfo) {
			if (!stateController.isDead)
				debuffs.Add (debuffInfo);
		}

		public void OnRespawn (Vector3 spawnPosition) {
			transform.position = spawnPosition;
		}

		private void CalculateSpeedMultipliers () {
			bool canWalk = true;
			bool canCast = true;
			speedMultiplier = 1f;
			for (int i = 0; i < debuffs.Count; i++) {
				if (debuffs [i].speedMultiplier == 0)
					canWalk = false;
				if (!debuffs [i].canCast)
					canCast = false;

				speedMultiplier += debuffs [i].speedMultiplier - 1f;
				speedMultiplier = Mathf.Clamp (speedMultiplier, 0f, float.MaxValue);
				debuffs [i].timeRemaining -= Time.deltaTime;
			}
			stateController.SetCanWalk (canWalk);
			stateController.SetCanAttack (canCast);
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
			stateController.OnRespawnEvent += OnRespawn;
		}

		private void OnDisable () {
			stateController.OnAddDebuffEvent -= AddDebuff;
			stateController.OnRespawnEvent -= OnRespawn;
		}
	}
}