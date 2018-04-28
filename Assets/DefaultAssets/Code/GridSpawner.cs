using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour {
    
    public bool refresh = false;
    public GameObject Tentacle;
    public float gridSpace;
    public int gCount = 5;

    public int xCount = 5;
    public int zCount = 5;

    public float xRange = 0.2f;
    public float zRange = 0.2f;
    public float scaleRange = 0.02f;
    public float rotRange = 0f;

    public bool doUp = true;
    public bool doColor = false;
    public Gradient grade;

    void Awake () {
        Pop();
	}

    private void Update() {
        if (refresh) {
            refresh = false;
            for (int n = transform.childCount-1; n>=0; n--) {
                Destroy(transform.GetChild(n).gameObject);
            }
            Pop();
        }
    }

    void Pop() {
        for (int x = -xCount; x <= xCount; x++) {
            for (int y = -zCount; y <= zCount; y++) {
                var tmp = Instantiate(Tentacle, transform).transform;
                tmp.localPosition = new Vector3(x * gCount * gridSpace + Random.Range(-xRange, xRange), 0, y * gCount * gridSpace + Random.Range(-zRange, zRange));
                if (doUp)
                    tmp.up = transform.up;

                if(rotRange != 0) {
                    tmp.localRotation = Quaternion.Euler( tmp.localRotation.eulerAngles + Vector3.up * Random.Range(-rotRange, rotRange) );
                }

                if(doColor)
                tmp.GetComponentInChildren<Renderer>().material.color = grade.Evaluate(Random.Range(0, 1f));
                tmp.localScale *= 1 + Random.Range(-scaleRange, scaleRange);

                tmp.gameObject.SetActive(true);
            }
        }
    }
}