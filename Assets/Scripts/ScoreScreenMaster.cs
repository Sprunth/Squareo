using System;
using UnityEngine;
using System.Collections;

public class ScoreScreenMaster : MonoBehaviour
{

    public UILabel GameModeLabel;
    public UILabel ScoreLabel;

	// Use this for initialization
	void Start ()
	{
	    switch (Globals.SelectedGameMode)
	    {
	        case Globals.GameMode.TimeTrial:
                GameModeLabel.text = "Game Mode: Time Trial";
	            break;
            case Globals.GameMode.ThirtySwipes:
	            GameModeLabel.text = "Game Mode: 30 Swipes";
	            break;
            case Globals.GameMode.Zen:
	            GameModeLabel.text = "Game Mode: Zen";
	            break;
	    }

	    ScoreLabel.text = Globals.Score.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
