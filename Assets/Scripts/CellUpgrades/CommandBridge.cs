using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBridge : CellUpgrade {
	public CommandBridge (){
		powerGridDemand = 1;
		crewDemand = 2;
		cellMass = 1;
	}

	public override void AddUpgrade(Ship playerShip){
		base.AddUpgrade(playerShip);

	}
	public override void RemoveUpgrade(Ship playerShip){
		base.RemoveUpgrade(playerShip);
	}
}