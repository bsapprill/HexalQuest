using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullSection : MonoBehaviour {
	int sectionOrder;
	public bool isInnerHull = false;
	public MeshHandler mH = new MeshHandler();
	MeshFilter mF;
	MeshRenderer mR;

	Vector3 meshOffset;

	void Start () {
		/*
		isInnerHull = GetComponentInParent<ShipCell>().hasCellNeighborAt[sectionOrder-1];
		Debug.Log(isInnerHull);
		mF = GetComponent<MeshFilter>();
		mR = GetComponent<MeshRenderer>();
		mH.SetMeshOffset(meshOffset);
		mH.AssignHullMeshData(sectionOrder, isInnerHull);
		mH.ReturnCompleteMesh(mF);
		mR.material.shader = Shader.Find("Unlit/Color");
		if(isInnerHull){			
			mR.material.color = GlobalData.innerHullColor;
		}
		else{
			mR.material.color = GlobalData.outerHullColor;
		}
		*/
	}

	#region Function

	#region Setter

	public void SetSectionOrder(int order){
		sectionOrder = order;
	}	
	public void SetMeshOffset(Vector3 _meshOffset){
		meshOffset = _meshOffset;
	}	
	public void SetisInnerHull(bool _isInnerHull){
		isInnerHull = _isInnerHull;
	}

	#endregion

	public void InitializeHullSection(Color innerHullColor, Color outerHullColor){

		if(GetComponentInParent<ShipCell>().hasCellNeighborAt.Contains(sectionOrder-1)){
			isInnerHull = true;
		}
			
		mF = GetComponent<MeshFilter>();
		mR = GetComponent<MeshRenderer>();
		mH.SetMeshOffset(meshOffset);
		mH.AssignHullMeshData(sectionOrder, isInnerHull);
		mH.ReturnCompleteMesh(mF);
		mR.material.shader = Shader.Find("Unlit/Color");

		if(isInnerHull){			
			mR.material.color = innerHullColor;
		}
		else{
			mR.material.color = outerHullColor;
		}

		GetComponentInParent<ShipCell>().hullSectionObjs[sectionOrder-1] = gameObject;
	}

	#endregion
}