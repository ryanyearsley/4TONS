using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VitalsHUD : MonoBehaviour
{
    /*
   ===================================================================================
   the PlayerController script does the following.
   1 - holds the overhead canvas.
   2 - recieves commands from playerVitals to Update overhead canvas.

   ===================================================================================
        */

    private Image healthBar;
    private Image manaBar;

    private float currentDisplayedHpChange;
    private float currentDisplayedManaChange;

    public void InitializeHUD(int playerNumber)
    {
        healthBar = transform.Find("healthBar").GetComponent<Image>();
        manaBar = transform.Find("manaBar").GetComponent<Image>();
        ResetHUD();
    }
    public void ResetHUD()
    {
        updateHealthBar(1);
        updateManaBar(1);
    }

    public void updateHealthBar(float healthPercentage)
    {
        healthBar.fillAmount = healthPercentage;
    }
    public void updateManaBar(float manaPercentage)
    {
        manaBar.fillAmount = manaPercentage;
    }

}

