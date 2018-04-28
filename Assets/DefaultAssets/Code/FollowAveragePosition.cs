using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAveragePosition : MonoBehaviour {

    public Transform tgtContainer;
    Vector3 avrg;
	// Update is called once per frame
	void Update () {
        avrg = tgtContainer.GetChild(0).position;
        foreach (Transform each in tgtContainer) {
            if(each.GetSiblingIndex() != 0)
            avrg += each.position;
        }
        avrg /= tgtContainer.childCount;
        transform.position = avrg;
    }
}
