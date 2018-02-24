using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour {

	// Use this for initialization
	public Vector3 targetPos;
	RectTransform rect;
	AudioSource a;
	void Start () {
		rect = GetComponent<RectTransform> ();
		a = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(rect.localPosition,targetPos)>1f)
			rect.localPosition = Vector3.Lerp (rect.localPosition, targetPos, 0.1f);
	}
	public void PlaySound(){
		a.Play ();
	}
}
