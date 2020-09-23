using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement {
	[RequireComponent (typeof (MovementController))]
	public class Player : MonoBehaviour {
		public float moveSpeed = 6f;
		[Range (0.01f, 0.9f)]
		public float acceleration = 0.1f;
		public float dashSpeedMultiplier = 4f;
		[Tooltip ("In seconds.")]
		[Range (0.01f, 1f)] public float dashDuration = 0.2f;

		private Vector2 velocity;
		private Vector2 velocitySmoothing;

		private MovementController controller;

		private Vector2 directionalInput;

		private bool isDashing;

		private void Start () {
			controller = GetComponent<MovementController> ();
		}

		private void FixedUpdate () {
			CalculateVelocity ();

			controller.Move (velocity * Time.fixedDeltaTime, directionalInput);
		}

		public void SetDirectionalInput (Vector2 input) {
			directionalInput = input;
		}

		public void OnDashInputDown () {
			if (!isDashing) {
				isDashing = true;
				StartCoroutine (ResetIsDashing (dashDuration));
			}
		}

		public void AddImpulseForce (Vector2 direction, float force) {
			velocity = direction.normalized * force;
		}

		private void CalculateVelocity () {
			Vector2 targetVelocity = directionalInput.normalized * moveSpeed;

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