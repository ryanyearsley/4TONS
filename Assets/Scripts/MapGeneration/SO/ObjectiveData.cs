using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Objective Data", menuName = "ScriptableObjects/Map Generation/ObjectiveData")]
public class ObjectiveData : ScriptableObject
{
	public Objective objective;
	public CreatureData playerObjectData;
	public SetPieceSpawnInfo[] objectiveSpawnInfos;
}
