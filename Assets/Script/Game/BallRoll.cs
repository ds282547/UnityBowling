using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameStatus {
	waitingStart,posSelect,dirSelect,drop
}
public class ScoringFrame {
	public  int[] scores;
	int  shotCount;
	public Text [] texts;
	public Text  textTotal;
	public bool isLastFrame = false;

	public bool hasScore = false;

	public ScoringFrame(bool lastFrame){
		isLastFrame = lastFrame;
		shotCount = isLastFrame ? 3 : 2;
		scores = new  int [shotCount];
		for (int i = 0; i < shotCount; ++i)
			scores [i] = -1;

		texts = new Text [shotCount];
	}
	public void scoring( int shot, int val,bool secondShot){

		if (secondShot)
			val -= scores [shot - 1];
		scores [shot] = val;

		string tx = val.ToString ();
		if (val == 10) {
			tx = "X";
		} else if (shot > 0) {
			if (scores [shot - 1] + val == 10) {
				tx = "/";
			} 
		}
		
		texts [shot].text = tx;

	}
	public void showScore( int totalScore){
		textTotal.text = totalScore.ToString ();
	}
	public bool firstShotStrike(){
		return scores [0] ==  10;
	}
	public bool secondShotStrike(){
		return scores [1] ==  10;
	}
	
}


public class BallRoll : MonoBehaviour {
	//for ball
	public Rigidbody rg;
	public Vector3 ball_initial_position;
	public Quaternion ball_initial_rotation;
	// Use this for initialization
	public float spd;
	public Vector3 tor;
	public GameObject dropDir;



	//pos selector
	public float posSelectMoveSpd = 0.1f;
	public Vector3 posSelect_v;
	public float posSelect_z_UBound;
	public float posSelect_z_LBound;
	public float posSelect_dis = 3.6f;

	//dir selector
	public Vector3 dirSelect_rv;
	public float dirSelect_r;
	public Quaternion dirSelect_initial_rotation;


	//Pin Bulider
	public PinBulider pinBulider;

	//audio sound
	AudioSource [] audioSources;

	int isRollingOnAlley = 0;
	bool isRollingOnGutter = false;
	bool buttonPress = false;
	//for clear pin
	bool hitFirstPin = false;
	public Animator doorAnimator;

	public Animator TextAnimator;

	// game & scoring
	public GameStatus status;
	int cell;
	int shot;
	int totalScore;
	public static  int cellCount = 3;
	ScoringFrame [] scoreframes;
	public ScoreBoardManager scoreBoard;

	public Button buttonConfirm;

	//pin hit
	public Image [] marks;

	Camera [] cameras;
	Camera currentCamera;
	Camera mainCamera;
	Camera followerCamera;
	Camera lookerCamera;
	Vector3 followerCameraPos;
	Vector3 lookerCameraPos;

	//panel score
	public CanvasGroup panelScore;

	void Awake(){
		if (PlayerPrefs.HasKey ("CellCount")) {
			cellCount = PlayerPrefs.GetInt ("CellCount");
		} else {
			PlayerPrefs.SetInt ("CellCount",cellCount);
		}
	}
	public void NextStep(){
		buttonPress = true;
	}

	public void InitGameScoring(){
		status = GameStatus.waitingStart;
		cell = 0;
		shot = 0;
		scoreframes = new ScoringFrame[cellCount];
		for (int i = 0; i < cellCount; ++i) {
			scoreframes [i] = new ScoringFrame (false);
		}
		scoreframes [cellCount-1] = new ScoringFrame (true);


		LinkScoreFrameToTextField ();
	}
	public void InitPinHitMark(){
		marks = new Image[10];
		for(int i=0;i<10;++i){
			marks [i] =	GameObject.Find ("Mark" + i).GetComponent<Image> ();
		}
	}
	public void ResetPinHitMark(){
		for(int i=0;i<10;++i){
			marks [i].color = Color.white;
		}
	}
	public void CalculateScore(){
		totalScore = 0;
		for (int i = 0; i <= cell; ++i) {
			ScoringFrame frame = scoreframes [i];
			if (frame.hasScore)
				continue;
			if (i < cellCount - 1) {
				if (frame.scores [0] == 10) {
					//current frame strike
					if (scoreframes [i + 1].scores [0] == -1)
						return;
					else if (scoreframes [i + 1].scores [0] == 10) {
						//next frame is strike
						if (i + 1 == cellCount - 1) {
							//next frame is last frame
							if (scoreframes [i + 1].scores [1] == -1)
								return;
							else
								totalScore += 20 + scoreframes [i + 1].scores [1];
						} else {
							//next frame is not last frame
							if (scoreframes [i + 2].scores [0] == -1)
								return;
							else
								totalScore += 20 + scoreframes [i + 2].scores [0];
						}

					} else if (scoreframes [i + 1].scores [0] > -1 && scoreframes [i + 1].scores [1] > -1) {
						//next frame is not strike but has scores
						totalScore += 10 + scoreframes [i + 1].scores [0] + scoreframes [i + 1].scores [1];
					} else {
						return;
					}
					frame.showScore (totalScore);
				} else if (frame.scores [1] > -1 && frame.scores [0] + frame.scores [1] == 10) {
					//current frame spare
					if (scoreframes [i + 1].scores [0] == -1)
						return;
					else
						totalScore += 10 + scoreframes [i + 1].scores [0];
					frame.showScore (totalScore);
				} else if (frame.scores [1] > -1) {
					totalScore += frame.scores [0] + frame.scores [1];
					frame.showScore (totalScore);
				} else
					return;

			} else {
				if (frame.scores [1]>-1 && frame.scores [0] + frame.scores [1]<10) {
					totalScore += frame.scores [0] + frame.scores [1];
					frame.showScore (totalScore);
				} else if(frame.scores [2]>-1) {
					totalScore += frame.scores [0] + frame.scores [1] + frame.scores [2];
					frame.showScore (totalScore);
				}
			}

		}

	}
	public void LinkScoreFrameToTextField(){
		for (int i = 0; i < cellCount; ++i) {
			scoreframes [i].texts [0] = scoreBoard.score1Text[i];
			scoreframes [i].texts [1]= scoreBoard.score2Text[i];
			scoreframes [i].textTotal = scoreBoard.scoreTotalText[i];
		}
		scoreframes [cellCount - 1].texts [2] = scoreBoard.score3Text;
	}
	IEnumerator WaitSecsToStart(){
		
		yield return new WaitForSeconds (3.6f);
		PosSelect ();


	}
	void Start () {
		// init audio sound
		audioSources = GetComponents<AudioSource> ();
		//init rigidbody of ball
		rg = GetComponent<Rigidbody> ();
		rg.useGravity = false;
		rg.isKinematic = true;

		ball_initial_position = this.gameObject.transform.position;
		ball_initial_rotation = this.gameObject.transform.rotation;


		//init pos selector
		posSelect_z_UBound = this.gameObject.transform.position.z + posSelect_dis;
		posSelect_z_LBound = this.gameObject.transform.position.z - posSelect_dis;
		posSelect_v = new Vector3 (0,0, posSelectMoveSpd);

		//init dir selector
		dirSelect_rv = new Vector3(0,2,0);
		dirSelect_r = 0f;
		dirSelect_initial_rotation = dropDir.transform.rotation;
		//init game scoring
		InitGameScoring();
		//init pinhit mark
		InitPinHitMark();

		//disable button
		buttonConfirm.gameObject.SetActive (false);

		//wait for 3 sec to start
		StartCoroutine(WaitSecsToStart());
		// init camera
		cameras = Camera.allCameras;
		foreach (Camera c in Camera.allCameras) {
			if (c.name != "Main Camera") {
				c.enabled = false;
			}
		}


		mainCamera = currentCamera = Camera.main;
		followerCamera = FindCamera("Camera1");
		followerCameraPos = followerCamera.gameObject.transform.position;
		lookerCamera = FindCamera("Camera2");
		lookerCameraPos = lookerCamera .gameObject.transform.position;
	}
	
	Camera FindCamera(string n="Main Camera"){
		return GameObject.Find (n).GetComponent<Camera> ();
	}
	void changeCamera(Camera c){
		
		currentCamera.enabled = false;
		currentCamera = c;
		currentCamera.enabled = true;

	}
	
	// Update is called once per frame
	void Update () {
		if(status == GameStatus.posSelect){
			
			float ballz = this.gameObject.transform.position.z;
			if (ballz > posSelect_z_UBound || ballz < posSelect_z_LBound)
				posSelect_v.z = -posSelect_v.z;
			
			this.gameObject.transform.Translate (posSelect_v);
			dropDir.transform.position = this.gameObject.transform.position;

			if (buttonPress) {
				buttonPress = false;
				//left click
				DirSelect();
			}
				
		}else if(status == GameStatus.dirSelect){
			
			if (dirSelect_r < -45 || dirSelect_r > 45)
				dirSelect_rv.y = -dirSelect_rv.y;

			dirSelect_r += dirSelect_rv.y;
			dropDir.transform.Rotate (dirSelect_rv);
			if (buttonPress) {
				buttonPress = false;
				DropBall();
			}
		}

		//for rolling sound
		if (!hitFirstPin && transform.localPosition.x < -65f) {
			hitFirstPin = true;
			StartCoroutine (PrepareToClear ());
		}
		//for rolling sound
		if(isRollingOnAlley==1 && transform.localPosition.x < -71f){
			audioSources[0].Stop ();
			isRollingOnAlley = 2;
		}

		//for mark
		if (transform.localPosition.x < -65f) {
			for (int i = 0; i < 10; i++) {
				if (pinBulider.pins [i].CheckFall ()) {
					marks [i].color = new Color (0.2f, 0.2f, 0.2f);
				}
			}
		}
		//camera
		//follower
		if (currentCamera.Equals(followerCamera) && transform.localPosition.x > -53f) {
			Vector3 pos = followerCameraPos;
			pos.x += transform.position.x - ball_initial_position.x;
			currentCamera.gameObject.transform.position = pos;
		}
		//looker
		if (currentCamera.Equals(lookerCamera) && transform.localPosition.x > -60f) {
			print ("looking");
			Vector3 d = transform.position - lookerCamera.gameObject.transform.position;
			d.Scale (new Vector3 (0.01f, 0.01f, 0.01f));
			lookerCamera.gameObject.transform.position += d ;
			lookerCamera.gameObject.transform.LookAt(transform.position);

		}
			
		if (Input.GetKey("escape"))
			Application.Quit();
	}
	public void PosSelect(){
		buttonConfirm.gameObject.SetActive (true);
		status = GameStatus.posSelect;
	}
	public void DirSelect(){
		status = GameStatus.dirSelect;
		// init
		dirSelect_rv = new Vector3(0,2,0);
		dirSelect_r = 0f;
	}
	public void DropBall() {
		status = GameStatus.drop;
		// Move out drop dir
		dropDir.GetComponent<MeshRenderer>().enabled = false;
		// hide button
		buttonConfirm.gameObject.SetActive (false);

		rg.useGravity = true;
		rg.isKinematic = false;

		float rad = dirSelect_r * Mathf.PI / 180f * 0.2f;
		Vector3 _spd = new Vector3 (spd * Mathf.Cos (rad), 0, -spd * Mathf.Sin (rad));
		rg.AddForce (_spd,ForceMode.Impulse);
		rg.AddTorque (tor,ForceMode.Force);
		//change camera

		changeCamera(cameras[Random.Range(0,cameras.Length)]);
	}
		
	public void ResetCamera(){
		//reset camera
		followerCamera.gameObject.transform.position = followerCameraPos;
		lookerCamera.gameObject.transform.position = lookerCameraPos;
		changeCamera (mainCamera );
	}
	public void Reset(){
		//ball
		rg.velocity = Vector3.zero;
		this.gameObject.transform.position = ball_initial_position;
		this.gameObject.transform.rotation = ball_initial_rotation;
		rg.velocity = Vector3.zero;
		rg.angularVelocity = Vector3.zero;
		rg.useGravity = false;
		rg.isKinematic = true;
		//dir selector
		dropDir.GetComponent<MeshRenderer>().enabled = true;
		dropDir.transform.rotation = dirSelect_initial_rotation ;

		hitFirstPin = false;

		isRollingOnAlley = 0;
		isRollingOnGutter = false;

		ResetCamera ();
	}
	void OnCollisionEnter (Collision collision) {
		
		if (collision.gameObject.name == "Alley" || collision.gameObject.tag == "Edge" ) {
			if (isRollingOnAlley==0) {
				print ("Start Rolling");
				audioSources[0].Play ();
				isRollingOnAlley = 1;
			}
		} 
		if (collision.gameObject.tag == "Edge"){
			if (!isRollingOnGutter) {
				isRollingOnGutter = true;
				audioSources[1].Play ();
			}
		}
		if (collision.gameObject.tag == "Wall"){
			audioSources[2].Play ();
		}
			
	}

	IEnumerator PrepareToClear(){
		print ("Prepare");
		yield return new WaitForSeconds (4);
		print ("Clear");
		doorAnimator.SetTrigger ("CloseOpen");
	}
	void MoveScoreBoard (){
		if(cell>1){
			scoreBoard.moveTo ( (cell-1));
		}
	}
	public void ResetPinAndMark(){
		pinBulider.ResetAllPins ();
		ResetPinHitMark ();
	}
	public void SetNextShot(){
		 int score = pinBulider.ClearFallPins ();

		print ("Shot=" + shot.ToString () + " Cell=" + cell.ToString ());
		if (cell < cellCount-1) {
			//not last cell
			scoreframes [cell].scoring (shot, score, shot==1);
			if (shot==0 && score == 10) {
				ResetPinAndMark();

				TextAnimator.SetTrigger ("Strike");
				shot = 0;
				++cell;
				MoveScoreBoard ();
			} else {
				if (shot==1 && score == 10)
					TextAnimator.SetTrigger ("Spare");
				if (++shot > 1) {
					shot = 0;
					ResetPinAndMark();
					++cell;
					MoveScoreBoard ();
				}

			}

		} else {
			switch (shot) {
			case 0:
				scoreframes [cell].scoring (shot, score, false);
				if (score == 10) { // [X][ ][ ]
					ResetPinAndMark();
					TextAnimator.SetTrigger ("Strike");
				}
				break;
			case 1:
				if (scoreframes [cell].firstShotStrike ()) {
					scoreframes [cell].scoring (shot, score, false);
					if(score==10) // [X][X][ ]
						ResetPinAndMark();
				} else {
					scoreframes [cell].scoring (shot, score, true);
					if (score == 10) {
						// second shot spare
						// [9][/][ ]
						ResetPinAndMark();
					} else {
						//[6][3][ ]
						EndGame ();
						return;
					}
				}
				break;
			case 2:
				if (scoreframes [cell].secondShotStrike () || scoreframes [cell].scores[0]+scoreframes [cell].scores[1]==10) {
					// [?][X][ ]
					scoreframes [cell].scoring (shot, score, false);
				} else {
					// [?][4][ ]
					scoreframes [cell].scoring (shot, score, true);
				}
				EndGame ();
				return;
			}
			shot++;
		}
		CalculateScore ();
		Reset ();
		PosSelect ();
	}
	public void EndGame(){
		ResetPinAndMark ();
		CalculateScore ();
		GameObject.Find ("TextScore").GetComponent<Text> ().text = totalScore.ToString ();
		ResetCamera ();
		StartCoroutine (PanelScoreFadeIn ());
	}

	IEnumerator PanelScoreFadeIn(){
		while(panelScore.alpha<1f){
			panelScore.alpha = Mathf.Clamp01 (panelScore.alpha+Time.deltaTime*2);
			yield return null;
		}
		panelScore.interactable = true;

		yield return null;
	}
	public void ReturnMenu(){
		StartCoroutine (ChangeLevel ());
	}
	IEnumerator ChangeLevel(){
		float fadeTime = GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("Title");
	}

}
