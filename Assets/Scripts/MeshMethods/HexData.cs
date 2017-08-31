using UnityEngine;

public static class HexData{

    public static Vector3[] hexCorners = {
        new Vector3(0f,0f,0f),
        new Vector3(0f, HexSizeData.outerRadius, 0f),
		new Vector3(HexSizeData.innerRadius,  HexSizeData.outerRadius * 0.5f, 0f),
		new Vector3(HexSizeData.innerRadius, -HexSizeData.outerRadius * 0.5f, 0f),
		new Vector3(0f, -HexSizeData.outerRadius, 0f),
		new Vector3(-HexSizeData.innerRadius, -HexSizeData.outerRadius * 0.5f, 0f),
		new Vector3(-HexSizeData.innerRadius, HexSizeData.outerRadius * 0.5f, 0f)
    };

    public static Vector2[] uvs = {
        new Vector2(0.5f,0.5f),
        new Vector2(0.5f, 1.0f),
        new Vector2(1.0f, 0.75f),
        new Vector2(1.0f, 0.25f),
        new Vector2(0.5f, 0f),
        new Vector2(0f, 0.25f),
        new Vector2(0f, 0.75f)
    };
}