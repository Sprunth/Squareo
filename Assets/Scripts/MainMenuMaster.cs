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
	void Update () {

	}

    public void OnTimerTrialClick()
    {
        Globals.SelectedGameMode = Globals.GameMode.TimeTrial;
    }

    public void OnZenClick()
    {
        Globals.SelectedGameMode = Globals.GameMode.Zen;
    }

    void SwitchToGame()
    {
        Application.LoadLevel("GameScreen");
    }
}
