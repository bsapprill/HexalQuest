using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCell : MonoBehaviour {

	GameObject[] cellSectionObjs = new GameObject[6];
	public GameObject[] hullSectionObjs = new GameObject[6];
	GameObject cellHub;

	public List<int> hasCellNeighborAt = new List<int>();
	public Dictionary<GridCoordinate, ShipCell> neighborShipCells = new Dictionary<GridCoordinate, ShipCell>();

	Vector3 meshOffset;

	MeshChanger meshChanger;

	public enum Customization {PowerCore, CommandBridge, WeaponModule, ShieldSystem,
		                WarpDrive, SensorRelay, CrewQuarter, ImpulseEngine}
	public Customization CustomizationType;

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
			cellSectionObjs[i].GetComponent<CellSection>().InitializeCellSection();
			cellSectionObjs[i].gameObject.name = "Section "+ (i+1);
		}
	}

	public void UpdateCustomization(){
		switch(CustomizationType){
			case Customization.PowerCore:
				
				break;
			case Customization.CommandBridge:

				break;
			case Customization.WeaponModule:

				break;
			case Customization.ShieldSystem:

				break;
			case Customization.WarpDrive:

				break;
			case Customization.SensorRelay:

				break;
			case Customization.CrewQuarter:

				break;
			case Customization.ImpulseEngine:

				break;
		}
	}

	public void UpdateHullSection(int order){
		if(hasCellNeighborAt.Contains(order)){				
			meshChanger.AlterInnerHull(hullSectionObjs[order],true);
		}
		else{
			meshChanger.AlterInnerHull(hullSectionObjs[order],false);
		}
	}
}