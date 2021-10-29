using UnityEngine;

[CreateAssetMenu(menuName = "FFT/Ability", fileName = "AbilityName.asset")]
public class Ability : ScriptableObject {
	public enum Type {
		Class,
		Support,
		Reaction,
		Movement
	}

	[SerializeField] private Type _type;
	[SerializeField] private int _jpCost;
	[SerializeField, TextArea] private string _description;
}
