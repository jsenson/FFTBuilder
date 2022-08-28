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
		Debug.Log($"Select {selectedCharacter.Name}");
		ValidateCharacter(selectedCharacter);
		_jobSelectionView.Refresh(new JobSelectionView.Data(selectedCharacter, _dataController.JobImporter));
		_passiveSelectionView?.Refresh(selectedCharacter);
		_abilityListView?.Clear();
	}

	private void OnCharacterCreated(CharacterBuild character) {
		ValidateCharacter(character);
		_dataController.Characters.Add(character);
	}

	private void OnCharacterDeleted(CharacterBuild character, bool selected) {
		_dataController.Characters.Remove(character);
		if (selected) {
			ClearSelectedSubViews();
		}
	}

	private void ClearSelectedSubViews() {
		_jobSelectionView.Clear();
		_passiveSelectionView?.Clear();
		_abilityListView?.Clear();
	}

	private void ValidateCharacter(CharacterBuild character) {
		if (character.MainJob == null) {
			character.SetMainJob(character.GetMainJobList(_dataController.JobImporter)[0], _dataController.JobImporter);
		}
	}
}
