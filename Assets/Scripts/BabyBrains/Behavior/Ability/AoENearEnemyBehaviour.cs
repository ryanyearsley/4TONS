using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoENearEnemyBehaviour : BabyBrainsBehaviour
{
    [SerializeField]
    protected float resourceCost;
    [SerializeField]
    protected float castSpeedPenaltyMultiplier;
    [SerializeField]
    protected float minAttackDistance;
    [SerializeField]
    protected float maxAttackDistance;

    [SerializeField]
    protected float accuracy;
    [SerializeField]
    protected GameObject aoePrefab;
    // Start is called before the first frame update
    public override bool Valid (SensoryInfo sensoryInfo) {
        if (sensoryInfo.targetVitals.trans != null
            && sensoryInfo.currentResource > resourceCost
            && sensoryInfo.isoDistanceToTarget > minAttackDistance
            && sensoryInfo.isoDistanceToTarget < maxAttackDistance)
            return true;
        else return false;
    }

    public override void OnTaskStart (SensoryInfo sensoryInfo) {
        SpeedAlteringEffect sae = new SpeedAlteringEffect(castSpeedPenaltyMultiplier, ExecutionTime, true);
        if (sensoryInfo.vitalsEntity.resource != null) {
            sensoryInfo.vitalsEntity.resource.SubtractResourceCost (resourceCost);
        }
        sensoryInfo.vitalsEntity.creatureObject.AddSpeedEffect (sae);
        sensoryInfo.vitalsEntity.creatureObject.OnAttack (new AttackInfo (ExecutionTime, castSpeedPenaltyMultiplier));

        float xPosition = sensoryInfo.targetVitals.trans.position.y +Random.Range(-accuracy, accuracy);
        float yPosition = sensoryInfo.targetVitals.trans.position.y + Random.Range(-accuracy/2, accuracy/2);
        Vector3 aoePosition = new Vector3 (xPosition, yPosition, 0);
        PoolManager.instance.ReuseSpellObject (aoePrefab, aoePosition, Quaternion.identity, sensoryInfo.vitalsEntity);
    }
}
