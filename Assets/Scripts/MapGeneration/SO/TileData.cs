using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileData {
	public Tile tile;
	public int id;
	[NonSerialized]
	public TileLayer layer;
}

public enum TileLayer {
	FLOOR, BASE
}
