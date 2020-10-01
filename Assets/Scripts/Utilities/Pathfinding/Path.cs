﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {

	public readonly Vector3[] lookPoints;
	public readonly Line[] turnBoundaries;
	public readonly int finishLineIndex;
	public readonly int slowDownIndex;

	public Path (Vector3[] waypoints, Vector3 startPosition, float turnDistance, float stoppingDistance) {
		lookPoints = waypoints;
		turnBoundaries = new Line [lookPoints.Length];
		finishLineIndex = turnBoundaries.Length - 1;

		Vector2 previousePoint = V3ToV2 (startPosition);
		for (int i = 0; i < lookPoints.Length; i++) {
			Vector2 currentPoint = V3ToV2 (lookPoints [i]);
			Vector2 directionToCurrentPoint = (currentPoint - previousePoint).normalized;
			Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - directionToCurrentPoint * turnDistance;
			turnBoundaries [i] = new Line (turnBoundaryPoint, previousePoint - directionToCurrentPoint * turnDistance);
			previousePoint = turnBoundaryPoint;
		}

		float distanceFromEndPoint = 0;
		for (int i = lookPoints.Length - 1; i > 0; i--) {
			distanceFromEndPoint += Vector3.Distance (lookPoints [i], lookPoints [i - 1]);
			if (distanceFromEndPoint > stoppingDistance) {
				slowDownIndex = i;
				break;
			}
		}
	}

	private Vector2 V3ToV2 (Vector3 v3) {
		return new Vector2 (v3.x, v3.y);
	}

	public void DrawWithGizmos () {
		Gizmos.color = Color.green;
		foreach (Vector3 p in lookPoints) {
			Gizmos.DrawCube (p + Vector3.back, Vector3.one * 0.1f);
		}

		Gizmos.color = Color.green;
		foreach (Line line in turnBoundaries) {
			line.DrawWithGizmos (1);
		}
	}
}
