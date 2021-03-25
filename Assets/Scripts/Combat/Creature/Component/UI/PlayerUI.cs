using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	public Text playerName;
	public Image playerPortait;
	public SpellUI[] spellUIs;

	public void InitializePlayerUI (Player player) {
		playerName.text = player.wizardSaveData.wizardName;
		playerName.color = player.wizardSaveData.spellSchoolData.schoolGemColor;
		playerPortait.sprite = player.wizardSaveData.spellSchoolData.portrait;
	}
}
