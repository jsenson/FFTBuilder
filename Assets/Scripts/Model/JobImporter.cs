using System;
using System.Collections.Generic;
using UnityEngine;

public class JobImporter {
	private const int NAME = 0;
	private const int ABILIY_NAME = 1;
	private const int REQUIREMENTS = 2;
	private const int UNIT_TYPE = 3;
	private const int SLOT = 4;
	private const int SUBJOB_COUNT = 5;
	private const int UNIQUE_UNIT = 6;
	private const int COLUMNS = 7;

	private List<Job> _records;

	public JobImporter() {
		_records = new List<Job>();
	}

	public void PrintRecords() {
		foreach (var record in _records) {
			Debug.Log(record.ToString());
		}
	}

	public void Load(TextAsset source) {
		Unload();
		var jobsByName = new Dictionary<string, Job>();
		string[][] data = GetRawRecords(source.text);
		foreach (string[] line in data) {
			var job = ParseJob(line);
			jobsByName[job.Name] = job;
			_records.Add(job);
		}

		foreach (string[] line in data) {
			Job job = jobsByName[line[NAME]];
			string entry = line[REQUIREMENTS];
			ParseRequirementsEntry(job, jobsByName, entry);
		}
	}

	public void Unload() {
		_records.Clear();
	}

	public List<Job> GetAll(Func<Job, bool> predicate = null) {
		var jobs = new List<Job>();
		foreach (var record in _records) {
			if (predicate == null || predicate(record)) {
				jobs.Add(record);
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
		var type = (UnitType)Enum.Parse(typeof(UnitType), entries[UNIT_TYPE]);
		var slot = (Job.SlotRestriction)Enum.Parse(typeof(Job.SlotRestriction), entries[SLOT]);
		int subJobCount = int.Parse(entries[SUBJOB_COUNT]);
		return new Job(entries[NAME], entries[ABILIY_NAME], type, slot, subJobCount, entries[UNIQUE_UNIT]);
	}

	private void ParseRequirementsEntry(Job job, Dictionary<string, Job> jobsByName, string entry) {
		if (string.IsNullOrEmpty(entry)) {
			return;
		}

		string[] reqs = entry.Split('|');
		foreach (string req in reqs) {
			string[] values = req.Split('-');
			var newReq = new Job.Requirement();
			newReq.Level = int.Parse(values[0]);
			newReq.Job = jobsByName[values[1]];
			if (newReq.Job == null) {
				Debug.LogError($"Could not find loaded Job for Requirement: {reqs}");
			} else {
				job.AddUnlockRequirement(newReq);
			}
		}
	}
}
