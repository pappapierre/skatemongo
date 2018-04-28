using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachAtBirth : MonoBehaviour {
	void Start () {
        transform.parent = null;
	}
}
