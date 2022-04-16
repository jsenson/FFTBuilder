using System;
using System.Linq;
using UnityEngine;

public class DataController : MonoBehaviour {
	[SerializeField] private SourceFiles[] _sources;
	[SerializeField] private TypeSelectionView _selectionView;
	[SerializeField] private JobSelectionView _jobSelectView;

	public SourceFiles[] Sources => _sources;

	private string _loadedSource;
	private JobImporter _jobImporter = new JobImporter();
	private AbilityImporter _abilityImporter = new AbilityImporter();

	public void Awake() {
		Load(_sources[0]);
		_selectionView.Initialize(_sources.Select(s => s.Name).ToList());
		InitJobSelectionView(_selectionView.SelectedType);
	}

	public void Load(SourceFiles source) {
		Unload();
		_jobImporter.Load(source.JobFile);
		_abilityImporter.Load(source.AbilityFile, _jobImporter.GetAll());
		_loadedSource = source.Name;
	}

	private void OnEnable() {
		_selectionView.onGameValueChanged += OnGameChanged;
		_selectionView.onTypeValueChanged += OnTypeChanged;
	}

	private void OnDisable() {
		_selectionView.onGameValueChanged += OnGameChanged;
		_selectionView.onTypeValueChanged += OnTypeChanged;
	}

	private void Unload() {
		_abilityImporter.Unload();
		_jobImporter.Unload();
		_loadedSource = null;
	}

	private void OnGameChanged(int newIndex) {
		Debug.Log($"OnGameChanged: {newIndex}");
		var source = _sources[newIndex];
		if (_loadedSource != source.Name) {
			Load(source);
			Debug.Log($"Loaded {source.Name}");
		}
	}

	private void OnTypeChanged(UnitType newType) {
		Debug.Log($"OnTypeChanged: {newType}");
		InitJobSelectionView(newType);
	}

	private void InitJobSelectionView(UnitType unitType) {
		var mainJobs = _jobImporter.GetAll(
			job => job.IsUnitType(unitType)
			&& (job.ValidSlots == Job.SlotRestriction.Main 
			|| job.ValidSlots == Job.SlotRestriction.Both)
		);

		var subJobs = _jobImporter.GetAll(
			job => job.IsUnitType(unitType) 
			&& (job.ValidSlots == Job.SlotRestriction.Sub 
			|| job.ValidSlots == Job.SlotRestriction.Both)
		);

		mainJobs.Sort((a, b) => a.Name.CompareTo(b.Name));
		subJobs.Sort((a, b) => a.Name.CompareTo(b.Name));
		_jobSelectView.Initialize(new JobSelectionView.Data() {
			MainJobOptions = mainJobs,
			SubJobOptions = subJobs
		});
	}

	[Serializable]
	public struct SourceFiles {
		public string Name;
		public TextAsset JobFile;
		public TextAsset AbilityFile;
	}
}
