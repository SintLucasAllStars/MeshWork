using UnityEngine;
using System.Collections;

public class MeshDeform : MonoBehaviour {
	public float baseRadius;
	public float hardness;
	
	Rigidbody rb;
	MeshCollider mc;
	Mesh mesh;
	
	bool[] damage;
	bool dirty;
	
	void Start () {
		rb = GetComponent<Rigidbody>();
		mc = GetComponent<MeshCollider>();
		mesh = GetComponent<MeshFilter>().mesh;
		damage = new bool[mesh.vertices.Length];
		dirty = false;
	}
	
	void Update () {
		//NOTE: There are probably better ways to deal with this...
		if (dirty)
		{
			DestroyImmediate(mc);
 			mc = this.gameObject.AddComponent<MeshCollider>();
 			mc.convex = true;
			mc.sharedMesh = mesh;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		//NOTE: Just using the first point of contact. We could loop through and apply deformation to all points.
		//This does the job though, and it is less expensive.
		Vector3 colPoint = transform.InverseTransformPoint(other.contacts[0].point);
		Vector3 velocity = other.relativeVelocity;

		Vector3[] normals = mesh.normals;
		Vector3[] vertices = mesh.vertices;
		
		//NOTE: Here we only consider the velocity of the deformable object. We should consider the relative velocity to the colliding object.
		float velocityEffect = Utils.Map(velocity.magnitude * rb.mass, 0, 5, 0, 1);

		for (int i = 0; i < vertices.Length; i++)
		{
			float distance = Vector3.Distance(vertices[i], colPoint);

			if (distance < baseRadius && !damage[i])
			{
				float percentEffect = Utils.Map(distance, 0, baseRadius, 1, 0);

				Vector3 deformation = (-normals[i].normalized * percentEffect) / (hardness / velocityEffect);
				vertices[i] += deformation;
				
				//NOTE: Each vertex only deforms once.
				damage[i] = true;
			}
		}

		mesh.vertices = vertices;
		dirty = true;
	}
}
