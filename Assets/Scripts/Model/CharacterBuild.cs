using System;
using System.Collections.Generic;

public class CharacterBuild {
	public event Action<CharacterBuild> OnNameChanged;
	public event Action<CharacterBuild> OnTypeChanged;
	public event Action<CharacterBuild> OnMainJobChanged;
	public event Action<CharacterBuild, int> OnSubJobChanged;
	public event Action<CharacterBuild, Ability.AbilityType> OnPassiveChanged;

	public string Name { get; private set; }
	public UnitType Type { get; private set; }
	public Job MainJob { get; private set; }

	private Job[] _subJobs = new Job[0];
	private Dictionary<Ability.AbilityType, Ability> _passives = new Dictionary<Ability.AbilityType, Ability>();

	public static CharacterBuild GetDefault(JobImporter jobImporter) {
		var defaultBuild = new CharacterBuild();
		defaultBuild.MainJob = defaultBuild.GetMainJobList(jobImporter)[0];
		var subJobs = defaultBuild.GetSubJobList(jobImporter);
		int subCount = subJobs.Count;
		defaultBuild._subJobs = new Job[defaultBuild.MainJob.NumSubjobs];
		for (int i = 0; i < defaultBuild.MainJob.NumSubjobs && i < subCount; i++) {
			defaultBuild._subJobs[i] = subJobs[i];
		}

		return defaultBuild;
	}

	public CharacterBuild() {
		Name = "<Name>";
		Type = UnitType.Male;
	}

	public void SetName(string newName) {
		if (newName != Name) {
			UnityEngine.Debug.Log($"{Name}: Set Name to '{newName}'");
			Name = newName;
			OnNameChanged?.Invoke(this);
		}
	}

	public void SetType(UnitType newType) {
		if (newType != Type) {
			Type = newType;
			MainJob = null;
			_subJobs = new Job[0];
			OnTypeChanged?.Invoke(this);
			UnityEngine.Debug.Log($"{Name}: Set Type to '{newType}'");
		}
	}

	public void SetMainJob(Job newJob, JobImporter importer) {
		if (newJob != MainJob) {
			MainJob = newJob;
			UpdateSubJobArrayLength(GetSubJobList(importer).ToArray());
			OnMainJobChanged?.Invoke(this);
			UnityEngine.Debug.Log($"{Name}: Set Main Job to '{MainJob}'");
		}
	}

	public Job[] GetCurrentSubJobs(Predicate<Job> predicate = null) {
		if (predicate == null) {
			var jobs = new Job[_subJobs.Length];
			Array.Copy(_subJobs, jobs, _subJobs.Length);
			return jobs;
		} else {
			return Array.FindAll(_subJobs, predicate);
		}
	}

	public Job GetSubJob(int index) {
		return index >= 0 && index < _subJobs.Length ? _subJobs[index] : null;
	}

	public void SetSubJob(int index, Job job) {
		if (index >= 0 && index < _subJobs.Length && _subJobs[index] != job) {
			_subJobs[index] = job;
			OnSubJobChanged?.Invoke(this, index);
			UnityEngine.Debug.Log($"{Name}: Set Sub Job {index} to '{job}'");
		}
	}

	public Ability GetPassive(Ability.AbilityType type) {
		return _passives.ContainsKey(type) ? _passives[type] : null;
	}

	public void SetPassive(Ability passive) {
		if (!_passives.ContainsKey(passive.Type) || _passives[passive.Type] != passive) {
			_passives[passive.Type] = passive;
			OnPassiveChanged?.Invoke(this, passive.Type);
		}
	}

	public List<Job> GetMainJobList(JobImporter importer, bool sortByName = true) {
		List<Job> jobs = importer.FindAll(j =>
			j.IsUnitType(Type)
			&& (j.ValidSlots == Job.SlotRestriction.Both || j.ValidSlots == Job.SlotRestriction.Main)
		);

		if (sortByName) {
			jobs.Sort((a, b) => a.Name.CompareTo(b.Name));
		}

		return jobs;
	}

	public List<Job> GetSubJobList(JobImporter importer, bool sortByName = true, int subJobSlot = -1) {
		if (MainJob == null) {
			throw new InvalidOperationException("Can not call GetSubJobList before setting a MainJob value.");
		}

		Job[] excludedJobs = GetRestrictedJobsForSubJobSlot(subJobSlot);
		bool includeNonGenerics = CanSelectGeneric(subJobSlot);
		List<Job> jobs = importer.FindAll(j =>
			j != MainJob
			&& j.IsUnitType(Type)
			&& (j.ValidSlots == Job.SlotRestriction.Both || j.ValidSlots == Job.SlotRestriction.Sub)
			&& (includeNonGenerics || j.IsGeneric)
			&& Array.Find(excludedJobs, ex => ex == j) == null
		);

		if (sortByName) {
			jobs.Sort((a, b) => a.Name.CompareTo(b.Name));
		}

		return jobs;
	}

	private bool CanSelectGeneric(int slotIndex) {
		bool canSelect = slotIndex < 0 || slotIndex >= _subJobs.Length;
		if (!canSelect) {
			canSelect = MainJob.IsGeneric;
			for (int i = 0; i < _subJobs.Length; i++) {
				if (i != slotIndex && !_subJobs[i].IsGeneric) {
					canSelect = false;
				}
			}
		}

		return canSelect;
	}

	private Job[] GetRestrictedJobsForSubJobSlot(int jobSlot) {
		if (jobSlot < 0 || jobSlot >= _subJobs.Length) {
			return new Job[0];
		}

		var jobs = new List<Job>() { MainJob };
		for (int i = 0; i < _subJobs.Length; i++) {
			if (i != jobSlot) {
				jobs.Add(_subJobs[i]);
			}
		}

		return jobs.ToArray();
	}

	private void UpdateSubJobArrayLength(Job[] subJobs) {
		int newLength = MainJob.NumSubjobs;
		if (newLength < _subJobs.Length) {
			Array.Resize(ref _subJobs, newLength);
		} else if (newLength > _subJobs.Length) {
			int i = _subJobs.Length;
			Array.Resize(ref _subJobs, newLength);
			for (int j = 0; j < subJobs.Length; j++) {
				if (MainJob != subJobs[j] && Array.Find(_subJobs, job => job == subJobs[j]) == null) {
					_subJobs[i] = subJobs[j];
					if (++i >= _subJobs.Length) {
						break;
					}
				}
			}
		}
	}
}
