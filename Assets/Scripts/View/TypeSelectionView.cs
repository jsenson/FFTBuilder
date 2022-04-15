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

	public void Initialize(List<string> gameNames) {
		_gameDropdown.ClearOptions();
		_gameDropdown.AddOptions(gameNames);
	}

	private void Awake() {
		var typeNames = Enum.GetNames(typeof(UnitType)).Where(s => !s.Equals("None")).ToList();
		_gameDropdown.ClearOptions();
		_typeDropdown.ClearOptions();
		_typeDropdown.AddOptions(typeNames);
		_typeDropdown.onValueChanged.AddListener(OnTypeChange);
		_gameDropdown.onValueChanged.AddListener(i => onGameValueChanged?.Invoke(i));
	}

	private void OnTypeChange(int newIndex) {
		// Lazy, just +1 the index since None is at index 0
		onTypeValueChanged?.Invoke((UnitType)(newIndex + 1));
	}
}
