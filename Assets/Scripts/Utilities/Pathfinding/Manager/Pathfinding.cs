using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour {


	private LevelManager levelManager;

	private void Awake () {
		levelManager = GetComponent<LevelManager> ();
	}

	public void FindPath (PathRequest request, Action<PathResult> callback) {

		Stopwatch sw = new Stopwatch ();
		sw.Start ();

		Vector3 [] waypoints = new Vector3 [0];
		bool pathSuccess = false;

		PathfindingNode startNode = levelManager.NodeFromWorldPoint (request.pathStart);
		PathfindingNode targetNode = levelManager.NodeFromWorldPoint (request.pathEnd);
		
		if (startNode != null && startNode.walkable && targetNode != null && targetNode.walkable) {
			Heap<PathfindingNode> openSet = new Heap<PathfindingNode> (levelManager.MaxSize);
			HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode> ();
			openSet.Add (startNode);

			while (openSet.Count > 0) {
				PathfindingNode currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				if (currentNode == targetNode) {
					sw.Stop ();
					//print ("Path found: " + sw.ElapsedMilliseconds + " ms");
					pathSuccess = true;
					break;
				}

				foreach (PathfindingNode neighbor in levelManager.GetNeighbors (currentNode)) {
					if (!neighbor.walkable || closedSet.Contains (neighbor)) {
						continue;
					}

					int newMovementCostToNeighbor = currentNode.gCost + GetDistance (currentNode, neighbor) + neighbor.movementPenalty;
					if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains (neighbor)) {
						neighbor.gCost = newMovementCostToNeighbor;
						neighbor.hCost = GetDistance (neighbor, targetNode);
						neighbor.parent = currentNode;

						if (!openSet.Contains (neighbor)) {
							openSet.Add (neighbor);
						} else {
							openSet.UpdateItem (neighbor);
						}
					}
				}
			}
		}
		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode);
			pathSuccess = waypoints.Length > 0;
		}
		callback (new PathResult (waypoints, pathSuccess, request.callback));
	}

	private Vector3[] RetracePath (PathfindingNode startNode, PathfindingNode endNode) {
		List<PathfindingNode> path = new List<PathfindingNode> ();
		PathfindingNode currentNode = endNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath (path);
		Array.Reverse (waypoints);
		return waypoints;
	}

	private Vector3[] SimplifyPath (List<PathfindingNode> path) {
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;
		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
			if (directionNew != directionOld) {
			} 
			waypoints.Add (path [i].worldPosition);
			directionOld = directionNew;
		}
		return waypoints.ToArray ();
	}

	private int GetDistance (PathfindingNode nodeA, PathfindingNode nodeB) {
		int distanceX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distanceY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (distanceX > distanceY) {
			return 14 * distanceY + 10 * (distanceX - distanceY);
		} else {
			return 14 * distanceX + 10 * (distanceY - distanceX);
		}
	}
}
