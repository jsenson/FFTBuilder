using System.Collections.Generic;

public class LearnBuildStep : IBuildStep {
	private Ability _ability;
	private Job _job;

	public Ability Ability => _ability;
	public Job Job => _job;
	public bool EnableSorting { get; set; } = true;

	public string Description => $"{_ability.Name}: ({_job.Name} - {_ability.Cost:N0} JP)";

	public LearnBuildStep(Ability ability, Job job) {
		_ability = ability;
		_job = job;
	}

	public void AppendToList(List<IBuildStep> steps) {
		var requirements = GetRequiredSteps();
		foreach (var requirement in requirements) {
			requirement.AppendToList(steps);
		}

		if (EnableSorting) {
			for (int i = 0, count = steps.Count; i < count; i++) {
				if (steps[i] is LevelBuildStep levelStep && levelStep.Job == _job) {
					steps.Insert(i, this);
					return;
				}
			}
		}

		steps.Add(this);
	}

	private List<IBuildStep> GetRequiredSteps() {
		var steps = new List<IBuildStep>();
		foreach (var requirement in _job.GetRequirements()) {
			requirement.AppendToList(steps);
		}

		return steps;
	}
}
