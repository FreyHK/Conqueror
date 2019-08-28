using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BotInput : Player {

	GameManager gameManager;

	void Awake () {
		gameManager = GetComponent<GameManager> ();
	}

	void Start () {
		float r = Random.value;
		InvokeRepeating ("CheckForMoves", 2f, 0.3f + r);
	}

	void Update(){
	}

	void CheckForMoves(){

		//foreach (HexCell cell in ownedCells) {
		if (ownedCells.Count == 0){
			return;
		}

		HexCell cell = ownedCells [Random.Range(0, ownedCells.Count)];

			HexCell[] neighbors = cell.GetNeighbors ();

			if (neighbors.Length == 0) {
			return;
			}

			List<HexCell> enemyTiles = new List<HexCell>();
			bool allNeighborsAreFriendly = false;

			for (int i = 0; i < neighbors.Length; i++) {

				HexCell neighbor = neighbors [i];

			if (neighbor == null || neighbor.isOcean) {
				continue;
				}

			if (neighbor.owner.color != color) {
					allNeighborsAreFriendly = false;
				} else {
				continue;
				}

				if (cell.units > neighbor.units) {

					enemyTiles.Add (neighbor);
				}
			}

			if (enemyTiles.Count == 0) {

				if (allNeighborsAreFriendly) {
					for (int i = 0; i < neighbors.Length; i++) {

						print ("AllNeighborsAreFriendly");
						if (neighbors[i] == null){
							continue;
						}

						gameManager.TransferUnits (GetList(cell, neighbors[i]), false);
						return;
					}
				}
			return;
			}

			int toMove = Random.Range (0, enemyTiles.Count);
			gameManager.TransferUnits (GetList(cell, enemyTiles[toMove]), false);
		//}
	}

	HexCell[] GetList(HexCell cell1, HexCell cell2){
		HexCell[] cells = new HexCell[2];
		cells [0] = cell1;
		cells [1] = cell2;
		return cells;
	}
}
