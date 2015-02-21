using System.Net;
using UnityEngine;
using System.Collections;

public class MainMenuMaster : MonoBehaviour
{
    public ParticleSystem ParticleSystem;

	// Use this for initialization
	void Start ()
	{
        ParticleSystem.startColor = SquareControl.CubeColors[Random.Range(0, SquareControl.CubeColors.Count)];
	}
	

	// Update is called once per frame
	void Update ()
	{
	    ParticleSystem.startColor = SquareControl.CubeColors[Random.Range(0, SquareControl.CubeColors.Count)];
        
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
