using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;

public class GameSelectionButtonScript : MonoBehaviour
{

    public Globals.GameMode Mode;

	// Use this for initialization
	void Start () {
	
	}

    public void OnClick()
    {
        Globals.SelectedGameMode = Mode;
        Application.LoadLevel("GameScreen");
    }

	// Update is called once per frame
	void Update () {
	
	}
}
