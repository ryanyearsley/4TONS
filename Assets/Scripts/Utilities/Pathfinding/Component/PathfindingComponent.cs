using PlayerManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PathfindingTargetType {
	NONE, CREATURE, LOCATION
}

[RequireComponent (typeof (MovementComponent), typeof (BabyBrainsObject))]
public class PathfindingComponent : BabyBrainsComponent  {

	private Transform trans;
	private MovementComponent moveController;

	//unstuck/sidestep variables
	public bool IsStuck { get; private set; }
	public Vector3 unstuckDirection;
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
	public Vector3 targetPosition;
	public PathfindingTargetType targetType;

	private Path path;
	private Vector3 currentPathNode;
	private Vector3 targetLookDirection;

	public bool canFollow { get; private set; }

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
		}
	}
	#endregion


	#region BabyBrainsComponent callbacks

	public override void OnSpawn (Vector3 spawnPosition) {
		base.OnSpawn (spawnPosition);
		canFollow = true;
	}
	public override void OnDeath () {
		base.OnDeath ();
		canFollow = false;
		StopPathfinding ();
	}

	#endregion
	#region Public methods (API)
	public void StopPathfinding () {
		moveDirection = Vector3.zero;
		StopAllCoroutines ();
		followingPath = false;
		targetTransform = null;
		path = null;
	}

	public void GoToPosition (Transform positionTransform) {
		StopPathfinding ();
		targetType = PathfindingTargetType.LOCATION;
		targetTransform = positionTransform;
		StartCoroutine (AutoFollowTargetRoutine ());
	}


	public void AutoFollowTarget (Transform targetTransform) {
		StopPathfinding ();
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
				StartCoroutine (UnstuckRoutine (targetLookDirection));
			}
			if (!followingPath) {
				StopCoroutine (AutoFollowTargetRoutine ());
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

			if (canFollow == false) {

			}

			if (pathIndex < path.finishLineIndex) {
				if (path.turnBoundaries [pathIndex].HasCrossedLine (position2D)) {
					pathIndex++;
				}
			} else {
				//unit has arrived at target destination.
				break;
			}

			currentPathNode = path.lookPoints [pathIndex];
			targetLookDirection = (currentPathNode - transform.position).SetZ (0).normalized;

			creatureObject.SetFaceDirection ((int)Mathf.Sign (targetLookDirection.x));
			if (!IsStuck)
				moveDirection = Vector3.Slerp (moveDirection, targetLookDirection, Time.deltaTime * turnSpeed);
			else {
				moveDirection = unstuckDirection;
			}
			yield return new WaitForSeconds (DIRECTION_UPDATE_INTERVAL);
		}
	}
	private IEnumerator UnstuckRoutine (Vector3 lookDir) {
		IsStuck = true;
		Vector3 unstuckDirectionLeft =  Quaternion.AngleAxis (-90, Vector3.forward) * lookDir;

		Vector3 unstuckDirectionRight =  Quaternion.AngleAxis (90, Vector3.forward) * lookDir;
		RaycastHit2D raycastLeft = Physics2D.Raycast(transform.position, unstuckDirectionLeft);
		RaycastHit2D raycastRight = Physics2D.Raycast(transform.position, unstuckDirectionRight);

		unstuckDirection = (raycastLeft.distance > raycastRight.distance ? unstuckDirectionLeft : unstuckDirectionRight);
		yield return new WaitForSeconds (0.25f);
		IsStuck = false;
		unstuckDirection = Vector3.zero;
	}
	#endregion
	public virtual void OnDrawGizmosSelected () {
		if (path != null && drawGizmos)
			path.DrawWithGizmos ();

		if (IsStuck) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay (transform.position, unstuckDirection);
		}
	}
}