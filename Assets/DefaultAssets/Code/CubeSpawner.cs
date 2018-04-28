using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour {

    public int currentCount;

    public GameObject CubePrefab;
    public GameObject spawnedObjs;
    public GameObject cubeInstance;

    public int depth = 1;
    public float t = 0;
    public float interval = 1;
    public float ClampMag;
    public float count = 2;
    public float mean = 0.5f;
    public float dev = 1f;
    public float scaleRange = 0.7f;
    public float normalizedV = 3;
    public float baseScale = 0.2f;
    public AnimationCurve curve;
    public float curvePeriod = 2f;
    public float time = 2f;

    public int numberOfChilds = 1;

    void Update () {
        time = curve.Evaluate(t % curvePeriod);
        for (int n = 0; n < time * count; n++) {
            MakeCube();
        }

        t += Time.deltaTime;
        currentCount = spawnedObjs.transform.childCount;
    }

    void MakeCube() {
        cubeInstance = Instantiate(CubePrefab, GaussVector3(transform.position, mean, dev), Quaternion.Euler(Random.insideUnitSphere * 360), spawnedObjs.transform);
        cubeInstance.transform.localScale = Vector3.one * (Mathf.Clamp(baseScale + Random.Range(-scaleRange, scaleRange), 0.01f, Mathf.Infinity) );
        Destroy(cubeInstance, 20f);
    }

    Vector3 GaussVector3(Vector3 origin, float mean, float dev) {
        var v = new Vector3(Gauss(mean, dev), Gauss(mean, dev), Gauss(mean, dev));
        v = origin + Vector3.Normalize(v) * normalizedV;
        return v;
    }

    float Gauss(float mean, float stdDev) {
        return mean + stdDev * Mathf.Sqrt(-2.0f * Mathf.Log(1.0f - Random.Range(0, 1f))) * Mathf.Sin(2.0f * Mathf.PI * (1.0f - Random.Range(0, 1f))); ;
    }
}
