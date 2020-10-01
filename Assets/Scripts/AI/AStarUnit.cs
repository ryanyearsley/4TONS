using PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MovementController))]
public class AStarUnit : MonoBehaviour {

	private const float PATH_UPDATE_MOVE_THRESHOLD = 0.25f;
	private const float MIN_PATH_UPDATE_TIME = 0.5f;

	public Transform target;
	public float speed = 2f;
	public Vector2 velocityMultiplier = new Vector2 (1f, 1f);
	public float turnSpeed = 3f;
	public float turnDistance = 0.5f;

	private MovementController controller;

	private Path path;
	private Vector3 lookDirection;

	private void Awake () {
		controller = GetComponent<MovementController> ();
	}

	private void Start () {
		StartCoroutine (UpdatePath ());
	}

	public void OnPathFound (Vector3 [] waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path (waypoints, transform.position, turnDistance, 0.5f);
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	private IEnumerator UpdatePath () {

		if (Time.timeSinceLevelLoad < 0.3f)
			yield return new WaitForSeconds (0.3f);
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

		bool followingPath = true;
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
				controller.Move (lookDirection.XY () * Time.deltaTime * (speed * velocityMultiplier));
			}

			yield return null;
		}
	}

	public void OnDrawGizmos () {
		if (path != null)
			path.DrawWithGizmos ();
	}
}