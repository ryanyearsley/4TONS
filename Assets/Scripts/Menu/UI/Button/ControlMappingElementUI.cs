
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlMappingElementUI : MonoBehaviour
{
    [SerializeField]
    private Image buttonBackgroundImage;
    [SerializeField]
    private TextMeshProUGUI actionText;
    [SerializeField]
    private TextMeshProUGUI controlText;
    public void DisplayControlUI (Control control) {
        buttonBackgroundImage.color = control.controlColor;
        actionText.text = control.action;
        controlText.text = control.controlString;
    }
}
