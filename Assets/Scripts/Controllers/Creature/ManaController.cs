﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaController : VitalsController, IHasMana
{
    [SerializeField]
    private float manaRegenPerSec;

    private OverheadVitalsBarUI overheadUI;
    // Start is called before the first frame update
    public override void InitializeVital()
    {
        base.InitializeVital();
    }

    public override void RegisterVital()
    {
        VitalsManager.Instance.RegisterHasManaObject(gameObject.GetInstanceID(), this);
    }
    public override void DeregisterVital()
    {
        VitalsManager.Instance.DeregisterHasManaObject(gameObject.GetInstanceID());
    }

    public void ApplyManaDamage(float manaDamage)
    {
        currentValue = Mathf.Clamp(currentValue -= manaDamage, 0, maxValue);
        UpdateVitalsBar();
    }

    public bool SubtractManaCost(float manaCost)
    {
        if (currentValue > manaCost)
        {
            currentValue = Mathf.Clamp(currentValue -= manaCost, 0, maxValue);
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

    public void RegenerateMana(float manaRegenAmount)
    {
        currentValue = Mathf.Clamp(currentValue += manaRegenAmount, 0, maxValue);
        UpdateVitalsBar();
    }

    public void RegenerateManaPerSecond()
    {
        currentValue = Mathf.Clamp(currentValue += manaRegenPerSec, 0, maxValue);
        UpdateVitalsBar();
    }

}
