using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpgradeLabel : MonoBehaviour {

	Animator anim;
	[HideInInspector]
	public HexCell cell;
	RectTransform goldUpgrade;
	RectTransform unitCapacityUpgrade;
	RectTransform unitRateUpgrade;

	bool isOpen = false;
	Player startOwner;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();

		goldUpgrade = transform.FindChild ("GoldUpgrade").GetComponent<RectTransform>();
		unitCapacityUpgrade = transform.FindChild ("UnitCapacityUpgrade").GetComponent<RectTransform>();
		unitRateUpgrade = transform.FindChild ("UnitRateUpgrade").GetComponent<RectTransform>();
	}

	public void Open () {
		anim.SetBool ("IsOpen", true);
		isOpen = true;
		startOwner = cell.owner;
	}

	public void Close () {
		anim.SetBool ("IsOpen", false);
		isOpen = false;
	}

	void Update(){
		if (isOpen) {

			if (cell.owner != startOwner) {
				Close ();
				return;
			}

			goldUpgrade.gameObject.SetActive (cell.CanBeImproved (ImprovementType.GoldProductionRate));
			unitCapacityUpgrade.gameObject.SetActive (cell.CanBeImproved (ImprovementType.UnitCapacity));
			unitRateUpgrade.gameObject.SetActive (cell.CanBeImproved (ImprovementType.UnitSpawnRate));

			goldUpgrade.GetComponentInChildren<Text> ().text = cell.GetImprovementCost (ImprovementType.GoldProductionRate).ToString ();
			unitCapacityUpgrade.GetComponentInChildren<Text> ().text = cell.GetImprovementCost (ImprovementType.UnitCapacity).ToString ();
			unitRateUpgrade.GetComponentInChildren<Text> ().text = cell.GetImprovementCost (ImprovementType.UnitSpawnRate).ToString ();
		}
	}
}
