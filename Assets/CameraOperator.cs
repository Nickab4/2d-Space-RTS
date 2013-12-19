using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CameraOperator : MonoBehaviour {

	public Texture2D selectionHighlight = null;
	public static Rect selection = new Rect(0,0,0,0);
	private Vector3 startClick = -Vector3.one;
	private static Vector3 moveToDestination = Vector3.zero;
	private static List<string> passables = new List<string>() { "Floor" };
	public static List<Unit> selectedUnits = new List<Unit>();
	public static List<Unit> allUnits = new List<Unit>();
	public static List<Resource> allResources = new List<Resource>();
	private string ships = " ";
	public static int Player = 1;
	// Update is called once per frame
	void Update () {
		CheckCamera();
		Cleanup();
		SelectedList();
		SelecteUpdate();

		//Do some key-handling stuff here for now
		if (Input.GetKeyDown(KeyCode.T)) {
			string s = "";
			foreach(Unit u in CameraOperator.allUnits) {
				s += u.name + "\n";
			}
			Debug.Log (s);
		}

	}

private void CheckCamera()
	{
	if (Input.GetMouseButtonDown(0))
			startClick = Input.mousePosition;
		else if (Input.GetMouseButtonUp(0))
		{
			if (selection.width < 0)
			{
				selection.x += selection.width;
					selection.width = - selection.width;
			}
			if (selection.height <0)
			{
				selection.y += selection.height;
				selection.height = - selection.height;
			}
			startClick = -Vector3.one;

		}
		if (Input.GetMouseButton(0))
			selection = new Rect(startClick.x, InvertMouseY(startClick.y), Input.mousePosition.x -startClick.x, InvertMouseY(Input.mousePosition.y) - InvertMouseY(startClick.y));

	}
	private void OnGUI()
	{
		if (startClick != -Vector3.one)
		{
			GUI.color = new Color(1,1,1, 0.3f);
			GUI.DrawTexture (selection, selectionHighlight);
		}
			//display selected ships list
			GUI.Label(new Rect(Screen.width -115,0, 115, 300), ships);
	}
	public static float InvertMouseY(float y)
		{
				return Screen.height - y;

		}
	public static Vector3 GetDestination()
	{
		if (moveToDestination == Vector3.zero)
		{
			RaycastHit hit;
			Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast (r, out hit))
			   {
				  while (!passables.Contains(hit.transform.gameObject.name))
				{
					if(!Physics.Raycast(hit.transform.position, r.direction, out hit))
					   break;
				}
					
			   }
			if(hit.transform != null)
				moveToDestination = hit.point;
		}
		return moveToDestination;
	}
	private void Cleanup()
	{
		if (!Input.GetMouseButtonUp (1))
			moveToDestination = Vector3.zero;
	}
	private void SelectedList()
	{
		ships = "";
		foreach(Unit u in CameraOperator.selectedUnits) {
			ships += u.name + "\n";
		}
	}
	private void SelecteUpdate()
	{
		CameraOperator.selectedUnits.Clear ();
		
		//Handle selected units
		foreach(Unit u in CameraOperator.allUnits) {
			if (u.selected) {
				CameraOperator.selectedUnits.Add(u);
			}
		}
	}
}
