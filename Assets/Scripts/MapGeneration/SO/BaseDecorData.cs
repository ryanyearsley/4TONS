using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu (fileName = "BaseDecorData", menuName = "ScriptableObjects/Environment/Tile Data", order = 3)]
public class BaseDecorData : ScriptableObject {
	public int id;
	public Zone zone;
	public Tile tile;
	public TileLayer layer;
}
