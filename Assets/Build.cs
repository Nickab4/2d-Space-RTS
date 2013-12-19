using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Build : MonoBehaviour {
	//Build Window Dimensions
	Rect windowRect = new Rect(Screen.width-300,Screen.height-480,250,400);
	bool showBuild = false;
	int  buildCase = 0;
	float buildTime = 0f;
	public int RU = 2000;
	int lastCost = 0;
	public Transform testship;
	public Transform FBX;
	public Transform harvester;
	public Transform FrBX;
	Unit ShipOwner;
	// Use this for initialization
	void Start () {
		ShipOwner = this.GetComponent<Unit>();
	}
	
	// Update is called once per frame
	void Update () {
		//If Not building, Reset Build Time
	if(buildCase == 0)
			buildTime = 0;

		BuildManager ();
		buildTime += Time.deltaTime;
	}

	//Gui!
	void OnGUI(){
		if(ShipOwner.Owner == CameraOperator.Player)
		{
			if(GUI.Button (new Rect(Screen.width - 125, Screen.height - 80, 75, 30), "Build"))
			{
				showBuild = !showBuild;
			}

			if(showBuild)
				windowRect = GUI.Window (0, windowRect, WindowFunction, "Build Menu");
		}
	}
	void WindowFunction(int windowID) {

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

	}
}
