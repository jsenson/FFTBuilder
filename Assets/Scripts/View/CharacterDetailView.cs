using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetailView : MonoBehaviour {
	[SerializeField] private TintOnSelect _selection;
	[SerializeField] private TMP_InputField _nameInput;
	[SerializeField] private TMP_Dropdown _typeDropdown;
	[SerializeField] private Button _selectButton;
	[SerializeField] private DeleteButton _deleteButton;

	public event Action<CharacterDetailView> OnDeletePressed;
	public event Action<CharacterDetailView> OnSelected;
	public event Action<CharacterDetailView> OnTypeChanged;

	public CharacterBuild Character { get; private set; }

	public bool Selected => _selection.Selected;

	public void Refresh(CharacterBuild character) {
		var typeNames = Enum.GetNames(typeof(UnitType)).Where(t => t != "Human").ToList();
		Character = character;
		_typeDropdown.SetValueWithoutNotify((int)(Character.Type - 1));
		_nameInput.text = Character.Name;
	}

	public void Select() {
		_selection.Select();
	}

	public void Deselect() {
		_selection.Deselect();
	}

	private void Awake() {
		var typeNames = Enum.GetNames(typeof(UnitType)).Where(t => t != "Human").ToList();
		_typeDropdown.ClearOptions();
		_typeDropdown.AddOptions(typeNames);
		_deleteButton.SetAction(OnDeleteButtonPressed);
	}

	private void OnEnable() {
		_nameInput.onValueChanged.AddListener(OnNameChanged);
		_typeDropdown.onValueChanged.AddListener(OnTypeSelected);
		_selectButton.onClick.AddListener(OnSelectButonPressed);
	}

	private void OnDisable() {
		_nameInput.onValueChanged.RemoveListener(OnNameChanged);
		_typeDropdown.onValueChanged.RemoveListener(OnTypeSelected);
		_selectButton.onClick.RemoveListener(OnSelectButonPressed);
	}

	private void OnNameChanged(string value) {
		Character.SetName(value);
	}

	private void OnTypeSelected(int newIndex) {
		// Offset by one because we strip the "Human" value at pos 0 and I'm too lazy to waste time calculating it correctly
		Character.SetType((UnitType)(newIndex + 1));
		OnTypeChanged?.Invoke(this);
	}

	private void OnDeleteButtonPressed() {
		OnDeletePressed?.Invoke(this);
	}

	private void OnSelectButonPressed() {
		if (!_selection.Selected) {
			_selection.Select();
			OnSelected?.Invoke(this);
		}
	}
}
