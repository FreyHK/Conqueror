using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapGenerator : MonoBehaviour {
		
	public HexGrid hexGrid;
	public Player defaultPlayer;

	public void Generate (Player[] players) {

		bool placedRed = false;
		bool placedGreen = false;

		for (int i = 0; i < hexGrid.cells.Length; i++) {

			HexCell cell = hexGrid.cells [i];

			if (IsOnEdgeOfMap (cell)) {
				cell.isOcean = true;
				continue;
			}

			if (Random.value < 0.05f && placedRed == false || i == hexGrid.cells.Length - 20 && placedRed == false) {
				placedRed = true;
				players[0].color = Color.red;
				players[0].hexGrid = hexGrid;

				SetCell (cell, players[0], 1, 5);

				continue;
			}
			if (Random.value < 0.05f && placedGreen == false || i == hexGrid.cells.Length - 21 && placedGreen == false) {
				placedGreen = true;

				players[1].color = Color.green;
				players[1].hexGrid = hexGrid;

				SetCell (cell, players[1], 1, 5);
				continue;
			}

			SetCell (cell, defaultPlayer, 1, 3);
		}

		hexGrid.Refresh();
	}

	void SetCell (HexCell cell, Player owner, int elevation, int units) {
		cell.owner = owner;
		cell.Elevation = elevation;
		cell.units = units;
	}

	bool IsOnEdgeOfMap(HexCell cell) {

			return cell.GetNeighbor (HexDirection.E) == null ||
				cell.GetNeighbor (HexDirection.NE) == null ||
				cell.GetNeighbor (HexDirection.NW) == null ||
				cell.GetNeighbor (HexDirection.SE) == null ||
				cell.GetNeighbor (HexDirection.SW) == null ||
				cell.GetNeighbor (HexDirection.W) == null;
	}
}
