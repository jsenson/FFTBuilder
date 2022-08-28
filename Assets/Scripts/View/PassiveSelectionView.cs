using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSelectionView : MonoBehaviour {
	[SerializeField] private PassiveSlotView _viewPrefab;

	private readonly List<PassiveSlotView> _views = new List<PassiveSlotView>();

	public void Refresh(CharacterBuild character) {
		// TODO
	}

	public void Clear() {
		foreach (var view in _views) {
			GameObject.Destroy(view.gameObject);
		}

		_views.Clear();
	}
}
