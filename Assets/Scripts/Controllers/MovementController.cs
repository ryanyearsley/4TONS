using System.Collections;
using System.Collections.Generic;
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

			if (moveAmount.x != 0)
				HorizontalCollisions (ref moveAmount);

			if (moveAmount.y != 0)
				VerticalCollisions (ref moveAmount);

			transform.Translate (moveAmount);
		}

		private void HorizontalCollisions (ref Vector2 moveAmount) {
			float directionX = collisions.faceDirectionX;
			float rayLength = Mathf.Abs (moveAmount.x) + SKIN_WIDTH;

			for (int i = 0; i < horizontalRayCount; i++) {
				Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
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
			float directionY = Mathf.Sign (moveAmount.y);
			float rayLength = Mathf.Abs (moveAmount.y) + SKIN_WIDTH;

			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
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
			public int faceDirectionX;

			public void Reset () {
				above = below = false;
				left = right = false;
			}
		}
	}
}