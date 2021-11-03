using PlayerManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PathfindingTargetType {
	NONE, CREATURE, LOCATION
}

[RequireComponent (typeof (MovementComponent), typeof (BabyBrainsObject))]
public class PathfindingComponent : BabyBrainsComponent {

	private Transform trans;
	private MovementComponent moveController;

	//unstuck/sidestep variables
	public bool isStuck { get; private set; }
	public Vector3 unstuckDirection;
	private const float TARGET_POSITION_THRESHOLD = 0.25f;
	private const float STUCK_MOVE_THRESHOLD = 0.25f;
	private const float TARGET_MOVE_THRESHOLD = 0.25f;
	private float stuckThresholdSqr;
	private float targetMoveThresholdSqr;

	private const float PATH_UPDATE_INTERVAL = 0.5f;
	private const float DIRECTION_UPDATE_INTERVAL = 0.05f;

	[Header ("Pathfinding Settings")]
	public bool drawGizmos = false;
	public float turnSpeed = 3f;
	public float turnDistance = 0.5f;

	public Transform targetTransform;
	public PathfindingTargetType targetType;

	private Path path;
	private Vector3 currentPathNode;
	private Vector3 targetLookDirection;
	private Vector3 currentFaceDirection;

	public bool isOnAutoFollow = false;

	public bool followingPath { get; private set; }
	protected Vector3 moveDirection { get; private set; }

	#region Unity callbacks
	private void Awake () {
		trans = transform;
		moveController = GetComponent<MovementComponent> ();
	}

	private void FixedUpdate () {
		if (followingPath) {
			moveController.MovementFixedUpdate (moveDirection.XY ());
		} else {
			moveController.MovementFixedUpdate (Vector3.zero);
		}
	}
	#endregion


	#region BabyBrainsComponent callbacks

	public override void OnSpawn (Vector3 spawnPosition) {
		base.OnSpawn (spawnPosition);
	}
	public override void OnDeath () {
		base.OnDeath ();
		StopPathfinding ();
	}

	#endregion
	#region Public methods (API)
	public void StopPathfinding () {
		moveDirection = Vector3.zero;
		StopAllCoroutines ();
		followingPath = false;
		isStuck = false;
		targetTransform = null;
		path = null;
	}

	public void GoToPosition (Transform positionTransform) {
		StopPathfinding ();
		isOnAutoFollow = false;
		targetType = PathfindingTargetType.LOCATION;
		targetTransform = positionTransform;
		StartCoroutine (GoToPositionRoutine ());
	}


	public void AutoFollowTarget (Transform targetTransform) {
		StopPathfinding ();
		isOnAutoFollow = true;
		targetType = PathfindingTargetType.CREATURE;
		this.targetTransform = targetTransform;
		StartCoroutine (AutoFollowTargetRoutine ());
	}
	#endregion
	#region Pathfinding System callbacks

	public void OnPathFound (Vector3 [] waypoints, bool pathSuccessful) {
		StopCoroutine (FollowPath ());
		if (pathSuccessful) {
			path = new Path (waypoints, trans.position, turnDistance, 0.5f);
			StartCoroutine (FollowPath ());
		} else {
			StopPathfinding ();
			StartCoroutine (UnstuckRoutine ());
		}
	}
	#endregion

	#region Pathfinding Coroutines
	//follows target indefinitely until coroutine is stopped.
	private IEnumerator AutoFollowTargetRoutine () {
		PathRequestManager.RequestPath (new PathRequest (transform.position, targetTransform.position, OnPathFound));
		stuckThresholdSqr = STUCK_MOVE_THRESHOLD * STUCK_MOVE_THRESHOLD;
		targetMoveThresholdSqr = TARGET_MOVE_THRESHOLD * TARGET_MOVE_THRESHOLD;
		Vector3 positionOld = trans.position;
		Vector3 targetPositionOld = targetTransform.position;
		while (true) {
			yield return new WaitForSeconds (PATH_UPDATE_INTERVAL);
			float distMovedSqr = (trans.position - positionOld).sqrMagnitude;
			if ((trans.position - positionOld).sqrMagnitude < stuckThresholdSqr) {
				StartCoroutine (UnstuckRoutine ());
			}
			if ((targetTransform.position - targetPositionOld).sqrMagnitude > targetMoveThresholdSqr) {
				PathRequestManager.RequestPath (new PathRequest (transform.position, targetTransform.position, OnPathFound));
			}
			positionOld = trans.position;
			targetPositionOld = targetTransform.position;
		}
	}

	private IEnumerator GoToPositionRoutine () {
		PathRequestManager.RequestPath (new PathRequest (transform.position, targetTransform.position, OnPathFound));
		stuckThresholdSqr = STUCK_MOVE_THRESHOLD * STUCK_MOVE_THRESHOLD;
		targetMoveThresholdSqr = TARGET_MOVE_THRESHOLD * TARGET_MOVE_THRESHOLD;

		Vector3 positionOld = trans.position;
		Vector3 targetPositionOld = targetTransform.position;
		while (true) {
			yield return new WaitForSeconds (PATH_UPDATE_INTERVAL);
			float distMovedSqr = (trans.position - positionOld).sqrMagnitude;
			if ((trans.position - positionOld).sqrMagnitude < stuckThresholdSqr) {
				StartCoroutine (UnstuckRoutine ());
			}
			if ((targetTransform.position - targetPositionOld).sqrMagnitude > targetMoveThresholdSqr) {
				PathRequestManager.RequestPath (new PathRequest (transform.position, targetTransform.position, OnPathFound));
			}
			positionOld = trans.position;
			targetPositionOld = targetTransform.position;
		}
	}
	private IEnumerator FollowPath () {
		followingPath = true;
		int pathIndex = 0;
		moveDirection = (path.lookPoints [0] - transform.position).SetZ (0).normalized;

		Vector3 transformPositionOld = transform.position;
		while (followingPath) {

			Vector2 position2D = transform.position.XY ();


			if (pathIndex < path.finishLineIndex) {
				if (path.turnBoundaries [pathIndex].HasCrossedLine (position2D)) {
					pathIndex++;
				}
			} else {
				if (!isOnAutoFollow)
					StopPathfinding ();
				moveDirection = Vector3.zero;
				break;
			}

			currentPathNode = path.lookPoints [pathIndex];
			targetLookDirection = (currentPathNode - transform.position).SetZ (0).normalized;
			if (!isStuck)
				moveDirection = Vector3.Slerp (moveDirection, targetLookDirection, Time.deltaTime * turnSpeed);
			else {
				moveDirection = unstuckDirection;
			}

			if (targetType == PathfindingTargetType.CREATURE && targetTransform != null) {
				currentFaceDirection = (targetTransform.position - trans.position).SetZ (0).normalized;

			} else {
				currentFaceDirection = (currentPathNode - trans.position).SetZ (0).normalized;

			}

			creatureObject.SetFaceDirection ((int)Mathf.Sign (currentFaceDirection.x));


			yield return new WaitForSeconds (DIRECTION_UPDATE_INTERVAL);
		}
	}
	private IEnumerator UnstuckRoutine () {
		isStuck = true;


		Vector3 unstuckDirectionLeft =  Quaternion.AngleAxis (-90, Vector3.forward) * moveDirection;

		Vector3 unstuckDirectionRight =  Quaternion.AngleAxis (90, Vector3.forward) * moveDirection;
		RaycastHit2D raycastLeft = Physics2D.Raycast(transform.position, unstuckDirectionLeft);
		RaycastHit2D raycastRight = Physics2D.Raycast(transform.position, unstuckDirectionRight);

		unstuckDirection = (raycastLeft.distance > raycastRight.distance ? unstuckDirectionLeft : unstuckDirectionRight);
		yield return new WaitForSeconds (0.25f);
		isStuck = false;
		unstuckDirection = Vector3.zero;
	}
	#endregion
	public virtual void OnDrawGizmosSelected () {
		if (path != null && drawGizmos)
			path.DrawWithGizmos ();

		if (isStuck) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay (transform.position, unstuckDirection);
		}
	}
}

public class UnstuckRaycastResult {
	public Vector3 direction;
	public RaycastHit2D raycastHit;

	public UnstuckRaycastResult(Vector3 dir, RaycastHit2D hit) {
		this.direction = dir;
		this.raycastHit = hit;
	}
}