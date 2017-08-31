using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSection : MonoBehaviour {
	int sectionOrder;
	MeshHandler mH = new MeshHandler();
	MeshFilter mF;
	MeshRenderer mR;

	GameObject hullSection;

	Vector3 meshOffset;

	void Start () {
		/*
		mF = GetComponent<MeshFilter>();
		mR = GetComponent<MeshRenderer>();
		mH.SetMeshOffset(meshOffset);
		mH.AssignSectionMeshData(sectionOrder);
		mH.ReturnCompleteMesh(mF);
		mR.material.shader = Shader.Find("Unlit/Color");
		mR.material.color = GlobalData.sectionColor;

		hullSection = new GameObject();
		GetComponentInParent<ShipCell>().hullSectionObjs[sectionOrder-1] = hullSection;
		hullSection.transform.SetParent(transform);
		hullSection.AddComponent<MeshFilter>();
		hullSection.AddComponent<MeshRenderer>();
		hullSection.AddComponent<HullSection>();
		hullSection.GetComponent<HullSection>().SetSectionOrder(sectionOrder);
		hullSection.GetComponent<HullSection>().SetMeshOffset(meshOffset);
		hullSection.gameObject.name = "Hull "+(sectionOrder);
		*/
	}

	public void SetSectionOrder(int order){
		sectionOrder = order;
	}

	public void SetMeshOffset(Vector3 _meshOffset){
		meshOffset = _meshOffset;
	}

	public void InitializeCellSection(){
		mF = GetComponent<MeshFilter>();
		mR = GetComponent<MeshRenderer>();
		mH.SetMeshOffset(meshOffset);
		mH.AssignSectionMeshData(sectionOrder);
		mH.ReturnCompleteMesh(mF);
		mR.material.shader = Shader.Find("Unlit/Color");
		mR.material.color = GlobalData.sectionColor;

		hullSection = new GameObject();
		GetComponentInParent<ShipCell>().hullSectionObjs[sectionOrder-1] = hullSection;
		hullSection.transform.SetParent(transform);
		hullSection.AddComponent<MeshFilter>();
		hullSection.AddComponent<MeshRenderer>();
		hullSection.AddComponent<HullSection>();
		hullSection.GetComponent<HullSection>().SetSectionOrder(sectionOrder);
		hullSection.GetComponent<HullSection>().SetMeshOffset(meshOffset);
		hullSection.GetComponent<HullSection>().InitializeHullSection();
		hullSection.gameObject.name = "Hull "+(sectionOrder);
	}
}