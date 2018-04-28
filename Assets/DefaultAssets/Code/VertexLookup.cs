using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexLookup : MonoBehaviour {

	public GameObject Geo;
	public bool hideMesh = true;
	public bool ignoreDuplicates = true;
	public GameObject thisResults;

	public GameObject Prefab;
	List<Vector3> tookenVerts = new List<Vector3> ();
	Mesh mesh;

	public int totalInstances = 0;
	public int maxInstances = 100;
	public int onlyEveryNthVert = 1;
	public bool random = false;
	public float randomChance = 50.0f;
	public bool preScale;

	// Use this for initialization
	void Start () {
		if(thisResults == null)
			thisResults = new GameObject ();
		
		if (Geo == null) {
			if (GetComponent<MeshFilter> ()) {
				mesh = GetComponent<MeshFilter> ().mesh;
			} else {
				return;
			}
		} else {
			mesh = Geo.GetComponent<MeshFilter> ().mesh;
		}

		if (mesh != null) {
			if (hideMesh) {
				if (GetComponent<Renderer> ()) {
					GetComponent<Renderer> ().enabled = false;
				}
			}

			for (int n = 0; n < mesh.vertexCount; n++) {
				bool dupCheck = false;
				if (!tookenVerts.Contains (mesh.vertices [n])) {
					tookenVerts.Add (mesh.vertices [n]);
					dupCheck = true;
				}
				if (!ignoreDuplicates || !dupCheck) {
					if ((maxInstances == 0 || totalInstances <= maxInstances) && n % Mathf.Clamp(onlyEveryNthVert, 1, 100000) == 0) {
						bool randPass = true;
						if (random) {
							randPass = false;
							if (Random.Range (0, 100f) > randomChance) {
								randPass = true;
							}
						}
						if (randPass) {
							Transform tmp = Instantiate (Prefab).transform;
							tmp.up = mesh.normals [n];
							Vector3 tmpScale = tmp.localScale;
							tmp.parent = thisResults.transform;

							if (preScale)
								tmp.localScale = tmpScale;
							
							tmp.localPosition = mesh.vertices [n];
							totalInstances++;
						}
					}
				}
			}
		}

		thisResults.transform.parent = transform;
		thisResults.transform.localPosition = Vector3.zero;
		thisResults.transform.localScale = Vector3.one;
		thisResults.transform.localRotation = Quaternion.identity;
		thisResults.SetActive (true);
	}
}
