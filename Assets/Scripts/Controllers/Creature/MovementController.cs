using System;
using UnityEngine;

namespace PlayerManagement {
	public class MovementController : RaycastController {

		[HideInInspector]
		public Vector2 playerInput;

		public CollisionInfo collisions;

		protected override void Start () {
			base.Start ();
			collisions.faceDirectionX = 1;
		}

		public void Move (Vector2 moveAmount) {
			Move (moveAmount, Vector2.zero);
		}

		public void Move (Vector2 moveAmount, Vector2 input) {
			UpdateRaycastOrigins ();
			collisions.Reset ();
			collisions.moveAmountOld = moveAmount;
			playerInput = input;

			if (moveAmount.x != 0)
				collisions.faceDirectionX = (int)Mathf.Sign (moveAmount.x);
			if (moveAmount.y != 0)
				collisions.faceDirectionY = (int)Mathf.Sign (moveAmount.y);

			if (moveAmount.x != 0)
				HorizontalCollisions (ref moveAmount);

			if (moveAmount.y != 0)
				VerticalCollisions (ref moveAmount);

			transform.Translate (moveAmount);
		}

		private void HorizontalCollisions (ref Vector2 moveAmount) {
			float directionX = collisions.faceDirectionX;
			float directionY = collisions.faceDirectionY;
			float rayLength = Mathf.Abs (moveAmount.x) + SKIN_WIDTH;

			Vector2[] originPoints = new Vector2[horizontalRayCount];
			for (int i = 0; i < horizontalRayCount; i++) {
				originPoints [i] = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
				originPoints [i] += Vector2.up * (horizontalRaySpacing * i);
			}

			if (directionY == 1)
				Array.Reverse (originPoints);

			foreach (Vector2 rayOrigin in originPoints) {
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

				Debug.DrawRay (rayOrigin, Vector2.right * directionX, Color.red);

				if (hit) {
					moveAmount.x = (hit.distance - SKIN_WIDTH) * directionX;
					rayLength = hit.distance;

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}

		private void VerticalCollisions (ref Vector2 moveAmount) {
			float directionX = collisions.faceDirectionX;
			float directionY = collisions.faceDirectionY;
			float rayLength = Mathf.Abs (moveAmount.y) + SKIN_WIDTH;

			Vector2[] originPoints = new Vector2[verticalRayCount];
			for (int i = 0; i < verticalRayCount; i++) {
				originPoints [i] = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				originPoints [i] += Vector2.right * (verticalRaySpacing * i);
			}

			if (directionX == 1)
				Array.Reverse (originPoints);

			foreach (Vector2 rayOrigin in originPoints) {
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

				Debug.DrawRay (rayOrigin, Vector2.up * directionY, Color.red);

				if (hit) {
					moveAmount.y = (hit.distance - SKIN_WIDTH) * directionY;
					rayLength = hit.distance;

					collisions.below = directionY == -1;
					collisions.above = directionY == 1;
				}
			}
		}

		public struct CollisionInfo {
			public bool above, below;
			public bool left, right;

			public Vector2 moveAmountOld;
			public int faceDirectionX, faceDirectionY;

			public float angle, angleOld;
			public Vector2 normal;

			public void Reset () {
				above = below = false;
				left = right = false;
				normal = Vector2.zero;

				angleOld = angle;
				angle = 0f;
			}
		}
	}
}