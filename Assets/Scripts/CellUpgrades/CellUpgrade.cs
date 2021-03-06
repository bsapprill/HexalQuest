using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellUpgrade{
	public int powerGridDemand;
	public int crewDemand;
	public int cellMass;

	public virtual void AddUpgrade(Ship playerShip){
		AddBaseElementsToPlayerShip(playerShip);
	}
	public virtual void RemoveUpgrade(Ship playerShip){
		RemoveBaseElementsFromPlayerShip(playerShip);
	}

	void AddBaseElementsToPlayerShip(Ship playerShip){
		playerShip.powerGridDemand += powerGridDemand;
		playerShip.crewDemand += crewDemand;
		playerShip.shipMass += cellMass;
	}

	void RemoveBaseElementsFromPlayerShip(Ship playerShip){
		playerShip.powerGridDemand -= powerGridDemand;
		playerShip.crewDemand -= crewDemand;
		playerShip.shipMass -= cellMass;
	}
}