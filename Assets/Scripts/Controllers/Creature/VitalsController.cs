using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VitalsController : MonoBehaviour, IVital
{

    public GameObject overheadUIPrefab;

    [SerializeField]
    protected float currentValue;

    [SerializeField]
    protected float maxValue;

    private OverheadVitalsBarUI overheadUI;
    
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
    }


    public void UpdateVitalsBar()
    {
        overheadUI.updateVitalsBar(currentValue / maxValue);
    }


    public virtual void RegisterVital() { }
    public virtual void DeregisterVital() { }

}
