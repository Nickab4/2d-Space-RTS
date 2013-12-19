using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	//Admin/Matinence
	public bool selected = false;
	private Vector3 moveToDest = Vector3.zero;
	private float floorOffset = 0;
	private bool mouseOver = false;
	private bool Attacking;
	private bool Moving = false;
	private bool Harvesting = false;
	private bool dropping = false;
	private Unit aTarget;
	private float atkTime = 0.0f;
	private float rgnTime = 0.0f;
	//Stats
	public int Owner = 1;
	public float speed = 5;
	public float turnSpeed = 100;
	public int maxHP = 100;
	public int currentHP = 100;
	public int damage = 5;
	public int range = 4;
	public float atkSpd = 0.5f;
	public float stopDistanceOffset = 0.25f;
	public int HPrgn = 2;
	public float UIHeight = 10;
	//Ship Definitions
	public bool Armed = true;
	public bool Harvester = false;
	public bool Turrets = false;
	public bool RUdrop = false;
	public bool RsrchVessel = false;
	//Harvester Stats
	private int CurrentRUs = 0;

	void Start () {
		//For All Units list
		CameraOperator.allUnits.Add(this);

	}
	
	// Update is called once per frame
	void Update () {
		//Selection Code
	if (Input.GetMouseButtonUp(0) && CameraOperator.Player == Owner)
		{
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = CameraOperator.InvertMouseY(camPos.y);
			selected = CameraOperator.selection.Contains(camPos);

			if(selected == false && mouseOver == true)
				selected = true;
		}
		//IFF Chekcs
		if(CameraOperator.Player != Owner)
		{
			renderer.material.color = Color.red;
		}
		else
		{
			if(!mouseOver)
				renderer.material.color = Color.white;
		}
		if(selected)
		{
			renderer.material.color = Color.green;
			if(CameraOperator.Player != Owner)
				selected = false;
		}
		else
		{
			if(!mouseOver && CameraOperator.Player == Owner)
				renderer.material.color = Color.white;
		}

		//Move Command
		if (selected && Input.GetMouseButtonUp(1))
		{
			//Code To Move
			Vector3 destination = CameraOperator.GetDestination();

			if (destination != Vector3.zero)
			{
				moveToDest = destination;
				moveToDest.z = floorOffset;
				Attacking = false;
				Moving = true;
				Harvesting = false;
			}
		}
		//Attack Command
		if (selected && Input.GetKeyDown(KeyCode.A))
		{
			//Code for Hull-Locked Weapons
			if(Armed) 
			{
				aTarget = null;
				foreach(Unit u in CameraOperator.allUnits)
				{
					if(u.mouseOver == true && this.Owner != u.Owner)
					{
						aTarget = u;
						Moving = false;
						Attacking = true;

					}
				}
				if(aTarget == null)
				{
					Vector3 destination = CameraOperator.GetDestination();
				
					if (destination != Vector3.zero)
					{
						Attacking = false;
						Moving = false;
						moveToDest = destination;
						moveToDest.z = floorOffset;
					}

				}
			}
			//Code for Turrets(NYI)
		}
		//Stop Command
		if (selected && Input.GetKeyDown(KeyCode.S))
		{
			moveToDest = Vector3.zero;
			Attacking = false;
			Moving = false;
			aTarget = null;
			Harvesting = false;

		}
		if (selected && Input.GetKeyDown(KeyCode.H) && Harvester)
		{
			Harvesting = true;
		}
		if (selected && Input.GetKeyDown(KeyCode.D) && Harvester)
		{
			dropping = true;
		}
		Attack ();
		UpdateMove();
		Death ();
		Regen ();
		if(Armed)
		{
			Aggro ();
		}
		if(Harvesting)
		{
			Harvest ();
		}
		if(dropping)
		{
			drop ();
			if(CurrentRUs == 0)
			{
				dropping = false;
				moveToDest = Vector3.zero;
				Moving = false;
				Harvesting = false;
			}
		}
			
	}
	private void UpdateMove()
	{
		if(moveToDest!= Vector3.zero && transform.position != moveToDest)
		{
			Vector3 direction = (moveToDest - transform.position).normalized;

			direction.z = 0;
			//Movement
			transform.rigidbody2D.velocity = direction * speed;
			//Turning --> Instant Z axis only
			transform.up = (moveToDest - transform.position);;
			//transform.up = Vector3.Slerp (transform.position, moveToDest, turnSpeed*Time.deltaTime);
			if (Vector3.Distance(transform.position, moveToDest) < stopDistanceOffset)
			{
				moveToDest = Vector3.zero;
				Moving = false;
			}
		}
		else
			//Stops  movement if arrived
		transform.rigidbody2D.velocity = Vector3.zero;
	}
	void OnMouseEnter(){
		mouseOver = true;
		if(CameraOperator.Player == Owner)
			renderer.material.color = new Color(0f,1f,0.6f);
	}
	void OnMouseExit(){
		mouseOver = false;
	}
	void Attack()
	{
		atkTime += Time.deltaTime;
		if(Attacking)
		{
			if(aTarget != null)
			{
				//Test Range
				if(Vector3.Distance(aTarget.transform.position, transform.position) <= range)
				{
					moveToDest = Vector3.zero;
					transform.up = (aTarget.transform.position - transform.position);
					if(atkTime > atkSpd)
					{
						Debug.Log (this.name + " attacked " + aTarget.name + " for " + damage + " damage.");
						atkTime = 0f;
						aTarget.currentHP = aTarget.currentHP - damage;
						if(!aTarget.Moving && aTarget.Armed)
							aTarget.moveToDest = this.transform.position;

						CallForHelp();

					}
				}
				else
				{
					moveToDest = aTarget.transform.position;
				}
			}
			else
			{
				Attacking = false;
			}
		}
	}
	void Death()
	{
		if(currentHP <= 0)
		{
			CameraOperator.selectedUnits.Remove(this);
			CameraOperator.allUnits.Remove(this);
			Debug.Log ("Ship Destroied! " + this.name);
			Destroy(this.gameObject);
			Destroy(this);
		}
	}
	void Regen()
	{
		rgnTime += Time.deltaTime;
		if (rgnTime > 5)
		{
			currentHP = currentHP + HPrgn;
			rgnTime = 0f;
			if (currentHP > maxHP)
				currentHP = maxHP;
		}
	}
	void OnGUI()
	{
		Vector2 Pos = Camera.main.WorldToScreenPoint(this.transform.position);
		float h = Screen.height / UIHeight;
		//Orders Logic
		string Orders = " S";
		if(Attacking)
			Orders = " A";		
		if(Moving)
			Orders = " M";
		if(moveToDest != Vector3.zero && !Moving)
			Orders = " A/M";
		if(Harvesting)
			Orders = " H";
		if(dropping)
			Orders = " D";
		if(Attacking && Moving)
			Orders = " Error!";
		if(Harvester)
			Orders += " " + CurrentRUs + "/600 RUs";
		//GUI Overhead Print
		GUI.Label(new Rect(Pos.x - 50 , (Screen.height - Pos.y)-h, 275f, 25f), this.name + ": " + currentHP + "/" + maxHP + Orders);
	} 
	void Aggro()
	{
		foreach(Unit u in CameraOperator.allUnits)
		{
			if(Owner != u.Owner && !Moving && aTarget == null)
			{
				if(Vector3.Distance(u.transform.position, transform.position) <= range)
				{
				aTarget = u;
				Attacking = true;
				}
				
			}
			
			
		}
	}
	void CallForHelp()
	{
		foreach(Unit u in CameraOperator.allUnits)
		{
			if(Vector3.Distance(u.transform.position, aTarget.gameObject.transform.position) <= 10)
			{
				if(u.Owner == aTarget.Owner && u.Armed)
					if(!u.Moving)
						u.moveToDest = this.gameObject.transform.position;
			}
		}
	}
	void Harvest()
	{
		//Finds Closest Resources
		float d = 99999;
		Resource closest = null;
		if(CameraOperator.allResources != null && CurrentRUs < 600) 
		{
			foreach(Resource r in CameraOperator.allResources)
			{
				if(Vector3.Distance (this.transform.position, r.transform.position) < d)
				{
					d = Vector3.Distance (this.transform.position, r.transform.position);
					closest = r;
				}
	
			}
			if(closest == null)
			{
				Debug.Log ("No Resources on Map");
				if(CurrentRUs > 0)
				{
					drop ();
				}
				else
				{
				Harvesting = false;
				}
			}
			if(closest != null && Vector3.Distance(this.transform.position, closest.transform.position) <= range)
			{
				moveToDest = Vector3.zero;
				transform.up = (closest.transform.position - transform.position);
				if(atkTime > atkSpd)
				{
					closest.RUs = closest.RUs - damage;
					CurrentRUs = CurrentRUs + damage;
					atkTime = 0;
					Debug.Log (this.name + " Harvested " + damage + " RUs from " + closest.name + "\n" + CurrentRUs + " in tank");
				}
				
			}
			else
			{
				if(closest != null)
					moveToDest = closest.transform.position;
			}
			
		}
		if(CurrentRUs>= 600)
		{
			drop ();
		}

	}
	void drop()
	{
		float D = 99999;
		Unit drop = null;
		foreach(Unit u in CameraOperator.allUnits)
		{
			if(u.RUdrop && Vector3.Distance (this.transform.position, u.transform.position) < D)
			{
				D = Vector3.Distance (this.transform.position, u.transform.position);
				drop = u;
			}
		}
		if(drop != null)
		{
			if(Vector3.Distance(this.transform.position, drop.transform.position) <= 5)
			{
				Build b = drop.GetComponent<Build>();
				b.RU = b.RU + CurrentRUs;
				Debug.Log ("Droped off " + CurrentRUs + "RUs at " +  b.name + " (" + b.RU +" total)");
				CurrentRUs = 0;
				moveToDest = Vector3.zero;
			}
			else
			{
				moveToDest = drop.transform.position;
			}
		}
		else
		{
			Debug.Log ("No Dropoff Exists!");
			Harvesting = false;
		}
	}
}
