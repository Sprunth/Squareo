using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour
{

    public GameObject squarePrefab;

    public GameObject[,] squares;

    private LineConnector lineConnector;

    private int sizeX, sizeY;

    private bool _mouseDrag = false;

    private bool _readyForNextSelection = true;

    private readonly List<XYCoord> _deadSquareCoordsToReplace = new List<XYCoord>(); 

	// Use this for initialization
	void Start ()
	{
	    lineConnector = GameObject.FindGameObjectWithTag("LineConnector").GetComponent<LineConnector>();

	    sizeX = 11;
	    sizeY = 15;

        squares = new GameObject[sizeX, sizeY];

	    for (var x = 0; x < sizeX; x++)
	    {
	        for (var y = 0; y < sizeY; y++)
	        {
	            var sqr = (GameObject)Instantiate(squarePrefab);
                sqr.transform.position = new Vector3(-5f*0.5f + x*0.5f, 7f*0.5f - y*0.5f);
	            sqr.GetComponent<SquareControl>().gridX = x;
                sqr.GetComponent<SquareControl>().gridY = y;
                sqr.GetComponent<SpriteRenderer>().color
                    = new Color(Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f);

	            squares[x, y] = sqr;
	        }
	    }
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!_readyForNextSelection)
	    {
	        var needMoreTime = false;

            // check if all death animation && new square movement is complete
            //  if so, next selection OK
	        _deadSquareCoordsToReplace.ForEach(coord =>
	        {
	            if (squares[coord.X, coord.Y] != null)
	                needMoreTime = true;
	        });
            /*
            for (var x = 0; x < sizeX; x++)
	        {
	            for (var y = 0; y < sizeY; y++)
	            {
	                if (squares[x, y] != null && squares[x,y].animation.isPlaying)
	                {
	                    Debug.Log("Playing animation");
	                    needMoreTime = true;
	                }
	                    
	            }
	        }
            */
	        if (!needMoreTime)
	            Debug.Log("Death done");

	        _readyForNextSelection = !needMoreTime;

	        return;
	    }

        HandleNewTouches();
	}

    void HandleNewTouches()
    {
        var touchPos = new Vector2(-1, -1);

        // Detect touch
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            var wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchPos = new Vector2(wp.x, wp.y);

        }
        else if (Input.GetMouseButtonDown(0))
        {
            _mouseDrag = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _mouseDrag = false;
            SelectionFinished();
        }

        if (_mouseDrag)
        {
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.touchCount == 0 && Application.platform == RuntimePlatform.Android)
        {
            SelectionFinished();
        }

        if (touchPos == new Vector2(-1, -1))
            return;

        CheckSquareTouch(touchPos);
    }

    void SelectionFinished()
    {
        Debug.Log("New Selection Clear");
        _readyForNextSelection = false;
        
        lineConnector.SelectedSquares.ForEach(o =>
        {
            o.GetComponent<Animator>().SetBool("Death", true);
            _deadSquareCoordsToReplace.Add(new XYCoord(o.GetComponent<SquareControl>().gridX, o.GetComponent<SquareControl>().gridY));
        });
        lineConnector.ClearSquareConnection();
    }

    void CheckSquareTouch(Vector2 touchPos)
    {
        // foreach square, check for touch
        for (var x = 0; x < sizeX; x++)
        {
            for (var y = 0; y < sizeY; y++)
            {
                // Cannot have missing squares...
                if (squares[x,y] != null && squares[x, y].collider2D == Physics2D.OverlapPoint(touchPos))
                {
                    //Debug.Log(string.Format("{0}, {1}", x, y));
                    lineConnector.AddSquareToConnect(squares[x, y]);
                }
            }
        }
    }

    struct XYCoord
    {
        public XYCoord(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X, Y;
    }
}
