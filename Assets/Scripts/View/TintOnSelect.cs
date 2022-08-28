using UnityEngine;
using UnityEngine.UI;

public class TintOnSelect : MonoBehaviour {
	[SerializeField] private Graphic[] _graphics;
	[SerializeField] private Color _selectedColor = Color.grey;
	[SerializeField] private Color _defaultColor = Color.white;

	public bool Selected { get; private set; }

	public void Select() {
		Selected = true;
		foreach(var graphic in _graphics) {
			graphic.color = _selectedColor;
		}
	}

	public void Deselect() {
		Selected = false;
		foreach(var graphic in _graphics) {
			graphic.color = _defaultColor;
		}
	}
}
