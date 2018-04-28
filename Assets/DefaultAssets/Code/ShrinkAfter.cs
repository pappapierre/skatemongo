using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAfter : MonoBehaviour {

    public float lifespan = 0;
    public float shrinkSpeed = 0.001f;
    public float deathScale = 0.02f;

    public float growScale = 0.2f;
    public float initScale;
    bool hasGrown = false;

    // Update is called once per frame
    void FixedUpdate() {
        if (!hasGrown && transform.localScale.x < initScale) {
            transform.localScale += Vector3.one * growScale;
            return;
        }

        hasGrown = true;

        if (lifespan < 2) {
            transform.localScale *= 1 - shrinkSpeed;
        } else {
        }
    }

    private void Update() {
        if (transform.localScale.magnitude < deathScale)
            Destroy(gameObject);

        lifespan -= Time.deltaTime;
    }
}
