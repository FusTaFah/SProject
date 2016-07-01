using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelCreator : MonoBehaviour {

    //the prefabricated game object to be loaded
	public GameObject voxel;
    //List of Trios Representing bool (whether or not the voxel belongs to the player's team), int (what state the voxel is in), GameObject (the voxel prefab)
    List<Trio<bool, int, GameObject>> voxels;
    //boolean to disallow certain code in the update function to run more than once
    bool seen;
    //width of the sphere world in world units (or voxels)
	public float width;

	// Use this for initialization
	void Start () {
        //initialise the empty list of voxels
        voxels = new List<Trio<bool, int, GameObject>>();
        //the code in update hasn't executed yet
        seen = false;
        //width of structure is 32 voxels across
		float widthOfStructure = 32.0f;
        //3 nested for loops which create a volume out of voxels
		for (float i = -widthOfStructure/2.0f; i <= widthOfStructure/2.0f; i += width) {
			for(float j = -widthOfStructure/2.0f; j <= widthOfStructure/2.0f; j += width){
				for(float k = -widthOfStructure/2.0f; k <= widthOfStructure/2.0f; k += width){
                    //position of voxel to be made
					Vector3 pos = new Vector3(i, j, k);
                    //if this position is less greater than the radius of the sphere, do not create it
                    //this is an improvable method to create a sphere; the midpoint rule can be
                    //used here instead
					if((pos - gameObject.transform.position).magnitude < 0.5 * widthOfStructure){
                        //instantiate a voxel at the supplied position and assign it to a temporary variable
						GameObject temp = Instantiate(voxel, gameObject.transform.position + pos, Quaternion.identity) as GameObject;
                        //if the Y co-ordinate of the voxel is 0 or above, the voxel belongs to the player
                        if (j >= 0)
                        {
                            //add the game object to the list
                            voxels.Add(new Trio<bool, int, GameObject>(true, INACTIVE, temp));
                            //set the alliegence to player team
                            temp.GetComponent<Voxel>().OnPlayerTeam = true;
                            //by default, the voxel is inactive
                            temp.GetComponent<Voxel>().State = INACTIVE;
                            //set its colour to red
                            temp.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                        }
                        //if the Y co-ordinate of the voxel is below 0, the voxel belongs to the enemy team
                        else
                        {
                            //add the game object to the list
                            voxels.Add(new Trio<bool, int, GameObject>(false, INACTIVE, temp));
                            //set the alliegence to enemy team
                            temp.GetComponent<Voxel>().OnPlayerTeam = false;
                            //by default, the voxel is inactive
                            temp.GetComponent<Voxel>().State = INACTIVE;
                            //set its colour to blue
                            temp.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                        }
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
        //execute this code once only
        if (!seen)
        {
            //for all voxels in the game world
            foreach (Trio<bool, int, GameObject> v in voxels)
            {
                //by default, do not draw this voxel until it is proven to be on the surface.
                bool draw = false;
                //get the voxel script reference from the voxel game object
                Voxel vS = v.right.GetComponent<Voxel>();
                //for each normal ray in the voxel
                foreach (Ray r in vS.getNormals())
                {
                    //prepare a raycase
                    RaycastHit rch;
                    //if the ray does not hit another object, it implies that the
                    //voxel lies on the surface of the sphere
                    if (!Physics.Raycast(r, out rch, width))
                    {
                        //so it should be drawn by default
                        draw = true;
                    }
                }
                //if the voxel lies below the surface of the sphere
                if (!draw)
                {
                    //set its state to inactive
                    v.middle = INACTIVE;
                    v.right.GetComponent<Voxel>().State = INACTIVE;
                    //v.right.GetComponent<Voxel>().OnSurface = false;
                }
                //if the voxel lies on the surface of the sphere
                if (draw)
                {
                    //set its state to active
                    v.middle = ACTIVE;
                    v.right.GetComponent<Voxel>().State = ACTIVE;
                    //v.right.GetComponent<Voxel>().OnSurface = true;
                }

            }
            //for each voxel in the voxel list
            foreach (Trio<bool, int, GameObject> g in voxels)
            {
                //if the voxel is on the surface, enable the renderer
                if (g.middle == ACTIVE) g.right.GetComponent<Renderer>().enabled = true;
                //if the voxel is below the surface, disable the renderer
                else if (g.middle == INACTIVE) g.right.GetComponent<Renderer>().enabled = false;
            }

            //ArrayList x = new ArrayList();
            //foreach (Trio<bool, int, GameObject> g in voxels)
            //{
            //    if (g.right.GetComponent<Voxel>().State == ACTIVE)
            //    {
            //        x.Add(g.right.transform.position);
            //    }
            //}
            //SphereMesh spm = GameObject.FindGameObjectWithTag("SphereMesh").GetComponent<SphereMesh>();
            //spm.RenderSphere(x);

            //do not execute this code again
            seen = true;
        }
	}

    //integer which represents the state of being in the interior "hollow" part of the sphere
    const int INACTIVE = 0;
    //integer which represents the state of being on the visible "outside" of the sphere
    const int ACTIVE = 1;
    //integer which represents the state of being destroyed, unable to be drawn for the rest of the game
    const int DESTROYED = 2;
}