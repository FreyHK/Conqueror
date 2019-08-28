using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MovingUnit {

	public RectTransform rect;
	public Vector3 fromPos;
	public Vector3 toPos;

	public MovingUnit(RectTransform rect, Vector3 fromPos, Vector3 toPos){
		this.rect = rect;
		this.fromPos = fromPos;
		this.toPos = toPos;
	}
}

public class GameManager : MonoBehaviour {

	public Text goldText;

	int unitMoveSpeed = 4;
	public RectTransform movingUnitPrefab;
	HexGrid hexGrid;

	public Player[] players;
	Player ourPlayer;
	HexMapGenerator generator;

	List<MovingUnit> unitsToMove = new List<MovingUnit>();

	public void TransferUnits(HexCell[] cellArray, bool half) {

		for (int i = 0; i < cellArray.Length - 1; i++) {

			HexCell thisCell = cellArray [i];
			HexCell nextCell = cellArray [i+1];

			if (thisCell.units <= 0) {
				continue;
			}

			int unitsToMove = half && thisCell.units > 1 ? (thisCell.units / 2) : (thisCell.units);

			AddMovingUnit(thisCell.cellLabelRect.anchoredPosition3D, nextCell.cellLabelRect.anchoredPosition3D, unitsToMove, thisCell.owner.color);

			nextCell.MoveUnits (unitsToMove, thisCell.owner, unitMoveSpeed);

			thisCell.RemoveUnits (unitsToMove);
		}
	}

	public void ImproveCell(HexCell cell, ImprovementType type){

		if (cell.owner.gold < cell.GetImprovementCost (type)) {
			return;
		}
		if (cell.goldImprovementCount == cell.maxImprovementCount && type == ImprovementType.GoldProductionRate || cell.unitRateImprovementCount == cell.maxImprovementCount && type == ImprovementType.UnitSpawnRate  || cell.unitCapacityImprovementCount == cell.maxImprovementCount && type == ImprovementType.UnitCapacity ) {
			return;
		}


		cell.owner.gold -= cell.GetImprovementCost (type);
		cell.Improve (type);
	}

	void AddMovingUnit(Vector3 fromPos, Vector3 toMove, int unitAmount, Color color){
		
		RectTransform rect = Instantiate<RectTransform>(movingUnitPrefab);

		rect.SetParent(transform.GetChild(0), false);
		rect.anchoredPosition3D = fromPos;
		rect.GetComponentInChildren<Text> ().text = unitAmount.ToString();
		rect.GetChild (0).GetComponent<Image> ().color = color;

		GameObject.Destroy (rect.gameObject, unitMoveSpeed);

		unitsToMove.Add (new MovingUnit (rect, fromPos, toMove));
	}

	void Start(){
		ourPlayer = players[1];
		hexGrid = GetComponent<HexGrid> ();
		generator = GetComponent<HexMapGenerator> ();

		generator.Generate (players);
	}

	void Update(){

		foreach (Player p in players) {
			p.ownedCells = hexGrid.GetAllCellsOfColor (p.color);


		}

		//Animate moving units
		if (unitsToMove.Count > 0) {
			for (int i = 0; i < unitsToMove.Count; i++) {
				MovingUnit m = unitsToMove [i];

				if (m.rect == null) {
					unitsToMove.Remove (m);
					continue;
				}
				m.rect.anchoredPosition3D = Vector3.MoveTowards (m.rect.anchoredPosition3D, m.toPos, Vector3.Distance (m.fromPos, m.toPos) * 1/unitMoveSpeed * Time.deltaTime);
			}
		}

		goldText.text = "Gold: " + ourPlayer.gold.ToString ();


	}

	void AddGold(){
	
	}

	public void PlaySound(SoundType sound){
	
		if (sound == SoundType.Error) {
			
		}
	
	}
}
