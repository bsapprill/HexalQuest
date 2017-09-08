using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorRelay : CellUpgrade {
	public int sensorStrength;
	public SensorRelay(){
		powerGridDemand = 1;
		crewDemand = 0;
		cellMass = 1;
		sensorStrength = 1;
	}

	public override void AddUpgrade(Ship playerShip){
		base.AddUpgrade(playerShip);
		playerShip.sensorStrength += sensorStrength;

	}
	public override void RemoveUpgrade(Ship playerShip){
		base.RemoveUpgrade(playerShip);
		playerShip.sensorStrength -= sensorStrength;
	}
}