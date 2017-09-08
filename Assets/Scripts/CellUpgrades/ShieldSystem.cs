using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSystem : CellUpgrade {
	public int shieldPower;
	public int shieldRecharge;
	public ShieldSystem (){
		powerGridDemand = 1;
		crewDemand = 0;
		cellMass = 1;
		shieldPower = 1;
	}

	public override void AddUpgrade(Ship playerShip){
		base.AddUpgrade(playerShip);
		playerShip.shieldPower += shieldPower;
	}
	public override void RemoveUpgrade(Ship playerShip){
		base.RemoveUpgrade(playerShip);
		playerShip.shieldPower -= shieldPower;
	}
}