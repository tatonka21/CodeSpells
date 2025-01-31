using UnityEngine;
using System.Collections;

public class FindSeed : MonoBehaviour {
	public GameObject obj;
	
	private bool atGrove = false;

	// Use this for initialization
	void Start () {
		transform.LookAt(obj.transform);
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!atGrove)
		{
			transform.Translate(Vector3.forward * Time.deltaTime);
			float dist = Vector3.Distance(obj.transform.position, transform.position);
			if(dist < 10)
			{
				atGrove = true;
			}
		}
	}
}
