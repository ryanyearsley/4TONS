using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractVitalsController : MonoBehaviour, IVital
{

    protected PlayerStateController stateController;

    [SerializeField]
    protected float currentValue;
    [SerializeField]
    protected float maxValue;

    public GameObject overheadUIPrefab;
    private OverheadVitalsBarUI overheadUI;

    void Start()
    {
        InitializeVital();
    }

    protected virtual void OnEnable()
    {
        stateController = GetComponentInParent<PlayerStateController>();
        stateController.OnRespawnEvent += OnRespawn;
    }
    protected virtual void OnDisable()
    {
        stateController = GetComponentInParent<PlayerStateController>();
        stateController.OnRespawnEvent -= OnRespawn;
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

    public virtual void RegisterVital()
    {
        //See inheriting classes for implementation
    }

    public virtual void DeregisterVital()
    {
        if (VitalsManager.Instance.vitalsObjects.ContainsKey(gameObject.GetInstanceID()))
        {
            VitalsManager.Instance.vitalsObjects.Remove(gameObject.GetInstanceID());
        }
    }

    public void UpdateVitalsBar()
    {
        overheadUI.updateVitalsBar(currentValue / maxValue);
    }

    public void OnRespawn()
    {
        currentValue = maxValue;
        overheadUI.updateVitalsBar(currentValue / maxValue);
    }

}
