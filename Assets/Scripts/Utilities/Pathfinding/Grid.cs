﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public TerrainType[] walkableRegions;
	public int obstacleProximityPenalty = 10;

	private LayerMask walkableMask;
	private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int> ();

	private Node[,] grid;

	private float nodeDiameter;
	private int gridSizeX, gridSizeY;

	private int penaltyMin = int.MaxValue;
	private int penaltyMax = int.MinValue;

	private void Start () {
		InitializeGrid ();
	}

	public void InitializeGrid () {
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);

		foreach (TerrainType region in walkableRegions) {
			walkableMask.value |= region.terrainMask.value;
			walkableRegionsDictionary.Add ((int) Mathf.Log (region.terrainMask.value, 2), region.terrainPenalty);
		}

		CreateGrid ();
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	private void CreateGrid () {
		grid = new Node [gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);

				//print ("Walkable: " + Physics2D.OverlapCircle (worldPoint, nodeRadius, unwalkableMask));
				bool walkable = !(Physics2D.OverlapCircle (worldPoint, nodeRadius, unwalkableMask));

				int movementPenalty = 0;

				Collider2D hitCollider2D = Physics2D.OverlapPoint (worldPoint, walkableMask);

				//print ("hit Collider 2d: " + ((hitCollider2D) ? "true" : "false"));
				if (hitCollider2D != null) {
					walkableRegionsDictionary.TryGetValue (hitCollider2D.gameObject.layer, out movementPenalty);
				}

				if (!walkable) {
					movementPenalty += obstacleProximityPenalty;
				}

				grid [x, y] = new Node (walkable, worldPoint, x, y, movementPenalty);
			}
		}

		BlurPenaltyMap (3);
	}

	private void BlurPenaltyMap (int blurSize) {
		int kernalSize = blurSize * 2 + 1;
		int kernalExtents = (kernalSize - 1) / 2;

		int [,] penaltiesHorizontalPass = new int [gridSizeX, gridSizeY];
		int [,] penaltiesVerticalPass = new int [gridSizeX, gridSizeY];

		for (int y = 0; y < gridSizeY; y++) {
			for (int x = -kernalExtents; x <= kernalExtents; x++) {
				int sampleX = Mathf.Clamp (x, 0, kernalExtents);
				penaltiesHorizontalPass [0, y] += grid [sampleX, y].movementPenalty;
			}

			for (int x = 1; x < gridSizeX; x++) {
				int removeIndex = Mathf.Clamp (x - kernalExtents - 1, 0, gridSizeX);
				int addIndex = Mathf.Clamp (x + kernalExtents, 0, gridSizeX - 1);

				penaltiesHorizontalPass [x, y] = penaltiesHorizontalPass [x - 1, y] - grid [removeIndex, y].movementPenalty + grid [addIndex, y].movementPenalty;
			}
		}

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = -kernalExtents; y <= kernalExtents; y++) {
				int sampleY = Mathf.Clamp (y, 0, kernalExtents);
				penaltiesVerticalPass [x, 0] += penaltiesHorizontalPass [x, sampleY];
			}

			int blurredPenalty = Mathf.RoundToInt ((float) penaltiesVerticalPass [x, 0] / (kernalSize * kernalSize));
			grid [x, 0].movementPenalty = blurredPenalty;

			for (int y = 1; y < gridSizeY; y++) {
				int removeIndex = Mathf.Clamp (y - kernalExtents - 1, 0, gridSizeY);
				int addIndex = Mathf.Clamp (y + kernalExtents, 0, gridSizeY - 1);

				penaltiesVerticalPass [x, y] = penaltiesVerticalPass [x, y - 1] - penaltiesHorizontalPass [x, removeIndex] + penaltiesHorizontalPass [x, addIndex];
				blurredPenalty = Mathf.RoundToInt ((float) penaltiesVerticalPass [x, y] / (kernalSize * kernalSize));
				grid [x, y].movementPenalty = blurredPenalty;

				if (blurredPenalty > penaltyMax) {
					penaltyMax = blurredPenalty;
				}
				if (blurredPenalty < penaltyMin) {
					penaltyMin = blurredPenalty;
				}
			}
		}
	}

	public List<Node> GetNeighbors (Node node) {
		List<Node> neighbors = new List<Node> ();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) {
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbors.Add (grid [checkX, checkY]);
				}
			}
		}

		return neighbors;
	}

	public Node NodeFromWorldPoint (Vector3 worldPosition) {
		float percentX = Mathf.Clamp01 ((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
		float percentY = Mathf.Clamp01 ((worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y);

		int x = Mathf.RoundToInt ((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt ((gridSizeY - 1) * percentY);

		return grid [x, y];
	}

	private void OnDrawGizmos () {
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, gridWorldSize.y, 1));
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {

				Gizmos.color = Color.Lerp (Color.white, Color.black, Mathf.InverseLerp (penaltyMin, penaltyMax, n.movementPenalty));

				Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
				Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter * 0.8f));
			}
		}
	}

	[System.Serializable]
	public class TerrainType {
		public LayerMask terrainMask;
		public int terrainPenalty;
	}
}