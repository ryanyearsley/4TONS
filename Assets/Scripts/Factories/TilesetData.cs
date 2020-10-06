using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ( menuName = "Scriptable Object/Tileset")]
public class TilesetData : ScriptableObject {

	[SerializeField]
	public List<GameObject> tilePrefabs;

}