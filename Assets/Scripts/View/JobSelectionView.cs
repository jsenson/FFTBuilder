using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobSelectionView : MonoBehaviour {
	[SerializeField] private VerticalLayoutGroup _layoutGroup;
	[SerializeField] private JobSlotView _viewPrefab;

	public event Action<Job> OnShowJobDetailsClicked;

	private Data _data;
	private JobSlotView _mainView;
	private List<JobSlotView> _subViews = new List<JobSlotView>();

	public void Refresh(Data data) {
		Clear();
		_data = data;
		_mainView = CreateView("Main", -1);
		PopulateSubViews(_mainView.SelectedJob);
	}

	public void Clear() {
		ClearMain();
		ClearSubs();
	}

	private void ClearSubs() {
		foreach (var view in _subViews) {
			// Too lazy to do pooling.
			Destroy(view.gameObject);
		}

		_subViews.Clear();
	}

	private void ClearMain() {
		if (_mainView != null) {
			Destroy(_mainView.gameObject);
			_mainView = null;
		}
	}

	private void PopulateSubViews(Job selectedMainJob) {
		int numSubJobs = selectedMainJob.NumSubjobs;
		for (int i = 0; i < numSubJobs; i++) {
			var view = CreateView("Sub", i);
			_subViews.Add(view);
		}
	}

	private JobSlotView CreateView(string name, int jobIndex) {
		var view = Instantiate(_viewPrefab, _layoutGroup.transform);
		view.OnSelectionChanged += OnJobChanged;
		view.OnExpandClicked += OnExpandClicked;
		view.Initialize(new JobSlotView.Data() {
			Name = name,
			Character = _data.Character,
			JobIndex = jobIndex,
			JobImporter = _data.JobImporter
		});

		return view;
	}

	private void OnJobChanged(JobSlotView sender, Job selectedJob) {
		if (sender.IsMain) {
			_data.Character.SetMainJob(selectedJob, _data.JobImporter);
		} else {
			_data.Character.SetSubJob(_subViews.IndexOf(sender), selectedJob);
		}

		if (sender.IsMain && selectedJob.NumSubjobs != _subViews.Count) {
			ClearSubs();
			PopulateSubViews(selectedJob);
			return;
		} else {
			foreach (var view in _subViews) {
				if (view != sender) {
					view.Refresh();
				}
			}
		}
	}

	private void OnExpandClicked(JobSlotView sender) {
		OnShowJobDetailsClicked?.Invoke(sender.SelectedJob);
	}

	public struct Data {
		public CharacterBuild Character;
		public JobImporter JobImporter;

		public Data(CharacterBuild character, JobImporter jobImporter) {
			Character = character;
			JobImporter = jobImporter;
		}
	}
}
