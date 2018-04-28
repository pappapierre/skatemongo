using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody_StickToInit : MonoBehaviour {

    public Vector3 initPos;
    public Rigidbody rb;
    public GameObject Visicube;
    public float stickyPower = 1f;

    void Start() {
        rb = GetComponent<Rigidbody>();
        initPos = transform.localPosition;
        GenerateSprings();

        if(Visicube)
        Instantiate(Visicube, transform);
    }

    void FixedUpdate() {
        rb.AddForce((initPos - transform.localPosition) * stickyPower);
    }

    public float neighbourRadius = 0.6f;
    public float springPower = 120f;
    public float damper = 0f;

    void GenerateSprings() {
        var overlaps = Physics.OverlapSphere(transform.position, neighbourRadius);
        foreach (Collider each in overlaps) {
            if(each.transform != transform && each.GetComponent<Rigidbody>() && each.tag == "GameController") {
                var spring = gameObject.AddComponent<SpringJoint>();
                spring.connectedBody = each.GetComponent<Rigidbody>();
                spring.spring = springPower;
                spring.damper = damper;
            }
        }
    }
}
