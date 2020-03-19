using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshAreaMask{

	public enum ENavMeshAreaMask { None = 0, Walkable = 1 << 0, NotWalkable = 1 << 1, Jump = 1 << 2, Pedestrian = 1 << 3, CarLaneRight = 1 << 4, CarLaneLeft = 1 << 5, Crossing = 1 << 6 }
}
