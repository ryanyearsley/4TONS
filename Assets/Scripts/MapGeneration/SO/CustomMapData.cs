using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Custom Map Data", menuName = "ScriptableObjects/Map Generation/Custom Map Data")]
public class CustomMapData : ScriptableObject
{
	public TextAsset mapFile;
	public MapSpawnPoints mapSpawnPoints;
}
