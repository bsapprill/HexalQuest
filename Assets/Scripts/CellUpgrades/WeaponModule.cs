using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModule : CellUpgrade {
	public int attackPower;
	public float attackCooldown;
	public WeaponModule (){
		powerGridDemand = 1;
		crewDemand = 0;
		cellMass = 1;
		attackPower = 1;
	}

	public override void AddUpgrade(Ship playerShip){
		base.AddUpgrade(playerShip);
		playerShip.attackPower += attackPower;

	}
	public override void RemoveUpgrade(Ship playerShip){
		base.RemoveUpgrade(playerShip);
		playerShip.attackPower -= attackPower;
	}
}