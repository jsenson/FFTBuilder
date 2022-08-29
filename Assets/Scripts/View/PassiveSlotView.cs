using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveSlotView : MonoBehaviour {
	[SerializeField] private TMP_Text _label;
	[SerializeField] private Button _button;
	[SerializeField] private TintOnSelect _selection;

	public event Action<PassiveSlotView> OnSelected;

	public Ability.AbilityType PassiveType { get; private set; }

	public void Refresh(CharacterBuild character, Ability.AbilityType type) {
		PassiveType = type;
		SetLabel(character.GetPassive(type), type);
	}

	public void Deselect() {
		_selection.Deselect();
	}

	private void OnEnable() {
		_button.onClick.AddListener(OnSelectClicked);
	}

	private void OnDisable() {
		_button.onClick.RemoveListener(OnSelectClicked);
	}

	private void SetLabel(Ability current, Ability.AbilityType type) {
		string currentName = current?.Name ?? "<None>";
		_label.SetText($"{type}: {currentName}");
	}

	private void OnSelectClicked() {
		if (!_selection.Selected) {
			_selection.Select();
			OnSelected?.Invoke(this);
		}
	}
}
