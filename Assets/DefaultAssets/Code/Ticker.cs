using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ticker {
	public float tick = 0.1f;
    public float time = 0;
	public bool Tick(){
		if(time>=tick){
			time = 0;
			return true;
		}else{
			time += Time.deltaTime;
			return false;
		}
	}
}
