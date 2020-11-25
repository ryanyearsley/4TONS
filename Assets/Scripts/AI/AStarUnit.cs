using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MovementController))]
public class AStarUnit : MonoBehaviour {

	private const float PATH_UPDATE_MOVE_THRESHOLD = 0.25f;
	private const float MIN_PATH_UPDATE_TIME = 0.5f;

	[Header ("Pathfinding Settings")]
	public bool drawGizmos = false;
	public float speed = 2f;
	public Vector2 velocityMultiplier = new Vector2 (1f, 1f);
	public float turnSpeed = 3f;
	public float turnDistance = 0.5f;

	public Transform target;
	private MovementController controller;

	private Path path;
	protected bool followingPath { get; private set; }
	protected Vector3 lookDirection { get; private set; }

	protected virtual void Awake () {
		controller = GetComponent<MovementController> ();
		StartCoroutine (FindPlayerAfterLoad());
	}

	private IEnumerator FindPlayerAfterLoad() {
		yield return new WaitForSeconds (0.2f);
		if (target == null) {
			Debug.Log ("Finding player target.");
			target = FindObjectOfType<PlayerStateController>().transform;
		}
	}
	private void Start () {
		StartCoroutine (UpdatePath ());
		
	}

	protected void GoToPosition (Vector2 position) {
		if (target == null)
			target = new GameObject ("A_Target").transform;
		StopCoroutine (UpdatePath ());
		target.position = position;
		StartCoroutine (UpdatePath ());
	}

	protected void StopPathfinding () {
		StopCoroutine (UpdatePath ());
		followingPath = false;
	}

	public void OnPathFound (Vector3 [] waypoints, bool pathSuccessful) {
		Debug.Log ("on path found");
		if (pathSuccessful) {
			StopCoroutine ("FollowPath");
			path = new Path (waypoints, transform.position, turnDistance, 0.5f);
			StartCoroutine ("FollowPath");
		}
	}

	private IEnumerator UpdatePath () {
		Debug.Log ("updating path");
		if (Time.timeSinceLevelLoad < 1f)
			yield return new WaitForSeconds (1.1f);
		PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));

		float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
		Vector3 targetPositionOld = target.position;

		while (true) {
			yield return new WaitForSeconds (MIN_PATH_UPDATE_TIME);
			if ((target.position - targetPositionOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));
				targetPositionOld = target.position;
			}
		}
	}

	private IEnumerator FollowPath () {
		Debug.Log ("following path");
		followingPath = true;
		int pathIndex = 0;
		lookDirection = (path.lookPoints [0] - transform.position).SetZ (0).normalized;

		while (followingPath) {
			Vector2 position2D = transform.position.XY ();
			while (path.turnBoundaries[pathIndex].HasCrossedLine (position2D)) {
				if (pathIndex == path.finishLineIndex) {
					followingPath = false;
					break;
				} else {
					pathIndex++;
				}
			}

			if (followingPath) {
				Vector3 targetLookDirection = (path.lookPoints [pathIndex] - transform.position).SetZ (0).normalized;
				lookDirection = Vector3.Slerp (lookDirection, targetLookDirection, Time.deltaTime * turnSpeed);
				controller.Move (lookDirection.XY () * (speed * velocityMultiplier));
			}

			yield return null;
		}
	}

	public virtual void OnDrawGizmos () {
		if (path != null && drawGizmos)
			path.DrawWithGizmos ();
	}
}