using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridCoordinate{
	public int x;
	public int y;

	public GridCoordinate (int _x, int _y){
		x = _x;
		y = _y;
	}

	public Vector2 SumCoordinatesAsVector2(GridCoordinate coordToAdd){		
		int newx = x - coordToAdd.x;
		int newy = y - coordToAdd.y;
		return new Vector2(newx,newy);
	}

	public GridCoordinate SumCoordinates(GridCoordinate coordToAdd){		
		int newx = x + coordToAdd.x;
		int newy = y + coordToAdd.y;
		return new GridCoordinate(newx,newy);
	}

	public GridCoordinate SumCoordinates(Vector2 coordToAdd){		
		int newx = x + (int)coordToAdd.x;
		int newy = y + (int)coordToAdd.y;
		return new GridCoordinate(newx,newy);
	}

	public override string ToString ()
	{
		return ("x: "+x+", "+"y: "+y);
	}
}