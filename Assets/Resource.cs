using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {

	public int RUs = 500;
	// Use this for initialization
	void Start () {
		CameraOperator.allResources.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
	if(RUs <= 0)
		{
			CameraOperator.allResources.Remove (this);
			Destroy(this.gameObject);
			Destroy(this);
		}
	}
}
