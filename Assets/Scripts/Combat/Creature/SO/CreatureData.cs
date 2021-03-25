using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "CustomCreatureData", menuName = "ScriptableObjects/Combat/Creature Data", order = 2)]

public class CreatureData : SpawnObjectData {
	public string creatureName;
	public float health;
	public float resourceMax;
	public float movementSpeed;
}
