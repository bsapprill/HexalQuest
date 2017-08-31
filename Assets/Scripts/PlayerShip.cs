using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {
	//public Dictionary<GameObject, ShipCell> shipCells = new Dictionary<GameObject, ShipCell>();

	//Each module type, excluding the power core, requires a certain amount of power grid to facilitate
	//On top of this, certain module types can be "boosted" by using the active power supply
	public int powerGridDemand;
	public int powerGridSupply;
	public int currentActivePower;
	public int maxActivePower;

	//The command bridge allows the player to lorically control every part of the ship from the bridge
	//The idea was to have diminishing returns in the event the ship's bridge could not support each
	//module type. However, this does not really come into play until larger ship classes come into effect
	//Sensically, the ship cell would be "wining it" if it did not have a bridge to take orders from...
	//Therefore, the idea of using diminishing returns does not seem sensical for this...
	//For now, the command bridge will simply be a required module with immersive justification
	public int commandRequirement;

	//For now, these two stats will handle simple battle calculation
	//I have more complex plans, but these are scoped down
	public int attackPower;
	public int attackRange;

	//Same as the attack stats, these will be simple enough for now
	public int shieldPower;
	public int shieldRecharge;

	//Warping around is very key for the "questing" aspect of the game
	public int warpSpoolUp;
	public int warpSpeed;

	public int sensorRange;
	public int sensorStrength;

	public int crewCount;
	public int crewFatigue;

	public int enginePower;
	public int shipMass;
}