using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu (fileName = "SmallSetpieceData", menuName = "ScriptableObjects/Environment/Small Setpiece Data")]
public class SmallSetpieceData : ScriptableObject {
	public int id;
	public Zone zone;
	public Tile tile;
	public TileLayer layer;
}
