using UnityEngine;
using System.Collections;

public class SmallestSphere : MonoBehaviour {
	private float _radius;
	private Vector3 _center;
	//For welzl calculations
	private const float RadiusEpsilon = 1.00001f;
	private GameObject sphere;
	
	void Start () {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		int[] indices = mesh.triangles;
		
		sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		
		ComputeFromPrimitives(vertices, indices);
	}
	public float Radius {
		get {
			return _radius;
		}
		set {
			_radius = value;
			Debug.Log("Set r="+value);
		}
	}
	
	void ComputeFromPrimitives(Vector3[] vertices, int[] indices)
	{
		Vector3[] copy = new Vector3[indices.Length];
		
		for(int i = 0; i < indices.Length; i++) {
			copy[i] = transform.TransformPoint(vertices[indices[i]]); // test
			//copy[i] = vertices[indices[i]];
		}
		
		CalculateWelzl(copy, copy.Length, 0, 0);
	}
	
	//Welzl minimum bounding sphere algorithm
	void CalculateWelzl(Vector3[] points, int length, int supportCount, int index) {
		switch(supportCount) {
			case 0:
				_radius = 0;
				_center = Vector3.zero;
				break;
			case 1:
				_radius = 1.0f - RadiusEpsilon;
				_center = points[index-1];
				break;
			case 2:
				
				SetSphere(points[index-1], points[index-2]);
				
				break;
				
			case 3:
				SetSphere(points[index-1], points[index-2], points[index-3]);
				break;
			case 4:
				SetSphere(points[index-1], points[index-2], points[index-3], points[index-4]);
				return;
		}
		
		for(int i = 0; i < length; i++)
		{
			Vector3 comp = points[i + index];
			float distSqr;
			
			distSqr = (comp-_center).sqrMagnitude;
			
			if(distSqr - (_radius * _radius) > RadiusEpsilon - 1.0f) {
				for(int j = i; j > 0; j--) {
					Vector3 a = points[j + index];
					Vector3 b = points[j - 1 + index];
					points[j + index] = b;
					points[j - 1 + index] = a;
				}
				CalculateWelzl(points, i, supportCount + 1, index + 1);
			}
		}
	}
	
	//For Welzl calc - 2 support points
	void SetSphere(Vector3 O, Vector3 A)
	{
		Radius = (float) System.Math.Sqrt(((A.x - O.x) * (A.x - O.x) + (A.y - O.y)
		                                   * (A.y - O.y) + (A.z - O.z) * (A.z - O.z)) / 4.0f) + RadiusEpsilon - 1.0f;
		float x = (1 - .5f) * O.x + .5f * A.x;
		float y = (1 - .5f) * O.y + .5f * A.y;
		float z = (1 - .5f) * O.z + .5f * A.z;
		
		// TODO:
		SetCenter(x, y, z);
		
	}
	
	//For Welzl calc - 3 support points
	void SetSphere(Vector3 O, Vector3 A, Vector3 B) {
		Vector3 a = A - O;
		Vector3 b = B - O;
		Vector3 aCrossB = Vector3.Cross(a, b);
		float denom = 2.0f * Vector3.Dot(aCrossB, aCrossB);
		if(denom == 0) {
			_center = Vector3.zero;
			_radius = 0;
		} else {
			
			Vector3 o = ((Vector3.Cross(aCrossB, a) * b.sqrMagnitude)+ (Vector3.Cross(b, aCrossB) * a.sqrMagnitude)) / denom;
			_radius = o.magnitude * RadiusEpsilon;
			_center = O + o;
		}
	}
	
	//For Welzl calc - 4 support points
	void SetSphere(Vector3 O, Vector3 A, Vector3 B, Vector3 C) {
		Vector3 a = A - O;
		Vector3 b = B - O;
		Vector3 c = C - O;
		
		float denom = 2.0f * (a.x * (b.y * c.z - c.y * b.z) - b.x
		                      * (a.y * c.z - c.y * a.z) + c.x * (a.y * b.z - b.y * a.z));
		if(denom == 0) {
			_center = Vector3.zero;
			_radius = 0;
		} else {
			Vector3 o = ((Vector3.Cross(a, b) * c.sqrMagnitude)
			             + (Vector3.Cross(c, a) * b.sqrMagnitude)
			             + (Vector3.Cross(b, c) * a.sqrMagnitude)) / denom;
			_radius = o.magnitude * RadiusEpsilon;
			_center = O + o;
		}
	}
	
	void SetCenter(float x,float y,float z)
	{
		sphere.transform.localScale = new Vector3(_radius*2,_radius*2,_radius*2);
		sphere.transform.position = new Vector3(x,y,z);
	}
}