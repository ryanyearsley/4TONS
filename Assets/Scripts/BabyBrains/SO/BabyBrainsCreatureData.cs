using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "BabyBrainsCreatureData", menuName = "ScriptableObjects/Editor/BabyBrainsCreatureData")]

public class BabyBrainsCreatureData : CreatureData
{
	public float aggroDistance;
	public float breakAggroDistance;
	public float meleeAttackDistance;
	public float rangedAttackDistance;
}
