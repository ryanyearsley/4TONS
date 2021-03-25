﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceComponent : VitalsComponent, IHasResource
{
    [SerializeField]
    private float resourceRegenPerSec;

    public override void SetUpComponent (GameObject rootObject) {
        base.SetUpComponent (rootObject);
		maxValue = base.creatureObject.creatureData.resourceMax;
    }
    public override void OnSpawn(Vector3 spawnPosition) {
        base.OnSpawn (spawnPosition);
        StartCoroutine (RegenerateResourcePerSecRoutine ());
    }
    public void ApplyResourceDamage(float resourceDamage)
    {
        currentValue = Mathf.Clamp(currentValue -= resourceDamage, 0, maxValue);
        UpdateVitalsBar();
    }

    public bool SubtractResourceCost(float resourceCost)
    {
        if (currentValue > resourceCost)
        {
            currentValue = Mathf.Clamp(currentValue -= resourceCost, 0, maxValue);
            UpdateVitalsBar();
            //since we could reduce mana,
            return true;
        }
        else
        {
            //since we couldn't...
            return false;
        }
    }
    public void RegenerateResourcePerSecond()
    {
        currentValue = Mathf.Clamp(currentValue += resourceRegenPerSec, 0, maxValue);
        UpdateVitalsBar();
    }
    IEnumerator RegenerateResourcePerSecRoutine () {
        for (int i = 0; i >= 0; i++) {
            RegenerateResourcePerSecond ();
            yield return new WaitForSeconds (1);
        }
    }
    public void RegenerateMana(float resourceRegenAmount)
    {
        currentValue = Mathf.Clamp(currentValue += resourceRegenAmount, 0, maxValue);
        UpdateVitalsBar();
    }
}
