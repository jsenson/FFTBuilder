using System;
using UnityEngine;

public class DataController : MonoBehaviour {
	[SerializeField] private SourceFiles[] _sources;

	public SourceFiles[] Sources => _sources;

	private SourceFiles _loadedSource;
	private JobImporter _jobImporter = new JobImporter();
	private AbilityImporter _abilityImporter = new AbilityImporter();

	public void Start() {
		Load(_sources[0]);

		// Yeah... super inefficient here.
		foreach (var job in _jobImporter.GetAll()) {
			Debug.Log(job.Name);
			foreach (var abilityType in (Ability.AbilityType[])Enum.GetValues(typeof(Ability.AbilityType))) {
				Debug.Log(abilityType.ToString());
				foreach (var ability in _abilityImporter.GetAll((a, j) => a.Type == abilityType && j.Name == job.Name)) {
					Debug.Log($"  {ability.Name}: {ability.Cost} - {ability.Description}");
				}
			}
		}
	}

	public void Load(SourceFiles source) {
		Unload();
		_jobImporter.Load(source.JobFile);
		_abilityImporter.Load(source.AbilityFile, _jobImporter.GetAll());
		_loadedSource = source;
	}

	private void Unload() {
		_abilityImporter.Unload();
		_jobImporter.Unload();
	}

	[Serializable]
	public struct SourceFiles {
		public string Name;
		public TextAsset JobFile;
		public TextAsset AbilityFile;
	}
}
