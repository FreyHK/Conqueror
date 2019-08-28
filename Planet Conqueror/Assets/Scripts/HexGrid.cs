using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class HexGrid : MonoBehaviour {

	public int width = 6;
	public int height = 6;

	public Player emptyPlayer;

	public HexCell cellPrefab;
	public RectTransform cellLabelPrefab;

	public HexCell[] cells;

	Canvas gridCanvas;
	HexMesh hexMesh;

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			}
		}
		Refresh ();
	}

	void Start () {
		hexMesh.Triangulate(cells);
	}

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		return cells[index];
	}

	public void Refresh () {
		hexMesh.Triangulate(cells);
	}

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		cell.owner = emptyPlayer;
		cell.units = 3;

		if (x > 0) {
			cell.SetNeighbor (HexDirection.W, cells [i - 1]);

		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor (HexDirection.SE, cells [i - width]);
				if (x > 0) {
					cell.SetNeighbor (HexDirection.SW, cells [i - width - 1]);
				}
			} else {
				cell.SetNeighbor (HexDirection.SW, cells [i - width]);
				if (x < width - 1) {
					cell.SetNeighbor (HexDirection.SE, cells [i - width + 1]);
				}

			}
		}

		RectTransform rect = Instantiate<RectTransform>(cellLabelPrefab);

		rect.SetParent(gridCanvas.transform, false);
		rect.anchoredPosition =
			new Vector2(position.x, position.z);

		CellLabel cellLabel = rect.GetComponent<CellLabel> ();

		cell.cellLabelRect = cellLabel.unitLabelRect;
		cell.cellLabelRect = cellLabel.unitLabelRect;
		cell.upgradeLabel = cellLabel.upgradeLabel;
		cell.unitText = cellLabel.unitText;
		cell.goldPerSecondText = cellLabel.goldPerSecondText;
		cell.progressBar = cellLabel.progressBar;

		cell.upgradeLabel.GetComponent<UpgradeLabel> ().cell = cell;
	}

	public bool IsNeighbors(HexCell cell1, HexCell cell2){

		HexCell[] neighbors = cell1.GetNeighbors ();

		foreach (HexCell h in neighbors) {
			if (h == cell2) {
				return true;
			}
		}

		return false;
	}

	public List<HexCell> GetAllCellsOfColor(Color color){

		List<HexCell> cellsOfColor = new List<HexCell>();

		for (int i = 0; i < cells.Length; i++) {
			
			if (cells[i].owner.color == color) {
				cellsOfColor.Add(cells [i]);
			}
		}

		return cellsOfColor;
	}
}