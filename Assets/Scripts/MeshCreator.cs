using UnityEngine;
using System.Collections;

public class MeshCreator : MonoBehaviour {

	private MeshFilter mf;
	private MeshRenderer mr;
	private MeshCollider mc;
	private BoxCollider bc;
	Vector3 [] meshTransform;

	public int width = 10;
	public int height= 10;

	public float brushSize = 2f;
	public float brushStrength = 1f;

	// Use this for initialization
	void Start () {
		mf = GetComponent<MeshFilter>();
		mr = GetComponent<MeshRenderer>();
		mf.mesh = new Mesh();
		mf.mesh.name = "GeneratedMesh";

		ClearMeshTransform();
		GenerateMesh();
		//GenerateMeshCollider();
		GenerateCollider();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R))
		{
			ClearMeshTransform();
			GenerateMesh();
		}

		if(Input.GetMouseButton(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(ray, out hit, 100f))
			{
				Vector3 hitPoint = hit.point;

				for(int i=0; i < mf.mesh.vertices.Length; i++)
				{
					Vector3 vertex = transform.TransformPoint(mf.mesh.vertices[i]);
					float distance = Vector3.Distance(vertex, hitPoint);

					if(distance <= brushSize)
					{
						meshTransform[i] += mf.mesh.normals[i] * brushStrength / (distance/brushSize);
					}
				}

				GenerateMesh();
				//GenerateMeshCollider();
				//GenerateCollider();
			}
		}
	}

	void ClearMeshTransform()
	{
		meshTransform = new Vector3[width*height];
		for(int i=0; i<meshTransform.Length; i++)
		{
			meshTransform[i] = Vector3.zero;
		}
	}

	void GenerateMesh()
	{
		mf.mesh.Clear();

		Vector3 [] verts = new Vector3[width*height];
		Vector2 [] uvs = new Vector2[width*height];
		int numTriangles = width * height * 6;
		int[] triangles = new int[numTriangles];
		float uvFactorX = 1.0f/(width-1);
		float uvFactorY = 1.0f/(height-1);


		for (int x=0; x<width; x++)
		{
			for(int y=0; y<height; y++)
			{
				int i = Index2Dto1D(x,y);
				verts[i] = new Vector3(x, y, 0f) + meshTransform[i];
				uvs[i] = new Vector2(x*uvFactorX, y*uvFactorY);
			}
		}

		int index = 0;
		for (int y = 0; y < height-1; y++)
		{
			for (int x = 0; x < width-1; x++)
			{
				triangles[index]   = (y     * width) + x;
				triangles[index+1] = ((y+1) * width) + x;
				triangles[index+2] = (y     * width) + x + 1;

				triangles[index+3] = ((y+1) * width) + x;
				triangles[index+4] = ((y+1) * width) + x + 1;
				triangles[index+5] = (y     * width) + x + 1;
				index += 6;
			}
		}

		mf.mesh.vertices = verts;
		mf.mesh.uv = uvs;
		mf.mesh.triangles = triangles;
		mf.mesh.RecalculateNormals();
	}

	int Index2Dto1D(int x, int y)
	{
		return x+y*width;
	}

	Vector2 Index1Dto2D(int i)
	{
		int x = i % width;
		int y = (i-x) / width;

		return new Vector2(x,y);
	}

	void GenerateMeshCollider()
	{
		if(mc != null)
		{
			DestroyImmediate(mc);
		}

		mc = gameObject.AddComponent<MeshCollider>();
		mc.convex = true;
		mc.sharedMesh = mf.mesh;
	}

	void GenerateCollider()
	{
		if(bc != null)
		{
			DestroyImmediate(bc);
		}

		bc = gameObject.AddComponent<BoxCollider>();
	}
}
