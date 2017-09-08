using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshChanger{
	GameObject objWithMesh;
	Mesh objMesh = new Mesh();
	MeshFilter objFilter;
	MeshRenderer objRenderer;

	void AssignMeshToChange(GameObject _objWithMesh){
		objWithMesh = _objWithMesh;
		objFilter = objWithMesh.GetComponent<MeshFilter>();
		objMesh = objFilter.mesh;
		objRenderer = objWithMesh.GetComponent<MeshRenderer>();
	}

	public void AlterInnerHull(GameObject _objToChange, bool hasNeighbor, Color innerHullColor, Color outerHullColor){
		AssignMeshToChange(_objToChange);

		List<Vector3> vertexList = new List<Vector3>();

		for (int i = 0; i < 4; i++) {
			vertexList.Add(objMesh.vertices[i]);
		}

		if(hasNeighbor){			
			Vector3 hullAdjustmentOne = (vertexList[1] - vertexList[0]) * 0.5f;
			Vector3 hullAdjustmentTwo = (vertexList[2] - vertexList[3]) * 0.5f;
			
			vertexList[1] -= hullAdjustmentOne;
			vertexList[2] -= hullAdjustmentTwo;

			objRenderer.material.color = innerHullColor;
		}
		else{
			Vector3 hullAdjustmentOne = (vertexList[1] - vertexList[0]);
			Vector3 hullAdjustmentTwo = (vertexList[2] - vertexList[3]);

			vertexList[1] += hullAdjustmentOne;
			vertexList[2] += hullAdjustmentTwo;

			objRenderer.material.color = outerHullColor;
		}

		objMesh.SetVertices(vertexList);
	}
}