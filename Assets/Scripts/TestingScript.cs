using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour {
	enum TestEnum {One, Two, Three, Four};
	TestEnum tester;
	string[] printer = { "One", "Two", "Three", "Four" };

	void Start () {
		tester = TestEnum.One;
	}
	void Update(){
		if(Input.GetKeyDown(KeyCode.F)){
			Debug.Log (printer [(int)tester]);
			tester++;
			if(tester == TestEnum.Four){
				Debug.Log ("good");
			}
		}
	}
}