using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityImporter {
	private const int REFERENCE = 0;
	private const int JOB = 1;
	private const int TYPE = 2;
	private const int NAME = 3;
	private const int DESCRIPTION = 4;
	private const int COST = 5;
	private const int CALC = 6;
	private const int COLUMNS = 7;

	private readonly Dictionary<string, Record> _records;

	public AbilityImporter() {
		_records = new Dictionary<string, Record>();
	}

	public void PrintRecords() {
		foreach (var record in _records) {
			Debug.Log($"{record.Key}: {record.Value}");
		}
	}

	public void Load(TextAsset source, Dictionary<string, Job> allJobs) {
		Unload();
		string[] lines = source.text.Split('\n');
		foreach (string line in lines) {
			if (string.IsNullOrWhiteSpace(line)) {
				continue;
			}

			string[] entries = SplitLine(line);
			var newRecord = new Record(entries, allJobs);
			if (newRecord.Ability != null) {
				if (_records.ContainsKey(newRecord.Ability.Reference)) {
					Debug.Log($"Duplicate Ability Reference {newRecord.Ability.Reference} found.");
				} else {
					_records[newRecord.Ability.Reference] = newRecord;
				}
			}
		}
	}

	public void Unload() {
		foreach (var record in _records) {
			record.Value.Job.RemoveAbility(record.Value.Ability);
		}

		_records.Clear();
	}

	public Ability Get(string reference) {
		_records.TryGetValue(reference, out Record record);
		return record?.Ability;
	}

	public Job GetSourceJob(string reference) {
		_records.TryGetValue(reference, out Record record);
		return record?.Job;
	}

	public List<Ability> FindAll(Func<Ability, Job, bool> predicate = null) {
		var abilities = new List<Ability>();
		foreach (var record in _records) {
			if (predicate == null || predicate(record.Value.Ability, record.Value.Job)) {
				abilities.Add(record.Value.Ability);
			}
		}

		return abilities;
	}

	public List<Ability> FindAllCalculatorSpells() {
		return FindAll((a, _) => a.IsCalculatorSpell);
	}

	private string[] SplitLine(string line) {
		string[] entries = line.Split('\t');
		if (entries.Length != COLUMNS) {
			Debug.LogError($"Record contained {entries.Length} entries but expected {COLUMNS}");
		}

		entries[entries.Length - 1] = entries[entries.Length - 1].Trim();
		return entries;
	}

	private class Record {
		public Ability Ability;
		public Job Job;

		public Record(string[] entries, Dictionary<string, Job> allJobs) {
			Ability = null;
			Job = null;

			if (entries.Length == COLUMNS) {
				allJobs.TryGetValue(entries[JOB], out Job);
				if (Job != null) {
					Ability = new Ability();
					Ability.Reference = entries[REFERENCE];
					Ability.Name = entries[NAME];
					Ability.Job = Job;
					Ability.Description = entries[DESCRIPTION];
					if (Enum.TryParse(entries[TYPE], out Ability.AbilityType type)) Ability.Type = type;
					if (Int32.TryParse(entries[COST], out int cost)) Ability.Cost = cost;
					if (Int32.TryParse(entries[CALC], out int calc)) Ability.IsCalculatorSpell = Convert.ToBoolean(calc);
					Job.AddAbility(Ability);

					return;
				} else {
					Debug.LogError($"Failed to find loaded Job with Reference: {entries[JOB]}.  Record will not be saved.");
				}
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
