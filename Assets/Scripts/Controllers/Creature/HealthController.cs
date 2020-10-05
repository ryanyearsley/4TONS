using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : AbstractVitalsController, IDamageable
{
    [SerializeField]
    private int lives;
    private Vector3 resetPosition;

    public override void InitializeVital()
    {
        base.InitializeVital();
        resetPosition = transform.position;
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
        animator.SetTrigger("hit");
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
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        animator.SetTrigger("die");
        lives--;
        Debug.Log("Creature Died. Lives remaining: " + lives);
       
        yield return new WaitForSeconds(1f);
        if (lives <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Respawn();
        }
    }

    public void Respawn()
    {    
        transform.position = resetPosition;
        ResetVitals();
    }

    protected override void ResetVitals()
    {
        base.ResetVitals();
        animator.Rebind();
    }
}
