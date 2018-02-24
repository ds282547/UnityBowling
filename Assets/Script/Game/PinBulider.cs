using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinBulider : MonoBehaviour {
	public GameObject pinPrefab;

	const float lineInterval = 5f;
	const float levelInterval = 4f;

	const int levelCount = 4;
	const int pinCount = 10;
	public Pin [] pins;
	// Use this for initialization
	void Start () {
		BuildPins();
	}
	public void BuildPins(){
		int i=0,level=0,counter=0;
		pins = new Pin[pinCount];
		while(level<levelCount){
			GameObject pin = Instantiate (pinPrefab);
			pin.name = "Pin"; 
			pin.transform.SetParent (transform);
			pin.transform.localPosition = new Vector3 (- level*levelInterval, 0.2f, i*lineInterval-level*lineInterval /2);

			pins [counter] = pin.GetComponent<Pin> ();
			pins [counter].no = (byte)counter;
			++counter;
			if (++i > level) {
				i = 0;
				++level;
			}
		}
	}
	public  int ClearFallPins(){
		//return number of fall pins
		 int fall = 0;
		List<Pin> notFall = new List<Pin> ();
		for (int i = 0; i < pinCount; ++i) {
			if (pins [i].CheckFall ()) {
				++fall;
				pins [i].Hide ();
			}else{
				notFall.Add (pins [i]);
			}
		}
		if (notFall.Count != pinCount) {
			foreach (Pin pin in notFall) {
				pin.Reset ();
			}
		}
		return fall;
	}
	public void ResetAllPins(){
		foreach (Pin pin in pins) {
			pin.Reset ();
		}
	}
}

