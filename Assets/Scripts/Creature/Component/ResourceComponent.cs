﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceComponent : VitalsComponent, IHasResource
{
    [SerializeField]
    private float slowRegenRate = 0f;
    [SerializeField]
    private float fastRegenRate = 5f;

    private float lastCastTime;
    private float timeBeforeFastRegen = 5f;

    public override void SetUpComponent (GameObject rootObject) {
        base.SetUpComponent (rootObject);
		maxValue = base.creatureObject.creatureData.maxResource;
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

    public bool HasEnoughMana(float resourceCost) {
        if (currentValue > resourceCost) {
            return true;
        } else return false;
    }
    public void SubtractResourceCost(float resourceCost)
    {
        if (currentValue > resourceCost)
        {
            currentValue = Mathf.Clamp(currentValue -= resourceCost, 0, maxValue);
            lastCastTime = Time.time;
            UpdateVitalsBar();
        }
        else
        {
            //invalid operation.
        }
    }
    public void RegenerateResourcePerSecond(float regenAmount)
    {
        currentValue = Mathf.Clamp(currentValue += regenAmount, 0, maxValue);
        UpdateVitalsBar();
    }
    IEnumerator RegenerateResourcePerSecRoutine () {
        for (int i = 0; i >= 0; i++) {
            float timeSinceLastCast = Time.time - lastCastTime;
            if (timeSinceLastCast > timeBeforeFastRegen)
                RegenerateResourcePerSecond(fastRegenRate);
            else
                RegenerateResourcePerSecond(slowRegenRate);
            yield return new WaitForSeconds (1);
        }
    }
    public void RegenerateMana(float resourceRegenAmount)
    {
        currentValue = Mathf.Clamp(currentValue += resourceRegenAmount, 0, maxValue);
        UpdateVitalsBar();
    }
}
