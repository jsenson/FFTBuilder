using System.Collections.Generic;

public class LevelBuildStep : IBuildStep {
	private Job _job;
	private int _level;

	public Job Job => _job;
	public int Level => _level;
	public bool EnableSorting { get; set; } = true;

	public string Description => $"{_level} {_job}";

	public LevelBuildStep(Job job, int level) {
		_job = job;
		_level = level;
	}

	public void AppendToList(List<IBuildStep> steps) {
		var requirements = GetRequiredSteps();
		foreach (var requirement in requirements) {
			requirement.AppendToList(steps);
		}

		if (EnableSorting) {
			for (int i = 0, count = steps.Count; i < count; i++) {
				// Check if a Level Step already exists and replace it if a higher level
				if (steps[i] is LevelBuildStep levelStep && _job == levelStep._job) {
					if (_level > levelStep._level) {
						steps[i] = this;
					}

					return;
				}
			}

			// Check for any Learn Steps and insert after the last one in the list
			for (int i = steps.Count - 2; i >= 0; i--) {
				if (steps[i] is LearnBuildStep learnStep && _job == learnStep.Job) {
					steps.Insert(i + 1, this);
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
