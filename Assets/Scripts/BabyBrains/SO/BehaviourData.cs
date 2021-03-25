using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "BehaviourData", menuName = "ScriptableObjects/BabyBrains/Behaviour Data", order = 2)]

//Used to statically manage priority-related behaviour info  
//(
public class BehaviourData : ScriptableObject
{
    public int Weight;
    public string behaviourName;
    public string behaviourDescription;
    public BehaviourType behaviourType;
}

public enum BehaviourType {
    ABILITY, MOVEMENT
}

