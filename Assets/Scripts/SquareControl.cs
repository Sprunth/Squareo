﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SquareControl : MonoBehaviour
{
    private readonly static List<Color> colors = new List<Color>
    {
        new Color(255/255f,97/255f,56/255f), //red
        new Color(98/255f,197/255f,74/255f), // green
        new Color(33/255f,133/255f,197/255f), // blue
        new Color(255/255f,240/255f,165/255f), // tan
        new Color(255/255f,211/255f,78/255f), // yellow
    };

    private GameMaster _gameMaster;

    public int gridX, gridY;

    public Vector2 origPos, newTarget;
    public bool movingToNewPosition = false;
    public float movementProgress = 0;

    public Color Color { get; private set; }

	// Use this for initialization
    private void Start()
    {
        _gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
	    GetComponent<SpriteRenderer>().color = Color = colors[Random.Range(0, colors.Count)];

        //gameObject.animation.enabled = false;
        //animation.Stop("SelectSquare");
        //animation.enabled = false;
        
    }
	
	// Update is called once per frame
	void Update () {

	    if (movingToNewPosition)
	    {
	        gameObject.transform.position = Vector2.Lerp(origPos, newTarget, movementProgress);
	        movementProgress += 0.02f;
	        if (movementProgress > 1)
	        {
                movementProgress = 0;
	            movingToNewPosition = false;
	            origPos = newTarget = gameObject.transform.position;
	        }
	    }
	    
	}


    public void OnSelectAnimationEnd()
    {
        // Todo: cache this?
        GetComponent<Animator>().SetBool("Selected", false);
    }

    public void OnDeathAnimationEnd()
    {
        DestroyObject(gameObject);
    }

    

}
