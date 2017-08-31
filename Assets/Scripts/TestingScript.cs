using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour {
	public Vector3[] positions;
	public float dirScale;
	GameObject[] cubes = new GameObject[3];
	Vector3 directionToGo;

	void Start () {
		for (int i = 0; i < 3; i++) {
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubes[i] = cube;
			cube.transform.position = positions[i];
		}
		directionToGo = positions[2] - positions[1];
	}
	void Update(){
		if(Input.GetKeyDown(KeyCode.F)){
		}
		cubes[0].transform.position = directionToGo.normalized * dirScale;
	}
}