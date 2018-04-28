using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimatorTime : MonoBehaviour {

    public Animator anim;

	// Use this for initialization
	void Start () {
        anim.Play(0, 0, Random.Range(0.0f, 1.0f));
    }
}
