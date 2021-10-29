public static class LevelJPCosts {
	public static int GetCostForLevel(int level) {
		int[] costs = { 0, 200, 400, 700, 1100, 1600, 2200, 3000 };
		level = UnityEngine.Mathf.Clamp(level - 1, 0, costs.Length - 1);
		return costs[level];
	}
}