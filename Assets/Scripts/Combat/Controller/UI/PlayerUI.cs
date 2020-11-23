using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	public Text playerName;
	public Image playerPortait;
	public Image playerStocks;
	public SpellUI[] spellUIs;

	public void InitializePlayerUI (Player player) {
		playerName.text = player.currentWizard.wizardName;
		playerPortait.sprite = player.currentWizard.spellSchoolData.portrait;
		playerStocks.fillAmount = 1;
	}
}
