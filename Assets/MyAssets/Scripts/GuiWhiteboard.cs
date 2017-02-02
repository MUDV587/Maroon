﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GuiWhiteboard : MonoBehaviour 
{
	private WhiteboardController whiteboardController;
	private GUIStyle textStyle;
	private Vector2 scrollViewVector = Vector2.zero;
	private int selectedLectureIndex = 0;
	private string[] lectureNames;
	private bool showMenu = true;

	public void Start () 
	{
		// Define GUI style
		this.textStyle = new GUIStyle("label");
		this.textStyle.alignment = TextAnchor.MiddleCenter;

		// Find Whiteboard GameObject in the scene
		GameObject whiteboard = GameObject.FindGameObjectWithTag ("WhiteboardPlane");
		if (null == whiteboard) {
			throw new System.ArgumentNullException("No WhiteboardPlane GameObject found");
		}
		this.whiteboardController = whiteboard.GetComponent<WhiteboardController> ();
		if (null == this.whiteboardController) {
			throw new System.ArgumentNullException("No WhiteboardController script attached to WhiteboardPlane GameObject");
		}

		// Get Lecture names
		this.lectureNames = new string[this.whiteboardController.Lectures.Count];
		int i = 0;
		foreach (Lecture lecture in this.whiteboardController.Lectures) {
			this.lectureNames[i++] = lecture.Name;
		}
	}
	
	public void Update()
	{
		// Check if [ESC] was pressed
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
            SceneManager.LoadScene("Laboratory");
        }
		// Check if [<-] or [A] was pressed
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) 
		{
			// Move back in slide show
			if(null != this.whiteboardController) {
				this.whiteboardController.Previous();
			}

		}
		// Check if [->] or [D] was pressed
		if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
		{
			// Move forward in slide show
			if(null != this.whiteboardController) {
				this.whiteboardController.Next();
			}
		}
		// Check if [TAB] was pressed
		if (Input.GetKeyDown (KeyCode.Tab)) {
			// Show menu to select the lecture of interest
			this.showMenu = !this.showMenu;
		}
	}

	public void OnGUI()
	{
		// Show control messages on top left corner
		GUI.Label (new Rect (10f, 10f, 300f, 200f), string.Format("[ESC] - Leave\r\n[TAB] - {0} Lecture Menu\r\n[<-] or [A] - Back\r\n[->] or [D] - Forward", this.showMenu ? "Hide" : "Show"));
		// Show navigation information on the middle lower screen
		if (null != this.whiteboardController && 
		    null != this.whiteboardController.SelectedLecture &&
		    null != this.whiteboardController.SelectedLecture.WebContents) {
			GUI.Label (new Rect (Screen.width / 2f - 50f, Screen.height - 50f, 100f, 50f), string.Format ("{0}/{1}", this.whiteboardController.CurrentWebContentIndex + 1, this.whiteboardController.SelectedLecture.WebContents.Count), this.textStyle);
		}

		// Show lecture menu when activated
		if (this.showMenu) {
			// Begin the ScrollView
			this.scrollViewVector = GUI.BeginScrollView (new Rect (Screen.width / 2f - 225f, 50f, 450f, Screen.height - 125f), this.scrollViewVector, new Rect (0f, 0f, 430f, 2000f));

			// Put something inside the ScrollView
			GUILayout.BeginArea (new Rect (0, 0, 430f, 2000f));
			GUILayout.Box ("PLEASE SELECT A LECTURE");
			this.selectedLectureIndex = GUILayout.SelectionGrid (this.selectedLectureIndex, this.lectureNames, 1);
			GUILayout.EndArea ();

			// End the ScrollView
			GUI.EndScrollView ();
		}

		if (GUI.changed) {
			Debug.Log("Selected Lecture Index: " + this.selectedLectureIndex);
			if(null != this.whiteboardController) {
				this.showMenu = false;
				this.whiteboardController.SelectLecture(this.selectedLectureIndex);
				this.whiteboardController.Refresh();
			}
		}

	}
}
