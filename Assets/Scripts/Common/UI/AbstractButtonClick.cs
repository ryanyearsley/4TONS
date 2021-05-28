using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class AbstractButtonClick : MonoBehaviour {

    protected Button m_Button;

    protected virtual void Awake () {
        m_Button = GetComponent<Button> ();
        m_Button.onClick.AddListener (OnClick);
    }




    protected virtual void OnClick () {
        AudioManager.instance.PlaySound ("Confirm");
	}

}