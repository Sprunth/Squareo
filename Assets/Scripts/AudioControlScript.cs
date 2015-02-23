using UnityEngine;
using System.Collections.Generic;

public class AudioControlScript : MonoBehaviour
{

    public List<GameObject> ContainsAudioSource;

	// Use this for initialization
	void Start () {
        HandleMute(PlayerPrefs.GetInt("Audio") == 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnClick()
    {
        if (PlayerPrefs.GetInt("Audio") == 1)
            HandleMute(true);
        else
            HandleMute(false);
    }

    void HandleMute(bool mute)
    {
        if (mute)
        {
            PlayerPrefs.SetInt("Audio", 0);
            GetComponentInChildren<UILabel>().text = "Audio: Off";
            ContainsAudioSource.ForEach(o => o.GetComponent<AudioSource>().mute = true);
        }
        else
        {
            PlayerPrefs.SetInt("Audio", 1);
            GetComponentInChildren<UILabel>().text = "Audio: On";
            ContainsAudioSource.ForEach(o => o.GetComponent<AudioSource>().mute = false);
        }
    }
}
