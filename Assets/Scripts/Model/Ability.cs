using System;

public class Ability : IEquatable<Ability> {
	public enum AbilityType {
		Class,
		Support,
		Reaction,
		Movement,
		Passive
	}

	public string Reference { get; set; }
	public string Name { get; set; }
	public AbilityType Type { get; set; }
	public int Cost { get; set; }
	public string Description { get; set; }

	public static bool operator ==(Ability a, Ability b) {
		if (Object.ReferenceEquals(a, null)) {
			return Object.ReferenceEquals(b, null);
		} else {
			return a.Equals(b);
		}
	}

	public static bool operator !=(Ability a, Ability b) {
		return !(a == b);
	}

	public bool Equals(Ability other) {
		return !Object.ReferenceEquals(other, null) && Reference == other.Reference;
	}

	public override bool Equals(object obj) {
		if (obj is Ability other) {
			return Equals(other);
		}

		return false;
	}

	public override int GetHashCode() {
		return base.GetHashCode() ^ Reference.GetHashCode();
	}
}
