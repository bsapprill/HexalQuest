using UnityEngine;

public static class GlobalData{
	public static Color sectionColor = Color.white;
	public static Color innerHullColor = new Color(.35f,.35f,.35f);
	//public static Color outerHullColor = new Color(.25f,.25f,.25f);
	public static Color outerHullColor = Color.red;
	public static Color hubColor = Color.gray;

	//Determines the sizes of the ship cell elements
	public static float hubSectionScale = 0.35f;
	public static float cellSectionScale = 0.55f;
	public static float hullSectionScale = 0.1f;
}