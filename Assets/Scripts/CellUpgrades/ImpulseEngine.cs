using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseEngine : CellUpgrade {
	public int enginePower;
	public ImpulseEngine (){
		powerGridDemand = 1;
		crewDemand = 0;
		cellMass = 1;
		enginePower = 5;
	}

	public override void AddUpgrade(Ship playerShip){
		base.AddUpgrade(playerShip);
		playerShip.enginePower += enginePower;
	}
	public override void RemoveUpgrade(Ship playerShip){
		base.RemoveUpgrade(playerShip);
		playerShip.enginePower -= enginePower;
	}
}