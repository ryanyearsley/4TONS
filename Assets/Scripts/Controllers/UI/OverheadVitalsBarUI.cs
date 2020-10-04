using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheadVitalsBarUI : MonoBehaviour
{
    [SerializeField]
    private Image vitalsBar;
    // Start is called before the first frame update
    public void InitializeUI()
    {
        ResetBar();
    }

    public void ResetBar()
    {
        updateVitalsBar(1);
    }

    public void updateVitalsBar(float percentage)
    {
        vitalsBar.fillAmount = percentage;
    }
}