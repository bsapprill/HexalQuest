using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell{
	public bool hasShipCell;
	public Vector3 gridPosition;
	public GridCoordinate gridCoordinate;

	public GameObject shipCell;
	public ShipCell shipCellComponent;

	public bool[] hasGridNeighborAt = {false,false,false,false,false,false};
	public List<GridCoordinate> neighborCoordinates = new List<GridCoordinate>();

	#region Constructor

	public GridCell(bool _hasCell, Vector3 _gridPos, Vector2 _gridCoord){
		hasShipCell = _hasCell;
		gridPosition = _gridPos;

		int gridX = (int)_gridCoord.x;
		int gridY = (int)_gridCoord.y;

		gridCoordinate = new GridCoordinate(gridX, gridY);
	}

	#endregion

	#region Function

	#endregion
}