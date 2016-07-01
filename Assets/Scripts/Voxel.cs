using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Voxel : MonoBehaviour {
    /*
     * code for rendering voxels procedurally from
     * "Unity Voxel Tutorial Part 1: Generating meshes from code" (Alexandros Stavrinou 2013),
     * http://studentgamedev.blogspot.no/2013/08/unity-voxel-tutorial-part-1-generating.html
    */

    //list to hold values of cube vertices
	public List<Vector3> newVertices = new List<Vector3> ();
    //list to hold values of the cube mesh's triangle indicies.
	public List<int> newTriangles = new List<int> ();
    //list for holding values of UV locations for texturing.
	public List<Vector2> newUV = new List<Vector2> ();

    //width of the voxel in world units
	public float width;
    //mesh of the voxel cube
	Mesh mesh;
    //number of triangles the cube mesh has
	int triangleCount;
    //list of rays which emanate from the centres of each face of the cube
	List<Ray> normals;
    //a boolean to say whether or not this voxel is on the player's team.
    bool onPlayerTeam;
    //field to keep track of the player
    GameObject player;
    //int to keep track of the state of this voxel, either INACTIVE, ACTIVE or DESTROYED
    int state;
    //boolean value to tell whether or not this voxel was on the surface on level start
    bool onSurface;
    //returns whether or not this voxel belongs to the player team, can be mutated
    public bool OnPlayerTeam { get { return onPlayerTeam; } set { onPlayerTeam = value; } }
    //returns the current state of the voxel, either INACTIVE, ACTIVE or DESTROYED, can be mutated
    public int State { get { return state;} set{ state = value; } }
    //returns whether or not this voxel was on the surface on level start
    public bool OnSurface { get { return onSurface; } set { onSurface = value; } }

	// Use this for initialization
	void Start () {
        //set the reference to the player
        player = GameObject.FindGameObjectWithTag("Player");
        //set the mesh
		mesh = GetComponent<MeshFilter> ().mesh;
        //render each face of the voxel using Direct3D's anti-clockwise winding
        RenderVoxel(0, 0, 0);
        //RenderImposter(0, 0, 0);
        //clear the mesh
		mesh.Clear ();
        //set the vertices of the mesh
		mesh.vertices = newVertices.ToArray();
        //set the trianges of the mesh
		mesh.triangles = newTriangles.ToArray();
        //optimize the mesh, allows for faster rendering, yet longer loading times
		mesh.Optimize ();
        //use unity's rendering engine to calculate the triangle normals of the mesh
		mesh.RecalculateNormals ();

        //using predifined positions of faces, create a list of rays that originate from the centre
        //of each cube face, 6 in total
		Vector3 or = gameObject.transform.position + new Vector3(width/2.0f, width/2.0f, width/2.0f);
		normals = new List<Ray>();
		Ray r0 = new Ray (or/* + new Vector3(0.0f, 0.0f, 0.5f)*/, new Vector3 (0.0f, 0.0f, width));
		Ray r1 = new Ray (or/* + new Vector3(0.0f, 0.5f, 0.0f)*/, new Vector3 (0.0f, width, 0.0f));
		Ray r2 = new Ray (or/* + new Vector3(0.5f, 0.0f, 0.0f)*/, new Vector3 (width, 0.0f, 0.0f));
		Ray r3 = new Ray (or/* + new Vector3(0.0f, 0.0f, -0.5f)*/, new Vector3 (0.0f, 0.0f, -width));
		Ray r4 = new Ray (or/* + new Vector3(0.0f, -0.5f, 0.0f)*/, new Vector3 (0.0f, -width, 0.0f));
		Ray r5 = new Ray (or/* + new Vector3(-0.5f, 0.0f, 0.0f)*/, new Vector3 (-width, 0.0f, 0.0f));
		normals.Add (r0);
		normals.Add (r1);
		normals.Add (r2);
		normals.Add (r3);
		normals.Add (r4);
		normals.Add (r5);
	}
	
	// Update is called once per frame
	void Update () {
        //if (state == ACTIVE)
        //{
        //    Vector3 toPlayer = (player.transform.position - gameObject.transform.position).normalized;
        //gameObject.transform.rotation = Quaternion.FromToRotation(-gameObject.transform.forward, toPlayer) * gameObject.transform.rotation;
        //}
        
        //if (state == ACTIVE)
        //{
        //    if ((gameObject.transform.position - player.transform.position).magnitude >= 20.0f)
        //    {
        //        RenderImposter(0, 0, 0);
        //        //clear the mesh
        //        mesh.Clear();
        //        //set the vertices of the mesh
        //        mesh.vertices = newVertices.ToArray();
        //        //set the trianges of the mesh
        //        mesh.triangles = newTriangles.ToArray();
        //        //optimize the mesh, allows for faster rendering, yet longer loading times
        //        mesh.Optimize();
        //        //use unity's rendering engine to calculate the triangle normals of the mesh
        //        mesh.RecalculateNormals();
        //    }
        //    else
        //    {
        //        RenderVoxel(0, 0, 0);
        //        //clear the mesh
        //        mesh.Clear();
        //        //set the vertices of the mesh
        //        mesh.vertices = newVertices.ToArray();
        //        //set the trianges of the mesh
        //        mesh.triangles = newTriangles.ToArray();
        //        //optimize the mesh, allows for faster rendering, yet longer loading times
        //        mesh.Optimize();
        //        //use unity's rendering engine to calculate the triangle normals of the mesh
        //        mesh.RecalculateNormals();
        //    }
        //}


        ////if the current state is "ACTIVE"
        //if (state == ACTIVE)
        //{
        //    //set a boolean with default value false
        //    bool draw = false;
        //    //loop through each ray in the "normals" list
        //    foreach (Ray r in normals)
        //    {
        //        //perform vector subtraction and normalization to obtain the unit vector going
        //        //towards the player
        //        Vector3 toPlayer = (player.transform.position - r.origin).normalized;
        //        //create this ray using the vector toPlayer and r
        //        Ray rayToPlayer = new Ray(r.origin, toPlayer);
        //        //prepare a raycast
        //        RaycastHit rch;
        //        Physics.Raycast(rayToPlayer, out rch);

        //        //if the raycast hits the player, it implies the player can "see" the voxel
        //        if (rch.collider.gameObject.tag == "Player")
        //        {
        //            //set the draw value to true;
        //            draw = true;
        //            //we have found that the player can see this voxel, so break out of the loop
        //            break;
        //        }
        //    }
        //    //show or do not show the voxel depending on the value of draw.
        //    //if the value of draw is true, render the voxel, else don't.
        //    gameObject.GetComponent<Renderer>().enabled = draw;
        //}
	}

	void OnCollisionEnter(Collision coll){
        //if the bullet object that collides with the voxel is of type "EarthBuster"
        if (coll.collider.gameObject.tag == "EarthBuster")
        {
            //call update and destroy on this voxel
            UpdateAndDestroy(gameObject.transform.position);
        }
        else if (coll.collider.gameObject.tag == "Replenisher")
        {
            //call update and replenish on this voxel
            UpdateAndReplenish(gameObject.transform.position);
        }
    }

	void RenderVoxel(int i, int j, int k){
		//vertex index list
		Vector3 v0 = new Vector3 (width + i, 0 + j, 0 + k);
		Vector3 v1 = new Vector3 (0 + i, 0 + j, 0 + k);
		Vector3 v2 = new Vector3 (0 + i, width + j, 0 + k);
		Vector3 v3 = new Vector3 (width + i, width + j, 0 + k);
		
		Vector3 v4 = new Vector3 (width + i, 0 + j, width + k);
		Vector3 v5 = new Vector3 (0 + i, 0 + j, width + k);
		Vector3 v6 = new Vector3 (0 + i, width + j, width + k);
		Vector3 v7 = new Vector3 (width + i, width + j, width + k);

        newVertices.Clear();
		//front face
        newVertices.Add(v0);	//0
        newVertices.Add(v1);	//1
        newVertices.Add(v2);	//2
        newVertices.Add(v3);	//3

        newTriangles.Add(0 + triangleCount);
        newTriangles.Add(1 + triangleCount);
        newTriangles.Add(2 + triangleCount);
        newTriangles.Add(0 + triangleCount);
        newTriangles.Add(2 + triangleCount);
        newTriangles.Add(3 + triangleCount);

        //right face
        newVertices.Add(v1);	//4
        newVertices.Add(v5);	//5
        newVertices.Add(v6);	//6
        newVertices.Add(v2);	//7

        newTriangles.Add(4 + triangleCount);
        newTriangles.Add(5 + triangleCount);
        newTriangles.Add(6 + triangleCount);
        newTriangles.Add(4 + triangleCount);
        newTriangles.Add(6 + triangleCount);
        newTriangles.Add(7 + triangleCount);

        //back face
        newVertices.Add(v5);	//8
        newVertices.Add(v4);	//9
        newVertices.Add(v7);	//10
        newVertices.Add(v6);	//11

        newTriangles.Add(8 + triangleCount);
        newTriangles.Add(9 + triangleCount);
        newTriangles.Add(10 + triangleCount);
        newTriangles.Add(8 + triangleCount);
        newTriangles.Add(10 + triangleCount);
        newTriangles.Add(11 + triangleCount);

        //left face
        newVertices.Add(v4);	//12
        newVertices.Add(v0);	//13
        newVertices.Add(v3);	//14
        newVertices.Add(v7);	//15

        newTriangles.Add(12 + triangleCount);
        newTriangles.Add(13 + triangleCount);
        newTriangles.Add(14 + triangleCount);
        newTriangles.Add(12 + triangleCount);
        newTriangles.Add(14 + triangleCount);
        newTriangles.Add(15 + triangleCount);

        //top face
        newVertices.Add(v3);	//16
        newVertices.Add(v2);	//17s
        newVertices.Add(v6);	//18
        newVertices.Add(v7);	//19

        newTriangles.Add(16 + triangleCount);
        newTriangles.Add(17 + triangleCount);
        newTriangles.Add(18 + triangleCount);
        newTriangles.Add(16 + triangleCount);
        newTriangles.Add(18 + triangleCount);
        newTriangles.Add(19 + triangleCount);

        //bottom face
        newVertices.Add(v1);	//20
        newVertices.Add(v0);	//21
        newVertices.Add(v4);	//22
        newVertices.Add(v5);	//23

        newTriangles.Add(20 + triangleCount);
        newTriangles.Add(21 + triangleCount);
        newTriangles.Add(22 + triangleCount);
        newTriangles.Add(20 + triangleCount);
        newTriangles.Add(22 + triangleCount);
        newTriangles.Add(23 + triangleCount);

        //triangleCount += 24;
	}

    void RenderImposter(int i, int j, int k)
    {
        Vector3 v0 = new Vector3(width + i, 0 + j, 0 + k);
        Vector3 v1 = new Vector3(0 + i, 0 + j, 0 + k);
        Vector3 v2 = new Vector3(0 + i, width + j, 0 + k);
        Vector3 v3 = new Vector3(width + i, width + j, 0 + k);

        newVertices.Clear();
        //front face
        newVertices.Add(v0);	//0
        newVertices.Add(v1);	//1
        newVertices.Add(v2);	//2
        newVertices.Add(v3);	//3

        newTriangles.Add(0);
        newTriangles.Add(1);
        newTriangles.Add(2);
        newTriangles.Add(0);
        newTriangles.Add(2);
        newTriangles.Add(3);
    }

    //returns the voxel's rays, which start from the centre of the faces of the voxel cube
	public List<Ray> getNormals(){
		return normals;
	}

    //function wrapper to clear the voxel's ray list from another script
    public void ClearRayLists()
    {
        normals.Clear();
    }

    void UpdateAndDestroy(Vector3 position)
    {
        //get all voxels in the scene
        GameObject[] voxels = GameObject.FindGameObjectsWithTag("Voxel");

        //loop through all voxels
        foreach (GameObject ga in voxels)
        {
            //if the currently examined voxel is less than 3 world units away from this voxel
            if ((position - ga.transform.position).magnitude < 3.0f)
            {
                //set that voxel's state to destroyed
                ga.GetComponent<Voxel>().State = DESTROYED;
            }
            //if the currently examined voxel is further than 3 but less than 4 world units away from this voxel
            else if ((position - ga.transform.position).magnitude < 4.0f)
            {
                //if the currently examined voxel is part of the hollow interior
                if (ga.GetComponent<Voxel>().State == INACTIVE)
                {
                    //set its state to active
                    ga.GetComponent<Voxel>().State = ACTIVE;
                }
            }
        }
        UpdateVoxels(voxels);
    }

    void UpdateAndReplenish(Vector3 position)
    {
        //get all voxels in the scene
        GameObject[] voxels = GameObject.FindGameObjectsWithTag("Voxel");

        //loop through all voxels
        foreach (GameObject ga in voxels)
        {
            //if the currently examined voxel is less than 3 world units away from this voxel
            if ((position - ga.transform.position).magnitude < 3.0f && ga.GetComponent<Voxel>().OnSurface)
            {
                //set that voxel's state to inactive
                ga.GetComponent<Voxel>().State = INACTIVE;
            }
            //if the currently examined voxel is further than 3 but less than 4 world units away from this voxel
            else if ((position - ga.transform.position).magnitude < 4.0f)
            {
                //set its state to active
                ga.GetComponent<Voxel>().State = ACTIVE;
            }
        }
        UpdateVoxels(voxels);
    }

    void UpdateVoxels(GameObject[] voxels)
    {
        //initialise player and enemy team scores for counting
        int playerTeamScore = 0;
        int enemyTeamScore = 0;

        //for each voxel in the scene
        foreach (GameObject ga in voxels)
        {
            //if it is part of the hollow interior
            if (ga.GetComponent<Voxel>().State == INACTIVE)
            {
                //disable its renderer and collider
                ga.GetComponent<BoxCollider>().enabled = false;
                ga.GetComponent<Renderer>().enabled = false;
                //add one to the score of the team player if it belongs to the player's team.
                //"Inactive" voxels still count as in play until they are destroyed, so their
                //existance needs to be accounted for
                if (ga.GetComponent<Voxel>().OnPlayerTeam)
                {
                    playerTeamScore++;
                }
                //else add one to the enemy team score if it belongs to the enemy team
                else
                {
                    enemyTeamScore++;
                }
                //ga.SetActive(false);
            }
            //else if the voxel is active
            else if (ga.GetComponent<Voxel>().State == ACTIVE)
            {
                //enable its collider and its renderer
                ga.GetComponent<BoxCollider>().enabled = true;
                ga.GetComponent<Renderer>().enabled = true;
                //same behaviour as before, add one to the relevant scores depending on which team
                //it belongs to
                if (ga.GetComponent<Voxel>().OnPlayerTeam)
                {
                    playerTeamScore++;
                }
                else
                {
                    enemyTeamScore++;
                }
                //ga.SetActive(true);
            }
            ////if the voxel's state is "Destroyed"
            else if (ga.GetComponent<Voxel>().State == DESTROYED)
            {
                //disable its renderer and collider
                ga.GetComponent<BoxCollider>().enabled = false;
                ga.GetComponent<Renderer>().enabled = false;
            }
        }
        //find the "VoxelCounter" game object
        VoxelCounter vCount = GameObject.Find("ScoreCounter").GetComponent<VoxelCounter>();
        //set its enemy and player team fields with the result of the score count done previously
        vCount.setEnemyTeamScore(enemyTeamScore);
        vCount.setPlayerTeamScore(playerTeamScore);

        //ArrayList x = new ArrayList();
        //foreach (GameObject g in GameObject.FindGameObjectsWithTag("Voxel"))
        //{
        //    if (g.GetComponent<Voxel>().State == ACTIVE)
        //    {
        //        x.Add(g.transform.position);
        //    }
        //}
        //Debug.Log(x.Count);
        //SphereMesh spm = GameObject.FindGameObjectWithTag("SphereMesh").GetComponent<SphereMesh>();
        //spm.RenderSphere(x);
    }

    //integer which represents the state of being in the interior "hollow" part of the sphere
    const int INACTIVE = 0;
    //integer which represents the state of being on the visible "outside" of the sphere
    const int ACTIVE = 1;
    //integer which represents the state of being destroyed, unable to be drawn for the rest of the game
    const int DESTROYED = 2;
}
