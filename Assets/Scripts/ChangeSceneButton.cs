using UnityEngine;
using System.Collections;

public class ChangeSceneButton : MonoBehaviour
{
    public string SceneName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick()
    {
        Application.LoadLevel(SceneName);
    }
}
