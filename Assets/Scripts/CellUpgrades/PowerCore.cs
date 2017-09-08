using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCore : CellUpgrade {
	public int powerGridValue;
	public PowerCore (){
		powerGridDemand = 0;
		crewDemand = 0;
		cellMass = 1;
		powerGridValue = 5;
	}

	public override void AddUpgrade(Ship playerShip){
		base.AddUpgrade(playerShip);
		playerShip.powerGridSupply += powerGridValue;

	}
	public override void RemoveUpgrade(Ship playerShip){
		base.RemoveUpgrade(playerShip);
		playerShip.powerGridSupply -= powerGridValue;
	}
}