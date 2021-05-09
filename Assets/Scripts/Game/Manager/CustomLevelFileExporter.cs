using UnityEngine.Tilemaps;
using UnityEngine;

public class CustomLevelFileExporter : MonoBehaviour {

	private TextAsset textAsset;
	[SerializeField]
	private Tilemap tilemap;
	[SerializeField]
	private Vector2Int mapBounds;
	public void Generate2DArrayFromTilemap (Tilemap tilemap) {
		int[,] output = new int[mapBounds.x, mapBounds.y];
		for (int i = 0; i < mapBounds.x; i++) {
			for (int j = 0; i < mapBounds.y; j++) {
				if (tilemap.GetTile (new Vector3Int (i, j, 0)) != null) {
					output [i, j] = 1;
				} else
					output [i, j] = 0;
			}
		}


	}
}
