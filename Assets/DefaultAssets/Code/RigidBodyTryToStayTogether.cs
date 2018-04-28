using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyTryToStayTogether : MonoBehaviour {
    public float keepClose = 0.2f;
    public float dPow = 10;
    Vector3 avrg;
    // Update is called once per frame
    void LateUpdate() {
        avrg = transform.GetChild(0).position;
        foreach (Transform each in transform) {
            if (each.GetSiblingIndex() != 0)
                avrg += each.position;
        }
        avrg /= transform.childCount;

        foreach (Transform each in transform) {
            if (each.GetComponent<Rigidbody>())
                each.GetComponent<Rigidbody>().AddForce((avrg - each.position) * keepClose * ((avrg - each.position).magnitude)/dPow) ;
        }
    }
}
