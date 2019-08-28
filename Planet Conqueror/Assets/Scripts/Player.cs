using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public int gold;
	public List<HexCell> ownedCells = new List<HexCell>();
	public Color color;
	public bool isEmpty = false;

	[HideInInspector]
	public HexGrid hexGrid;

	float curGoldCooldown;
	float goldSpawnRate;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
