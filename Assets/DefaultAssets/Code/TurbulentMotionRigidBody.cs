//
// Reaktion - An audio reactive animation toolkit for Unity.
//
// Copyright (C) 2013, 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;

public class TurbulentMotionRigidBody : MonoBehaviour{

    public bool slaver = false;

    // Noise parameters.
    public float density      = 0.1f;
    public Vector3 linearFlow = Vector3.up * 0.2f;

    // Amplitude and coefficient (wave number).
    public Vector3 displacement = Vector3.one;
    public float coeffDisplacement = 1.0f;

    // Initial states.
    Vector3  initialPosition;

    public float posWavePower = 3;
    Vector3 offset = new Vector3(13, 17, 19);

    public Rigidbody rb;
    //public Animator anim;
    public float velPow = 1f;
    public float randomPower = 3f;

    void OnEnable() {
        rb = GetComponent<Rigidbody>();

        if (!slaver) {
            ApplyTransform();
        }

        foreach(Transform each in transform) {
            var tmrb = each.GetComponent<TurbulentMotionRigidBody>();
            if(tmrb != null){
                tmrb.density = density;
                tmrb.linearFlow = linearFlow;
                tmrb.displacement = displacement;
                tmrb.coeffDisplacement = coeffDisplacement;
                tmrb.posWavePower = posWavePower;
                tmrb.velPow = velPow;
                tmrb.randomPower = randomPower;

                tmrb.UseTicker = UseTicker;
                tmrb.t = t;
                tmrb.tOn = tOn;
                tmrb.tOff = tOff;
                tmrb.dir = dir;
            }
            each.gameObject.SetActive(true);
        }
    }


    void FixedUpdate() {
        if (UseTicker) {
          if (!EvalTicker())
                return;
        }

        if (!slaver) {
            ApplyTransform();
        }
    }

    [Header("Ticker")]
    public bool UseTicker = false;
    public float t = 0;
    public float tOn = 0;
    public float tOff = 0;
    public int dir = -1;

    public bool EvalTicker() {

        //time moves inexorably forward regardless of our wants/needs
        t += Time.deltaTime;

        //ticker process
        if (dir > 0) {  //ticker is ON
            if (t > tOn) {
                dir *= -1;
                t = 0;
                if(rb) rb.drag = 100;
                return false;
            } else {
                return true;
            }
        } else { //ticker is OFF
            if (t > tOff) {
                dir *= -1;
                t = 0;
                if (rb) rb.drag = 0;
                return true;
            } else {
                return false;
            }
        }
    }

    void ApplyTransform(){
        // Noise position.
        var np = initialPosition * density + linearFlow * Time.time;

        // Offset for the noise position.
        offset = transform.position * posWavePower;
        var offs = offset;

        // Displacement.
        if (displacement != Vector3.zero){
            var npd = np * coeffDisplacement;

            // Get noise values.
            var vd = new Vector3(
                displacement.x == 0.0f ? 0.0f : displacement.x * Reaktion.Perlin.Noise(npd),
                displacement.y == 0.0f ? 0.0f : displacement.y * Reaktion.Perlin.Noise(npd + offs),
                displacement.z == 0.0f ? 0.0f : displacement.z * Reaktion.Perlin.Noise(npd + offs * 2)
            );

            rb.AddForce(vd * velPow);
            rb.AddForce(Random.insideUnitSphere * randomPower);
            //transform.forward = Vector3.Lerp(transform.forward, vd, Time.deltaTime * 20f);

            //anim.SetFloat("x", Mathf.Clamp(rb.velocity.normalized.x, -1, 1));
            //anim.SetFloat("y", Mathf.Clamp(rb.velocity.normalized.y, -1, 1));
            //anim.SetFloat("z", Mathf.Clamp(rb.velocity.normalized.z, -1, 1));
        }
    }
}