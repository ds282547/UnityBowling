using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour {

	// Use this for initialization
	public BallRoll ballRoll;
	public void DoorClosed(){
		ballRoll.SetNextShot();
	}
	public void DoorSound(){
		GetComponent<AudioSource> ().Play ();
	}
}
