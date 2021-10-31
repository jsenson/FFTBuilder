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

	public Type AbilityType {
		get => _type;
		set => _type = value;
	}

	public string Description {
		get => _description;
		set => _description = value;
	}

	public int JpCost {
		get => _jpCost;
		set => _jpCost = value;
	}
}
