using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SpellUI : MonoBehaviour
{
	[SerializeField]
	private string fallbackBindingString = "??";

	[SerializeField]
	private Image spellImage;
	private Sprite emptySpellSlotSprite;
	[SerializeField]
	private TMP_Text spellBindingText;
	private string spellBindingInputString;

	private void Awake () {
		emptySpellSlotSprite = spellImage.sprite;
		spellImage.fillAmount = 1;
		spellBindingInputString = fallbackBindingString;
	}

	public void UpdateBindingString(string bindingString) {
		spellBindingInputString = bindingString;
		if (spellImage.fillAmount == 1) {
			spellBindingText.text = spellBindingInputString;
		}
	}
	public void SetSpellUIToSpell(SpellData spellData) {
		spellImage.sprite = spellData.icon;
		spellImage.fillAmount = 1;
	}

	public void GreyOutSpellUI() {
		spellImage.color = new Color (1, 1, 1, 0.5f);
	}
	public void ActivateSpellUI () {
		spellImage.color = new Color (1, 1, 1, 1);
		spellImage.fillAmount = 1;
		spellBindingText.text = spellBindingInputString;
	}

	public void ClearSpellBinding() {
		spellImage.color = new Color (1, 1, 1, 1);
		spellImage.fillAmount = 1;
		spellImage.sprite = emptySpellSlotSprite;

		spellBindingText.text = spellBindingInputString;
	}

	public void UpdateSpellUICooldown(float fillPercentage, float cooldownRemaining) {
		spellImage.fillAmount = fillPercentage;
		spellBindingText.text = Math.Round (cooldownRemaining, 1).ToString();
		if (cooldownRemaining <= 0) {
			ActivateSpellUI ();
		}
	}
}
