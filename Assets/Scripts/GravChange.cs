using UnityEngine;
using System.Collections;

public class GravChange : MonoBehaviour {
    /*
     * the code from this script is from a tutorial by Sebastian Lague (2014)
     * "[Unity Tutorial] First Person Controller: Spherical Worlds".
     * https://github.com/SebLague/Spherical-Gravity/blob/master/GravityAttractor.cs
    */
    //set the gravity to -10 world units per second per second
    public float gravity = -10.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	}

	 
	public void Attract(GameObject body){
        //obtain the direction from the gravity body and the combatant body
		Vector3 targetDir = (body.transform.position - gameObject.transform.position).normalized;
        //obtain the up vector of the body
		Vector3 bodyUp = body.transform.up;
        //rotate a world body such that the y axis of the body is parallel to the vector between the core and the combatant
		body.transform.rotation = Quaternion.FromToRotation (bodyUp, targetDir) * body.transform.rotation;
        //if the rigidbody is not null, apply the gravity to the body
		if(body.GetComponent<Rigidbody>() != null) body.GetComponent<Rigidbody> ().AddForce (targetDir * gravity);
	}
}
