using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSelectionView : MonoBehaviour {
	[SerializeField] private TMP_Dropdown _gameDropdown;

	public event Action<int> OnSelectedGameChanged;

	public string SelectedGameName => _gameNames[_gameDropdown.value];

	private List<string> _gameNames;

	public void Initialize(List<string> gameNames) {
		_gameNames = gameNames;
		_gameDropdown.ClearOptions();
		_gameDropdown.AddOptions(gameNames);
	}

	public void SelectGameIndex(int gameIndex, bool triggerEvents = true) {
		if (triggerEvents) {
			_gameDropdown.value = gameIndex;
		} else {
			_gameDropdown.SetValueWithoutNotify(gameIndex);
		}
	}

	private void Awake() {
		_gameDropdown.onValueChanged.AddListener(OnGameChange);
	}

	private void OnGameChange(int newIndex) {
		OnSelectedGameChanged?.Invoke(newIndex);
	}
}
