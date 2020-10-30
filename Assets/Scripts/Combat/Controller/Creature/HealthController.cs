using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : AbstractVitalsController, IDamageable
{
    [SerializeField]
    private int lives;

    public override void InitializeVital()
    {
        base.InitializeVital();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        stateController.OnDeathEvent += OnDeath;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        stateController.OnDeathEvent -= OnDeath;
    }

    public override void RegisterVital()
    {
        if (VitalsManager.Instance.vitalsObjects.ContainsKey(gameObject.GetInstanceID()))
        {
            VitalsManager.Instance.vitalsObjects[gameObject.GetInstanceID()].iDamageable = this;
        }
        else
        {
            VitalsEntity insertVitalsEntity = new VitalsEntity();
            insertVitalsEntity.iDamageable = this;
            VitalsManager.Instance.vitalsObjects.Add(this.gameObject.GetInstanceID(), insertVitalsEntity);
        }
    }

    public void ApplyDamage(float damage)
    {
        Debug.Log("Controller applying dmg");
        currentValue = Mathf.Clamp(currentValue -= damage, 0, maxValue);
        UpdateVitalsBar();
        stateController.OnHit(Vector2.zero);
        if (currentValue <= 0)
        {
            stateController.OnDeath();
        }
    }

    public void Heal(float healAmount)
    {
        currentValue = Mathf.Clamp(currentValue += healAmount, 0, maxValue);
        UpdateVitalsBar();
    }

    public void OnDeath()
    {
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        lives--;
       
        if (lives <= 0)
        {
            Debug.Log("No more lives. Get fucked! ");
        }
        else
        {
            Debug.Log("Creature Died. Lives remaining: " + lives);
            yield return new WaitForSeconds(1f);
            stateController.OnRespawn();
        }
    }
}
