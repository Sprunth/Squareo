using UnityEngine;
using System.Collections;

public class MainMenuBackground : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    transform.Rotate(Vector3.forward, 1f);
        transform.Rotate(Vector3.up, 0.3f);
	}
}
