using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {


	public bool selected = false;
	private Vector3 moveToDest = Vector3.zero;
	public float floorOffset = 0;
	public float speed = 5;
	public float turnSpeed = 25;
	public float stopDistanceOffset = 0.25f;
	public bool mouseOver = false;
	public Camera myCam;
	void Start () {
		myCam = Camera.main;
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
			if(selected)
			{
				CameraOperator.RegisterUnit(this);
			}
			if(!selected)
			{
				CameraOperator.UnRegisterUnit(this);
			}
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

			}
		}
		UpdateMove();
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

}
