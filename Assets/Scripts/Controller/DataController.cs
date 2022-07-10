using System;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour {
	[SerializeField] private SourceFiles[] _sources;

	public JobImporter JobImporter => _jobImporter;
	public AbilityImporter AbilityImporter => _abilityImporter;
	public List<CharacterBuild> Characters => _characters;

	private string _loadedSource;
	private readonly JobImporter _jobImporter = new JobImporter();
	private readonly AbilityImporter _abilityImporter = new AbilityImporter();
	private readonly List<CharacterBuild> _characters = new List<CharacterBuild>();

	private void Awake() {
		Load(_sources[0]);
		LoadCharacters();
	}

	public void Load(int gameIndex) {
		Load(_sources[Mathf.Clamp(gameIndex, 0, _sources.Length)]);
	}

	public void Unload() {
		_abilityImporter.Unload();
		_jobImporter.Unload();
		_characters.Clear();
		_loadedSource = null;
	}

	private void Load(SourceFiles source) {
		if (source.Name != _loadedSource) {
			Unload();
			_jobImporter.Load(source.JobFile);
			_abilityImporter.Load(source.AbilityFile, _jobImporter.GetAllByReference());
			_loadedSource = source.Name;
			LoadCharacters();
		}
	}

	// private void OnGameChanged(int newIndex) {
	// 	Debug.Log($"OnGameChanged: {newIndex}");
	// 	var source = _sources[newIndex];
	// 	if (_loadedSource != source.Name) {
	// 		Load(source);
	// 		Debug.Log($"Loaded {source.Name}");
	// 	}
	// }

	// private void OnTypeChanged(UnitType newType) {
	// 	Debug.Log($"OnTypeChanged: {newType}");
	// 	InitJobSelectionView(newType);
	// }

	// private void InitJobSelectionView(UnitType unitType) {
	// 	var mainJobs = _jobImporter.GetAll(
	// 		job => job.IsUnitType(unitType)
	// 		&& (job.ValidSlots == Job.SlotRestriction.Main 
	// 		|| job.ValidSlots == Job.SlotRestriction.Both)
	// 	);

	// 	var subJobs = _jobImporter.GetAll(
	// 		job => job.IsUnitType(unitType) 
	// 		&& (job.ValidSlots == Job.SlotRestriction.Sub 
	// 		|| job.ValidSlots == Job.SlotRestriction.Both)
	// 	);

	// 	mainJobs.Sort((a, b) => a.Name.CompareTo(b.Name));
	// 	subJobs.Sort((a, b) => a.Name.CompareTo(b.Name));
	// 	_jobSelectView.Initialize(new JobSelectionView.Data() {
	// 		MainJobOptions = mainJobs,
	// 		SubJobOptions = subJobs
	// 	});
	// }

	private void LoadCharacters() {
		// No saved char set loading yet.
		_characters.Add(CharacterBuild.GetDefault(_jobImporter));
	}

	[Serializable]
	public struct SourceFiles {
		public string Name;
		public TextAsset JobFile;
		public TextAsset AbilityFile;
	}
}
