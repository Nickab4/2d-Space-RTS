using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {
	//Owner Ship
	Unit Ship = null;
	//Stats
	public int damage = 5;
	public int range = 7;
	public float atkSpd = 0.5f;
	//Management
	private float atkTime = 0.0f;
	private Unit aTarget;
	private Unit pTarget;
	// Use this for initialization
	void Start () {
		Ship = this.transform.parent.GetComponent<Unit>();
		Debug.Log (Ship.name);
	}
	
	// Update is called once per frame
	void Update () {
		if (Ship.selected && Input.GetKeyDown(KeyCode.A))
		{
			foreach(Unit u in CameraOperator.allUnits)
			{
				if(u.mouseOver == true && CameraOperator.Player != u.Owner)
				{
					pTarget = u;
					Debug.Log (pTarget.name);
				}
			}
		}


		Aggro ();
		Attack ();

	}
	void Attack()
	{
		atkTime += Time.deltaTime;

		if(pTarget != null)
		{
				//Test Range
			if(Vector3.Distance(pTarget.transform.position, transform.position) <= range)
			{

				transform.up = (pTarget.transform.position - transform.position);
				if(atkTime > atkSpd)
				{
					Debug.Log (this.name + " attacked " + pTarget.name + " for " + damage + " damage.");
					atkTime = 0f;
					pTarget.currentHP = pTarget.currentHP - damage;
					if(!pTarget.Moving && pTarget.Armed)
						pTarget.moveToDest = this.transform.position;
						
					CallForHelp();
						
				}
			}
		}
		if(atkTime > atkSpd)
			AttackAggro();

		if(aTarget == null)
		{
			transform.up = Ship.transform.up;
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
	void Aggro()
	{
		aTarget = null;
		foreach(Unit u in CameraOperator.allUnits)
		{
			if(Ship.Owner != u.Owner && aTarget == null)
			{
				if(Vector3.Distance(u.transform.position, transform.position) <= range)
				{
					aTarget = u;
				
				}
				
			}
			
			
		}
	}
	void AttackAggro()
	{
		if(aTarget != null)
		{
			//Test Range
			if(Vector3.Distance(aTarget.transform.position, transform.position) <= range)
			{
				
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
		}
	}
}
