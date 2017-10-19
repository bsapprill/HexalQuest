using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipConstructor : MonoBehaviour {
	public int gridDimensionX;
	public int gridDimensionY;

	int globalCellCount = 0;

	GridCell[] GridCells;

	ShipCell selectedShipCell;

	public GameObject playersShipObj;
	public float hullHeight;
	public float shellDepth;
	public float shellInset;
	Ship playersShip;

	public Camera localCamera;

	#region Constructor

	public ShipConstructor (){
		
	}

	#endregion

	Dictionary<ShipCell,CellUpgrade> currentUpgradeList = new Dictionary<ShipCell, CellUpgrade>();

	public enum Customization {PowerCore, CommandBridge, WeaponModule, ShieldSystem,
							   WarpDrive, SensorRelay, CrewQuarter, ImpulseEngine}

	#region Neighboring Values

	enum HexDirection {UP_RIGHT, RIGHT, DOWN_RIGHT, DOWN_LEFT, LEFT, UP_LEFT};
	HexDirection neighborEnum;

	//Loops clockwise, starting from top-right
	Vector2[] neighborDirection = new Vector2[] { 
		new Vector2( 0,  1), new Vector2( 1, 0 ), new Vector2( 1, -1),
		new Vector2( 0, -1), new Vector2(-1,  0), new Vector2( -1, 1)
	};

	#endregion

	#region Color Data

	[Serializable]
	public class ColorDataHolder{
		public Color[] UpgradeColors = new Color[8];
		public Color[] HullColors = new Color[3];
	}

	public ColorDataHolder ColorData;

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

			public Text powerGridUsed;
			public Text powerGridMax;

			public Text crewAvailable;
			public Text crewNeeded;

			public Text warpSpeed;

			public Text shipSpeed;

			public Text sensorStrength;

			public Text weaponPower;

			public Text shieldPower;
		}

		public BuildStatePanelElements BuildStatePanel;
		public CellOptionsPanelElements CellOptionsPanel;
		public StatsPanelElements StatsPanel;
		public Button LaunchButton;
	}

	public UISystem UIData;

	#region UI Function

	public void EnterHullEdit(){
		UIData.BuildStatePanel.HullBuild.interactable = false;
		UIData.BuildStatePanel.CellBuild.interactable = true;
		if(selectedShipCell != null){
			RevertSelectedShipCellTransform();
		}
		selectedShipCell = default(ShipCell);
		SetStateBuildHullMode();
	}
		
	public void EnterCellEdit(){
		UIData.BuildStatePanel.HullBuild.interactable = true;
		UIData.BuildStatePanel.CellBuild.interactable = false;
		SetStateIdle();
	}

	public void UpdateUIStats(){

		#region Power Grid

		UIData.StatsPanel.powerGridUsed.text = "Used: "+playersShip.powerGridDemand;
		UIData.StatsPanel.powerGridMax.text = "Max: "+playersShip.powerGridSupply;

		bool powerGridOver = playersShip.powerGridDemand > playersShip.powerGridSupply;
		if (powerGridOver) {
			UIData.StatsPanel.powerGridUsed.color = Color.red;
		}
		else{
			UIData.StatsPanel.powerGridUsed.color = Color.gray;
		}

		#endregion

		#region Crew

		UIData.StatsPanel.crewAvailable.text = "Have: "+playersShip.crewCount;
		UIData.StatsPanel.crewNeeded.text = "Need: "+playersShip.crewDemand;

		bool crewCountUnder = playersShip.crewCount < playersShip.crewDemand;
		if (crewCountUnder) {
			UIData.StatsPanel.crewNeeded.color = Color.red;
		}
		else{
			UIData.StatsPanel.crewNeeded.color = Color.gray;
		}

		#endregion

		#region Warp

		UIData.StatsPanel.warpSpeed.text = "Speed: "+playersShip.GetWarpSpeed();

		#endregion

		#region Engine Speed

		UIData.StatsPanel.shipSpeed.text = "Speed: "+playersShip.GetShipSpeed();

		#endregion

		#region Sensor

		UIData.StatsPanel.sensorStrength.text = "Strength: "+playersShip.sensorStrength;

		#endregion

		#region Weapon

		UIData.StatsPanel.weaponPower.text = "Power: "+playersShip.attackPower;

		#endregion

		#region Shield

		UIData.StatsPanel.shieldPower.text = "Power: "+playersShip.shieldPower;	

		#endregion
	}

	#endregion

	#region Cell Customizers

	public void ApplyCellCustomization(int i){
		selectedShipCell.UpdateCellHubVisuals(ColorData.UpgradeColors[i]);
		if (selectedShipCell.hasCellUpgrade) {			
			RemoveCustomization(selectedShipCell);
			AddCustomization((Customization)i);
		}
		else{
			AddCustomization((Customization)i);
			selectedShipCell.hasCellUpgrade = true;
		}
		RevertSelectedShipCellTransform();
		selectedShipCell = default(ShipCell);
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
		}

		if(Input.GetKeyDown(KeyCode.F)){
			LaunchShipIntoSpace();
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
						if(selectedShipCell != null){
							RevertSelectedShipCellTransform();
						}
						selectedShipCell = gridCell.shipCellComponent;
						selectedShipCell.gameObject.transform.position += new Vector3(0f,0f,-0.1f);
						selectedShipCell.gameObject.transform.localScale = new Vector3(1.5f,1.5f);
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
		BuildState = Construction.BuildHullMode;
		AlignCameraToGridCenter();

		stateMain.Add(Construction.Idle, new Action(Idle));
		stateMain.Add(Construction.BuildHullMode, new Action(BuildHullMode));
		stateMain.Add(Construction.CustomizeShipCell, new Action(CustomizeShipCell));

		playersShipObj = new GameObject();
		playersShipObj.AddComponent<Ship>();
		playersShipObj.AddComponent<MeshFilter>();
		playersShipObj.AddComponent<MeshRenderer>();
		playersShip = playersShipObj.GetComponent<Ship>();
		playersShipObj.name = "PlayerShip";
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
		localCamera.transform.position = newCameraPosition/2f;
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
				localShipCell.SetHullColors(ColorData.HullColors);

				int listAdjustment = 0;
				for (int i = 0; i < 6; i++) {
					if(localGridCell.hasGridNeighborAt[i]==false){
						listAdjustment++;
						continue;
					}
					GridCoordinate neighborCoord = localGridCell.neighborCoordinates[i-listAdjustment];
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
				newCell.transform.SetParent(transform);
				newCell.name = "Ship Cell "+globalCellCount;
				globalCellCount++;
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

				for (int i = 0; i < neighborCoords.Count; i++) {
					int neighborElement = CoordToElement(neighborCoords[i]);
					GridCell neighborCell = GridCells[neighborElement];
					ShipCell neighborShipCell = neighborCell.shipCellComponent;

					Vector2 neighborCheck = 
						localGridCell.gridCoordinate.SumCoordinatesAsVector2(neighborCell.gridCoordinate);
					
					for (int j = 0; j < 6; j++) {
						if(neighborDirection[j]==neighborCheck){							
							neighborShipCell.hasCellNeighborAt.Remove(j);
							neighborShipCell.UpdateHullSection(j);
						}
					}						
					neighborShipCell.neighborShipCells.Remove(localGridCell.gridCoordinate);
				}

				if(localGridCell.shipCellComponent.hasCellUpgrade){
					RemoveCustomization(localGridCell.shipCellComponent);
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
		Vector3 mousePosition = localCamera.ScreenToWorldPoint(mouseVector);
		
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

	#region SelectedShipCell Helpers

	void RevertSelectedShipCellTransform(){
		selectedShipCell.gameObject.transform.position += new Vector3(0f,0f,0.1f);
		selectedShipCell.gameObject.transform.localScale = Vector3.one;
	}

	#endregion

	#region ShipCellUpgrade

	public void AddCustomization(Customization CustomizationType){		
		switch(CustomizationType){
		case Customization.PowerCore:
			currentUpgradeList.Add(selectedShipCell, new PowerCore());
			currentUpgradeList[selectedShipCell].AddUpgrade(playersShip);
			break;
		case Customization.CommandBridge:
			currentUpgradeList.Add(selectedShipCell, new CommandBridge());
			currentUpgradeList[selectedShipCell].AddUpgrade(playersShip);
			break;
		case Customization.WeaponModule:
			currentUpgradeList.Add(selectedShipCell, new WeaponModule());
			currentUpgradeList[selectedShipCell].AddUpgrade(playersShip);
			break;
		case Customization.ShieldSystem:
			currentUpgradeList.Add(selectedShipCell, new ShieldSystem());
			currentUpgradeList[selectedShipCell].AddUpgrade(playersShip);
			break;
		case Customization.WarpDrive:
			currentUpgradeList.Add(selectedShipCell, new WarpDrive());
			currentUpgradeList[selectedShipCell].AddUpgrade(playersShip);
			break;
		case Customization.SensorRelay:
			currentUpgradeList.Add(selectedShipCell, new SensorRelay());
			currentUpgradeList[selectedShipCell].AddUpgrade(playersShip);
			break;
		case Customization.CrewQuarter:
			currentUpgradeList.Add(selectedShipCell, new CrewQuarter());
			currentUpgradeList[selectedShipCell].AddUpgrade(playersShip);
			break;
		case Customization.ImpulseEngine:
			currentUpgradeList.Add(selectedShipCell, new ImpulseEngine());
			currentUpgradeList[selectedShipCell].AddUpgrade(playersShip);
			break;
		}

		UpdateUIStats();
	}

	public void RemoveCustomization(ShipCell shipCell){
		currentUpgradeList[shipCell].RemoveUpgrade(playersShip);
		currentUpgradeList.Remove(shipCell);
		UpdateUIStats();
	}

	#endregion

	#region Launch

	public void LaunchShipIntoSpace(){
		BuildShipsSpaceMesh(playersShipObj.GetComponent<MeshFilter>());
		ShipCell[] shipCells = GetComponentsInChildren<ShipCell>();
		for (int i = 0; i < shipCells.Length; i++) {
			shipCells[i].gameObject.SetActive(false);
		}
		playersShipObj.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");
		gameObject.SetActive(false);
	}

	void BuildShipsSpaceMesh(MeshFilter shipFilter){
		List<Vector3> outerHullVertices = new List<Vector3>();
		List<Vector3> outerVertexDirections = new List<Vector3> ();
		RetrieveOuterHullVertices(outerHullVertices, outerVertexDirections);
		MeshHandler mH = new MeshHandler();
		mH.AssignOuterHullMeshData(outerHullVertices, outerVertexDirections, hullHeight, shellDepth, shellInset);
		mH.ReturnCompleteMesh(shipFilter);
	}

	void RetrieveOuterHullVertices(List<Vector3> localVertexList, List<Vector3> shellDirectionList){
		List<GameObject> hullObjs = new List<GameObject>();
		PopulateHullObjList(hullObjs);
		for (int i = 0; i < hullObjs.Count; i++) {
			HullSection localHullSection = hullObjs[i].GetComponent<HullSection>();
			if(localHullSection.isInnerHull == false){
				MeshFilter localMF = hullObjs[i].GetComponent<MeshFilter>();									

				Vector3 shellVertexDirectionOne = localMF.mesh.vertices[1] - localMF.mesh.vertices[0];
				Vector3 shellVertexDirectionTwo = localMF.mesh.vertices [2] - localMF.mesh.vertices [3];
				Vector3 hullAdjustmentOne = localMF.mesh.vertices[1] - ((shellVertexDirectionOne) * 0.5f);
				Vector3 hullAdjustmentTwo = localMF.mesh.vertices[2] - ((shellVertexDirectionTwo) * 0.5f);

				//There is a potential optimization here b/c this repeats vertices
				localVertexList.Add(hullAdjustmentOne + hullObjs[i].transform.position);
				localVertexList.Add(hullAdjustmentTwo + hullObjs[i].transform.position);
				shellDirectionList.Add ((hullAdjustmentOne * 0.5f).normalized);
				shellDirectionList.Add((hullAdjustmentTwo * 0.5f).normalized);
			}
		}
	}

	void PopulateHullObjList(List<GameObject> localHullObjs){
		ShipCell[] shipCells = GetComponentsInChildren<ShipCell>();
		for (int i = 0; i < shipCells.Length; i++) {
			for (int j = 0; j < shipCells[i].hullSectionObjs.Length; j++) {
				localHullObjs.Add(shipCells[i].hullSectionObjs[j]);
			}
		}
	}
		
	#endregion

	void DebugOuterHullSections(){
		List<GameObject> gOList = new List<GameObject>();
		PopulateHullObjList(gOList);
		for (int i = 0; i < gOList.Count; i++) {
			Debug.Log(gOList[i].GetComponent<HullSection>().isInnerHull);
		}
	}

	void DebugCube(Vector3 position){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = position;
		cube.transform.localScale *= 0.025f;
		cube.GetComponent<Renderer>().material.color = Color.red;
	}

	#endregion
}