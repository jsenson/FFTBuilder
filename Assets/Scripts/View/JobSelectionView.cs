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

	public void Initialize(Data data) {
		Clear();
		_data = data;
		_mainView = CreateView("Main", data.MainJobOptions, true);
		PopulateSubViews(_mainView.SelectedJob);
	}

	public void Refresh(CharacterBuild character) {
		// TODO
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
			GameObject.Destroy(_mainView.gameObject);
			_mainView = null;
		}
	}

	private void PopulateSubViews(Job selectedMainJob) {
		int numSubJobs = selectedMainJob.NumSubjobs;
		for (int i = 0; i < numSubJobs; i++) {
			_subViews.Add(CreateView("Sub", _data.SubJobOptions, false, GetOtherSelectedJobs(null)));
		}
	}

	private JobSlotView CreateView(string name, List<Job> options, bool isMainJob, List<Job> restrictedJobs = null) {
		var view = Instantiate(_viewPrefab, _layoutGroup.transform);
		view.OnSelectionChanged += OnJobChanged;
		view.OnExpandClicked += OnExpandClicked;
		view.Initialize(new JobSlotView.Data() {
			Name = name,
			IsMainJob = isMainJob,
			Options = options,
			RestrictedJobs = restrictedJobs
		});

		return view;
	}

	private void OnJobChanged(JobSlotView sender, Job selectedJob) {
		if (sender.IsMain && selectedJob.NumSubjobs != _subViews.Count) {
			ClearSubs();
			PopulateSubViews(selectedJob);
			return;
		} else {
			foreach (var view in _subViews) {
				if (view != sender) {
					view.SetRestrictedJobs(GetOtherSelectedJobs(view));
				}
			}
		}
	}

	private List<Job> GetOtherSelectedJobs(JobSlotView view) {
		List<Job> selectedJobs = new List<Job>();
		var selectedMainJob = _mainView != view ? _mainView?.SelectedJob : null;
		if (selectedMainJob != null) {
			selectedJobs.Add(selectedMainJob);
		}

		foreach (var currentView in _subViews) {
			if (currentView != view) {
				var selectedJob = currentView.SelectedJob;
				if (selectedJob != null) {
					selectedJobs.Add(selectedJob);
				}
			}
		}

		return selectedJobs;
	}

	private void OnExpandClicked(JobSlotView sender) {
		OnShowJobDetailsClicked?.Invoke(sender.SelectedJob);
	}

	public struct Data {
		public List<Job> MainJobOptions;
		public List<Job> SubJobOptions;
	}
}
