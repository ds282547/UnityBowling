using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartNewGame : MonoBehaviour {

	// Use this for initialization
	public void OnClick(){
		SceneManager.LoadScene ("GameScene");
	}

}
