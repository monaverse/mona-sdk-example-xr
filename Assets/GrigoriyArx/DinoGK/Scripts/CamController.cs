using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour {
	
	//private CharacterController controller;
	public float speed = 1.0f;
	public float rotSpeed = 1f;
	private Vector3 currentPosition = Vector3.zero;


	
	
	// Use this for initialization
	void Start () 
	{
		currentPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentPosition.z = Input.GetAxis("Horizontal") * -speed * Time.deltaTime;
		currentPosition.x = Input.GetAxis("Vertical") * speed * Time.deltaTime;

		//this.transform.localPosition = currentPosition;

			transform.Translate(currentPosition, Space.Self);


		if (Input.GetKey("q"))
		{
			transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
		}
		if (Input.GetKey("e"))
		{
			transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime * -1, 0));
		}
	}
}

