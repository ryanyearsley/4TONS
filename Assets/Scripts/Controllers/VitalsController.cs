using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalsController : MonoBehaviour, IDamagable
{
    [SerializeField]
    private float maxHp;
    [SerializeField]
    private float maxMana;

    private Vector3 resetPosition;

    private float hp;
    private float mana;

    private VitalsHUD vitalsHUD;

    [SerializeField]
    private GameObject overheadCanvas;

    void Start()
    {
        hp = maxHp;
        mana = maxMana;
        resetPosition = transform.position;
        RegisterDamagableObject();
        GameObject go = Instantiate(overheadCanvas, transform.position, transform.rotation);
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = new Vector3(0, 1.25f, 0);
        vitalsHUD = go.GetComponent<VitalsHUD>();
        vitalsHUD.InitializeHUD(1);
        vitalsHUD.ResetHUD();
    }

    public void RegisterDamagableObject()
    {
        DamageManager.Instance.RegisterDamagableObject(gameObject.GetInstanceID(), this);
    }

    public void ApplyDamage(float damage)
    {
        hp -= damage;
        vitalsHUD.updateHealthBar(hp/maxHp);
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player Died");
        ResetObject();
    }
    private void ResetObject()
    {
        transform.position = resetPosition;
        hp = maxHp;
        mana = maxMana;
        vitalsHUD.ResetHUD();
    }
}
