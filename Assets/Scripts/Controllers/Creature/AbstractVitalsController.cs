using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractVitalsController : MonoBehaviour, IVital
{

    [SerializeField]
    protected float currentValue;
    [SerializeField]
    protected float maxValue;

    public GameObject overheadUIPrefab;
    private OverheadVitalsBarUI overheadUI;

    protected Animator animator;
    void Start()
    {
        InitializeVital();
    }

    public virtual void InitializeVital()
    {
        //data init
        currentValue = maxValue;
        RegisterVital();
        //UI creation/init
        GameObject go = Instantiate(overheadUIPrefab, transform.position, transform.rotation);
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = new Vector3(0, 1.25f, 0);
        overheadUI = go.GetComponent<OverheadVitalsBarUI>();
        overheadUI.InitializeUI();
        animator = transform.parent.GetComponent<Animator>();
    }

    public virtual void RegisterVital()
    {
        //See inheriting classes for implementation
    }

    public void UpdateVitalsBar()
    {
        overheadUI.updateVitalsBar(currentValue / maxValue);
    }

    protected virtual void ResetVitals()
    {
        currentValue = maxValue;
        overheadUI.updateVitalsBar(currentValue / maxValue);
    }

    public virtual void DeregisterVital()
    {
        if (VitalsManager.Instance.vitalsObjects.ContainsKey(gameObject.GetInstanceID())) {
            VitalsManager.Instance.vitalsObjects.Remove(gameObject.GetInstanceID());
        }
    }
}
