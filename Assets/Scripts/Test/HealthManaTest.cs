using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManaTest : MonoBehaviour
{
    [SerializeField]
    private GameObject playerOne;
    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //deal damage to P1
            Debug.Log("Pressing alpha 1 (Health Mana Test)");
            VitalsManager.Instance.ApplyDamage(playerOne.GetInstanceID(), 10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Pressing alpha 2 (Health Mana Test)");
            //Change Mana on P1
            VitalsManager.Instance.ApplyManaDamage(playerOne.GetInstanceID(), 10);
        }
    }
}
