using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptableCastBehaviour : BabyBrainsBehaviour
{
    [SerializeField]
    private InterruptableCastIndicatorUI castIndicatorUI;

    [SerializeField]
    private float breakDamageRequired = 20f;

    private float interruptCastHealthThreshold;

    public override void OnTaskStart (SensoryInfo sensoryInfo) {
        base.OnTaskStart (sensoryInfo);
        castIndicatorUI.ShowCastIndicator ();
        interruptCastHealthThreshold = sensoryInfo.currentHealth - breakDamageRequired;
        castIndicatorUI.ResetCastIndicator ();
    }
    public override void UpdateBehaviour (SensoryInfo sensoryInfo, float interval) {
        if (sensoryInfo.currentHealth < interruptCastHealthThreshold) {
            InterruptBehaviour ();
        }
        executionTimer += interval;
        if (executionTimer >= ExecutionTime) {
            ExecuteInterruptableTask (sensoryInfo);
            _finished = true;
        }
        castIndicatorUI.UpdateCastIndicator (executionTimer / ExecutionTime * 100);
    }

	public override void InterruptBehaviour () {
        base.InterruptBehaviour ();
        castIndicatorUI.HideCastIndicator ();
    }

	public virtual void ExecuteInterruptableTask(SensoryInfo sensoryInfo) {

	}
    public override void OnTaskEnd () {
        base.OnTaskEnd (); 
        castIndicatorUI.HideCastIndicator ();
    }

}
