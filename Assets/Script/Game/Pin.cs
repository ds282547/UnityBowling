using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour {

	// Use this for initialization
	AudioSource audioSource;
	public AudioClip pinHit;
	public AudioClip pinHitOther;

	public Vector3 initialPosition;
	public Quaternion initialRotation;
	bool isFall;
	public byte no;

	Rigidbody _rigidbody;
	void Awake(){
		audioSource = GetComponent<AudioSource> ();
		_rigidbody = GetComponent<Rigidbody> ();

		isFall = false;
	}
	void Start(){
		initialPosition = transform.localPosition;
		initialRotation = transform.localRotation;
	}
	void OnCollisionEnter(Collision collision){
		string _name = collision.gameObject.name;

		if (_name == "Ball" || _name == "Pin") {
			audioSource.clip = pinHit;
			audioSource.pitch = Random.Range (0.9f, 1.2f);
			audioSource.volume = Mathf.Clamp01 (_rigidbody.velocity.magnitude / 10f);
			audioSource.Play ();
		} else if (collision.gameObject.CompareTag ("Wall")) {
			audioSource.clip = pinHitOther;
			audioSource.pitch = 1f;
			audioSource.volume = 1;
			audioSource.Play ();
		}
	}
	public bool CheckFall(){
		if(!isFall)
			isFall = Mathf.Abs (Quaternion.Angle (transform.localRotation, initialRotation)) > 50f || Vector3.Distance(transform.localPosition,initialPosition)>15f;
		return isFall;
	}
	public void Hide(){
		gameObject.SetActive (false);
	}
	public void Reset(){
		isFall = false;
		_rigidbody.velocity = Vector3.zero;
		transform.localPosition = initialPosition;
		transform.localRotation = initialRotation;
		gameObject.SetActive (true);
	}
}
	