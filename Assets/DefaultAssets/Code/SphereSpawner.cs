using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour {

    public GameObject Tentacle;
    public int totalCount = 100;
    public float sphereScale = 0.2f;
    public float scaleRange = 0.02f;

    public bool normalize = false;

    public bool refresh = false;

    void Start() {
        Pop();
    }

    private void Update() {
        if (refresh) {
            refresh = false;
            for (int n = transform.childCount - 1; n >= 0; n--) {
                Destroy(transform.GetChild(n).gameObject);
            }
            Pop();
        }
    }

    void Pop() {
        for (int i = 0; i<totalCount; i++) {
            var tmp = Instantiate(Tentacle, transform).transform;
            tmp.localPosition = Random.insideUnitSphere * sphereScale;

            if (normalize)
                tmp.localPosition = tmp.localPosition.normalized* sphereScale;

            tmp.up = tmp.localPosition;
            tmp.localScale *= 1 + Random.Range(-scaleRange, scaleRange);
            tmp.gameObject.SetActive(true);
        }
    }
}
