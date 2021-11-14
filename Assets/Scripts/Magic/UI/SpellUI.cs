using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellUI : MonoBehaviour
{
	[SerializeField]
	private string fallbackBindingString = "??";

	[SerializeField]
	private Image spellIcon;
	private Sprite emptySpellbindSprite;
	[SerializeField]
	private Text spellBindingText;
	private string spellBindingInputString;

	private void Awake () {
		emptySpellbindSprite = spellIcon.sprite;
		spellIcon.fillAmount = 1;
		spellBindingInputString = fallbackBindingString;
	}

	public void UpdateBindingString(string bindingString) {
		spellBindingInputString = bindingString;
		if (spellIcon.fillAmount == 1) {
			spellBindingText.text = spellBindingInputString;
		}
	}
	public void SetSpellUIToSpell(SpellData spellData) {
		spellIcon.sprite = spellData.icon;
		spellIcon.fillAmount = 1;
	}

	public void GreyOutSpellUI() {
		spellIcon.color = new Color (1, 1, 1, 0.5f);
	}
	public void ActivateSpellUI () {
		spellIcon.color = new Color (1, 1, 1, 1);
		spellIcon.fillAmount = 1;
		spellBindingText.text = spellBindingInputString;
	}

	public void ClearSpellBinding() {
		spellIcon.color = new Color (1, 1, 1, 1);
		spellIcon.fillAmount = 1;
		spellIcon.sprite = emptySpellbindSprite;
		spellBindingText.text = spellBindingInputString;
	}

	public void UpdateSpellUICooldown(float fillPercentage, float cooldownRemaining) {
		spellIcon.fillAmount = fillPercentage;
		spellBindingText.text = Math.Round (cooldownRemaining, 1).ToString();
		if (cooldownRemaining <= 0) {
			ActivateSpellUI ();
		}
	}
}
