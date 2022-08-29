using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditViewController : MonoBehaviour {
	[SerializeField] private DataController _dataController;
	[SerializeField] private GameSelectionView _gameSelectView;
	[SerializeField] private CharacterListView _characterListView;
	[SerializeField] private JobSelectionView _jobSelectionView;
	[SerializeField] private PassiveSelectionView _passiveSelectionView;
	[SerializeField] private AbilityListView _abilityListView;
	[SerializeField] private VerticalLayoutGroup _fucker;

	private void Start() {
		_gameSelectView.Initialize(_dataController.GetGameNames());
	}

	private void OnEnable() {
		_gameSelectView.OnSelectedGameChanged += OnSelectedGameChanged;
		_characterListView.OnCharacterSelected += OnCharacterSelected;
		_characterListView.OnCharacterCreated += OnCharacterCreated;
		_characterListView.OnCharacterDeleted += OnCharacterDeleted;
		_jobSelectionView.OnShowJobDetailsClicked += OnJobSlotClicked;
		_passiveSelectionView.OnSlotSelected += OnPassiveSlotClicked;
	}

	private void OnDisable() {
		_gameSelectView.OnSelectedGameChanged -= OnSelectedGameChanged;
		_characterListView.OnCharacterSelected -= OnCharacterSelected;
		_characterListView.OnCharacterCreated -= OnCharacterCreated;
		_characterListView.OnCharacterDeleted -= OnCharacterDeleted;
		_jobSelectionView.OnShowJobDetailsClicked += OnJobSlotClicked;
		_passiveSelectionView.OnSlotSelected -= OnPassiveSlotClicked;
	}

	private void OnSelectedGameChanged(int selectedIndex) {
		_dataController.Load(selectedIndex);
		_characterListView.Refresh(_dataController.Characters);
		ClearSelectedSubViews();
	}

	private void OnCharacterSelected(CharacterBuild selectedCharacter) {
		ValidateCharacter(selectedCharacter);
		_jobSelectionView.Refresh(new JobSelectionView.Data(selectedCharacter, _dataController.JobImporter));
		_passiveSelectionView.Refresh(selectedCharacter);
		_abilityListView?.Clear();
		StartCoroutine(FixTheAssholeLayout());
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

	private void OnPassiveSlotClicked(PassiveSelectionView sender) {
		_jobSelectionView.ClearSelection();
		ShowAbilityView(sender.Character, sender.SelectedType);
	}

	private void OnJobSlotClicked(CharacterBuild character, Job job) {
		_passiveSelectionView.ClearSelection();
		ShowAbilityView(character, Ability.AbilityType.Class, job);
	}

	private void ShowAbilityView(CharacterBuild character, Ability.AbilityType type, Job job = null) {
		_abilityListView?.Refresh(new AbilityListView.Data() {
			Character = character,
			Type = type,
			Importer = _dataController.AbilityImporter,
			Job = job
		});
	}

	private void ClearSelectedSubViews() {
		_jobSelectionView.Clear();
		_passiveSelectionView.Clear();
		_abilityListView?.Clear();
	}

	private void ValidateCharacter(CharacterBuild character) {
		if (character.MainJob == null) {
			character.SetMainJob(character.GetMainJobList(_dataController.JobImporter)[0], _dataController.JobImporter);
		}
	}

	private IEnumerator FixTheAssholeLayout() {
		// Vertical layout is being a bitch and won't detect the height changes in children when _passiveSelectionView is Refreshed...
		_fucker.childScaleHeight = true;
		yield return new WaitForEndOfFrame();
		_fucker.childScaleHeight = false;
	}
}
