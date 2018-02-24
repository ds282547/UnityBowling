using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	public CanvasGroup panelIntro;
	public CanvasGroup panelConfig;
	public AudioClip scrollSound;
	AudioSource audioSource;
	public RectTransform scrollViewContent;

	public Text cellCountText;
	public Slider cellCountSlider;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		int width = 562;
		int height = 1000;
		int cellCount = 3;
		if (!PlayerPrefs.HasKey ("ResHeight")) {
			PlayerPrefs.SetInt ("ResHeight", height );
		} else {
			height = PlayerPrefs.GetInt ("ResHeight");
		}
		if (!PlayerPrefs.HasKey ("ResWdith")) {
			PlayerPrefs.SetInt ("ResWdith",  width);
		} else {
			width = PlayerPrefs.GetInt ("ResWdith");
		}
		if (height < 500) {
			width = 562;
			height = 1000;
			PlayerPrefs.SetInt ("ResHeight", height );
			PlayerPrefs.SetInt ("ResWdith",  width);
		}

		cellCountText = GameObject.Find ("TextCellCount").GetComponent<Text>();
		cellCountSlider = GameObject.Find ("CellCountSlider").GetComponent<Slider>();

		if (!PlayerPrefs.HasKey ("CellCount")) {
			PlayerPrefs.SetInt ("CellCount", cellCount);
		} else {
			cellCount = PlayerPrefs.GetInt ("CellCount");
		}

		cellCountSlider.value = cellCount;
		cellCountText.text = cellCount.ToString ();

		Screen.SetResolution (width, height, false);
		GameObject.Find ("InputFieldHeight").GetComponent<InputField> ().text = Screen.height.ToString();
		GameObject.Find ("InputFieldWidth").GetComponent<InputField> ().text = Screen.width.ToString();


	}
	public void cellCountSliderChange(){
		cellCountText.text = cellCountSlider.value.ToString();

	}
	

	public void ShowPanelIntro(){
		scrollViewContent.localPosition = Vector3.zero;
		panelIntro.blocksRaycasts = true;
		StartCoroutine (PanelFadeIn (panelIntro));

		audioSource.clip = scrollSound;
		audioSource.Play ();
	}
	public void ShowPanelConfig(){
		panelConfig.blocksRaycasts = true;
		StartCoroutine (PanelFadeIn (panelConfig));

		audioSource.clip = scrollSound;
		audioSource.Play ();
	}
	IEnumerator PanelFadeIn(CanvasGroup panel){
		while(panel.alpha<1f){
			panel.alpha = Mathf.Clamp01 (panel.alpha+Time.deltaTime*2);
			yield return null;
		}
		panel.interactable = true;

		yield return null;
	}

	public void SetResolutionWidth(){
		int width, height;

		width = int.Parse(GameObject.Find("InputFieldWidth").GetComponent<InputField>().text);
		height = (int)((double)width * 16 / 9);
		GameObject.Find ("InputFieldHeight").GetComponent<InputField> ().text = height.ToString ();
		PlayerPrefs.SetInt ("ResHeight", height);
		PlayerPrefs.SetInt ("ResWdith", width);
		Screen.SetResolution (width, height, GameObject.Find ("Toggle").GetComponent<Toggle> ().isOn);
	}
	public void SetResolutionHeight(){
		int width, height;

		height = int.Parse(GameObject.Find("InputFieldHeight").GetComponent<InputField>().text);
		width = (int)((double)height * 9 / 16);
		GameObject.Find ("InputFieldWidth").GetComponent<InputField> ().text = width.ToString ();
		PlayerPrefs.SetInt ("ResHeight", height);
		PlayerPrefs.SetInt ("ResWdith", width);
		Screen.SetResolution (width, height, GameObject.Find ("Toggle").GetComponent<Toggle> ().isOn);

	}
	public void HidePanelIntro(){
		
		StartCoroutine (PanelFadeOut (panelIntro));
	}
	public void HidePanelConfig(){
		PlayerPrefs.SetInt ("CellCount", (int)cellCountSlider.value);
		StartCoroutine (PanelFadeOut (panelConfig));
	}
	IEnumerator PanelFadeOut(CanvasGroup panel){
		while(panel.alpha>0f){
			panel.alpha = Mathf.Clamp01 (panel.alpha-Time.deltaTime*2);
			yield return null;
		}
		panel.interactable = false;
		panel.blocksRaycasts = false;
		yield return null;
	}
	public void fullScreenChange(){
		Screen.SetResolution (Screen.width,Screen.height, GameObject.Find ("Toggle").GetComponent<Toggle> ().isOn);
	}
	public void LoadGame(){
		StartCoroutine (ChangeLevel ());
	}
	IEnumerator ChangeLevel(){
		float fadeTime = GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene ("GameScene");
	}
}
