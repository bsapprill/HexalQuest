using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewQuarter : CellUpgrade {
	public int crewProvided;
	public CrewQuarter (){
		powerGridDemand = 1;
		crewDemand = 0;
		cellMass = 1;
		crewProvided = 2;
	}

	public override void AddUpgrade(Ship playerShip){
		base.AddUpgrade(playerShip);
		playerShip.crewCount += crewProvided;

	}
	public override void RemoveUpgrade(Ship playerShip){
		base.RemoveUpgrade(playerShip);
		playerShip.crewCount -= crewProvided;
	}
}