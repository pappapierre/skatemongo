/// <summary>
/// --- Component Name ---
/// Auto_Transform.cs
/// 
/// --- Reasoning ---
/// Automate generic translations like moving, rotating and scaling over time.
/// Use "modules" to seperate the 3 translation options, move, rotate and scale;
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Auto_Transform : MonoBehaviour {
	#region init vars
	[Header("Positional")]
	public bool doPosition = false;
	public bool PosWorldSpace = false;
	public Vector3 movePerSecond;
	public bool useCurveForPosition = false;
	public bool ResetAtEndOfCurvePosition = false; 
	public float timeSpanForPosition = 10f;
	public AnimationCurve powerOverLifetimePosition;
	public bool loopPosition = true;

	[Header("Rotational")]
	public bool doRotation = false;
	public bool RotWorldSpace = false;
	public Vector3 rotatePerSecond;
	public bool useCurveForRotation = false;
	public bool ResetAtEndOfCurveRotation = false;
	public float timeSpanForRotation = 10f;
	public AnimationCurve powerOverLifetimeRotation;
	public bool loopRotation = true;

	[Header("Scaling")]
	public bool doScale = false;
	public Vector3 scalePerSecond;
	public bool ResetAtEndOfCurveScale = false; 
	public bool useCurveForScale = false;
	public float timeSpanForScale = 10f;
	public AnimationCurve powerOverLifetimeScale;
	public bool loopScale = true;

	[HideInInspector]
	public Transform initParent;
	[HideInInspector]
	public Vector3 InitPosition;
	[HideInInspector]
	public Vector3 InitLocalPosition;
	[HideInInspector]
	public Quaternion initRot;
	[HideInInspector]
	public Quaternion initLocalRot;
	[HideInInspector]
	public Vector3 initScale;
	[HideInInspector]
	public float initScaleMagnitude;

	bool hasInit = false;
	[Header("TimeActive")]
	public float t = 0;

	[Header("Curve Evals")]
	public float posEval = 1;
	public float rotEval = 1;
	public float scaleEval = 1;

	#endregion //init vars
	//=======================================================================================================================//

	//keep unity methods in one place, and follow up with unique methods
	#region unity methods
	void Start () {
		Init ();
	}

	void Update () {
		Tick ();
		UpdateTransform ();
	}
	#endregion //unity methods
	//=======================================================================================================================//

	#region auto_transform methods
	//initialiaze all vars at first run
	public void Init(){
		//ONLY init if necessary.
		if (hasInit)
			return;
		
		//cache this transform for ease of use:
		var me = transform;

		//Set initial properties;
		initParent = me;

		InitPosition = me.position;
		InitLocalPosition = me.localPosition;

		initRot = me.rotation;
		initLocalRot = me.localRotation;

		initScale = me.localScale;
		initScaleMagnitude = me.localScale.magnitude;

		//Declare init set
		hasInit = true;
	}

	void Tick (){
		t += Time.deltaTime;
	}

	public void UpdateTransform(){
		//cache this transform for ease of use:
		var me = transform;

		DoPositionNow 	(me, doPosition);
		DoRotationNow 	(me, doRotation);
		DoScaleNow 		(me, doScale);
	}
	//=======================================================================================================================//

	#region "moduleSpecific" methods
	//Positioning methods
	float pe; //position evalSecondary
	float pp; //position phase
	public void DoPositionNow(Transform tgt, bool Bool){
		if (!Bool)
			return;

		if (useCurveForPosition) {
			if (loopPosition) {
				pe = (t % timeSpanForPosition)/timeSpanForPosition;
			}else{
				pe = Mathf.Clamp(t/timeSpanForPosition, 0, 1);
			}
			posEval = powerOverLifetimePosition.Evaluate (pe);
		}

		if (PosWorldSpace) {
			tgt.position += movePerSecond * Time.deltaTime * posEval;
		} else {
			tgt.localPosition += movePerSecond * Time.deltaTime * posEval;
		}
		if (ResetAtEndOfCurvePosition) {
			if (Mathf.Round((t % timeSpanForPosition) * 10)/10 == 0) {
				if (PosWorldSpace) {
					tgt.position = InitPosition;
				} else {
					tgt.localPosition = InitLocalPosition;
				}
			}
		}
	}

	//Rotation methods
	public void DoRotationNow(Transform tgt, bool Bool){
		if (!Bool)
			return;

		if (useCurveForRotation) {
			if (loopRotation) {
				pe = (t % timeSpanForPosition)/timeSpanForPosition;
			}else{
				pe = Mathf.Clamp(t/timeSpanForPosition, 0, 1);
			}
			rotEval = powerOverLifetimeRotation.Evaluate ((t % timeSpanForRotation) /timeSpanForRotation);
		}

		if (RotWorldSpace) {
			tgt.rotation = Quaternion.Euler( tgt.rotation.eulerAngles + rotatePerSecond * Time.deltaTime * rotEval);
		} else {
			tgt.localRotation = Quaternion.Euler( tgt.localRotation.eulerAngles + rotatePerSecond * Time.deltaTime * rotEval);
		}
		if (ResetAtEndOfCurveRotation) {
			if (Mathf.Round((t % timeSpanForRotation) * 10)/10 == 0) {
				if (RotWorldSpace) {
					tgt.rotation = initRot;
				} else {
					tgt.localRotation = initLocalRot;
				}
			}
		}
	}

	//Scaling methods
	public void DoScaleNow(Transform tgt, bool Bool){
		if (!Bool)
			return;

		if (useCurveForScale) {
			if (loopScale) {
				pe = (t % timeSpanForScale)/timeSpanForScale;
			}else{
				pe = Mathf.Clamp(t/timeSpanForScale, 0, 1);
			}
			scaleEval = powerOverLifetimeScale.Evaluate ((t % timeSpanForScale) /timeSpanForScale);
		}

		tgt.localScale += scalePerSecond * Time.deltaTime * scaleEval;
		if (ResetAtEndOfCurveScale) {
			if (Mathf.Round((t % timeSpanForScale) * 10)/10 == 0) {
				tgt.localScale = initScale;
			}
		}
	}
	#endregion //"moduleSpecific" methods
	//=======================================================================================================================//
	#endregion //auto_transform methods
}

//EDITOR EXTENSION for this component type
#if UNITY_EDITOR
[CustomEditor(typeof(Auto_Transform))]
[CanEditMultipleObjects]
public class Auto_Transform_Editor : Editor{
	string property;
	string Label;

	public override void OnInspectorGUI(){
		var serializedObject = new SerializedObject (targets);
		serializedObject.Update ();

		//GENERAL VALUES FOR ALL ELEMENTS
		EditorStyles.label.wordWrap = true;

		//START LAYOUT
		Label = "Do Position";
		property = "doPosition";
		EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));
		if(serializedObject.FindProperty (property).boolValue) {

			Label = "per second translate";
			property = "movePerSecond"; 
			EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

			Label = "world space";
			property = "PosWorldSpace"; 
			EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

			Label = "use curve";
			property = "useCurveForPosition"; 
			EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

			Label = "use curve";
			property = "useCurveForPosition"; 
			if(serializedObject.FindProperty (property).boolValue) {

				Label = "looping";
				property = "loopPosition"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "reset after cycle";
				property = "ResetAtEndOfCurvePosition"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "base time";
				property = "timeSpanForPosition"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "curve over basetime";
				property = "powerOverLifetimePosition"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));
			}
		}

		Label = "Do Rotation";
		property = "doRotation"; 
		EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));
		if(serializedObject.FindProperty (property).boolValue) {
			Label = "per second rotate";
			property = "rotatePerSecond"; 
			EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

			Label = "world space";
			property = "RotWorldSpace"; 
			EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

			Label = "use curve";
			property = "useCurveForRotation"; 
			EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

			Label = "use curve";
			property = "useCurveForRotation"; 
			if(serializedObject.FindProperty (property).boolValue) {

				Label = "looping";
				property = "loopRotation"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "reset after cycle";
				property = "ResetAtEndOfCurveRotation"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "base time";
				property = "timeSpanForRotation"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "curve over basetime";
				property = "powerOverLifetimeRotation"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));
			}
		}

		Label = "Do Scale";
		property = "doScale"; 
		EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));
		if(serializedObject.FindProperty (property).boolValue) {
			Label = "per second rotate";
			property = "scalePerSecond"; 
			EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

			Label = "use curve";
			property = "useCurveForScale"; 
			EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

			Label = "use curve";
			property = "useCurveForScale"; 
			if(serializedObject.FindProperty (property).boolValue) {

				Label = "looping";
				property = "loopScale"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "reset after cycle";
				property = "ResetAtEndOfCurveScale"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "base time";
				property = "timeSpanForScale"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));

				Label = "curve over basetime";
				property = "powerOverLifetimeScale"; 
				EditorGUILayout.PropertyField (serializedObject.FindProperty (property), new GUIContent (Label));
			}
		}

		//END LAYOUT
		serializedObject.ApplyModifiedProperties();
	}
}
#endif