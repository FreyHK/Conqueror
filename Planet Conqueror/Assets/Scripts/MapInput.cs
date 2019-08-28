using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MapInput : Player {

	public GameObject selectionMarkerPrefab;

	int draggingButton = -1;

	List<HexCell> dragCells = new List<HexCell>();
	List<GameObject> selectionMarkers = new List<GameObject> ();

	GameManager gameManager;

	HexCell hoveringCell;

	void Awake () {
		gameManager = GetComponent<GameManager> ();
	}

	void Update () {

		if (draggingButton == 0 || draggingButton == -1) {
			if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject ()) {

				draggingButton = 0;
				Drag (false);
			}
			if (Input.GetMouseButtonUp (0) && !EventSystem.current.IsPointerOverGameObject ()) {

				draggingButton = -1;
				EndDrag (false);
			}
		}

		if (draggingButton == 1 || draggingButton == -1) {
			if (Input.GetMouseButton (1) && !EventSystem.current.IsPointerOverGameObject ()) {

				draggingButton = 1;
				Drag (true);
			}
			if (Input.GetMouseButtonUp (1) && !EventSystem.current.IsPointerOverGameObject ()) {

				draggingButton = -1;
				EndDrag (true);
			}
		}
		//Upgrading

		CheckForUpgradableTiles ();

		if (Input.GetKeyDown (KeyCode.Q)) {
			if (hoveringCell != null) {
				ImproveSelectedCell(ImprovementType.UnitSpawnRate);
			}
		}
		if (Input.GetKeyDown (KeyCode.W)) {
			if (hoveringCell != null) {
				ImproveSelectedCell(ImprovementType.GoldProductionRate);
			}
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			if (hoveringCell != null) {
				ImproveSelectedCell(ImprovementType.UnitCapacity);
			}
		}
	}

	void ImproveSelectedCell(ImprovementType type){
		if (hoveringCell != null)
			gameManager.ImproveCell (hoveringCell, type);
	}

	void Drag (bool half) {

		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {

			HexCell thisCell = hexGrid.GetCell(hit.point);

			if (thisCell.isOcean || thisCell == null) {
				RemoveDragCellsFromIndex (0);
				return;
			}

			if (dragCells.Count == 0) {

				GameObject m = Instantiate (selectionMarkerPrefab, thisCell.cellLabelRect.position, Quaternion.identity) as GameObject;
				selectionMarkers.Add (m);
				dragCells.Add (thisCell);
				return;
			}

			HexCell lastCell = dragCells [dragCells.Count - 1];

			foreach (HexCell cell in dragCells) {
				if (cell == thisCell) {
					RemoveDragCellsFromIndex (dragCells.IndexOf(cell) + 1);
					return;
				}
			}

			if (!hexGrid.IsNeighbors (lastCell, thisCell) || lastCell.owner != this) {
				return;
			}

			GameObject marker = Instantiate (selectionMarkerPrefab, thisCell.cellLabelRect.position, Quaternion.identity) as GameObject;
			LineRenderer lr = marker.GetComponent<LineRenderer> ();
			lr.SetPosition (0, thisCell.cellLabelRect.position);
			lr.SetPosition (1, lastCell.cellLabelRect.position);

			selectionMarkers.Add (marker);
			dragCells.Add(thisCell);
		}
	}

	void EndDrag (bool half) {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (inputRay, out hit)) {

			if (dragCells.Count == 0) {
				return;
			}

			gameManager.TransferUnits (dragCells.ToArray (), half);

			for (int i = 0; i < selectionMarkers.Count; i++) {
				GameObject.Destroy (selectionMarkers [i]);
			}
			selectionMarkers.Clear ();
			dragCells.Clear ();
		}
	}

	void RemoveDragCellsFromIndex(int index) {
		for (int i = index; i < dragCells.Count; i++) {
			Destroy (selectionMarkers [i]);
			selectionMarkers.RemoveAt (i);
			dragCells.RemoveAt (i);
		}
	}

	void CheckForUpgradableTiles(){

		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (inputRay, out hit)) {
			HexCell hitCell = hexGrid.GetCell (hit.point);

			ShowUpgradeMenu (hitCell);
		}
	}

	void ShowUpgradeMenu(HexCell hitCell){

		if (hoveringCell == hitCell) {
			return;
		}

		if (hoveringCell != null)
			hoveringCell.upgradeLabel.Close ();

		if (hitCell != null && hitCell.owner == this)
			hitCell.upgradeLabel.Open ();

		hoveringCell = hitCell;
	}
}
