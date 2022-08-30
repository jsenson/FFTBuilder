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

	public void Refresh(Ability ability) {
		Ability = ability;
		_nameLabel.text = ability.Name;
		_descriptionLabel.text = ability.Description;
		_costLabel.text = $"{ability.Cost:N0} JP";
	}

	private void OnEnable() {
		_selectedToggle.onValueChanged.AddListener(OnToggleChanged);
	}

	private void OnDisable() {
		_selectedToggle.onValueChanged.AddListener(OnToggleChanged);
	}

	private void OnToggleChanged(bool active) {
		OnToggled?.Invoke(this);
	}
}
