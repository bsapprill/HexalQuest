using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCell : MonoBehaviour {

	GameObject[] cellSectionObjs = new GameObject[6];
	public GameObject[] hullSectionObjs = new GameObject[6];
	GameObject cellHub;
	MeshRenderer hubRenderer;

	public List<int> hasCellNeighborAt = new List<int>();
	public Dictionary<GridCoordinate, ShipCell> neighborShipCells = new Dictionary<GridCoordinate, ShipCell>();

	Vector3 meshOffset;

	MeshChanger meshChanger;

	public bool hasCellUpgrade = false;
	public CellUpgrade cellUpgrade;

	Color innerHullColor;
	Color outerHullColor;
	Color sectionColor;

	void Start(){
		/*
		GenerateCellHub();

		//Generates CellSections
		for (int i = 0; i < 6; i++) {
			cellSectionObjs[i] = new GameObject();
			cellSectionObjs[i].transform.SetParent(transform);
			cellSectionObjs[i].AddComponent<MeshFilter>();
			cellSectionObjs[i].AddComponent<MeshRenderer>();
			cellSectionObjs[i].AddComponent<CellSection>();
			cellSectionObjs[i].GetComponent<CellSection>().SetSectionOrder(i+1);
			cellSectionObjs[i].GetComponent<CellSection>().SetMeshOffset(meshOffset);
			cellSectionObjs[i].gameObject.name = "Section "+ (i+1);
		}
		*/
	}

	void GenerateCellHub(){
		cellHub = new GameObject();
		cellHub.transform.SetParent(transform);
		cellHub.AddComponent<MeshFilter>();
		cellHub.AddComponent<MeshRenderer>();
		hubRenderer = cellHub.GetComponent<MeshRenderer>();
		cellHub.AddComponent<CellHub>();
		cellHub.GetComponent<CellHub>().SetMeshOffset(meshOffset);
		cellHub.GetComponent<CellHub>().InitializeCellHub();
		cellHub.gameObject.name = "Cell Hub";
	}

	public void SetMeshOffset(Vector3 _meshOffset){
		meshOffset = _meshOffset;
	}

	public void InitializeShipCell(){
		meshChanger = new MeshChanger();

		GenerateCellHub();

		//Generates CellSections
		for (int i = 0; i < 6; i++) {
			cellSectionObjs[i] = new GameObject();
			cellSectionObjs[i].transform.SetParent(transform);
			cellSectionObjs[i].AddComponent<MeshFilter>();
			cellSectionObjs[i].AddComponent<MeshRenderer>();
			cellSectionObjs[i].AddComponent<CellSection>();
			cellSectionObjs[i].GetComponent<CellSection>().SetSectionOrder(i+1);
			cellSectionObjs[i].GetComponent<CellSection>().SetMeshOffset(meshOffset);
			cellSectionObjs[i].GetComponent<CellSection>().InitializeCellSection(sectionColor, innerHullColor, outerHullColor);
			cellSectionObjs[i].gameObject.name = "Section "+ (i+1);
		}
	}

	//This function will probably change over time.
	//For now it just updates the hub color to match the upgrade type
	public void UpdateCellHubVisuals(Color hubColor){
		hubRenderer.material.color = hubColor;
	}

	public void SetHullColors(Color[] colors){
		innerHullColor = colors[0];
		outerHullColor = colors[1];
		sectionColor = colors[2];
	}

	public void UpdateHullSection(int order){
		if(hasCellNeighborAt.Contains(order)){
			hullSectionObjs[order].GetComponent<HullSection>().isInnerHull = true;
			meshChanger.AlterInnerHull(hullSectionObjs[order],true,innerHullColor,outerHullColor);
		}
		else{
			meshChanger.AlterInnerHull(hullSectionObjs[order],false,innerHullColor,outerHullColor);
		}
	}
}