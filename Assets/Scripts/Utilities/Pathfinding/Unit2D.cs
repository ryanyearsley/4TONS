using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class Unit2D : PoolObject {

	private const float minPathUpdateTime = 0.2f;
	private const float pathUpdateMoveThreshold = 0.5f;

	[Header ("Pathfinding / Movement")]
	public float speed = 20;
	public float verticalMultiplier = 0.5f;
	public float turnSpeed = 3;
	public float turnDistance = 5;
	public float stoppingDistance = 10;
    private Vector3 move;
	public Transform lookIndicator;

	protected Rigidbody2D rb2D;
	private Transform target;
    [SerializeField]
	protected Transform Target {
		get {
			return target;
		}
	}
	private Vector3 lookDirection;

	private Path path;
	private bool followingPath;

	protected bool FollowingPath {
		get {
			return followingPath;
		}
	}
	private bool updatingPath;

	protected virtual void Awake () {
		rb2D = GetComponent<Rigidbody2D> ();
	}

	protected void StartPathfinding (Transform _target) {
		target = _target;
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

	public void OnPathFound (Vector3 [] waypoints, bool pathSuccessful) {
        //print("Path successful = " + pathSuccessful);
		if (pathSuccessful) {
			path = new Path (waypoints, transform.position, turnDistance, stoppingDistance);

			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	private IEnumerator UpdatePath () {
		if (Time.timeSinceLevelLoad < 0.3f) {
			yield return new WaitForSeconds (0.3f);

            print("updating path");
        }
		PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));
		updatingPath = true;

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPositionOld = target.position;

		while (updatingPath) {
			yield return new WaitForSeconds (minPathUpdateTime);
			if ((target.position - targetPositionOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));
				targetPositionOld = target.position;
			}
		}
	}

	private IEnumerator FollowPath () {
        //print("zombie following path");
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
					rb2D.drag = MapValue (speedPercent, 0.03f, 1, 7.5f, 2);
					if (speedPercent < 0.3f) {
						followingPath = false;
						rb2D.drag = 2f;
					}
				}

				lookDirection = (path.lookPoints [pathIndex] - transform.position).normalized;
				lookDirection.z = 0;
				lookIndicator.localPosition = lookDirection / 2f;
				move = (Vector3.right * speed * speedPercent * lookDirection.x) + (Vector3.up * speed * speedPercent * verticalMultiplier * lookDirection.y);
                //rb2D.AddRelativeForce(move);
            }
			yield return null;
		}
	}
    
    private void FixedUpdate()
    {
        if (followingPath)
            rb2D.AddRelativeForce(move);
    }
    
    public static float MapValue (float referenceValue, float fromMin, float fromMax, float toMin, float toMax) {
		/* This function maps (converts) a Float value from one range to another */
		return toMin + (referenceValue - fromMin) * (toMax - toMin) / (fromMax - fromMin);
	}

	protected void DrawWithGizmos () {
		if (path != null) {
			path.DrawWithGizmos ();
		}
	}
}