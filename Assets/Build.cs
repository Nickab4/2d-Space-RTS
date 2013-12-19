using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Build : MonoBehaviour {
	//Build Window Dimensions
	Rect windowRect = new Rect(Screen.width-300,Screen.height-480,250,400);
	bool showBuild = false;
	bool showResearch = false;
	int  buildCase = 0;
	float buildTime = 0f;
	int researchCase = 0;
	float researchTime = 0f;
	public int RU = 2000;
	int lastCost = 0;
	int researchNumber = 0;
	public Transform testship;
	public Transform FBX;
	public Transform harvester;
	public Transform FrBX;
	public Transform BeamFrigate;
	public Transform ResearchVessel;
	Unit ShipOwner;
	//ResearchList
	bool TestRsrch = false;
	// Use this for initialization
	void Start () {
		ShipOwner = this.GetComponent<Unit>();
	}
	
	// Update is called once per frame
	void Update () {
		//If Not building, Reset Build Time
	if(buildCase == 0)
			buildTime = 0;

	//if not researching, Reset Research Time
	if(researchCase == 0)
			researchTime = 0;

		ResearchManager ();
		BuildManager ();
		buildTime += Time.deltaTime;
		researchTime += Time.deltaTime;
		researchNumber = 0;
		foreach(Unit u in CameraOperator.allUnits)
		{
			if(u.RsrchVessel)
				researchNumber += 1;
			
		}
	
	}

	//Gui!
	void OnGUI(){
		if(ShipOwner.Owner == CameraOperator.Player)
		{
			if(GUI.Button (new Rect(Screen.width - 125, Screen.height - 80, 75, 30), "Build"))
			{
				showBuild = !showBuild;
				showResearch = false;
			}
			if(GUI.Button (new Rect(Screen.width - 200, Screen.height - 80, 75, 30), "Research"))
			{
				showBuild = false;
				showResearch = !showResearch;
			}
			if(showResearch)
				windowRect = GUI.Window (1, windowRect, WindowResearch, "Research Menu");
			if(showBuild)
				windowRect = GUI.Window (0, windowRect, WindowBuild, "Build Menu");
		}
	}
	void WindowResearch(int windowID) {

		GUI.Label (new Rect(5,15,200,25),"Research Vessels: " + researchNumber);
		if(GUI.Button (new Rect(135,20,110,25), "Cancel"))
		{
			if(researchCase != 0)
			{
				researchCase = 0;
			}
		}
		GUI.Label (new Rect(120,45,110,25), "100rsrch");
		if(GUI.Button (new Rect(5,40,110,25), "TestRsrch"))
		{
			if(researchCase == 0 && !TestRsrch)
			{
				researchCase = 1;
			}
		}
		//ShowTestresearch Time
		if(researchCase == 1)
		{
			int maxTime = 100/researchNumber;
			int time = Mathf.FloorToInt( researchTime);
			GUI.Label (new Rect(170,45,110,25), " |Time: " + (maxTime - time));
		}
		if(TestRsrch)
		{
			GUI.Label (new Rect(170,45,110,25), " |Complete ");
		}
	}
	void ResearchManager() {
		if(researchCase == 1 && researchTime >= (100/researchNumber))
		{
			researchCase = 0;
			TestRsrch = true;
			Debug.Log ("Research Complete!");
		}
	}
	void WindowBuild(int windowID) {

		GUI.Label (new Rect(5,15,100,25),"Resources: " + RU);
		if(GUI.Button (new Rect(135,20,110,25), "Cancel"))
		{
			if(buildCase != 0)
			{
				RU += lastCost;
				buildCase = 0;
				lastCost = 0;
			}
		}
		GUI.Label (new Rect(120,45,110,25), "500 RUs");
		if(GUI.Button (new Rect(5,40,110,25), "TestShip"))
		{
			if(buildCase == 0 && RU >= 500)
			{
				RU += -500;
				buildCase = 1;
				lastCost = 500;
			}
		}
		GUI.Label (new Rect(120,75,110,25), "400 RUs");
		if(GUI.Button (new Rect(5,70,110,25), "Harvester"))
		{
			if(buildCase == 0 && RU >= 400)
			{
				RU += -400;
				buildCase = 2;
				lastCost = 400;
			}
		}
		if(TestRsrch)
		{
			GUI.Label (new Rect(120,105,110,25), "800 RUs");
			if(GUI.Button (new Rect(5,100,110,25), "Beam Frigate"))
			{
				if(buildCase == 0 && RU >= 800)
				{
					RU += -800;
					buildCase = 3;
					lastCost = 800;
				}
			}
		}
		GUI.Label (new Rect(120,135,110,25), "600 RUs");
		if(GUI.Button (new Rect(5,130,110,25), "Rsrch Vessel"))
		{
			if(buildCase == 0 && RU >= 600)
			{
				RU += -600;
				buildCase = 4;
				lastCost = 600;
			}
		}
		//Show Test Ship Production Timer
		if(buildCase == 1)
		{
			int time = Mathf.FloorToInt( buildTime);
			GUI.Label (new Rect(170,45,110,25), " |Time: " + (30 - time));
		}
		if(buildCase == 2)
		{
			int time = Mathf.FloorToInt( buildTime);
			GUI.Label (new Rect(170,75,110,25), " |Time: " + (60 - time));
		}
		if(buildCase == 3)
		{
			int time = Mathf.FloorToInt( buildTime);
			GUI.Label (new Rect(170,105,110,25), " |Time: " + (120 - time));
		}
		if(buildCase == 4)
		{
			int time = Mathf.FloorToInt( buildTime);
			GUI.Label (new Rect(170,135,110,25), " |Time: " + (80 - time));
		}
	}
	void BuildManager(){
		//TestShip
		if(buildCase == 1 && buildTime >= 30)
		{
			buildCase = 0;
			Object ship = Instantiate(testship, FBX.transform.position, Quaternion.identity);
			ship.name = "NewShip";
			GameObject s = GameObject.Find ("NewShip");
			ship.name = "TestShip";
			Unit u = s.GetComponent<Unit>();
			u.Owner = ShipOwner.Owner;
			lastCost = 0;
			Debug.Log (u.name + " Complete!");
		}
		if(buildCase == 2 && buildTime >= 60)
		{
			buildCase = 0;
			Object ship = Instantiate(harvester, FrBX.transform.position, Quaternion.identity);
			ship.name = "NewShip";
			GameObject s = GameObject.Find ("NewShip");
			ship.name = "ResourceCollector";
			Unit u = s.GetComponent<Unit>();
			u.Owner = ShipOwner.Owner;
			lastCost = 0;
			Debug.Log (u.name + " Complete!");
		}
		if(buildCase == 3 && buildTime >= 120)
		{
			buildCase = 0;
			Object ship = Instantiate(BeamFrigate, FrBX.transform.position, Quaternion.identity);
			ship.name = "NewShip";
			GameObject s = GameObject.Find ("NewShip");
			ship.name = "BeamFrigate";
			Unit u = s.GetComponent<Unit>();
			u.Owner = ShipOwner.Owner;
			lastCost = 0;
			Debug.Log (u.name + " Complete!");
		}
		if(buildCase == 4 && buildTime >= 80)
		{
			buildCase = 0;
			Object ship = Instantiate(ResearchVessel, FrBX.transform.position, Quaternion.identity);
			ship.name = "NewShip";
			GameObject s = GameObject.Find ("NewShip");
			ship.name = "ResearchVessel";
			Unit u = s.GetComponent<Unit>();
			u.Owner = ShipOwner.Owner;
			lastCost = 0;
			Debug.Log (u.name + " Complete!");
		}
	}
}
