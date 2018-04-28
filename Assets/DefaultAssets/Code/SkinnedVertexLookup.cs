using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedVertexLookup : MonoBehaviour {

	public Material MeshMaterial;
	public GameObject Geo;
	public GameObject thisResults;
	public bool hideMesh = true;
	public bool ignoreDuplicates = true;


	public GameObject Prefab;
	public int totalInstances = 0;
	public int maxInstances = 100;
	public int onlyEveryNthVert = 1;
	public bool random = false;
	public float randomChance = 50.0f;

	List<Vector3> tookenVerts = new List<Vector3> ();
	public Mesh mesh;
	SkinnedMeshRenderer smRef;

	public bool keepUpdated = false;

	// Use this for initialization
	void OnEnable () {
		print("cleared SkinnedVertexLookup");
		for(int f = thisResults.transform.childCount-1; f>=0; f--){
			Destroy( thisResults.transform.GetChild(f).gameObject );
		}

		if(thisResults == null)
		thisResults = new GameObject ();
		
		mesh = new Mesh ();

		if (Geo == null) {
			if (GetComponent<SkinnedMeshRenderer> ()) {
				GetComponent<SkinnedMeshRenderer> ().BakeMesh(mesh);
				smRef = GetComponent<SkinnedMeshRenderer> ();
			} else {
				return;
			}
		} else {
			if (Geo.GetComponent<SkinnedMeshRenderer> () != null) {
				Geo.GetComponent<SkinnedMeshRenderer> ().BakeMesh (mesh);
				smRef = Geo.GetComponent<SkinnedMeshRenderer> ();
			} else {
				print ("Missing Skinned Mesh");
			}
		}

		if (mesh != null) {
			
			if (hideMesh) {
				smRef.material = MeshMaterial;
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
							if (Random.Range (0, 100f) < randomChance) {
								randPass = true;
							}
						}
						if (randPass) {
							Transform tmp = Instantiate (Prefab).transform;
							tmp.up = mesh.normals [n];
							tmp.parent = thisResults.transform;
							tmp.localPosition = mesh.vertices [n];
							totalInstances++;
							if (tmp.GetComponent<Spec_StickToVert> ()) {
								tmp.GetComponent<Spec_StickToVert> ().vertNumber = n;
								tmp.GetComponent<Spec_StickToVert> ().svl = this;
							}
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

	void LateUpdate(){
		if (keepUpdated) {
			smRef.BakeMesh(mesh);
		}
	}
}
