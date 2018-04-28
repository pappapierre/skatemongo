using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosBasedWave : MonoBehaviour {

    public bool slaver = false;

    public Vector3 initPos;
    public Vector3 initPosCoeffecient;

    public Vector3 period = Vector3.one;
    public Vector3 offset = Vector3.one;
    public Vector3 scale = Vector3.one;

    void Start () {
        initPos = transform.localPosition;
    }
	
	void Update () {

        if (slaver) {
            foreach (Transform each in transform) {
                var pos = each.GetComponent<PosBasedWave>();
                pos.initPosCoeffecient = initPosCoeffecient;
                pos.period = period;
                pos.offset = offset;
                pos.scale = scale;
            }
        }

        var x = Mathf.Sin(Time.timeSinceLevelLoad * period.x + offset.x + ((initPos.x * initPos.y * initPos.z) * initPosCoeffecient.x)) * scale.x * transform.localPosition.magnitude * 0.1f;
        var y = Mathf.Cos(Time.timeSinceLevelLoad * period.y + offset.y + ((initPos.x * initPos.y * initPos.z) * initPosCoeffecient.y)) * scale.y * transform.localPosition.magnitude * 0.1f;
        var z = Mathf.Cos(Time.timeSinceLevelLoad * period.z + offset.z + ((initPos.x * initPos.y * initPos.z) * initPosCoeffecient.y)) * scale.z * transform.localPosition.magnitude * 0.1f;
        transform.localPosition = initPos + new Vector3(x, y, z);
	}
}
