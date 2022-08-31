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

	private CharacterBuild _character;
	private bool _eventsSubscribed = false;

	public void Refresh(CharacterBuild character, Ability.AbilityType type) {
		UnsubscribeCharacterEvents();
		_character = character;
		PassiveType = type;
		SetLabel(character.GetPassive(type), type);
		SubscribeCharacterEvents();
	}

	public void Deselect() {
		_selection.Deselect();
	}

	private void OnEnable() {
		SubscribeCharacterEvents();
		_button.onClick.AddListener(OnSelectClicked);
	}

	private void OnDisable() {
		UnsubscribeCharacterEvents();
		_button.onClick.RemoveListener(OnSelectClicked);
	}

	private void SubscribeCharacterEvents() {
		if (_character != null && !_eventsSubscribed) {
			_character.OnPassiveChanged += OnPassiveSet;
			_eventsSubscribed = true;
		}
	}

	private void UnsubscribeCharacterEvents() {
		if (_character != null && _eventsSubscribed) {
			_character.OnPassiveChanged -= OnPassiveSet;
			_eventsSubscribed = false;
		}
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

	private void OnPassiveSet(CharacterBuild sender, Ability.AbilityType type) {
		if (type == PassiveType) {
			SetLabel(sender.GetPassive(type), type);
		}
	}
}
