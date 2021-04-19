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

	private void Awake () {
		emptySpellbindSprite = spellIcon.sprite;
		spellIcon.fillAmount = 1;
		spellBindingDefault = defaultBindingString;
	}

	public void SetSpellUIToSpell(SpellData spellData) {
		spellIcon.sprite = spellData.icon;
	}

	public void GreyOutSpellUI() {
		spellIcon.color = new Color (1, 1, 1, 0.5f);
	}
	public void ActivateSpellUI () {
		spellIcon.color = new Color (1, 1, 1, 1);
		spellIcon.fillAmount = 1;
		spellBindingText.text = spellBindingDefault;
	}

	public void ClearSpellBinding() {
		spellIcon.color = new Color (1, 1, 1, 1);
		spellIcon.sprite = emptySpellbindSprite;
		spellBindingText.text = spellBindingDefault;
	}

	public void UpdateSpellUICooldown(float fillPercentage, float cooldownRemaining) {
		spellIcon.fillAmount = fillPercentage;
		spellBindingText.text = Math.Round (cooldownRemaining, 1).ToString();
		if (cooldownRemaining <= 0) {
			ActivateSpellUI ();
		}
	}
}
