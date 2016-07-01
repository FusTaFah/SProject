using UnityEngine;
using System.Collections;

public class FreeCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float rotX = Input.GetAxis("Mouse X");
        float rotY = Input.GetAxis("Mouse Y");

        Vector3 newPos = new Vector3(x, 0.0f, y);
        gameObject.transform.Rotate(0.0f, rotX, 0.0f);
        Camera.main.transform.Rotate(-rotY, 0.0f, 0.0f);

        gameObject.transform.position += gameObject.transform.TransformDirection(newPos)/* * Time.deltaTime*/;
	}
}
