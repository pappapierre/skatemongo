using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetSkyBox : MonoBehaviour {

    public Material SkyboxMat;
    public float ambientIntensity = 1;

    void Start () {
        if (SkyboxMat != null && RenderSettings.skybox != SkyboxMat) {
            RenderSettings.skybox = SkyboxMat;
        }
        RenderSettings.ambientIntensity = ambientIntensity;
        DynamicGI.UpdateEnvironment();
    }
}
