using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AbilityImporter {
	private const int COLUMNS = 5;

	private List<Record> _records;

	public AbilityImporter() {
		_records = new List<Record>();
	}

	public void PrintRecords() {
		foreach (var record in _records) {
			Debug.Log(record.ToString());
		}
	}

	public void Load(TextAsset source, List<Job> jobList) {
		Unload();
		string[] lines = source.text.Split('\n');
		foreach (string line in lines) {
			var newRecord = new Record(line.Trim(), jobList);
			if (newRecord.Ability != null) {
				_records.Add(newRecord);
			}
		}
	}

	public void Unload() {
		foreach (var record in _records) {
			record.Job.RemoveAbility(record.Ability);
		}

		_records.Clear();
	}

	public List<Ability> GetAll(Func<Ability, Job, bool> predicate = null) {
		var abilities = new List<Ability>();
		foreach (var record in _records) {
			if (predicate == null || predicate(record.Ability, record.Job)) {
				abilities.Add(record.Ability);
			}
		}

		return abilities;
	}

	private class Record {
		public Ability Ability;
		public Job Job;

		public Record(string record, List<Job> jobList) {
			Ability = null;
			Job = null;

			string[] entries = record.Split('\t');
			if (entries.Length == COLUMNS) {
				Job = jobList.Find(job => job.Name == entries[0]);
				if (Job != null) {
					Ability = new Ability();
					Ability.Name = entries[2];
					Ability.Description = entries[3];
					if (Enum.TryParse(entries[1], out Ability.AbilityType type)) Ability.Type = type;
					if (Int32.TryParse(entries[4], out int cost)) Ability.Cost = cost;
					Job.AddAbility(Ability);
					return;
				} else {
					Debug.LogError($"Failed to find loaded Job with name: {entries[0]}.  Record will not be saved.");
				}
			} else {
				Debug.LogError($"Record contained {entries.Length} entries but expected {COLUMNS}");
			}
		}

		public override string ToString() {
			if (Ability == null) {
				return string.Empty;
			} else {
				return $"{Job?.Name} - {Ability.Type} - {Ability.Name} - {Ability.Description} - {Ability.Cost}";
			}
		}
	}
}
