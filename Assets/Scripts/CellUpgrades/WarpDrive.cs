using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpDrive : CellUpgrade {
	public int warpPower;
	public WarpDrive(){
		powerGridDemand = 3;
		crewDemand = 0;
		cellMass = 1;
		warpPower = 8;
	}
	public override void AddUpgrade(Ship playerShip){
		base.AddUpgrade(playerShip);
		playerShip.warpPower += warpPower;

	}
	public override void RemoveUpgrade(Ship playerShip){
		base.RemoveUpgrade(playerShip);
		playerShip.warpPower -= warpPower;
	}
}