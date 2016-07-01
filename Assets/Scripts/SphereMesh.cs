using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphereMesh : MonoBehaviour {
    //amount of triangles
    int triangleCount;
    //list to hold values of cube vertices
    public List<Vector3> newVertices = new List<Vector3>();
    //list to hold values of the cube mesh's triangle indicies.
    public List<int> newTriangles = new List<int>();
    //list for holding values of UV locations for texturing.
    public List<Vector2> newUV = new List<Vector2>();
    //mesh of the sphere
    Mesh mesh;

    ArrayList vertexPositions;

	// Use this for initialization
	void Start () {
        //initialise triangleCount to 0
        triangleCount = 0;
        //initialise vertexPositions to new ArrayList
        vertexPositions = new ArrayList();
        //set the mesh
        mesh = GetComponent<MeshFilter>().mesh;
        //render each face of the voxel using Direct3D's anti-clockwise winding
        RenderSphere(vertexPositions);
        //RenderImposter(0, 0, 0);
        //clear the mesh
        mesh.Clear();
        //set the vertices of the mesh
        mesh.vertices = newVertices.ToArray();
        //set the trianges of the mesh
        mesh.triangles = newTriangles.ToArray();
        //optimize the mesh, allows for faster rendering, yet longer loading times
        mesh.Optimize();
        //use unity's rendering engine to calculate the triangle normals of the mesh
        mesh.RecalculateNormals();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RenderSphere(ArrayList posList)
    {
        newVertices.Clear();
        newTriangles.Clear();
        triangleCount = 0;
        for (int i = 0; i < posList.Count; i++)
        {
            Vector3 vec2 = new Vector3(20000, 20000, 20000);
            Vector3 vec3 = new Vector3(20000, 20000, 20000);

            for (int j = 0; j < posList.Count; j++)
            {
                Vector3 temp = (Vector3)posList[j];
                if ((temp - (Vector3)posList[i]).magnitude <= (vec2 - (Vector3)posList[i]).magnitude)
                {
                    vec2 = temp;
                }
            }

            for (int j = 0; j < posList.Count; j++)
            {
                Vector3 temp = (Vector3)posList[j];
                if ((temp - (Vector3)posList[i]).magnitude <= (vec3 - (Vector3)posList[i]).magnitude && (vec3 - (Vector3)posList[i]).magnitude > (vec2 - (Vector3)posList[i]).magnitude)
                {
                    vec3 = temp;
                }
            }
            //front face
            newVertices.Add((Vector3)posList[i]);	//0
            newVertices.Add(vec3);	//1
            newVertices.Add(vec2);	//2

            newTriangles.Add(0 + triangleCount);
            newTriangles.Add(1 + triangleCount);
            newTriangles.Add(2 + triangleCount);
            triangleCount += 3;   
        }
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();

    }
}
