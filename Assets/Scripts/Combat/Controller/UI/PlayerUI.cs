using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	public Text playerName;
	public Image playerPortait;
	public SpellUI[] spellUIs;

	public void InitializePlayerUI (Player player) {
		playerName.text = player.currentWizard.wizardName;
		playerName.color = player.currentWizard.spellSchoolData.schoolGemColor;
		playerPortait.sprite = player.currentWizard.spellSchoolData.portrait;
	}
}
