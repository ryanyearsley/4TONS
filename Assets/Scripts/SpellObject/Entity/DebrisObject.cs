using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisObject : PoolObject
{

    [SerializeField]
    private bool isAlive;

    [SerializeField]
    private float lifeTime;
    private float lifeTimer;

    private Transform trans;

    private void Awake () {
        trans = this.transform;
    }

    public override void ReuseObject () {
        lifeTimer = 0;
        isAlive = true;
    }

    public virtual void Update () {
        if (isAlive) {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= lifeTime) {
                print ("lifetime exceeded. object destroyed");
                Destroy ();
            }
        }
    }

    private void OnDisable () {
        TerminateObjectFunctions ();
    }
    public override void TerminateObjectFunctions () {
        isAlive = false;
    }

}
