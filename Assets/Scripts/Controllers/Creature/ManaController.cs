using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaController : AbstractVitalsController, IHasMana
{
    [SerializeField]
    private float manaRegenPerSec;

    private float passiveManaRegeneration;

    private OverheadVitalsBarUI overheadUI;
    // Start is called before the first frame update
    public override void InitializeVital()
    {
        base.InitializeVital();
    }

    public override void RegisterVital()
    {
        if (VitalsManager.Instance.vitalsObjects.ContainsKey(gameObject.GetInstanceID()))
        {
            VitalsManager.Instance.vitalsObjects[gameObject.GetInstanceID()].iHasMana = this;
        }
        else
        {
            VitalsEntity insertVitalsEntity = new VitalsEntity();
            insertVitalsEntity.iHasMana = this;
            VitalsManager.Instance.vitalsObjects.Add(this.gameObject.GetInstanceID(), insertVitalsEntity);
        }
    }
    public override void DeregisterVital()
    {
        if (VitalsManager.Instance.vitalsObjects.ContainsKey(gameObject.GetInstanceID()))
        {
            VitalsManager.Instance.vitalsObjects.Remove(GetInstanceID());
        }
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
    public void RegenerateManaPerSecond()
    {
        currentValue = Mathf.Clamp(currentValue += passiveManaRegeneration, 0, maxValue);
        UpdateVitalsBar();
    }
    public void RegenerateMana(float manaRegenAmount)
    {
        currentValue = Mathf.Clamp(currentValue += manaRegenAmount, 0, maxValue);
        UpdateVitalsBar();
    }
}
