using UnityEngine;
using System.Collections;

public class SetYPosition : MonoBehaviour
{

    public float PixelsFromTop;

	// Use this for initialization
	void Start ()
	{
	    Debug.Log(Screen.height);
	    transform.position = new Vector3(transform.position.x, (Screen.height/2.0f) - PixelsFromTop);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
