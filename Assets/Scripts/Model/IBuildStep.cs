using System.Collections.Generic;

public interface IBuildStep {
	string Description { get; }
	bool EnableSorting { get; }

	void AppendToList(List<IBuildStep> steps);
}
