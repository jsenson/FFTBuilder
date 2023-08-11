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
	private HashSet<Ability> _classAbilities = new HashSet<Ability>();
	private JobImporter _jobImporter;
	private AbilityImporter _abilityImporter;

	public CharacterBuild(JobImporter jobImporter, AbilityImporter abilityImporter) {
		_jobImporter = jobImporter;
		_abilityImporter = abilityImporter;
		Name = "<Name>";
		Type = UnitType.Male;
	}

	public void Initialize() {
		MainJob = GetMainJobList()[0];
		AddClassAbilities(MainJob);

		var subJobs = GetSubJobList();
		int subCount = subJobs.Count;
		_subJobs = new Job[MainJob.NumSubjobs];
		for (int i = 0; i < MainJob.NumSubjobs && i < subCount; i++) {
			AddClassAbilities(subJobs[i]);
			_subJobs[i] = subJobs[i];
		}
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
			_classAbilities.Clear();
			OnTypeChanged?.Invoke(this);
			UnityEngine.Debug.Log($"{Name}: Set Type to '{newType}'");
		}
	}

	public void SetMainJob(Job newJob) {
		if (newJob != MainJob) {
			RemoveClassAbilities(MainJob);
			AddClassAbilities(newJob);
			MainJob = newJob;
			UpdateSubJobArrayLength(GetSubJobList().ToArray());
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
			RemoveClassAbilities(_subJobs[index]);
			AddClassAbilities(job);
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
			UnityEngine.Debug.Log($"{Name}: Set {passive.Type} Passive to '{passive.Name}'");
			OnPassiveChanged?.Invoke(this, passive.Type);
		}
	}

	public bool HasClassAbility(Ability ability) {
		return _classAbilities.Contains(ability);
	}

	public void AddClassAbilities(Job job) {
		foreach (var ability in job.GetAbilities(Ability.AbilityType.Class)) {
			AddClassAbility(ability);
		}
	}

	public void AddClassAbility(Ability ability) {
		if (ability.Type == Ability.AbilityType.Class) {
			_classAbilities.Add(ability);
			// UnityEngine.Debug.Log($"{Name}: Added Class ability '{ability.Name}'");
		} else {
			throw new ArgumentException($"AddClassAbility called with ability of type '{ability.Type}'.  Only AbilityType.Class is valid.");
		}
	}

	public void RemoveClassAbility(Ability ability) {
		_classAbilities.Remove(ability);
		// UnityEngine.Debug.Log($"{Name}: Removed Class ability '{ability.Name}'");
	}

	public List<Job> GetMainJobList() {
		List<Job> jobs = _jobImporter.FindAll(j =>
			j.IsUnitType(Type)
			&& (j.ValidSlots == Job.SlotRestriction.Both || j.ValidSlots == Job.SlotRestriction.Main)
		);

		jobs.Sort((a, b) => a.Name.CompareTo(b.Name));
		return jobs;
	}

	public List<Job> GetSubJobList(int subJobSlot = -1) {
		if (MainJob == null) {
			throw new InvalidOperationException("Can not call GetSubJobList before setting a MainJob value.");
		}

		Job[] excludedJobs = GetRestrictedJobsForSubJobSlot(subJobSlot);
		bool includeNonGenerics = CanSelectGeneric(subJobSlot);
		List<Job> jobs = _jobImporter.FindAll(j =>
			j != MainJob
			&& j.IsUnitType(Type)
			&& (j.ValidSlots == Job.SlotRestriction.Both || j.ValidSlots == Job.SlotRestriction.Sub)
			&& (includeNonGenerics || j.IsGeneric)
			&& Array.Find(excludedJobs, ex => ex == j) == null
		);

		jobs.Sort((a, b) => a.Name.CompareTo(b.Name));
		return jobs;
	}

	public List<Ability> GetAvailablePassivesList(Ability.AbilityType type, Job job) {
		List<Ability> passives = _abilityImporter.FindAll((a, j) =>
			a.Type == type
			&& (job == null || j == job)
			&& j.IsUnitType(Type)
		);

		if (type == Ability.AbilityType.Class) {
			// Sort class abilities by reference by default to matchin-game order
			passives.Sort((a, b) => a.Reference.CompareTo(b.Reference));
		} else {
			passives.Sort((a, b) => a.Name.CompareTo(b.Name));
		}

		return passives;
	}

	public List<IBuildStep> GetBuildSteps(bool includeBasicSkills = true) {
		var requirements = new List<IBuildStep>();
		if (includeBasicSkills && Type != UnitType.Monster) {
			new LearnBuildStep(_abilityImporter.Get("Squire-10"), _jobImporter.Get("Squire")).AppendToList(requirements);	// Gained-JP Up
			new LearnBuildStep(_abilityImporter.Get("Squire-05"), _jobImporter.Get("Squire")).AppendToList(requirements);	// Move +1
			new LearnBuildStep(_abilityImporter.Get("Chemist-16"), _jobImporter.Get("Chemist")).AppendToList(requirements);	// Auto-Potion
		}

		// Prioritize unlocking sub-jobs first
		foreach (var subJob in _subJobs) {
			AddClassSkillSteps(subJob, requirements, true);
		}

		// then passives
		foreach (var kvp in _passives) {
			if (kvp.Value != null) {
				var job = _abilityImporter.GetSourceJob(kvp.Value.Reference);
				new LearnBuildStep(kvp.Value, job).AppendToList(requirements);
			}
		}

		// End on main job
		AddClassSkillSteps(MainJob, requirements, false);
		return requirements;
	}

	private void AddClassSkillSteps(Job job, List<IBuildStep> requirements, bool enableSorting) {
		if (job.Reference == "Calculator") {
			// Special case for Calculator.  Master all jobs with magic it can cast (Being lazy and ignoring that this includes spells it can't cast)
			new MasterBuildStep(_jobImporter.Get("Priest")).AppendToList(requirements);
			new MasterBuildStep(_jobImporter.Get("Wizard")).AppendToList(requirements);
			new MasterBuildStep(_jobImporter.Get("Time Mage")).AppendToList(requirements);
			new MasterBuildStep(_jobImporter.Get("Oracle")).AppendToList(requirements);
		}

		var jobAbilities = job.GetAbilities(Ability.AbilityType.Class);
		var toLearn = new List<Ability>(jobAbilities);
		bool learnAll = true;
		foreach (var ability in jobAbilities) {
			if (!_classAbilities.Contains(ability)) {
				learnAll = false;
				toLearn.Remove(ability);
			}
		}

		if (learnAll) {
			var step = new MasterBuildStep(job);
			step.EnableSorting = enableSorting;
			step.AppendToList(requirements);
		} else {
			foreach (var ability in toLearn) {
				var step = new LearnBuildStep(ability, job);
				step.EnableSorting = enableSorting;
				step.AppendToList(requirements);
			}
		}
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

	private void RemoveClassAbilities(Job job) {
		if (job != null) {
			var jobAbilities = _abilityImporter.FindAll((a, j) => a.Type == Ability.AbilityType.Class && j == job);
			foreach (var ability in jobAbilities) {
				_classAbilities.Remove(ability);
			}
		}
	}

	public struct Writer {
		public void Write(CharacterBuild source, System.IO.BinaryWriter writer) {
			if (source.MainJob == null) {
				source.SetMainJob(source.GetMainJobList()[0]);
			}

			writer.Write(source.Name);
			writer.Write((int)source.Type);
			writer.Write(source.MainJob.Reference);
			writer.Write(source._subJobs.Length);
			for (int i = 0; i < source._subJobs.Length; i++) {
				writer.Write(source._subJobs[i].Reference);
			}

			writer.Write(source._passives.Count);
			foreach (var kvp in source._passives) {
				writer.Write((int)kvp.Key);
				writer.Write(kvp.Value.Reference);
			}

			writer.Write(source._classAbilities.Count);
			foreach (var ability in source._classAbilities) {
				writer.Write(ability.Reference);
			}
		}
	}

	public struct Reader {
		public CharacterBuild Read(System.IO.BinaryReader reader, JobImporter jobImporter, AbilityImporter abilityImporter) {
			var build = new CharacterBuild(jobImporter, abilityImporter);
			build.Name = reader.ReadString();
			build.Type = (UnitType)reader.ReadInt32();
			build.MainJob = jobImporter.Get(reader.ReadString());

			int count = reader.ReadInt32();
			build._subJobs = new Job[count];
			for (int i = 0; i < count; i++) {
				build._subJobs[i] = jobImporter.Get(reader.ReadString());
			}

			count = reader.ReadInt32();
			build._passives = new Dictionary<Ability.AbilityType, Ability>();
			for (int i = 0; i < count; i++) {
				var type = (Ability.AbilityType)reader.ReadInt32();
				string reference = reader.ReadString();
				var ability = abilityImporter.Get(reference);
				if (ability != null) {
					build._passives[type] = ability;
				} else {
					UnityEngine.Debug.LogError($"{build.Name}: Read invalid {type} ability reference '{reference}'");
				}
			}

			count = reader.ReadInt32();
			build._classAbilities = new HashSet<Ability>();
			for (int i = 0; i < count; i++) {
				string reference = reader.ReadString();
				var ability = abilityImporter.Get(reference);
				if (ability != null) {
					build._classAbilities.Add(ability);
				} else {
					UnityEngine.Debug.LogError($"{build.Name}: Read invalid {build.MainJob} ability reference '{reference}'");
				}
			}

			return build;
		}
	}
}
