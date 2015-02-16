using System.Net;
using UnityEngine;
using System.Collections;

public class MainMenuMaster : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
        
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public GUISkin GuiSkin;
    void OnGUI()
    {
        GUI.skin = GuiSkin;



    }

    public void OnTimeTrialClick()
    {
        Globals.SelectedGameMode = Globals.GameMode.TimeTrial;
        SwitchToGame();
    }

    public void OnZenClick()
    {
        Globals.SelectedGameMode = Globals.GameMode.Zen;
        SwitchToGame();
    }

    void SwitchToGame()
    {
        Application.LoadLevel("GameScreen");
    }
}
