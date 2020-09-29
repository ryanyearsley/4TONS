using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MovementController))]
public class AITest : MonoBehaviour {

	private const float MIN_PATH__UPDATE_TIME = 0.2f;
	private const float PATH_UPDATE_MOVE_THRESHOLD = 0.5f;

	[Header ("Movement")]
	public float moveSpeed = 4f;
	[Tooltip ("Scaling amount used to adjust the speeds on different axis.")]
	public Vector2 velocityScaling = new Vector2 (1f, 0.5f);
	[Range (0.01f, 0.9f)]
	public float acceleration = 0.1f;

	public SpriteRenderer sprite;

	[Header ("Pathfinding")]
	public float turnSpeed = 3f;
	public float turnDistance = 5f;
	public float stoppingDistance = 10f;
	public Transform lookIndicator;

	private Vector2 velocity;
	private Vector2 velocitySmoothing;

	private MovementController controller;

	private Transform target;

	protected Transform Target {
		get {
			return target;
		}
	}
	private Vector2 lookDirection;

	private Path path;
	private bool followingPath;

	protected bool FollowingPath {
		get {
			return followingPath;
		}
	}
	private bool updatingPath;

	private void Start () {
		controller = GetComponent<MovementController> ();
	}

	private void FixedUpdate () {
		CalculateVelocity ();

		controller.Move (velocity * Time.fixedDeltaTime);
	}

	private void CalculateVelocity () {

	}

	#region Pathfinding

	protected void StartPathfinding (Transform target) {
		this.target = target;
		StartPathfinding ();
	}

	protected void StartPathfinding () {
		StopCoroutine ("UpdatePath");
		StartCoroutine ("UpdatePath");
	}

	protected void EndPathfinding () {
		followingPath = false;
		updatingPath = false;
		StopCoroutine ("FollowPath");
		StopCoroutine ("UpdatePath");
	}

	public void OnPathFound (Vector3[] waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path (waypoints, transform.position, turnDistance, stoppingDistance);

			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	private IEnumerator UpdatePath () {
		if (Time.timeSinceLevelLoad < 0.3f) {
			yield return new WaitForSeconds (0.3f);

			print ("Updating path");
		}
		PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));
		updatingPath = true;

		float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
		Vector3 targetPositionOld = target.position;

		while (updatingPath) {
			yield return new WaitForSeconds (MIN_PATH__UPDATE_TIME);
			if ((target.position - targetPositionOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));
				targetPositionOld = target.position;
			}
		}
	}

	private IEnumerator FollowPath () {
		followingPath = true;
		int pathIndex = 0;
		lookDirection = (path.lookPoints [0] - transform.position).normalized;

		float speedPercent = 1;

		while (followingPath) {
			Vector2 position2D = new Vector2 (transform.position.x, transform.position.y);
			while (path.turnBoundaries [pathIndex].HasCrossedLine (position2D)) {
				if (pathIndex == path.finishLineIndex) {
					followingPath = false;
					break;
				} else {
					pathIndex++;
				}
			}

			if (followingPath) {
				if (pathIndex >= path.slowDownIndex && stoppingDistance > 0) {
					speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFromPoint (position2D) / stoppingDistance);
					
				}
			}
			yield return null;
		}
	}

	#endregion
}