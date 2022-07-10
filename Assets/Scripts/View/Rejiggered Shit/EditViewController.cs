using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditViewController : MonoBehaviour {
	[SerializeField] private DataController _dataController;
	[SerializeField] private TMP_Dropdown _gameDropdown;
	[SerializeField] private CharacterListView _characterListView;
	[SerializeField] private JobSelectionView _jobSelectionView;
	[SerializeField] private PassiveSelectionView _passiveSelectionView;
	[SerializeField] private AbilityListView _abilityListView;
	// [SerializeField] private CharacterBuildView _buildView;

	private void Awake() {
		
	}

	private void OnEnable() {
		_gameDropdown.onValueChanged.AddListener(OnSelectedGameChanged);
		_characterListView.OnCharacterSelected += OnCharacterSelected;
	}

	private void OnDisable() {
		_gameDropdown.onValueChanged.RemoveListener(OnSelectedGameChanged);
		_characterListView.OnCharacterSelected -= OnCharacterSelected;
	}

	private void OnSelectedGameChanged(int selectedIndex) {
		_dataController.Load(selectedIndex);
		_characterListView.Refresh(_dataController.Characters);
		_jobSelectionView.Clear();
		_abilityListView.Clear();
		_passiveSelectionView.Clear();
	}

	private void OnCharacterSelected(CharacterBuild selectedCharacter) {
		_jobSelectionView.Refresh(selectedCharacter);
		_passiveSelectionView.Refresh(selectedCharacter);
		_abilityListView.Clear();
	}
}
