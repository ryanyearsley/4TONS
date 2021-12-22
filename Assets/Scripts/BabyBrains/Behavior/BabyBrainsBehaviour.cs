using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BabyBrainsBehaviour : MonoBehaviour {

    public BehaviourData behaviourData;
    public float ExecutionTime;
    public float executionTimer { get; set; }
    public float Cooldown;
    //TODO: Refactor to SO
    public float cdTimer { get; set; }

    public virtual void SetUpBehaviour (SensoryInfo sensoryInfo) {

	}
    public virtual bool Valid(SensoryInfo sensoryInfo) {
        return false;
	}

    public virtual bool Started () {
        return _started;
    }
    public bool _started = false;

    public virtual void OnTaskStart (SensoryInfo sensoryInfo) {
        _started = true;
        _finished = false;
        executionTimer = 0;
        cdTimer = Cooldown;
    }

    //monitors variables during task execution and typically triggers OnTaskEnd (unless the task is interrupted)l
    public virtual void UpdateBehaviour (SensoryInfo sensoryInfo, float interval) {
        executionTimer += interval;
        if (executionTimer >= ExecutionTime) {
            _finished = true;
        }
    }
    public virtual void InterruptBehaviour () {
        _finished = true;
    }

    public virtual bool Finished () {
        return _finished;
	}
    public bool _finished = false;
    public virtual void OnTaskEnd () {
        _started = false;
        _finished = true;
    }
}
