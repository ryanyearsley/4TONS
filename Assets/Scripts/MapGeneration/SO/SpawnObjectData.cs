using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectData : ScriptableObject
{
	public int id;
	public int poolSize;
	public int outerClearance;
	public SpellSchool school;
	public GameObject spawnObjectPrefab;
}
