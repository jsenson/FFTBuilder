using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class AbilityImportFile {
	private string _path;
	private List<Record> _records;

	public AbilityImportFile(string path) {
		_path = path;
		_records = new List<Record>();
		Read();
	}

	public void WriteScriptableObjects() {
		foreach (var record in _records) {
			record.SaveScriptableObject();
		}

		AssetDatabase.SaveAssets();
	}

	public void PrintRecords() {
		foreach (var record in _records) {
			Debug.Log(record.ToString());
		}
	}

	private void Read() {
		if (File.Exists(_path)) {
			_records.Clear();
			string[] lines = File.ReadAllLines(_path);
			foreach (string line in lines) {
				_records.Add(new Record(line));
			}
		} else {
			Debug.LogError($"Failed to find file: {_path}");
		}
	}

	private static void CreatePath(string path) {
		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
			Debug.LogWarning($"Created missing directory: {path}");
		}
	}

	private struct Record {
		public Job job;
		public Ability.Type abilityType;
		public string abilityName;
		public string description;
		public int jpCost;

		public Record(string record) {
			string[] entries = record.Split('\t');
			if (entries.Length == 5) {
				job = ScriptableObjectHelper.GetByName<Job>(entries[0]);
				if (job != null) {
					Enum.TryParse(entries[1], out abilityType);
					abilityName = entries[2];
					description = entries[3];
					Int32.TryParse(entries[4], out jpCost);
					return;
				} else {
					Debug.LogError($"Record contained invalid Job name: {entries[0]}.  Record will not be saved.");
				}
			} else {
				Debug.LogError($"Record contained {entries.Length} entries but expected 5");
			}

			job = null;
			abilityType = Ability.Type.Class;
			abilityName = null;
			description = null;
			jpCost = 0;
		}

		public void SaveScriptableObject() {
			if (job != null) {
				Ability ability = ScriptableObjectHelper.GetOrCreateAbility(abilityName, abilityType, job.name);
				ability.AbilityType = abilityType;
				ability.Description = description;
				ability.JpCost = jpCost;
				job.AddAbility(ability);
			}
		}

		public override string ToString() {
			return $"{job?.name} - {abilityType} - {abilityName} - {description} - {jpCost}";
		}
	}
}
