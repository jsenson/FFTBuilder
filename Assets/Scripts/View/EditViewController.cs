using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EditViewController : MonoBehaviour {
	[SerializeField] private DataController _dataController;
	[SerializeField] private GameSelectionView _gameSelectView;
	[SerializeField] private CharacterListView _characterListView;
	[SerializeField] private JobSelectionView _jobSelectionView;
	[SerializeField] private PassiveSelectionView _passiveSelectionView;
	[SerializeField] private AbilityListView _abilityListView;
	[SerializeField] private VerticalLayoutGroup _fucker;
	[SerializeField] private Button _saveButton;
	[SerializeField] private Button _loadButton;
	[SerializeField] private Button _exportButton;

	private void Start() {
		_dataController.Load(0);
		_dataController.Characters.Add(CharacterBuild.GetDefault(_dataController.JobImporter, _dataController.AbilityImporter));
		_gameSelectView.Initialize(_dataController.GetGameNames());
		RefreshCharacterListView();
	}

	private void OnEnable() {
		_saveButton.onClick.AddListener(OnSaveButtonClicked);
		_loadButton.onClick.AddListener(OnLoadButtonClicked);
		_exportButton.onClick.AddListener(OnExportButtonClicked);
		_gameSelectView.OnSelectedGameChanged += OnSelectedGameChanged;
		_characterListView.OnCharacterSelected += OnCharacterSelected;
		_characterListView.OnCharacterCreated += OnCharacterCreated;
		_characterListView.OnCharacterDeleted += OnCharacterDeleted;
		_jobSelectionView.OnShowJobDetailsClicked += OnJobSlotClicked;
		_passiveSelectionView.OnSlotSelected += OnPassiveSlotClicked;
	}

	private void OnDisable() {
		_saveButton.onClick.RemoveListener(OnSaveButtonClicked);
		_loadButton.onClick.RemoveListener(OnLoadButtonClicked);
		_exportButton.onClick.RemoveListener(OnExportButtonClicked);
		_gameSelectView.OnSelectedGameChanged -= OnSelectedGameChanged;
		_characterListView.OnCharacterSelected -= OnCharacterSelected;
		_characterListView.OnCharacterCreated -= OnCharacterCreated;
		_characterListView.OnCharacterDeleted -= OnCharacterDeleted;
		_jobSelectionView.OnShowJobDetailsClicked -= OnJobSlotClicked;
		_passiveSelectionView.OnSlotSelected -= OnPassiveSlotClicked;
	}

	private void OnSelectedGameChanged(int selectedIndex) {
		_dataController.Load(selectedIndex);
		RefreshCharacterListView();
		ClearSelectedSubViews();
	}

	private void OnCharacterSelected(CharacterBuild selectedCharacter) {
		ValidateCharacter(selectedCharacter);
		_jobSelectionView.Refresh(selectedCharacter);
		_passiveSelectionView.Refresh(selectedCharacter);
		_abilityListView.Clear();
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
		_abilityListView.Refresh(new AbilityListView.Data() {
			Character = character,
			Type = type,
			Job = job
		});
	}

	private void RefreshCharacterListView() {
		_characterListView.Refresh(new CharacterListView.Data() {
			Characters = _dataController.Characters,
			JobImporter = _dataController.JobImporter,
			AbilityImporter = _dataController.AbilityImporter
		});
	}

	private void ClearSelectedSubViews() {
		_jobSelectionView.Clear();
		_passiveSelectionView.Clear();
		_abilityListView.Clear();
	}

	private void ValidateCharacter(CharacterBuild character) {
		if (character.MainJob == null) {
			character.SetMainJob(character.GetMainJobList()[0]);
		}
	}

	private void OnSaveButtonClicked() {
		var dialog = new FileDialog(_dataController.JobImporter, _dataController.AbilityImporter);
		StartCoroutine(dialog.SaveAsync(
			gameIndex: _dataController.LoadedIndex,
			characters: _dataController.Characters.ToArray(),
			onSuccess: null,
			onCancel: null)
		);
	}

	private void OnLoadButtonClicked() {
		var dialog = new FileDialog(_dataController.JobImporter, _dataController.AbilityImporter);
		dialog.OnReadGameIndexFromFile += (gameIndex) => {
			_dataController.Unload();
			_dataController.Load(gameIndex);
			dialog.UpdateImporters(_dataController.JobImporter, _dataController.AbilityImporter);
		};

		StartCoroutine(dialog.LoadAsync(OnCharacterFileLoaded, null));
	}

	private void OnCharacterFileLoaded(int gameIndex, CharacterBuild[] loadedCharacters) {
		_dataController.Characters.AddRange(loadedCharacters);
		_gameSelectView.SelectGameIndex(gameIndex, false);
		RefreshCharacterListView();
		ClearSelectedSubViews();
	}

	private void OnExportButtonClicked() {
		var exporter = new SheetExporter();
		StartCoroutine(exporter.ExportAsync(_dataController.Characters.ToArray(), null, null));
	}

	private IEnumerator FixTheAssholeLayout() {
		// Vertical layout is being a bitch and won't detect the height changes in children when _passiveSelectionView is Refreshed...
		_fucker.childScaleHeight = true;
		yield return new WaitForEndOfFrame();
		_fucker.childScaleHeight = false;
	}
}
