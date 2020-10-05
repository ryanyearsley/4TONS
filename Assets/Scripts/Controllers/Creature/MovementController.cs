using System;
using UnityEngine;

namespace PlayerManagement {
	[RequireComponent (typeof (Rigidbody2D), typeof (BoxCollider2D))]
	public class MovementController : MonoBehaviour {

		private Vector2 velocity;
		private Rigidbody2D rb;

		public new BoxCollider2D collider { get; private set; }

		public int faceDirection { get; private set; }

		private void Awake () {
			rb = GetComponent<Rigidbody2D> ();
			collider = GetComponent<BoxCollider2D> ();
		}

		public void Move (Vector2 velocity) {
			this.velocity = velocity;
		}

		public void FixedUpdate () {
			if (velocity == Vector2.zero)
				return;

			faceDirection = (int)Mathf.Sign (velocity.x);
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}
	}
}