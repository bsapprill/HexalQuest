using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	#region Variables

	#region Ship Construction Elements

	public GameObject currentShipConstructorObj;
	ShipConstructor currentShipConstructor;
	public GameObject shipConstructionCamera;
	public GameObject ShipConstructionCanvas;

	#endregion

	#region SubSpace Elements

	public GameObject subSpaceCamera;
	public GameObject subSpaceCanvas;
	public GameObject subSpaceLight;

	#endregion

	#endregion

	#region State Machine

	#region Initials

	enum InteractionTier {ShipConstruction, SubSpace, Local, System, Constellation, Region, Cluster}

	InteractionTier currentTier;

	Dictionary<InteractionTier, Action> currentInteractionTier = new Dictionary<InteractionTier, Action>();

	#endregion

	void ShipConstruction(){
		if(currentShipConstructorObj.gameObject.activeInHierarchy == false){
			ActivateSubSpaceElements();
			ShipConstructionCanvas.SetActive(false);
			currentTier = InteractionTier.SubSpace;
		}
	}

	void SubSpace(){
		
	}

	void Local(){
		
	}

	void System(){
	}

	#endregion

	#region Defaults
	void Awake(){
		
	}

	void Start(){
		#region State Initilization

		currentInteractionTier.Add(InteractionTier.ShipConstruction, new Action(ShipConstruction));
		currentInteractionTier.Add(InteractionTier.SubSpace, new Action(SubSpace));
		currentInteractionTier.Add(InteractionTier.Local, new Action(Local));
		currentInteractionTier.Add(InteractionTier.System, new Action(System));
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
		subSpaceCamera.SetActive(true);
		subSpaceCanvas.SetActive(true);
	}

	void DeactivateSubSpaceElements(){
		subSpaceLight.SetActive(false);
		subSpaceCamera.SetActive(false);
		subSpaceCanvas.SetActive(false);
	}

	#endregion

	#endregion
}