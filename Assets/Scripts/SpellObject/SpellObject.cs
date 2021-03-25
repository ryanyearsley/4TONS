using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellObject : PoolObject, ISpellObject {

    protected VitalsEntity casterVitalsEntity;
    [SerializeField]
    protected SpellObjectData spellObjectData;
    [SerializeField]
    private bool isAlive;

    [SerializeField]
    private float lifeTimer;
    
    private Transform trans;
    
	public override void SetupObject () {
		base.SetupObject ();
        trans = this.transform;
    }

	public virtual void ReuseSpellObject(VitalsEntity vitalsEntity)
    {
        casterVitalsEntity = vitalsEntity;
        lifeTimer = 0;
        isAlive = true;
    }

    public virtual void Update()
    {
        if (isAlive)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= spellObjectData.lifeTime)
            {
                print("lifetime exceeded. object destroyed");
                Destroy();
            }
        }
    }
    private void OnDisable()
    {
        TerminateObjectFunctions();
    }
    public override void TerminateObjectFunctions()
    {
        isAlive = false;
    }
}
