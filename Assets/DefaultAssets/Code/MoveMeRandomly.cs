using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMeRandomly : MonoBehaviour {

    public float t = 0;
    public float interval = 0;
    public float mint = 0.02f;
    public float maxt = 0.03f;

    void Update() {
        if (t > mint) {
            t = Random.Range(mint, maxt);
            transform.localPosition = -Vector3.forward * 100f * Random.Range(0,100f);
        } else {
            t += Time.deltaTime;
        }
    }
}
