using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyManualControl : MonoBehaviour {

    public Rigidbody rb;
    public Animator anim;
    public float controlPower = 1;

    void Start () {
        if (rb == null) { 
            rb = GetComponent<Rigidbody>();
        }

        if (anim == null) {
            anim = GetComponent<Animator>();
        }
    }
	
	void Update () {
        // X axis
        if (Input.GetKey(KeyCode.A)) {
            rb.AddForce(-transform.right * controlPower);
        }
        if (Input.GetKey(KeyCode.D)) {
            rb.AddForce(transform.right * controlPower);
        }

        // Z axis
        if (Input.GetKey(KeyCode.W)) {
            rb.AddForce(transform.forward * controlPower);
        }
        if (Input.GetKey(KeyCode.S)) {
            rb.AddForce(-transform.forward * controlPower);
        }

        // Y axis
        if (Input.GetKey(KeyCode.E)) {
            rb.AddForce(transform.up * controlPower);
        }
        if (Input.GetKey(KeyCode.Q)) {
            rb.AddForce(-transform.up * controlPower);
        }

        anim.SetFloat("x", Mathf.Clamp(rb.velocity.x, -1, 1));
        anim.SetFloat("y", Mathf.Clamp(rb.velocity.y, -1, 1));
        anim.SetFloat("z", Mathf.Clamp(rb.velocity.z, -1, 1));
    }
}
