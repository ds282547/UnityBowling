using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardManager : MonoBehaviour {

	// Use this for initialization
	GameObject[] cellNos;
	GameObject[] score1s;
	GameObject[] score2s;
	GameObject score3;
	GameObject[] scoreTotals;


	public Text [] cellText;
	public Text [] score1Text;
	public Text [] score2Text;
	public Text score3Text;
	public Text [] scoreTotalText;




	int cellCount = BallRoll.cellCount;
	public GameObject scoreBoardCell;
	public GameObject scoreBoardCellEnd;
	public BallRoll ballRoll;
	public RectTransform scoreBoardContent;
	void Start () {
		if (PlayerPrefs.HasKey ("CellCount")) {
			cellCount = PlayerPrefs.GetInt ("CellCount");
		} 

		int i;
		Vector3 identity = new Vector3 (1f, 1f, 1f);
		GameObject cell;
		for (i = 0; i < cellCount-1; ++i) {
			cell = Instantiate (scoreBoardCell);
			cell.GetComponent<RectTransform> ().SetParent (scoreBoardContent);
			cell.GetComponent<RectTransform> ().localScale = identity;

		}
		cell = Instantiate (scoreBoardCellEnd);
		cell.GetComponent<RectTransform> ().SetParent (scoreBoardContent);
		cell.GetComponent<RectTransform> ().localScale = identity;


		cellNos = GameObject.FindGameObjectsWithTag ("CellNo");
		score1s = GameObject.FindGameObjectsWithTag ("Score1");
		score2s = GameObject.FindGameObjectsWithTag ("Score2");
		score3 = GameObject.Find ("Score3");
		scoreTotals = GameObject.FindGameObjectsWithTag ("ScoreTotal");

		cellText = new Text[cellNos.Length];
		score1Text = new Text[score1s.Length];
		score2Text = new Text[score2s.Length];
		scoreTotalText = new Text[scoreTotals.Length];

		score3Text = score3.GetComponent<Text> ();
		score3Text.text = "";

		for (i = 0; i < cellCount; ++i) {
			cellText [i] = cellNos [i].GetComponent<Text> ();
			score1Text [i] = score1s [i].GetComponent<Text> ();
			score2Text [i] = score2s  [i].GetComponent<Text> ();
			scoreTotalText [i] = scoreTotals [i].GetComponent<Text> ();

			cellText [i].text = (i + 1).ToString ();
			score1Text [i].text = "";
			score2Text [i].text = "";
			scoreTotalText [i].text = "";
		}
	}
	
	public void moveTo( int cell){
		
		StartCoroutine (moving (Mathf.Clamp(-70f * cell,-scoreBoardContent.sizeDelta.x,0f)));
	}
	IEnumerator moving(float target){
		Vector3 pos = scoreBoardContent.localPosition;
		float dis = pos.x - target;
		do{
			print((pos.x-target)/dis);
			pos.x -= 3f*(pos.x-target)/dis;
			scoreBoardContent.localPosition = pos;
			yield return null;
		}while(Mathf.Abs(pos.x-target)>0.5f);
		pos.x = target;
		scoreBoardContent.localPosition = pos;
		yield return null;
	}

}

