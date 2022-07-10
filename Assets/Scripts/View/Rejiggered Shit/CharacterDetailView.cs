using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetailView : MonoBehaviour {
	[SerializeField] private TMP_InputField _nameInput;
	[SerializeField] private TMP_Dropdown _typeDropdown;
	[SerializeField] private Button _deleteButton;

	public event Action<CharacterBuild> OnDeletePressed;

	private CharacterBuild _character;

	public void Refresh(CharacterBuild character) {
		var typeNames = Enum.GetNames(typeof(UnitType)).Where(t => t != "Human").ToList();
		_character = character;
		_typeDropdown.SetValueWithoutNotify((int)(_character.Type - 1));
		_nameInput.text = _character.Name;
	}

	private void Awake() {
		var typeNames = Enum.GetNames(typeof(UnitType)).Where(t => t != "Human").ToList();
		_typeDropdown.ClearOptions();
		_typeDropdown.AddOptions(typeNames);
	}

	private void OnEnable() {
		_nameInput.onValueChanged.AddListener(OnNameChanged);
		_typeDropdown.onValueChanged.AddListener(OnTypeChanged);
		_deleteButton.onClick.AddListener(OnDeleteButtonPressed);
	}

	private void OnDisable() {
		_nameInput.onValueChanged.RemoveListener(OnNameChanged);
		_typeDropdown.onValueChanged.RemoveListener(OnTypeChanged);
		_deleteButton.onClick.RemoveListener(OnDeleteButtonPressed);
	}

	private void OnNameChanged(string value) {
		_character.SetName(value);
	}

	private void OnTypeChanged(int newIndex) {
		// Offset by one because we strip the "Human" value at pos 0 and I'm too lazy to waste time calculating it correctly
		_character.SetType((UnitType)(newIndex + 1));
	}

	private void OnDeleteButtonPressed() {
		OnDeletePressed?.Invoke(_character);
	}
}
