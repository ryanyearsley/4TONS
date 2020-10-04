using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : VitalsController, IDamageable
{

    private Vector3 resetPosition;

    private OverheadVitalsBarUI overheadUI;

    public override void InitializeVital()
    {
        base.InitializeVital();
        resetPosition = transform.position;
    }

    public override void RegisterVital()
    {
        Debug.Log("Registering HP");
        VitalsManager.Instance.RegisterDamageableObject(gameObject.GetInstanceID(), this);
    }
    public override void DeregisterVital()
    {
        VitalsManager.Instance.DeregisterDamageableObject(gameObject.GetInstanceID());
    }

    public void ApplyDamage(float damage)
    {
        Debug.Log("Controller applying dmg");
        currentValue = Mathf.Clamp(currentValue -= damage, 0, maxValue);
        UpdateVitalsBar();
        if (currentValue <= 0)
        {
            Die();
        }
    }
    public void Heal(float healAmount)
    {
        currentValue = Mathf.Clamp(currentValue += healAmount, 0, maxValue);
        UpdateVitalsBar();
    }
    public void Die()
    {
        Debug.Log("Player Died");
        ResetObject();
    }
    private void ResetObject()
    {
        transform.position = resetPosition;
        currentValue = maxValue;
        overheadUI.ResetBar();
    }
}
