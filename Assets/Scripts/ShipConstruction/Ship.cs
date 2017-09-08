using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	#region Ship Stats

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
	public float attackSpeed;
	public float attackRange;

	//Same as the attack stats, these will be simple enough for now
	public int shieldPower;
	public int shieldRecharge;

	//More power in the warp drive means faster speed and warp spool up
	public int warpPower;		
	//Warp speed is a result of some kind of ratio between warp power and ship mass
	float warpSpeed;
	public float GetWarpSpeed(){
		float speedVal = warpPower - shipMass;
		if(warpPower == 0f){
			speedVal = 0f;
		}
		else if(speedVal < 1){
			float modifier = 0.1f;
			speedVal = 1 - Mathf.Abs(speedVal) * modifier - modifier;
		}
		warpSpeed = speedVal;
		return warpSpeed;
	}

	//Similar to warp speed, spooling up has some kind of ratio with ship mass. More mass means longer spool up
	public float warpSpoolUp;

	//Sensor range effectively limits warp range because the warp drive will not have a target to warp upon
	//Sensor strength can determine a few things. It can determine how fast the sensors do their job
	//It can also determine how clearly they work. However, this is starting to sound like scan resolution from EVE
	//Ideally, these two go hand-in-hand. Being able to use the power core to boost sensor power will make
	//nerds gizz. Strength pushes the range? Lorically, the sensors would need to push in every direction to get a
	//general scan of the area, and then process of elimination would need to be applied... For this stage of the
	//game, because the maps are in 2D, a very simple circular sensor system can be applied to the game. This means
	//the player could cast out their sensor and it would return hits on tings it scans... This invites the player
	//to be able to scan down basically anything that could be in space... It feels too open to retain immersion...
	//For inital simplicity, controlling what the player can scan towards would make more sense? This almost defeats
	//the purpose tho, cause why does the player need to scan at all if the signatures are already determined for
	//them? Very broad swathes of scanning sounds like the compromise I am going for? At first, the player will
	//not really need to scan for discovery, they will just need to scan in order to warp to certain stuff? Once
	//again, this starts to feel like D-Scanning in EVE, or just sig scanning in EVE. Scanning for missions sake
	//still sounds like a good idea. However, I do not want it to be as complex or range limited as in EVE.
	//Using this sensor system will nail down the explorative element of the game... 
	public int sensorRange;
	public int sensorStrength;

	//Each customization on the ship will require a certain number of crew to facilitate the system...
	//The only exception to this would be the crew quarters themselves? While lorically speaking someone would
	//need to be the doctor, chef, etc... Getting that specific at this point does not add anything to the gameplay,
	//while it also complicates the prototype... For simplicity's sake, each non-crew quarter system has a crew
	//requirement. This sums up to a running crew count. Here is the key mechanic for this; the ship does not
	//require any crew quarters at all. While the crew count goes up, there is nothing stopping the player from
	//having no crew quarter. Where this bites them in the ass is in the crew fatigue. Essentially, the function
	//of crew quarters is to re-coup the crew fatigue. The longer the crew stays in action, the higher and higher
	//the crew fatigue becomes... As this increases, the overall effectiveness of the ship systems begins to deteriorate.
	//This makes alert levels become a mechanic! Different alert levels affect the rate of fatigue change... This
	//still begs for being able to allocate crew members, but that becomes extremely micro-managerial...
	//ALERT LEVELS!!! But this invites delegating crew as a mechanic
	public int crewCount;
	public int crewDemand;
	public int crewFatigue;

	float shipSpeed;
	public float GetShipSpeed(){
		float speedVal = enginePower - shipMass;
		if(enginePower == 0f){
			speedVal = 0f;
		}
		else if(speedVal < 1){
			float modifier = 0.1f;
			speedVal = 1 - Mathf.Abs(speedVal) * modifier - modifier;
		}
		shipSpeed = speedVal;
		return shipSpeed;
	}

	//This is simple. More engine power means the ship moves faster out of warp... However, as I have not yet nailed
	//down how the player actually navigates in sub-space, this mechanic is kind of in limbo at the moment.
	//Also, this engine power begs for the 'inertia' dampers module... which is not a mechanic; it is just a
	//lorical cosmetic... It is a feature I like, but one that is unnecessary for this point in development
	public int enginePower;

	//Ship mass affects two systems; how much engine power is required to move a certain speed, and how much mass
	//the warp drive needs to compensate for when warping. Mass simply comes from total hull size and module count.
	//Perhaps certain modules will add more weight than others? The specifics only invite complexity... Ship mass
	//is the crux counter-balance to more powerful ship designs; higher ship classes. The extra mass must be met
	//with more committment to navigational systems; it creates the navigational dynamic between ship classes
	public int shipMass;

	#endregion
}