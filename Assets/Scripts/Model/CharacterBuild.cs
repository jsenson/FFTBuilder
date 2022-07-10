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

	private List<Job> _subJobs = new List<Job>();
	private Dictionary<Ability.AbilityType, Ability> _passives = new Dictionary<Ability.AbilityType, Ability>();

	public static CharacterBuild GetDefault(JobImporter jobImporter) {
		var defaultBuild = new CharacterBuild();
		defaultBuild.MainJob = defaultBuild.GetMainJobList(jobImporter)[0];
		var subJobs = defaultBuild.GetSubJobList(jobImporter);
		int subCount = subJobs.Count;
		for (int i = 0; i < defaultBuild.MainJob.NumSubjobs && i < subCount; i++) {
			defaultBuild._subJobs.Add(subJobs[i]);
		}

		return defaultBuild;
	}

	public CharacterBuild() {
		Name = "<Name>";
		Type = UnitType.Male;
	}

	public void SetName(string newName) {
		if (newName != Name) {
			Name = newName;
			OnNameChanged?.Invoke(this);
		}
	}

	public void SetType(UnitType newType) {
		if (newType != Type) {
			Type = newType;
			OnTypeChanged?.Invoke(this);
		}
	}

	public void SetMainJob(Job newJob) {
		if (newJob != MainJob) {
			MainJob = newJob;
			OnMainJobChanged?.Invoke(this);
		}
	}

	public Job[] GetCurrentSubJobs() {
		return _subJobs.ToArray();
	}

	public Job GetSubJob(int index) {
		return index > 0 && index < _subJobs.Count ? _subJobs[index] : null;
	}

	public void SetSubJob(int index, Job job) {
		if (index > 0 && index < _subJobs.Count && _subJobs[index] != job) {
			_subJobs[index] = job;
			OnSubJobChanged?.Invoke(this, index);
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
		bool includeNonGenerics = IsGeneric();
		List<Job> jobs = importer.FindAll(j =>
			j.IsUnitType(Type)
			&& (j.ValidSlots == Job.SlotRestriction.Both || j.ValidSlots == Job.SlotRestriction.Main)
			&& (includeNonGenerics || j.isGeneric)
		);

		if (sortByName) {
			jobs.Sort((a, b) => a.Name.CompareTo(b.Name));
		}

		return jobs;
	}

	public List<Job> GetSubJobList(JobImporter importer, bool sortByName = true) {
		bool includeNonGenerics = IsGeneric();
		List<Job> jobs = importer.FindAll(j =>
			j != MainJob
			&& j.IsUnitType(Type)
			&& (j.ValidSlots == Job.SlotRestriction.Both || j.ValidSlots == Job.SlotRestriction.Sub)
			&& (includeNonGenerics || j.isGeneric)
		);

		if (sortByName) {
			jobs.Sort((a, b) => a.Name.CompareTo(b.Name));
		}

		return jobs;
	}

	private bool IsGeneric() {
		bool generic = MainJob == null || MainJob.isGeneric;
		if (generic) {
			foreach (var subJob in _subJobs) {
				if (!subJob.isGeneric) {
					generic = false;
					break;
				}
			}
		}

		return generic;
	}
}
