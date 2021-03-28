using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellUI : MonoBehaviour
{
	[SerializeField]
	private string defaultBindingString;

	[SerializeField]
	private Image spellIcon;
	private Sprite emptySpellbindSprite;
	[SerializeField]
	private Text spellBindingText;
	private string spellBindingDefault;

	public void InitializeSpellUI(SpellData spellData) {
		emptySpellbindSprite = spellIcon.sprite;
		spellIcon.sprite = spellData.icon;
		spellIcon.fillAmount = 1;
		spellBindingDefault = defaultBindingString;
	}

	public void GreyOutSpellUI() {
		spellIcon.color = new Color (1, 1, 1, 0.5f);
	}
	public void ActivateSpellUI () {
		spellIcon.color = new Color (1, 1, 1, 1);
		spellIcon.fillAmount = 1;
		spellBindingText.text = spellBindingDefault;
	}

	public void UnbindSpellUI() {
		spellIcon.color = new Color (1, 1, 1, 1);
		spellIcon.sprite = emptySpellbindSprite;
	}

	public void UpdateSpellUICooldown(float fillPercentage, float cooldownRemaining) {
		spellIcon.fillAmount = fillPercentage;
		spellBindingText.text = Math.Round (cooldownRemaining, 1).ToString();
	}
}
