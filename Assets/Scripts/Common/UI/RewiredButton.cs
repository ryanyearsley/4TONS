using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Rewired doesn't allow you to use OnUISubmit on left mouse button click. This is a workaround
public class RewiredButton : Button {

    // Trigger all registered callbacks.
    public override void OnPointerClick (PointerEventData eventData) {
        if (!IsActive () || !IsInteractable ()) return; // ignore click entirely if button is already disabled

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press ();

    }

    private void Press () {
        if (!IsActive () || !IsInteractable ())
            return;

        onClick.Invoke ();
    }


}
