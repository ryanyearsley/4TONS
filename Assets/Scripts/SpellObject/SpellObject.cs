using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellObject : PoolObject, ISpellObject {

    protected VitalsEntity casterVitalsEntity;
    [SerializeField]
    protected SpellObjectData spellObjectData;
    [SerializeField]
    protected bool isAlive;

    [SerializeField]
    private float lifeTimer;

    protected Transform trans;

    public override void SetupObject () {
        base.SetupObject ();
        trans = this.transform;
    }

    public virtual void SetSpellObjectTag(VitalsEntity vitalsEntity) {
        casterVitalsEntity = vitalsEntity;
         if (vitalsEntity.tag != this.tag) {
            this.tag = vitalsEntity.tag;
        }
    }


    public virtual void ReuseSpellObject (VitalsEntity vitalsEntity) {
        casterVitalsEntity = vitalsEntity;
        lifeTimer = 0;
        isAlive = true;
    }
	public override void TerminateObjectFunctions () {
        isAlive = false;
        casterVitalsEntity = null;
        this.tag = "Untagged";

    }
    public virtual void Update () {
        if (isAlive) {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= spellObjectData.lifeTime) {
                print ("lifetime exceeded. object destroyed");
                Destroy ();
            }
        }
    }
}
