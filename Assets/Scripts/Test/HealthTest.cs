using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : MonoBehaviour
{
    [SerializeField]
    private GameObject playerOne;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //deal damage to P1
            DamageManager.Instance.ApplyDamage(playerOne.GetInstanceID(), 10);
        } 
    }
}
