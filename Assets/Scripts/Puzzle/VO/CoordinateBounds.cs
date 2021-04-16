using UnityEngine;
using System;

[Serializable]
public struct CoordinateBounds
{
	public Vector2Int dimensions;
	public Vector2Int minCoord;
	public Vector2Int maxCoord;

	public CoordinateBounds (Vector2Int minCoord, Vector2Int dimensions) {
		this.minCoord = minCoord;
		this.dimensions = dimensions;
		this.maxCoord = minCoord + dimensions;
	}
	public override string ToString() {
		string output = "Min coord: " + minCoord.ToString() + ", Max Coord: " + maxCoord.ToString();
		return output;
	}

	public bool isWithinBounds (Vector2Int targetCoordinates) {

		if (targetCoordinates.x < minCoord.x || targetCoordinates.x > maxCoord.x)
			return false;//outside of bounds in X axis
		if (targetCoordinates.y < minCoord.y || targetCoordinates.y > maxCoord.y)
			return false;//outside of bounds in Y axis

		return true;
	}
}
