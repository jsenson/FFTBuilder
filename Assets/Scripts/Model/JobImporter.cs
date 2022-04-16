using System;
using System.Collections.Generic;
using UnityEngine;

public class JobImporter {
	private const int REFERENCE = 0;
	private const int NAME = 1;
	private const int ABILITY_NAME = 2;
	private const int REQUIREMENTS = 3;
	private const int UNIT_TYPE = 4;
	private const int SLOT = 5;
	private const int SUBJOB_COUNT = 6;
	private const int UNIQUE_UNIT = 7;
	private const int COLUMNS = 8;

	private Dictionary<string, Job> _records;

	public JobImporter() {
		_records = new Dictionary<string, Job>();
	}

	public void PrintRecords() {
		foreach (var record in _records) {
			Debug.Log($"{record.Key}: {record.Value}");
		}
	}

	public void Load(TextAsset source) {
		Unload();
		string[][] data = GetRawRecords(source.text);
		foreach (string[] line in data) {
			var job = ParseJob(line);
			if (_records.ContainsKey(job.Reference)) {
				Debug.Log($"Duplicate Job Reference {job.Reference} found.");
			} else {
				_records[job.Reference] = job;
			}
		}

		foreach (string[] line in data) {
			Job job = Get(line[REFERENCE]);
			string entry = line[REQUIREMENTS];
			ParseRequirementsEntry(job, entry);
		}
	}

	public void Unload() {
		_records.Clear();
	}

	public Job Get(string reference) {
		_records.TryGetValue(reference, out Job record);
		return record;
	}

	public Dictionary<string, Job> GetAllByReference() {
		return new Dictionary<string, Job>(_records);
	}

	public List<Job> GetAll(Func<Job, bool> predicate = null) {
		var jobs = new List<Job>();
		foreach (var kvp in _records) {
			if (predicate == null || predicate(kvp.Value)) {
				jobs.Add(kvp.Value);
			}
		}

		return jobs;
	}

	private string[][] GetRawRecords(string source) {
		string[] lines = source.Split('\n');
		string[][] data = new string[lines.Length][];
		for (int i = 0; i < lines.Length; i++) {
			string[] values = lines[i].Split('\t');
			values[values.Length - 1] = values[values.Length - 1].Trim();
			if (values.Length != COLUMNS) {
				throw new ArgumentException($"Job Record {i + 1} contains {values.Length} columns but expected {COLUMNS}");
			}

			data[i] = values;
		}

		return data;
	}

	private Job ParseJob(string[] entries) {
		Job job = new Job();
		job.Reference = entries[REFERENCE];
		job.Name = entries[NAME];
		job.AbilityName = entries[ABILITY_NAME];
		job.UniqueCharacterName = entries[UNIQUE_UNIT];
		job.UnitType = (UnitType)Enum.Parse(typeof(UnitType), entries[UNIT_TYPE]);
		job.ValidSlots = (Job.SlotRestriction)Enum.Parse(typeof(Job.SlotRestriction), entries[SLOT]);
		job.NumSubjobs = int.Parse(entries[SUBJOB_COUNT]);
		return job;
	}

	private void ParseRequirementsEntry(Job job, string entry) {
		if (string.IsNullOrEmpty(entry)) {
			return;
		}

		string[] reqs = entry.Split('|');
		foreach (string req in reqs) {
			string[] values = req.Split('-');
			var newReq = new Job.Requirement();
			newReq.Level = int.Parse(values[0]);
			newReq.Job = Get(values[1]);
			if (newReq.Job == null) {
				Debug.LogError($"Could not find loaded Job for Requirement: {reqs}");
			} else {
				job.AddUnlockRequirement(newReq);
			}
		}
	}
}
