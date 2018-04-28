using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTextureOffset : MonoBehaviour {
	void Start () {
        transform.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(Random.Range(0, 1f), Random.Range(-1f, 1f)));
	}
}
