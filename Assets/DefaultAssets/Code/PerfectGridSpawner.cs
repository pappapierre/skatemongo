using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectGridSpawner : MonoBehaviour {

    public bool refresh = false;
    public GameObject ObjectPrefab;
    public float spacing;
    public int xCount = 5;
    public int zCount = 5;

    float xRange = 0;
    float zRange = 0;
    float scaleRange = 0;
    float rotRange = 0f;

    bool doUp = false;
    bool doColor = false;
    Gradient grade;

    void Awake() {
        Pop();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            refresh = true;
        }

        if (refresh) {
            refresh = false;
            for (int n = transform.childCount - 1; n >= 0; n--) {
                Destroy(transform.GetChild(n).gameObject);
            }
            Pop();
        }
    }

    void Pop() {
        int i = 0;
        for (int x = 0; x <= xCount; x++) {
            for (int y = 0; y <= zCount; y++) {
                var tmp = Instantiate(ObjectPrefab, transform).transform;
                tmp.localPosition = new Vector3(x * spacing + Random.Range(-xRange, xRange), y * spacing + Random.Range(-zRange, zRange), 0);

                if (rotRange != 0) {
                    tmp.localRotation = Quaternion.Euler(tmp.localRotation.eulerAngles + Vector3.up * Random.Range(-rotRange, rotRange));
                }

                if (scaleRange != 0)
                    tmp.localScale *= 1 + Random.Range(-scaleRange, scaleRange);

                if (doUp)
                    tmp.up = transform.up;

                if (doColor)
                    tmp.GetComponentInChildren<Renderer>().material.color = grade.Evaluate(Random.Range(0, 1f));

                tmp.gameObject.SetActive(true);
                i++;
            }
        }
    }
}
