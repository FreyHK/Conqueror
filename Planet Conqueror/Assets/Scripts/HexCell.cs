using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public Player owner;
	public Color defaultColor;

	public RectTransform cellLabelRect;
	public UpgradeLabel upgradeLabel;

	public Text unitText;
	public Text goldPerSecondText;
	public Image progressBar;

	public bool isOcean = false;

	HexGrid hexGrid;

	public int Elevation {
		get {
			return elevation;
		}
		set {
			elevation = value;
			Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
			transform.localPosition = position;

			Vector3 uiPosition = cellLabelRect.localPosition;
			uiPosition.z = elevation * -HexMetrics.elevationStep;
			cellLabelRect.localPosition = uiPosition;
		}
	}

	int elevation;

	public int units = 10;
	int maxUnits = 15;

	HexCell[] neighbors = new HexCell[8];

	void Start(){
		hexGrid = GameObject.FindObjectOfType<HexGrid> ();
		if (isOcean) {
			print ("Hiding label");
			cellLabelRect.gameObject.SetActive (false);
		}

		UpdateStats ();
	}

	float unitSpawnRate;
	float curUnitCooldown;

	float goldSpawnRate;
	float curGoldCooldown;

	void Update(){

		RefreshLabel ();

		if (isOcean) {
			cellLabelRect.gameObject.SetActive (false);
			//this.enabled = false;
			return;
		}

		if (owner.isEmpty) {
			
			progressBar.fillAmount = 0;
			return;
		}

		bool isOverMax = units > maxUnits;

		curUnitCooldown -= (isOverMax ? (Time.deltaTime * 2) : Time.deltaTime) * unitSpawnRate;
		if (curUnitCooldown <= 0) {

			if (units < maxUnits) {
				units++;
				curUnitCooldown = 1;
				progressBar.fillClockwise = true;
				progressBar.color = Color.cyan;
			} else if (isOverMax) {
				units -= 1;
				curUnitCooldown = 1;
				progressBar.fillClockwise = false;
				progressBar.color = Color.red;
			} else if (units == maxUnits){
				progressBar.fillAmount = 1;
			}
		}

		curGoldCooldown -= Time.deltaTime * goldSpawnRate;
		if (curGoldCooldown <= 0) {
			curGoldCooldown = 1;
		
			owner.gold++;
		}
	}

	public void MoveUnits(int amount, Player attacking, float t){

		if (isOcean) {
			Debug.LogError ("Cannot move units to ocean tile");
		}

		StartCoroutine (MoveUnitsAfterTime(amount, attacking, t));
	}

	IEnumerator MoveUnitsAfterTime(int amount, Player attacking, float t){

		yield return new WaitForSecondsRealtime (t);

		if (owner == attacking) {
			//This is our units

			units += amount;

		} else {
			//This is someone else's

			units -= amount;

			if (units < 0) {
				//Switch owner 

				SwitchOwner (attacking);
				units = Mathf.Abs (units);

				hexGrid.Refresh ();
			}
		}
	}

	void SwitchOwner(Player newOwner){
		owner = newOwner;
		unitCapacityImprovementCount = 0;
		unitRateImprovementCount = 0;
		goldImprovementCount = 0;
		UpdateStats ();
	}

	public int RemoveUnits(int amount){
		units -= amount;

		return units;
	}

	void RefreshLabel(){
		progressBar.fillAmount = 1 - curUnitCooldown;
		progressBar.color = owner.color;
		unitText.text = units.ToString ();
		goldPerSecondText.text = goldSpawnRate.ToString ();
	}

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexEdgeType GetEdgeType (HexDirection direction) {
		return HexMetrics.GetEdgeType(
			elevation, neighbors[(int)direction].elevation
		);
	}

	public HexEdgeType GetEdgeType (HexCell otherCell) {
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation
		);
	}

	public HexCell[] GetNeighbors(){
		return neighbors;
	}

	//Improvement
	int costMuliplier = 10;
	public int goldImprovementCount = 0;
	public int unitRateImprovementCount = 0;
	public int unitCapacityImprovementCount = 0;
	public int maxImprovementCount = 5;

	public void Improve(ImprovementType type) {

		if (type == ImprovementType.GoldProductionRate && goldImprovementCount < maxImprovementCount) {
			goldImprovementCount++;
		}
		if (type == ImprovementType.UnitCapacity && unitCapacityImprovementCount < maxImprovementCount) {
			unitCapacityImprovementCount++;
		}
		if (type == ImprovementType.UnitSpawnRate && unitRateImprovementCount < maxImprovementCount) {
			unitRateImprovementCount++;
		}
		UpdateStats ();
	}

	void UpdateStats(){
		unitSpawnRate = 0.2f + unitRateImprovementCount * 0.1f;
		goldSpawnRate = 0.05f + goldImprovementCount * 0.025f;
		maxUnits = 5 + unitCapacityImprovementCount * 5;
	}

	public bool CanBeImproved(){
		return owner.gold >= GetImprovementCost(ImprovementType.GoldProductionRate) || owner.gold >= GetImprovementCost(ImprovementType.UnitCapacity) || owner.gold >= GetImprovementCost(ImprovementType.UnitSpawnRate);
	}

	public int GetImprovementCost(ImprovementType type){
		if (type == ImprovementType.GoldProductionRate) {
			return goldImprovementCount * costMuliplier + 15;
		}
		if (type == ImprovementType.UnitCapacity) {
			return unitCapacityImprovementCount * costMuliplier + 8;
		}
		if (type == ImprovementType.UnitSpawnRate) {
			return unitRateImprovementCount * costMuliplier + 12;
		}
		return 0;
	}

	public bool CanBeImproved(ImprovementType type){

		if (type == ImprovementType.GoldProductionRate) {
			return owner.gold >= GetImprovementCost(ImprovementType.GoldProductionRate) && goldImprovementCount < maxImprovementCount;
		}
		if (type == ImprovementType.UnitCapacity) {
			return owner.gold >= GetImprovementCost(ImprovementType.UnitCapacity) && unitCapacityImprovementCount < maxImprovementCount;
		}
		if (type == ImprovementType.UnitSpawnRate) {
			return owner.gold >= GetImprovementCost(ImprovementType.UnitSpawnRate) && unitRateImprovementCount < maxImprovementCount;
		}
		return false;
	}
}