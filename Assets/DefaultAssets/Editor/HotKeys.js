import System.Collections.Generic;

@MenuItem ("Shortcuts/ResetTransform #%r")

static function ResetTransform () {
	for(var each : Transform in Selection.transforms){
    	Undo.RecordObject(each, each.name + " Reset" );
		each.localPosition = Vector3.zero;
		each.localRotation = Quaternion.identity;
//		each.localScale = Vector3.one;
		Debug.Log ("Reset");
	}
}
@MenuItem ("Shortcuts/HideUnhide #%H")
static function HideUnhideTransform () {
	for(var each : Transform in Selection.transforms){
		if(each.gameObject.activeSelf){
   			Undo.RecordObject(each, each.name + " Hidden" );
			each.gameObject.SetActive(false);
		}else{
   			Undo.RecordObject(each, each.name + " UnHidden" );
			each.gameObject.SetActive(true);
		}
	}
}

//@MenuItem ("Shortcuts/Count #&%Q")
static function Count () {
	var n : int = 0;
	for(var each : Transform in Selection.transforms){
		n += CountEm(each);
		n++;//
   	}
   	Debug.Log(n);
}

static function CountEm(t : Transform):int{
	return t.childCount;
}


//@MenuItem ("Shortcuts/Dropit &D")
static function Dropit(){
	var hit : RaycastHit[];
	for(var each : Transform in Selection.transforms){
		if(each.gameObject.activeSelf){
			hit = Physics.RaycastAll(each.position + Vector3.up*10, -Vector3.up);
			for(var eachH : RaycastHit in hit){
    			Undo.RecordObject(each, each.name + " Dropit" );
				if(eachH.collider.name == "Terrain"){
					each.position = eachH.point;
//					each.up = eachH.normal;
				}
			}
		}
	}
}

@MenuItem ("Shortcuts/RandomizeScale #&%E")
static function RandomizeScale(){
	for(var each : Transform in Selection.transforms){
		if(each.gameObject.activeSelf){
			Undo.RecordObject(each, each.name + " RandomizeScale" );
			each.localScale = each.localScale * Random.Range(0.9f,1.1f);
		}
	}
}

@MenuItem ("Shortcuts/RandomizeRotation #&%W")
static function RandomizeRotation(){
	for(var each : Transform in Selection.transforms){
		if(each.gameObject.activeSelf){
			Undo.RecordObject(each, each.name + " RandomizeScale" );
			each.localRotation *= Quaternion.Euler(0, Random.Range(-180f,180f), 0);
		}
	}
}

@MenuItem ("Shortcuts/StandardizeRotation #&%S")
static function StandardizeRotation(){
	for(var each : Transform in Selection.transforms){
		if(each.gameObject.activeSelf){
			Undo.RecordObject(each, each.name + " RandomizeScale" );
			each.localRotation = Quaternion.Euler(0, Random.Range(-360f,360f), 0);
		}
	}
}
