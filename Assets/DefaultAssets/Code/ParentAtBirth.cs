using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentAtBirth : MonoBehaviour {

    public Transform tgt;

    public void Start() {
        transform.SetParent(tgt);
    }
}
        