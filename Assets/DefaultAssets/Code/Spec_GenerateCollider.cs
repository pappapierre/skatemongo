using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spec_GenerateCollider : MonoBehaviour {

	public bool normalsOnly;
	public bool DoNormals;
	public bool onceOnly;
	public MeshFilter Surrogate;

	GameObject meshContainer;
	MeshCollider col;
	Mesh tmpMesh;
	Mesh tmpMeshHardedged;

	public Ticker ticker;

	void Start () {
		tmpMesh = new Mesh ();
		if (normalsOnly) {
			GetComponent<SkinnedMeshRenderer> ().BakeMesh (tmpMesh);
			tmpMeshHardedged = tmpMesh;
			Surrogate.sharedMesh = HardEdger (tmpMeshHardedged);
		} else {

			if (DoNormals) {
				GetComponent<SkinnedMeshRenderer> ().BakeMesh (tmpMesh);
				tmpMeshHardedged = tmpMesh;
				Surrogate.sharedMesh = HardEdger (tmpMeshHardedged);
			}

            meshContainer =  new GameObject ();
            meshContainer.layer = 10;
			Transform tmp = meshContainer.transform;
			tmp.parent = transform;
			tmp.localScale = new Vector3 (1 / transform.lossyScale.x, 1 / transform.lossyScale.y, 1 / transform.lossyScale.z);
			tmp.localPosition = Vector3.zero;
			tmp.localRotation = Quaternion.identity;
			col = meshContainer.AddComponent<MeshCollider> ();
		}
	}

	void Update () {
		if (ticker.Tick()) {
			if (normalsOnly) {
				GetComponent<SkinnedMeshRenderer> ().BakeMesh (tmpMesh);
				tmpMeshHardedged = tmpMesh;
				Surrogate.sharedMesh = HardEdger (tmpMeshHardedged);
			} else {
				if (DoNormals) {
					GetComponent<SkinnedMeshRenderer> ().BakeMesh (tmpMesh);
					tmpMeshHardedged = tmpMesh;
					Surrogate.sharedMesh = HardEdger (tmpMeshHardedged);
				}

				GetComponent<SkinnedMeshRenderer> ().BakeMesh (tmpMesh);
				col.sharedMesh = tmpMesh;
                print("baked");
				if (onceOnly) {
					this.enabled = false;
				}
			}
		}
	}

	void OnDestroy() {
		DestroyImmediate (tmpMesh);
		DestroyImmediate (tmpMeshHardedged);
	}

	Mesh HardEdger(Mesh mesh) {
		// Get mesh info from attached mesh
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		int[] triangles = mesh.triangles;

		// Set up new arrays to use with rebuilt mesh
		Vector3[] newVertices = new Vector3[triangles.Length];
		Vector2[] newUV = new Vector2[triangles.Length];

		// Rebuild mesh so that every triangle has unique vertices
		for (int i = 0; i < triangles.Length; i++) {
			newVertices[i] = vertices[triangles[i]];
			newUV[i] = uv[triangles[i]];
			triangles[i] = i;
		}

		// Assign new mesh and rebuild normals
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		return mesh;
	}
}
