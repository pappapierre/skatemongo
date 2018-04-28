using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spec_StickToVert : MonoBehaviour {

	public SkinnedMeshRenderer renderer;
	public int vertNumber;
	public float interval = 0.1f;
	public float t = 0;
	public SkinnedVertexLookup svl;
	public Mesh mesh;
	public bool RetainNormals = true;

	// Update is called once per frame
	void Update () {
		if (Tick()) {
			if (svl != null) {
				transform.localPosition = Vector3.Scale(svl.mesh.vertices [vertNumber], new Vector3(1/transform.parent.lossyScale.x, 1/transform.parent.lossyScale.y, 1/transform.parent.lossyScale.z));
				if (RetainNormals)
					transform.up = -svl.mesh.normals [vertNumber];//Quaternion.LookRotation (svl.mesh.normals [vertNumber]);
//				Debug.DrawRay(transform.position, transform.up, Color.red, interval);
			}
		}
	}

	//TICKER TIMER
	public bool Tick(){
		if(t>=interval){
			t = 0;
			return true;
		}else{
			t += Time.deltaTime;
			return false;
		}
	}
}
