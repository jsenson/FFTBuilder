using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TypeSelectionView : MonoBehaviour {
	[SerializeField] private TMP_Dropdown _gameDropdown;
	[SerializeField] private TMP_Dropdown _typeDropdown;

	public event Action<int> onGameValueChanged;
	public event Action<UnitType> onTypeValueChanged;

	public UnitType SelectedType => (UnitType)(_typeDropdown.value + 1);
	public string SelectedGameName => _gameNames[_gameDropdown.value];

	private List<string> _gameNames;

	public void Initialize(List<string> gameNames) {
		_gameNames = gameNames;
		_gameDropdown.ClearOptions();
		_gameDropdown.AddOptions(gameNames);
	}

	private void Awake() {
		var typeNames = Enum.GetNames(typeof(UnitType)).Where(t => t != "Human").ToList();
		_typeDropdown.ClearOptions();
		_typeDropdown.AddOptions(typeNames);
		_typeDropdown.onValueChanged.AddListener(OnTypeChange);
		_gameDropdown.onValueChanged.AddListener(i => onGameValueChanged?.Invoke(i));
	}

	private void OnTypeChange(int newIndex) {
		onTypeValueChanged?.Invoke((UnitType)(newIndex + 1));
	}
}
