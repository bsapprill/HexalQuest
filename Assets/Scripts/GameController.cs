using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	#region Variables

	#region Ship Construction Elements

	public GameObject currentShipConstructorObj;
	ShipConstructor currentShipConstructor;
	public GameObject ShipConstructionCanvas;

	#endregion

	#region SubSpace Elements

	public GameObject constructorSubSpaceObj;
	public GameObject subSpaceCanvas;
	public GameObject subSpaceLight;

	public float zoomScaler;
	float shipSpeed;
	int zoomCounter = 0;

	GameObject SubSpace_ColliderPlane;
	//This refers to the parent obj used to change the pivot
	GameObject playerShipObj;
	//The actual ship obj and mesh created by the ship constructor
	GameObject playerShip;
	Ship playerShipComponent;


	Quaternion constructionCameraRot;
	Vector3 constructionCameraPos;
	Vector3 currentSubSpaceDestination;

	#endregion

	#region TierSpace

	GameObject shipObjectSprite;
	GameObject mapObjectSprite;

	TierEntity activeTier;
	TierEntity startingPlanet;
	int tierCounter;

	#endregion

	//Determines how close the camera can get to the player ship
	public float closestCameraRadius;
	public int maxZoomTicksSubSpace;
	public float initialCameraAngle;

	#region Environment Tiering
	static int maxGridRadius = 9;
	Vector3[,] gridPositionTable = new Vector3[maxGridRadius,maxGridRadius];

	static int radiusMin = 3;
	static int radiusMax = 5;
	static int clusterCount = 1;
	static int randomMin = 1;
	static int randomMax = 4;

	TierEntity[] clusters = new TierEntity[clusterCount];
	//static string[] entityNames = { "Cluster", "Region", "Constellation", "StarSystem", "Planet" };

	public float planetScaler;
	public Vector3 shipStartScaler;

	#endregion

	#endregion

	#region State Machine

	#region Initials

	enum InteractionTier {ShipConstruction, TierTransition, TierHandler, SubSpace, Planet, System, Constellation, Region, Cluster}

	InteractionTier currentTier;

	Dictionary<InteractionTier, Action> currentInteractionTier = new Dictionary<InteractionTier, Action>();

	#endregion

	void ShipConstruction(){
		//Ship constructor turns itself off... the game controller will transition once
		//this happens
		if(currentShipConstructorObj.gameObject.activeInHierarchy == false){
			playerShipObj = GameObject.Find ("PlayerShipObj");
			if (playerShip == null) {
				playerShip = GameObject.Find ("PlayerShip");
			}
			playerShip.SetActive (true);
			playerShipComponent = playerShip.GetComponent<Ship>();
			shipSpeed = playerShipComponent.GetShipSpeed();
			constructionCameraPos = Camera.main.transform.position;
			constructionCameraRot = Camera.main.transform.rotation;
			ActivateSubSpaceElements();
			ShipConstructionCanvas.SetActive(false);
			currentTier = InteractionTier.SubSpace;
		}
	}

	void SubSpace(){
		
		if(Input.GetKeyDown(KeyCode.F)){
		}
		if(Input.GetKey(KeyCode.E)){
			Camera.main.transform.RotateAround (playerShipObj.transform.position,
				Vector3.up, -50f * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.Q)){
			Camera.main.transform.RotateAround (playerShipObj.transform.position,
				Vector3.up, 50f * Time.deltaTime);
		}
		MoveShipAndCamera ();
		ZoomControl ();
		if(Input.GetMouseButton(0)){
		}
		if(Input.GetMouseButtonDown(1)){
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				if(hit.collider.name=="Constructor_SubSpace"){
					currentShipConstructorObj.SetActive (true);
					ShipCell[] shipCells = currentShipConstructorObj.GetComponentsInChildren<ShipCell> (true);
					for (int i = 0; i < shipCells.Length; i++) {
						shipCells[i].gameObject.SetActive (true);
					}
					ShipConstructionCanvas.SetActive(true);
					DeactivateSubSpaceElements(false);
					currentTier = InteractionTier.ShipConstruction;
				}
				else if(hit.collider.gameObject == SubSpace_ColliderPlane){
					currentSubSpaceDestination = new Vector3 (hit.point.x, playerShipObj.transform.position.y, hit.point.z);
					//Ship must smoothly travel to this point
					//It would be easy enough to make the ship go straight to the point
					//Making it flow there increases visual appeal, but is extra work?
					//May not be so hard if slerp does what I think it does
				}
			}
		}
		if(currentSubSpaceDestination != playerShipObj.transform.position){
			playerShipObj.transform.Translate (Vector3.forward * Time.deltaTime * shipSpeed);
			//Quaternion turnAngle = Quaternion.FromToRotation()
			//playerShipObj.transform.rotation = Quaternion.Lerp
		}
	}
		
	void TierTransition(){		
		if(tierCounter == (int)InteractionTier.Planet){
			HandlePlanetActiveTier ();
		}
		else if(tierCounter == (int)InteractionTier.System){
			HandleSystemActiveTier ();
		}
		currentTier = InteractionTier.TierHandler;
	}

	void HandlePlanetActiveTier(){
		SetBodyObjectPosition ();
		shipObjectSprite.SetActive (true);
		shipObjectSprite.transform.position = activeTier.positionInGrid + shipStartScaler;
	}

	void HandleSystemActiveTier(){
		SetBodyObjectPosition ();
		for (int a = 0; a < activeTier.childEntities.Count; a++) {
			GameObject localBody = activeTier.childEntities [a].objectToView;
			if(localBody == null){
				localBody = Instantiate (mapObjectSprite) as GameObject;
				localBody.SetActive (true);
			}
			localBody.transform.position = activeTier.childEntities [a].positionInGrid;
			localBody.transform.localScale = Vector3.one * planetScaler;
		}
	}

	void SetBodyObjectPosition(){
		if(activeTier.objectToView == null){
			activeTier.objectToView = Instantiate (mapObjectSprite) as GameObject;
			activeTier.objectToView.gameObject.SetActive (true);
		}
		Vector3 bodyPosition = gridPositionTable[activeTier.entityRadius/2, activeTier.entityRadius/2];
		ChangeMapCameraPosition (bodyPosition);
		activeTier.objectToView.transform.position = bodyPosition;
	}

	void TierHandler(){
		ZoomControl ();
	}

	#endregion

	#region Classes

	class TierEntity{
		public int number;
		public InteractionTier entityTier;
		public Dictionary<int, TierEntity> childEntities = new Dictionary<int, TierEntity>();
		public TierEntity parentEntity;
		public Dictionary<int, TierEntity> neighborEntities = new Dictionary<int, TierEntity>();
		public int entityRadius;
		public Vector3 positionInGrid;
		public GameObject objectToView;
		public List<GameObject> orbitingEntities = new List<GameObject>();

		public TierEntity(){
			
		}

		public TierEntity(int _number){
			number = _number;
		}

		public TierEntity (int _number, InteractionTier _tier, TierEntity _parentEntity){
			number = _number;
			entityTier = _tier;
			parentEntity = _parentEntity;
		}

		/*
		public TierEntity (int _number, InteractionTier _tier, int _radius, TierEntity _parentEntity){
			number = _number;
			entityTier = _tier;
			entityRadius = _radius;
			parentEntity = _parentEntity;
		}*/


		public TierEntity (int _number, InteractionTier _tier, int _radius, TierEntity _parentEntity){
			number = _number;
			entityTier = _tier;
			entityRadius = _radius;
			parentEntity = _parentEntity;

			//Stops recursion if at local tier
			if(entityTier != InteractionTier.Planet){				
				//Number of sub entities
				int randomRange = UnityEngine.Random.Range (randomMin, randomMax + 1);
				//Generates sub entities
				for (int a = 0; a < randomRange; a++) {
					int randomRadius = UnityEngine.Random.Range (radiusMin, radiusMax+1);
					TierEntity newEntity = new TierEntity (a, entityTier-1, randomRadius, this);
					childEntities.Add(a, newEntity);
				}
			}
		}
	}

	void GenerateTierEntityChains(TierEntity firstEntity){
		int regionCount = UnityEngine.Random.Range (randomMin, randomMax + 1);
		for (int a = 0; a < regionCount; a++) {
			int constellationCount = UnityEngine.Random.Range (randomMin, randomMax + 1);
			int regionRadius = UnityEngine.Random.Range (radiusMin, radiusMax+1);
			TierEntity newRegion = new TierEntity (a, InteractionTier.Region, regionRadius, firstEntity);
			Debug.Log (newRegion);
			//firstEntity.childEntities.Add(a, newRegion);

			for (int b = 0; b < constellationCount; b++) {
				int systemCount = UnityEngine.Random.Range (randomMin, randomMax + 1);
				int constellationRadius = UnityEngine.Random.Range (radiusMin, radiusMax+1);
				TierEntity newConstellation = new TierEntity (b, InteractionTier.Constellation, constellationRadius, newRegion);
				newRegion.childEntities.Add (b, newConstellation);

				for (int c = 0; c < systemCount; c++) {
					int planetCount = UnityEngine.Random.Range (randomMin, randomMax + 1);
					int systemRadius = UnityEngine.Random.Range (radiusMin, radiusMax+1);
					TierEntity newSystem = new TierEntity (c, InteractionTier.System, systemRadius, newConstellation);
					newConstellation.childEntities.Add (c, newSystem);

					for (int d = 0; d < planetCount; d++) {
						TierEntity newPlanet = new TierEntity (d, InteractionTier.Planet, newSystem);
						newSystem.childEntities.Add (d, newPlanet);
					}
				}
			}
		}
	}

	#endregion

	#region Defaults

	void Awake(){
		SubSpace_ColliderPlane = GameObject.Find ("SubSpace_ColliderPlane");

		//Builds the grid table
		for (int y = 0, w = 0; y < maxGridRadius; y++) {
			for (int x = 0; x < maxGridRadius; x++, w++) {
				float localX = (x + y * 0.5f - y / 2) * HexSizeData.innerRadius * 2f;
				float localY = (y * HexSizeData.outerRadius * 1.5f);
				gridPositionTable [x,y] = new Vector3 (localX, localY);
			}
		}

		shipObjectSprite = GameObject.CreatePrimitive (PrimitiveType.Quad);
		shipObjectSprite.GetComponent<Renderer> ().material.shader = Shader.Find ("Unlit/Color");
		shipObjectSprite.GetComponent<Renderer> ().material.color = Color.white;
		shipObjectSprite.gameObject.SetActive (false);

		mapObjectSprite = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		mapObjectSprite.GetComponent<Renderer> ().material.shader = Shader.Find ("Unlit/Color");
		mapObjectSprite.GetComponent<Renderer> ().material.color = Color.white;
		mapObjectSprite.gameObject.SetActive (false);

		GenerateEnvironment ();
	}
		
	void Start(){
		#region State Initialization

		currentInteractionTier.Add(InteractionTier.ShipConstruction, new Action(ShipConstruction));
		currentInteractionTier.Add(InteractionTier.SubSpace, new Action(SubSpace));
		currentInteractionTier.Add(InteractionTier.TierTransition, new Action(TierTransition));
		currentInteractionTier.Add(InteractionTier.TierHandler, new Action(TierHandler));
		currentTier = InteractionTier.ShipConstruction;

		#endregion
	}

	void Update(){
		currentInteractionTier[currentTier].Invoke();
	}

	#endregion

	#region Functions

	#region State Transition

	void ActivateSubSpaceElements(){
		subSpaceLight.SetActive(true);
		subSpaceCanvas.SetActive(true);
		Camera.main.orthographic = false;
		constructorSubSpaceObj.SetActive (true);
		SetShipLaunchPosition ();
		CalculateDefaultLaunchCamera();
	}

	void DeactivateSubSpaceElements(bool shipBool){
		subSpaceLight.SetActive(false);
		subSpaceCanvas.SetActive(false);
		RevertCameraPositionToShipConstruction();
		constructorSubSpaceObj.SetActive (false);
		playerShip.SetActive (shipBool);
	}

	void SetTierSpaceElements(){
		if (activeTier == null) {
			activeTier = startingPlanet;
		}
		tierCounter = (int)InteractionTier.Planet;		
	}

	void EnterTierSpace(){
		//Leaves the player ship obj on for use in tier space
		DeactivateSubSpaceElements (false);
		SetTierSpaceElements ();
		currentTier = InteractionTier.TierTransition;
	}

	void EnterHigherTierSpace(){
		tierCounter++;
		activeTier = activeTier.parentEntity;
		if(tierCounter == (int)InteractionTier.System){
			
		}
		currentTier = InteractionTier.TierTransition;
	}

	void EnterLowerTierSpace(){
		tierCounter--;
		currentTier = InteractionTier.TierTransition;
	}

	void ZoomControl(){
		if(Input.GetAxis("Mouse ScrollWheel") > 0f){
			if (zoomCounter > 0) {
				zoomCounter--;
				Camera.main.transform.Translate (Vector3.forward * zoomScaler);
			}
			else if(currentTier != InteractionTier.SubSpace){
				EnterLowerTierSpace ();
			}
		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0f) {
			if (zoomCounter < maxZoomTicksSubSpace) {
				zoomCounter++;
				Camera.main.transform.Translate (Vector3.back * zoomScaler);
			}
			else if(currentTier == InteractionTier.SubSpace){
				EnterTierSpace();
			}
			else if(currentTier != InteractionTier.Cluster){
				EnterHigherTierSpace ();
			}
		}
	}

	#endregion

	#region MapGeneration

	void GenerateEnvironment(){		
		for (int a = 0; a < clusterCount; a++) {
			int randomRadius = UnityEngine.Random.Range (radiusMin, radiusMax+1);
			clusters [a] = new TierEntity (a, InteractionTier.Cluster, randomRadius, null);
		}
		//GenerateTierEntityChains (clusters [0]);
		SetStartingPlanet ();
		SetEntityPositions (clusters [0]);
	}

	void SetEntityPositions(TierEntity _entity){
		List<Vector2> childPosition = new List<Vector2> ();

		for (int a = 0; a < _entity.childEntities.Count; a++) {
			int randomGridX = UnityEngine.Random.Range (0, _entity.entityRadius);
			int randomGridY = UnityEngine.Random.Range (0, _entity.entityRadius);
			Vector2 positionCheck = new Vector2 (randomGridX, randomGridY);
			Vector2 gridCenter = new Vector2 (_entity.entityRadius / 2, _entity.entityRadius / 2);

			while(childPosition.Contains(positionCheck) || positionCheck == gridCenter){
				randomGridX = UnityEngine.Random.Range (0, _entity.entityRadius);
				randomGridY = UnityEngine.Random.Range (0, _entity.entityRadius);
				positionCheck = new Vector2 (randomGridX, randomGridY);
			}
			childPosition.Add (positionCheck);
			_entity.childEntities [a].positionInGrid = gridPositionTable [(int)positionCheck.x, (int)positionCheck.y];
			if (_entity.entityTier != InteractionTier.System) {
				SetEntityPositions (_entity.childEntities [a]);
			}
		}
	}

	void SetStartingPlanet(){		
		int regionCount = clusters[0].childEntities.Count;

		int startRegion = UnityEngine.Random.Range (0, regionCount);

		TierEntity region = clusters [0].childEntities [startRegion];
		int constellationCount = region.childEntities.Count;
		int startConstellation = UnityEngine.Random.Range (0, constellationCount);

		TierEntity constellation = region.childEntities [startConstellation];
		int systemCount = constellation.childEntities.Count;
		int startSystem = UnityEngine.Random.Range (0, systemCount);

		TierEntity system = constellation.childEntities[startSystem];
		int planetCount = system.childEntities.Count;
		int startPlanet = UnityEngine.Random.Range (0, systemCount);

		startingPlanet = system.childEntities[startPlanet];
	}

	#endregion

	void RevertCameraPositionToShipConstruction(){
		Camera.main.orthographic = true;
		Camera.main.transform.position = constructionCameraPos;
		Camera.main.transform.rotation = constructionCameraRot;
	}

	void ChangeCameraPosition(Vector3 newPos){
		
		Camera.main.transform.position = newPos;
	}

	void ChangeMapCameraPosition(Vector3 newPos){
		newPos = new Vector3 (newPos.x, newPos.y, -1f);
		Camera.main.transform.position = newPos;
	}

	void CalculateDefaultLaunchCamera(){

		//Equals 45 degrees around the forward axis while pointed right
		Vector3 initialDirection = Quaternion.AngleAxis (initialCameraAngle, playerShipObj.transform.forward)
								   * playerShipObj.transform.right;
		Vector3 initialPos = initialDirection * closestCameraRadius;
		initialPos.x = playerShipObj.transform.position.x;
		Camera.main.transform.position = initialPos;

		Camera.main.transform.LookAt (playerShipObj.transform.position);
	}

	//Default launch position as of 0.1
	void SetShipLaunchPosition(){		
		OffsetShipMeshForLaunch ();
		playerShipObj.transform.Rotate (0f, 90f, 0f);

		playerShipObj.transform.Translate(Vector3.forward * 5f);
	}

	void OffsetShipMeshForLaunch(){
		playerShipObj.transform.position = playerShip.GetComponent<Renderer> ().bounds.center;
		playerShip.transform.SetParent (playerShipObj.transform);
		playerShipObj.transform.position = Vector3.zero;
		playerShipObj.transform.rotation = Quaternion.Euler (Vector3.zero);
	}

	void MoveShipAndCamera(){
		Vector3 forwardPos = playerShipObj.transform.forward * shipSpeed * Time.deltaTime;

		playerShipObj.transform.position += forwardPos;
		Camera.main.transform.position += forwardPos;
	}

	#endregion

	void DebugCube(Vector3 position){
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = position;
		cube.transform.localScale *= 0.025f;
		cube.GetComponent<Renderer>().material.color = Color.red;
	}
}