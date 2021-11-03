using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheadVitalsBarUI : MonoBehaviour
{
    [SerializeField]
    private Image vitalsBar;
    // Start is called before the first frame update
    public void InitializeUI(float barHieght)
    {
        ResetBar();
        transform.localPosition = new Vector3 (0, barHieght, 0);
    }

    public void ResetBar()
    {
        UpdateVitalsBar(1);
    }

    public void UpdateVitalsBar(float percentage)
    {
        vitalsBar.fillAmount = percentage;
    }
}