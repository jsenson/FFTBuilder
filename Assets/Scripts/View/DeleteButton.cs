using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DeleteButton : MonoBehaviour {
	[SerializeField] private float _deselectDelaySeconds = 2f;
	[SerializeField] private TintOnSelect _selection;

	private Button _button;
	private Coroutine _deselectRoutine;
	private Action _onClickAction;

	public void SetAction(Action onClickAction) {
		_onClickAction = onClickAction;
	}

	private void Awake() {
		_button = GetComponent<Button>();
	}

	private void OnEnable() {
		_button.onClick.AddListener(OnButtonClick);
	}

	private void OnDisable() {
		_button.onClick.RemoveListener(OnButtonClick);
	}

	private void OnButtonClick() {
		if (_selection.Selected) {
			Deselect();
			_onClickAction?.Invoke();
		} else {
			Select();
		}
	}

	private void Select() {
		_selection.Select();
		if (_deselectRoutine == null) {
			_deselectRoutine = StartCoroutine(DeselectRoutine());
		}
	}

	private void Deselect() {
		_selection.Deselect();
		if (_deselectRoutine != null) {
			StopCoroutine(_deselectRoutine);
			_deselectRoutine = null;
		}
	}

	private IEnumerator DeselectRoutine() {
		yield return new WaitForSeconds(_deselectDelaySeconds);
		_selection.Deselect();
		_deselectRoutine = null;
	}
}
