using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Environment/Tile Data", order = 3)]
public class TileData : ScriptableObject
{
	public int id;
	public Zone zone;
	public Tile tile;
}
