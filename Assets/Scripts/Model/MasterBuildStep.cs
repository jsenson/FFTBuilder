using System.Collections.Generic;

public class MasterBuildStep : IBuildStep {
	private Job _job;

	public string Description => $"Master {_job.Name}! ({GetTotalCost():N0} JP)";
	public bool EnableSorting { get; set; } = true;

	public MasterBuildStep(Job job) {
		_job = job;
	}

	public void AppendToList(List<IBuildStep> steps) {
		var reqs = GetRequiredSteps();
		foreach (var req in reqs) {
			req.AppendToList(steps);
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

	private int GetTotalCost() {
		int total = 0;
		foreach (var ability in _job.GetAbilities(Ability.AbilityType.Class)) {
			total += ability.Cost;
		}

		return total;
	}
}
