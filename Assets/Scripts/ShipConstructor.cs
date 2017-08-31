using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipConstructor : MonoBehaviour {
	public int gridDimensionX;
	public int gridDimensionY;

	GridCell[] GridCells;

	ShipCell selectedShipCell;

	GameObject playersShip;

	#region Neighboring Values

	enum HexDirection {UP_RIGHT, RIGHT, DOWN_RIGHT, DOWN_LEFT, LEFT, UP_LEFT};
	HexDirection neighborEnum;

	//Loops clockwise, starting from top-right
	Vector2[] neighborDirection = new Vector2[] { 
		new Vector2( 0,  1), new Vector2( 1, 0 ), new Vector2( 1, -1),
		new Vector2( 0, -1), new Vector2(-1,  0), new Vector2( -1, 1)
	};

	#endregion

	#region UI

	[Serializable]
	public class UISystem{
		public Canvas mainCanvas;

		[Serializable]
		public class BuildStatePanelElements{
			public GameObject panelObj;
			public Button HullBuild;
			public Button CellBuild;			
		}

		[Serializable]
		public class CellOptionsPanelElements{
			public GameObject panelObj;
			public Button PowerCore;
			public Button CommandBridge;
			public Button WeaponModule;
			public Button ShieldSystem;
			public Button WarpDrive;
			public Button SensorRelay;
			public Button CrewQuarter;
			public Button ImpulseEngine;
		}

		[Serializable]
		public class StatsPanelElements{
			public GameObject panelObj;
		}

		public BuildStatePanelElements BuildStatePanel;
		public CellOptionsPanelElements CellOptionsPanel;
		public StatsPanelElements StatsPanel;
	}

	public UISystem UIData;

	#region UI Function

	public void EnterHullEdit(){
		UIData.BuildStatePanel.HullBuild.interactable = false;
		UIData.BuildStatePanel.CellBuild.interactable = true;
		selectedShipCell = default(ShipCell);
		SetStateBuildHullMode();
	}
		
	public void EnterCellEdit(){
		UIData.BuildStatePanel.HullBuild.interactable = false;
		UIData.BuildStatePanel.CellBuild.interactable = false;
		SetStateIdle();
	}

	#endregion

	#region Cell Customizers

	public void ApplyCellCustomization(int i){
		selectedShipCell.CustomizationType = (ShipCell.Customization)i;
	}

	#endregion

	#endregion

	#region State Machine

	enum Construction {Idle, BuildHullMode, CustomizeShipCell}
	Construction BuildState;
	Dictionary<Construction,Action> stateMain = new Dictionary<Construction, Action>();

	#region stateMain

	void Idle(){

		if(Input.GetKeyDown(KeyCode.D)){
			Vector3 curMosPos = ReturnCurrentMousePosition();
			GridCoordinate gridCoord = ReturnCellGridCoordinate(curMosPos);
			GridCell gridCell = GridCells[CoordToElement(gridCoord)];
			for (int i = 0; i < gridCell.neighborCoordinates.Count; i++) {					
				int testElement = CoordToElement(gridCell.neighborCoordinates[i]);
				GridCell testCell = GridCells[testElement];
				DebugCube(testCell.gridPosition);
			}
		}

		if(Input.GetKeyDown(KeyCode.F)){
			
		}

		if(Input.GetKeyDown(KeyCode.Alpha1)){
			EnterHullEdit();
		}

		if(Input.GetMouseButtonDown(0)){
			Vector3 mousePos = ReturnCurrentMousePosition();
			GridCoordinate gridCoord = ReturnCellGridCoordinate(mousePos);
			if(IsValidGridCell(gridCoord)){
				GridCell gridCell = GridCells[CoordToElement(gridCoord)];
				if(gridCell.hasShipCell){
					if(selectedShipCell != gridCell.shipCellComponent){						
						selectedShipCell = gridCell.shipCellComponent;
					}
				}
			}
		}
	}

	void BuildHullMode(){

		//Hotkey to swap build mode is currently '1'
		if(Input.GetKeyDown(KeyCode.Alpha2)){
			EnterCellEdit();
		}

		if(Input.GetMouseButtonDown(0)){
			GenerateNewShipCell();
		}
		if(Input.GetMouseButtonDown(1)){
			RemoveShipCell();
		}
	}

	void CustomizeShipCell(){
		
	}

	#region StateChangers

	public void SetStateIdle(){
		BuildState = Construction.Idle;
	}

	public void SetStateBuildHullMode(){
		BuildState = Construction.BuildHullMode;
	}

	public void SetStateCustomizeShipCell(){
		BuildState = Construction.CustomizeShipCell;
	}

	#endregion

	#endregion
	#endregion //State Machine

	#region Defaults
	void Awake(){
		#region Initialize UI

		#endregion
	}

	void Start(){
		GridCells = new GridCell[gridDimensionX*gridDimensionY];
		BuildGrid();
		BuildState = Construction.Idle;
		AlignCameraToGridCenter();

		stateMain.Add(Construction.Idle, new Action(Idle));
		stateMain.Add(Construction.BuildHullMode, new Action(BuildHullMode));
		stateMain.Add(Construction.CustomizeShipCell, new Action(CustomizeShipCell));

		playersShip = new GameObject();
		playersShip.AddComponent<PlayerShip>();
	}

	void Update(){
		stateMain[BuildState].Invoke();
	}

	#endregion

	#region Function

	#region Build Grid

	void BuildGrid(){
		for (int y = 0, i = 0; y < gridDimensionY; y++) {
			for (int x = 0; x < gridDimensionX; x++, i++) {
				Vector3 gridVector = ReturnCurrentGridVector(x,y);
				Vector2 gridCoordinate = ReturnGridCellCoordinate(gridVector);

				GridCells[i] = new GridCell(false, gridVector, gridCoordinate);

				for (int j = 0; j < 6; j++) {					
					if(IsValidGridCell(gridCoordinate)){						
						GridCoordinate neighborCoord = 
							GridCells[i].gridCoordinate.SumCoordinates(neighborDirection[j]);
						if(IsValidGridCell(neighborCoord)){
							GridCells[i].neighborCoordinates.Add(neighborCoord);
							GridCells[i].hasGridNeighborAt[j] = true;
						}
					}					
				}
			}
		}
	}

	Vector3 ReturnCurrentGridVector(int x, int y){
		float localX = (x + y * 0.5f - y / 2) * HexSizeData.innerRadius * 2f;
		float localY = (y * HexSizeData.outerRadius * 1.5f);
		Vector3 localVector3 = new Vector3(localX, localY);
		return localVector3;
	}

	Vector3 ReturnVectorAtGridCoordinate(GridCoordinate coord){
		float localX = (coord.x + coord.y * 0.5f - coord.y / 2) * HexSizeData.innerRadius * 2f;
		float localY = (coord.y * HexSizeData.outerRadius * 1.5f);
		return new Vector3(localX, localY);
	}

	void AlignCameraToGridCenter(){
		int finalElement = gridDimensionX * gridDimensionY - 1;
		Vector3 newCameraPosition = GridCells[finalElement].gridPosition;
		newCameraPosition.z = -10f;
		Camera.main.transform.position = newCameraPosition/2f;
	}

	/*
	void BuildObjectGroup(GameObject toBuild){
		for (int y = 0; y < gridDimensionY; y++) {
			for (int x = 0; x < gridDimensionX; x++) {
				Instantiate(toBuild,GridCells[x,y].cellPosition ,Quaternion.identity);
			}
		}
	}
	*/

	#endregion

	void GenerateNewShipCell(){		
		Vector3 currentMousePosition = ReturnCurrentMousePosition();
		GridCoordinate gridCoord = ReturnCellGridCoordinate(currentMousePosition);
		if(IsValidGridCell(gridCoord)){
			int gridElement = CoordToElement(gridCoord);
			GridCell localGridCell = GridCells[gridElement];
			if(localGridCell.hasShipCell == false){
				Vector3 spawnPos = localGridCell.gridPosition;
				GameObject newCell = new GameObject();
				localGridCell.shipCell = newCell;
				localGridCell.hasShipCell = true;
				newCell.AddComponent<ShipCell>();

				ShipCell localShipCell = localGridCell.shipCellComponent = newCell.GetComponent<ShipCell>();

				int listAdjustment = 0;
				for (int i = 0; i < 6; i++) {
					if(localGridCell.hasGridNeighborAt[i]==false){
						listAdjustment++;
						continue;
					}
					GridCoordinate neighborCoord = localGridCell.neighborCoordinates[i-listAdjustment];
					Debug.Log(neighborCoord);
					int gridElementOfNeighbor = CoordToElement(neighborCoord);			
					GridCell neighborCell = GridCells[gridElementOfNeighbor];
					if(neighborCell.hasShipCell){
						localShipCell.neighborShipCells.Add(neighborCoord,neighborCell.shipCellComponent);
						neighborCell.shipCellComponent.neighborShipCells.Add(gridCoord, localShipCell);

						localShipCell.hasCellNeighborAt.Add(i);
						neighborCell.shipCellComponent.hasCellNeighborAt.Add((i+3)%6);

						neighborCell.shipCellComponent.UpdateHullSection((i+3)%6);

					}
				}
					
				localShipCell.InitializeShipCell();
				newCell.transform.position = spawnPos;
			}
		}
	}		

	void RemoveShipCell(){
		Vector3 currentMousePosition = ReturnCurrentMousePosition();
		GridCoordinate gridCoord = ReturnCellGridCoordinate(currentMousePosition);
		if(IsValidGridCell(gridCoord)){
			int gridElement = CoordToElement(gridCoord);
			GridCell localGridCell = GridCells[gridElement];
			if(localGridCell.hasShipCell == true){
				localGridCell.hasShipCell = false;

				List<GridCoordinate> neighborCoords 
					= new List<GridCoordinate>(localGridCell.shipCellComponent.neighborShipCells.Keys);

				//Debug.Log(neighborCoords.Count);
				for (int i = 0; i < neighborCoords.Count; i++) {
					int neighborElement = CoordToElement(neighborCoords[i]);
					GridCell neighborCell = GridCells[neighborElement];
					ShipCell localShipCell = localGridCell.shipCellComponent;
					ShipCell neighborShipCell = neighborCell.shipCellComponent;

					Vector2 neighborCheck = 
						localGridCell.gridCoordinate.SumCoordinatesAsVector2(neighborCell.gridCoordinate);
					
					for (int j = 0; j < 6; j++) {
						if(neighborDirection[j]==neighborCheck){							
							neighborShipCell.hasCellNeighborAt.Remove(j);
							neighborShipCell.UpdateHullSection(j);
						}
					}

					neighborShipCell.UpdateNeighborInts();

					neighborShipCell.neighborShipCells.Remove(localGridCell.gridCoordinate);
					//update hull meshse on relevant cells
				}

				Destroy(localGridCell.shipCell);
				localGridCell.shipCell = default(GameObject);
				localGridCell.shipCellComponent = default(ShipCell);
			}
		}
	}

	#region IsValidGridCell

	bool IsValidGridCell(Vector3 inputPos){		
		Vector2 gridVector = ReturnGridCellCoordinate(inputPos);
		bool isValidX = (0 <= gridVector.x) && (gridVector.x < gridDimensionX);
		bool isValidY = (0 <= gridVector.y) && (gridVector.y < gridDimensionY);
		
		return (isValidX && isValidY);
	}
	
	bool IsValidGridCell(Vector2 gridVector){		
		bool isValidX = (0 <= (gridVector.x + gridVector.y / 2)) && ((gridVector.x + gridVector.y / 2)< gridDimensionX);
		bool isValidY = (0 <= gridVector.y) && (gridVector.y < gridDimensionY);
		
		return (isValidX && isValidY);
	}

	bool IsValidGridCell(int x, int y){
		bool isValidX = (0 <= x) && (x < gridDimensionX);
		bool isValidY = (0 <= y) && (y < gridDimensionY);

		return (isValidX && isValidY);
	}

	bool IsValidGridCell(GridCoordinate coord){
		bool isValidX = (0 <= (coord.x + coord.y / 2)) && ((coord.x + coord.y / 2) < gridDimensionX);
		bool isValidY = (0 <= coord.y) && (coord.y < gridDimensionY);

		return (isValidX && isValidY);
	}

	#endregion

	#region Grid Orientation

	Vector3 ReturnCurrentMousePosition(){
		float mouseX = Input.mousePosition.x;
		float mouseY = Input.mousePosition.y;
		
		Vector3 mouseVector = new Vector3(mouseX, mouseY, 0f);
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouseVector);
		
		return mousePosition;
	}
	
	int ReturnGridCellElement(Vector3 position){		
		float x = position.x / (HexSizeData.innerRadius * 2f);
		float offset = position.y / (HexSizeData.outerRadius * 3f);
		
		float z = -x;
		
		x -= offset;
		z -= offset;
		
		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(z);
		int iZ = Mathf.RoundToInt(-x - z);
		
		if (iX + iY + iZ != 0){
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(z - iY);
			float dZ = Mathf.Abs(-x - z - iZ);
			
			if (dX > dY && dX > dZ){
				iX = -iY - iZ;
			}
			else if (dZ > dY){
				iZ = -iX - iY;
			}
		}
		
		return iX + iZ * gridDimensionX + iZ / 2;
	}
	
	Vector2 ReturnGridCellCoordinate(Vector3 position){		
		float x = position.x / (HexSizeData.innerRadius * 2f);
		float offset = position.y / (HexSizeData.outerRadius * 3f);
		
		float z = -x;
		
		x -= offset;
		z -= offset;
		
		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(z);
		int iZ = Mathf.RoundToInt(-x - z);
		
		if (iX + iY + iZ != 0){
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(z - iY);
			float dZ = Mathf.Abs(-x - z - iZ);
			
			if (dX > dY && dX > dZ){
				iX = -iY - iZ;
			}
			else if (dZ > dY){
				iZ = -iX - iY;
			}
		}
		return new Vector2(iX, iZ);
	}
	
	GridCoordinate ReturnCellGridCoordinate(Vector3 position){		
		float x = position.x / (HexSizeData.innerRadius * 2f);
		float offset = position.y / (HexSizeData.outerRadius * 3f);
		
		float z = -x;
		
		x -= offset;
		z -= offset;
		
		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(z);
		int iZ = Mathf.RoundToInt(-x - z);
		
		if (iX + iY + iZ != 0){
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(z - iY);
			float dZ = Mathf.Abs(-x - z - iZ);
			
			if (dX > dY && dX > dZ){
				iX = -iY - iZ;
			}
			else if (dZ > dY){
				iZ = -iX - iY;
			}
		}
		
		return new GridCoordinate(iX,iZ);
	}
	
	int CoordToElement(GridCoordinate coord){
		return coord.x + coord.y * gridDimensionX + coord.y / 2;
	}

	#endregion

	void DebugCube(Vector3 position){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = position;
		cube.transform.localScale *= 0.25f;
		cube.GetComponent<Renderer>().material.color = Color.red;
	}

	#endregion
}