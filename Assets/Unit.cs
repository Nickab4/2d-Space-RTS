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
	private Unit aTarget;
	private float atkTime = 0.0f;
	//Stats
	public float speed = 5;
	public int maxHP = 100;
	public int currentHP = 100;
	public int damage = 5;
	public int range = 4;
	public float atkSpd = 0.5f;
	public float stopDistanceOffset = 0.25f;

	void Start () {
		myCam = Camera.main;
		CameraOperator.allUnits.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
		//Selection Code
	if (Input.GetMouseButtonUp(0))
		{
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = CameraOperator.InvertMouseY(camPos.y);
			selected = CameraOperator.selection.Contains(camPos);

			if(selected == false && mouseOver == true)
				selected = true;
		}
		if(selected)
		{
			renderer.material.color = Color.green;
		}
		else
		{
			if(!mouseOver)
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
			}
		}
		if (selected && Input.GetKeyDown(KeyCode.A))
		{
			foreach(Unit u in CameraOperator.allUnits)
			{
				if(u.mouseOver == true && this.gameObject != u.gameObject)
				{
					aTarget = u;
					Attacking = true;

				}


			}
		}
		UpdateMove();
		Attack ();
		Death ();
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
			transform.up = (moveToDest - transform.position);

			// Checks if arrived
			if (Vector3.Distance(transform.position, moveToDest) < stopDistanceOffset)
			{
				moveToDest = Vector3.zero;
			}
		}
		else
			//Stops  movement if arrived
		transform.rigidbody2D.velocity = Vector3.zero;
	}
	void OnMouseEnter(){
		mouseOver = true;
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
}
