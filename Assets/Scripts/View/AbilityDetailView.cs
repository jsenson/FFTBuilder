using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDetailView : MonoBehaviour {
	[SerializeField] private TMP_Text _nameLabel;
	[SerializeField] private TMP_Text _descriptionLabel;
	[SerializeField] private TMP_Text _costLabel;
	[SerializeField] private Toggle _selectedToggle;

	public event Action<AbilityDetailView> OnToggled;

	public Ability Ability { get; private set; }

	public bool Selected => _selectedToggle.isOn;

	public void Refresh(Ability ability, bool selected) {
		Ability = ability;
		_nameLabel.text = ability.Name;
		_descriptionLabel.text = ability.Description;
		_costLabel.text = $"{ability.Cost:N0} JP";
		_selectedToggle.SetIsOnWithoutNotify(selected);
	}

	public void Deselect(bool fireEvent = true) {
		if (fireEvent) {
			_selectedToggle.isOn = false;
		} else {
			_selectedToggle.SetIsOnWithoutNotify(false);
		}
	}

	private void OnEnable() {
		_selectedToggle.onValueChanged.AddListener(OnToggleChanged);
	}

	private void OnDisable() {
		_selectedToggle.onValueChanged.AddListener(OnToggleChanged);
	}

	private void OnToggleChanged(bool active) {
		if (Ability.Type != Ability.AbilityType.Class && !active) {
			_selectedToggle.SetIsOnWithoutNotify(true);
		} else {
			OnToggled?.Invoke(this);
		}
	}
}
