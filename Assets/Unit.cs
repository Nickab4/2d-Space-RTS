using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	//Admin/Matinence
	public bool selected = false;
	private Vector3 moveToDest = Vector3.zero;
	private float floorOffset = 0;
	private bool mouseOver = false;
	private Camera myCam;
	private bool Attacking;
	private bool Moving = false;
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

	void Start () {
		myCam = Camera.main;
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

		//Selected Orders
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
			}
		}
		//Attack Command
		if (selected && Input.GetKeyDown(KeyCode.A))
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

		Attack ();
		UpdateMove();
		Death ();
		Regen ();
		Aggro ();
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
						if(!aTarget.Moving)
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
		GUI.Label(new Rect(Pos.x - 25 , (Screen.height - Pos.y)-40, 100f, 25f), currentHP + "/" + maxHP);
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
				if(u.Owner == aTarget.Owner)
					if(!u.Moving)
						u.moveToDest = this.gameObject.transform.position;
			}
		}
	}
}
