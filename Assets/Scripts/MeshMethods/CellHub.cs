using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellHub : MonoBehaviour {
	MeshHandler mH = new MeshHandler();
	MeshFilter mF;
	MeshRenderer mR;

	Vector3 meshOffset;

	void Start () {
		/*
		mF = GetComponent<MeshFilter>();
		mR = GetComponent<MeshRenderer>();
		mH.SetMeshOffset(meshOffset);
		mH.AssignHexMeshData();
		mH.ReturnCompleteMesh(mF);
		mR.material.shader = Shader.Find("Unlit/Color");
		mR.material.color = GlobalData.hubColor;
		*/
	}

	public void SetMeshOffset(Vector3 _meshOffset){
		meshOffset = _meshOffset;
	}

	public void InitializeCellHub(){
		mF = GetComponent<MeshFilter>();
		mR = GetComponent<MeshRenderer>();
		mH.SetMeshOffset(meshOffset);
		mH.AssignHexMeshData();
		mH.ReturnCompleteMesh(mF);
		mR.material.shader = Shader.Find("Unlit/Color");
		mR.material.color = GlobalData.hubColor;
	}
}