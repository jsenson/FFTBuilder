using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditViewController : MonoBehaviour {
	[SerializeField] private DataController _dataController;
	[SerializeField] private GameSelectionView _gameSelectView;
	[SerializeField] private CharacterListView _characterListView;
	[SerializeField] private JobSelectionView _jobSelectionView;
	[SerializeField] private PassiveSelectionView _passiveSelectionView;
	[SerializeField] private AbilityListView _abilityListView;

	private void Start() {
		_gameSelectView.Initialize(_dataController.GetGameNames());
	}

	private void OnEnable() {
		_gameSelectView.OnSelectedGameChanged += OnSelectedGameChanged;
		_characterListView.OnCharacterSelected += OnCharacterSelected;
		_characterListView.OnCharacterCreated += OnCharacterCreated;
		_characterListView.OnCharacterDeleted += OnCharacterDeleted;
	}

	private void OnDisable() {
		_gameSelectView.OnSelectedGameChanged -= OnSelectedGameChanged;
		_characterListView.OnCharacterSelected -= OnCharacterSelected;
		_characterListView.OnCharacterCreated -= OnCharacterCreated;
		_characterListView.OnCharacterDeleted -= OnCharacterDeleted;
	}

	private void OnSelectedGameChanged(int selectedIndex) {
		_dataController.Load(selectedIndex);
		_characterListView.Refresh(_dataController.Characters);
		ClearSelectedSubViews();
	}

	private void OnCharacterSelected(CharacterBuild selectedCharacter) {
		_jobSelectionView?.Refresh(selectedCharacter);
		_passiveSelectionView?.Refresh(selectedCharacter);
		_abilityListView?.Clear();
	}

	private void OnCharacterCreated(CharacterBuild character) {
		_dataController.Characters.Add(character);
	}

	private void OnCharacterDeleted(CharacterBuild character, bool selected) {
		_dataController.Characters.Remove(character);
		if (selected) {
			ClearSelectedSubViews();
		}
	}

	private void ClearSelectedSubViews() {
		_jobSelectionView?.Clear();
		_passiveSelectionView?.Clear();
		_abilityListView?.Clear();
	}
}
