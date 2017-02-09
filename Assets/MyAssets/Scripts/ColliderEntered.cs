﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ColliderEntered : MonoBehaviour {

	public string LevelName;
	public string DisplayedText;	// starts with "Press [E] "
	private bool insideTriggerSphere = false;
	private GUIStyle textStyle;
    private GameObject player;

	public void Start()
	{
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player")) {
			Debug.Log("Player entered");
			this.insideTriggerSphere = true;
            player = other.gameObject;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player")) {
			Debug.Log("Player exit");
			this.insideTriggerSphere = false;
            player = null;
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown (KeyCode.E) && this.insideTriggerSphere) {
            if(player != null && ! player.GetComponent<NetworkPlayerController>().isFocused())
            { 
			    Debug.Log(LevelName);
			    SceneManager.LoadScene(LevelName);
            }
        }
	}

	public void OnGUI()
	{
		if (this.insideTriggerSphere) {
			GUI.Label(new Rect(Screen.width / 2 - 200f, Screen.height / 2 - 100f, 400f, 200f), "Press [E] " + DisplayedText, this.textStyle);
		}
	}
}