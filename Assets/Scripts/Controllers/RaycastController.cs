using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

	protected const float SKIN_WIDTH = 0.025f;
	private const float DISTANCE_BETWEEN_RAYS = 0.2f;

	[Header ("Raycast Controller")]
	public LayerMask collisionMask;
	protected int horizontalRayCount;
	protected int verticalRayCount;

	protected float horizontalRaySpacing;
	protected float verticalRaySpacing;

	[HideInInspector]
	public new BoxCollider2D collider;
	protected RaycastOrigins raycastOrigins;

	protected virtual void Awake () {
		collider = GetComponent<BoxCollider2D> ();
	}

	protected virtual void Start () {
		CalculateRaySpacing ();
	}

	protected void UpdateRaycastOrigins () {
		Bounds bounds = collider.bounds;
		bounds.Expand (SKIN_WIDTH * -2f);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	protected void CalculateRaySpacing () {
		Bounds bounds = collider.bounds;
		bounds.Expand (SKIN_WIDTH * -2f);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

		horizontalRayCount = Mathf.RoundToInt (boundsHeight / DISTANCE_BETWEEN_RAYS);
		verticalRayCount = Mathf.RoundToInt (boundsWidth / DISTANCE_BETWEEN_RAYS);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	protected struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}