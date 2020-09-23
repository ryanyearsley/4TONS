using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement {
	[RequireComponent (typeof (MovementController))]
	public class Player : MonoBehaviour {
		public float moveSpeed = 6f;
		[Range (0.01f, 0.9f)]
		public float acceleration = 0.1f;

		private Vector2 velocity;
		private Vector2 velocitySmoothing;

		private MovementController controller;

		private Vector2 directionalInput;

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

		private void CalculateVelocity () {
			Vector2 targetVelocity = directionalInput.normalized * moveSpeed;
			velocity = Vector2.SmoothDamp (velocity, targetVelocity, ref velocitySmoothing, acceleration);
		}
	}
}