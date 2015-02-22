using System;
using System.Collections.Generic;
using System.Diagnostics;
using NGenerics.DataStructures.Queues;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GameMaster : MonoBehaviour
{

    public GameObject squarePrefab;

    public GameObject[,] squares;

    private LineConnector lineConnector;

    /// Size of the grid
    private int sizeX, sizeY;

    private bool _mouseDrag = false;

    private bool _readyForNextSelection = true;

    private readonly List<XYCoord> _deadSquareCoordsToReplace = new List<XYCoord>();

    private Direction _clearDirection;

    private int _swipesLeft = 30;
    public UILabel ScoreText;
    public UILabel InfoText;
    private TimeSpan timeLeft;
    private Stopwatch stopWatch;
    private bool endGameWait = false;

    public AudioSource clearAudioSource;

	// Use this for initialization
	void Start ()
	{
	    SetupGameMode();

	    lineConnector = GameObject.FindGameObjectWithTag("LineConnector").GetComponent<LineConnector>();

	    sizeX = 11;
	    sizeY = 15;

        squares = new GameObject[sizeX, sizeY];

	    for (var x = 0; x < sizeX; x++)
	    {
	        for (var y = 0; y < sizeY; y++)
	        {
	            CreateSquareAt(x, y);
	        }
	    }

	    clearAudioSource = GetComponent<AudioSource>();

        timeLeft = new TimeSpan(0, 0, 2, 0);
        stopWatch = new Stopwatch();
        stopWatch.Start();

	}

    private void SetupGameMode()
    {
        Debug.Log("SelectedGameMode: " + Globals.SelectedGameMode);

        switch (Globals.SelectedGameMode)
        {
            case Globals.GameMode.TimeTrial:
            {
                break;
            }
            case Globals.GameMode.Zen:
            {
                break;
            }
            case Globals.GameMode.ThirtySwipes:
            {
                break;
            }
        }

    }
	
	// Update is called once per frame
	void Update ()
	{
        // Once the game ends, wait a bit before transitioning
        if (endGameWait)
        {
            Debug.Log(stopWatch.ElapsedMilliseconds);
            if (stopWatch.ElapsedMilliseconds > 1400)
                Application.LoadLevel("ScoreScreen");
            return;
        }


        // Time Trial stuff
	    if (Globals.SelectedGameMode == Globals.GameMode.TimeTrial)
	    {
            timeLeft -= new TimeSpan(stopWatch.ElapsedTicks);
            stopWatch.Reset();stopWatch.Start();
            if (timeLeft.Seconds < 10)
	            InfoText.text = string.Format("{0}:0{1}", timeLeft.Minutes, timeLeft.Seconds);
            InfoText.text = timeLeft.Seconds < 10 ? 
                string.Format("{0}:0{1}", timeLeft.Minutes, timeLeft.Seconds) :
                string.Format("{0}:{1}", timeLeft.Minutes, timeLeft.Seconds);
	        if (timeLeft.TotalSeconds <= 0)
	        {
	            // TODO: get to endscreen/score screen
	            endGameWait = true;
                stopWatch.Reset(); stopWatch.Start();
	        }
	    }
        else if (Globals.SelectedGameMode == Globals.GameMode.ThirtySwipes)
        {
            InfoText.text = string.Format("Swipes Left: {0}", _swipesLeft);
            if (_swipesLeft <= 0)
            {
                // TODO: get to endscreen/score screen
                endGameWait = true;
                stopWatch.Reset(); stopWatch.Start();
            }
        }

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

	        if (!needMoreTime)
	        {
	            Debug.Log("Death done");
                UpdateGridSquares();
	        }

	        _readyForNextSelection = !needMoreTime;

	        return;
	    }

        HandleNewTouches();
	}

    GameObject CreateSquareAt(int x, int y)
    {
        var sqr = (GameObject)Instantiate(squarePrefab);
        sqr.transform.position = GridPosToVec(x, y);
        sqr.GetComponent<SquareControl>().gridX = x;
        sqr.GetComponent<SquareControl>().gridY = y;

        squares[x, y] = sqr;
        return sqr;
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

    /// <summary>
    /// Called after all death animations are done and new squares need to be moved in
    /// </summary>
    void UpdateGridSquares()
    {
        // I don't know a better way to do this them individually for each direction
        switch (_clearDirection)
        {
            case Direction.BottomToTop:
            {
                NewSquaresBottomToTop();
                break;
            }
            case Direction.TopToBottom:
            {
                NewSquaresTopToBottom();
                break;
            }
            case Direction.LeftToRight:
            {
                NewSquaresLeftToRight();
                break;
            }
            case Direction.RightToLeft:
            {
                NewSquaresRightToLeft();
                break;
            }
        }
        _deadSquareCoordsToReplace.Clear();
    }

    private void NewSquaresTopToBottom()
    {
        // use a priority queue to keep track of coords
        var toProcess = new PriorityQueue<XYCoord>(PriorityQueueType.Maximum);
        _deadSquareCoordsToReplace.ForEach(coord => toProcess.Enqueue(coord));


        while (toProcess.Count > 0)
        {
            var coord = toProcess.Dequeue();

            var foundValidSquare = false;

            // seach upward for valid squares
            var currentY = coord.Y - 1;
            while (currentY >= 0 && !foundValidSquare)
            {
                var potentialSquareCandidate = squares[coord.X, currentY];
                if (potentialSquareCandidate == null)
                {
                    currentY--;
                    continue;
                }


                // valid square, so reassign it
                potentialSquareCandidate.GetComponent<SquareControl>().newTarget = GridPosToVec(coord.X,
                    coord.Y);
                potentialSquareCandidate.GetComponent<SquareControl>().origPos =
                    potentialSquareCandidate.transform.position;
                potentialSquareCandidate.GetComponent<SquareControl>().movingToNewPosition = true;
                potentialSquareCandidate.GetComponent<SquareControl>().gridX = coord.X;
                potentialSquareCandidate.GetComponent<SquareControl>().gridY = coord.Y;

                // assign it new position
                squares[coord.X, coord.Y] = potentialSquareCandidate;
                // remove old position, and add it to the queue
                squares[coord.X, currentY] = null;
                toProcess.Enqueue(new XYCoord(coord.X, currentY));

                foundValidSquare = true;

            }

            // if still not found, then we spawn a new square
            if (!foundValidSquare)
            {
                var sqr = CreateSquareAt(coord.X, coord.Y);
                // override the position
                sqr.GetComponent<SquareControl>().transform.position = GridPosToVec(coord.X, -1);
                sqr.GetComponent<SquareControl>().origPos = GridPosToVec(coord.X, -1);
                sqr.GetComponent<SquareControl>().movingToNewPosition = true;
                sqr.GetComponent<SquareControl>().newTarget = GridPosToVec(coord.X, coord.Y);
            }

        }
    }

    private void NewSquaresBottomToTop()
    {
        // use a priority queue to keep track of coords
        var toProcess = new PriorityQueue<XYCoord>(PriorityQueueType.Minimum);
        _deadSquareCoordsToReplace.ForEach(coord => toProcess.Enqueue(coord));


        while (toProcess.Count > 0)
        {
            var coord = toProcess.Dequeue();

            var foundValidSquare = false;

            // seach downward for valid squares
            var currentY = coord.Y;
            while (currentY < sizeY && !foundValidSquare)
            {
                var potentialSquareCandidate = squares[coord.X, currentY];
                if (potentialSquareCandidate == null)
                {
                    currentY++;
                    continue;
                }


                // valid square, so reassign it
                potentialSquareCandidate.GetComponent<SquareControl>().newTarget = GridPosToVec(coord.X,
                    coord.Y);
                potentialSquareCandidate.GetComponent<SquareControl>().origPos =
                    potentialSquareCandidate.transform.position;
                potentialSquareCandidate.GetComponent<SquareControl>().movingToNewPosition = true;
                potentialSquareCandidate.GetComponent<SquareControl>().gridX = coord.X;
                potentialSquareCandidate.GetComponent<SquareControl>().gridY = coord.Y;

                // assign it new position
                squares[coord.X, coord.Y] = potentialSquareCandidate;
                // remove old position, and add it to the queue
                squares[coord.X, currentY] = null;
                toProcess.Enqueue(new XYCoord(coord.X, currentY));

                foundValidSquare = true;

            }

            // if still not found, then we spawn a new square
            if (!foundValidSquare)
            {
                var sqr = CreateSquareAt(coord.X, coord.Y);
                // override the position
                sqr.GetComponent<SquareControl>().transform.position = GridPosToVec(coord.X, sizeY);
                sqr.GetComponent<SquareControl>().origPos = GridPosToVec(coord.X, sizeY);
                sqr.GetComponent<SquareControl>().movingToNewPosition = true;
                sqr.GetComponent<SquareControl>().newTarget = GridPosToVec(coord.X, coord.Y);
            }

        }
    }

    private void NewSquaresLeftToRight()
    {
        // use a priority queue to keep track of coords
        var toProcess = new PriorityQueue<XYCoord>(PriorityQueueType.Maximum);
        _deadSquareCoordsToReplace.ForEach(coord => toProcess.Enqueue(coord));


        while (toProcess.Count > 0)
        {
            var coord = toProcess.Dequeue();

            var foundValidSquare = false;

            // seach upward for valid squares
            var currentX = coord.X - 1;
            while (currentX >= 0 && !foundValidSquare)
            {
                var potentialSquareCandidate = squares[currentX, coord.Y];
                if (potentialSquareCandidate == null)
                {
                    currentX--;
                    continue;
                }


                // valid square, so reassign it
                potentialSquareCandidate.GetComponent<SquareControl>().newTarget = GridPosToVec(coord.X,
                    coord.Y);
                potentialSquareCandidate.GetComponent<SquareControl>().origPos =
                    potentialSquareCandidate.transform.position;
                potentialSquareCandidate.GetComponent<SquareControl>().movingToNewPosition = true;
                potentialSquareCandidate.GetComponent<SquareControl>().gridX = coord.X;
                potentialSquareCandidate.GetComponent<SquareControl>().gridY = coord.Y;

                // assign it new position
                squares[coord.X, coord.Y] = potentialSquareCandidate;
                // remove old position, and add it to the queue
                squares[currentX, coord.Y] = null;
                toProcess.Enqueue(new XYCoord(currentX, coord.Y));

                foundValidSquare = true;

            }

            // if still not found, then we spawn a new square
            if (!foundValidSquare)
            {
                var sqr = CreateSquareAt(coord.X, coord.Y);
                // override the position
                sqr.GetComponent<SquareControl>().transform.position = GridPosToVec(-1, coord.Y);
                sqr.GetComponent<SquareControl>().origPos = GridPosToVec(-1, coord.Y);
                sqr.GetComponent<SquareControl>().movingToNewPosition = true;
                sqr.GetComponent<SquareControl>().newTarget = GridPosToVec(coord.X, coord.Y);
            }

        }
    }

    private void NewSquaresRightToLeft()
    {
        // use a priority queue to keep track of coords
        var toProcess = new PriorityQueue<XYCoord>(PriorityQueueType.Minimum);
        _deadSquareCoordsToReplace.ForEach(coord => toProcess.Enqueue(coord));


        while (toProcess.Count > 0)
        {
            var coord = toProcess.Dequeue();

            var foundValidSquare = false;

            // seach downward for valid squares
            var currentX = coord.X;
            while (currentX < sizeX && !foundValidSquare)
            {
                var potentialSquareCandidate = squares[currentX, coord.Y];
                if (potentialSquareCandidate == null)
                {
                    currentX++;
                    continue;
                }


                // valid square, so reassign it
                potentialSquareCandidate.GetComponent<SquareControl>().newTarget = GridPosToVec(coord.X,
                    coord.Y);
                potentialSquareCandidate.GetComponent<SquareControl>().origPos =
                    potentialSquareCandidate.transform.position;
                potentialSquareCandidate.GetComponent<SquareControl>().movingToNewPosition = true;
                potentialSquareCandidate.GetComponent<SquareControl>().gridX = coord.X;
                potentialSquareCandidate.GetComponent<SquareControl>().gridY = coord.Y;

                // assign it new position
                squares[coord.X, coord.Y] = potentialSquareCandidate;
                // remove old position, and add it to the queue
                squares[currentX, coord.Y] = null;
                toProcess.Enqueue(new XYCoord(currentX, coord.Y));

                foundValidSquare = true;

            }

            // if still not found, then we spawn a new square
            if (!foundValidSquare)
            {
                var sqr = CreateSquareAt(coord.X, coord.Y);
                // override the position
                sqr.GetComponent<SquareControl>().transform.position = GridPosToVec(sizeX, coord.Y);
                sqr.GetComponent<SquareControl>().origPos = GridPosToVec(sizeX, coord.Y);
                sqr.GetComponent<SquareControl>().movingToNewPosition = true;
                sqr.GetComponent<SquareControl>().newTarget = GridPosToVec(coord.X, coord.Y);
            }
        }
    }

    private Vector3 GridPosToVec(int x, int y)
    {
        return new Vector3(-5f * 0.5f + x * 0.5f, 7f * 0.5f - y * 0.5f);
    }

    /// <summary>
    /// User has selected some, and want to clear it
    /// </summary>
    void SelectionFinished()
    {
        //TODO: Need to check if no more valid moves
        if (lineConnector.SelectedSquares.Count < 3)
        {
            // basically ignore the swipe
            lineConnector.ClearSquareConnection();
            return;
        }
        
        // play sfx
        clearAudioSource.Play();

        Debug.Log("New Selection Clear");
        _readyForNextSelection = false;

        // set the clear direction from the first pair connection
        _clearDirection = SetClearDirection(lineConnector.SelectedSquares[0], lineConnector.SelectedSquares[1]);

        // Update score
        Globals.Score += (int)Math.Pow(lineConnector.SelectedSquares.Count, 2);
        ScoreText.text = String.Format("Score: {0}", Globals.Score);

        // update swipes count
        if (Globals.SelectedGameMode == Globals.GameMode.ThirtySwipes)
        {
            _swipesLeft--;
        }

        lineConnector.SelectedSquares.ForEach(o =>
        {
            o.GetComponent<Animator>().SetBool("Death", true);
            _deadSquareCoordsToReplace.Add(new XYCoord(o.GetComponent<SquareControl>().gridX, o.GetComponent<SquareControl>().gridY));
        });
        lineConnector.ClearSquareConnection();
    }

    /// <summary>
    /// Given two objects, find the direction of the swipe from obj1 to obj2
    /// </summary>
    Direction SetClearDirection(GameObject obj1, GameObject obj2)
    {
        var x1 = obj1.GetComponent<SquareControl>().gridX;
        var x2 = obj2.GetComponent<SquareControl>().gridX;
        var y1 = obj1.GetComponent<SquareControl>().gridY;
        var y2 = obj2.GetComponent<SquareControl>().gridY;

        if (x1 > x2 && y1 == y2)
            return Direction.RightToLeft;
        if (x1 < x2 && y1 == y2)
            return Direction.LeftToRight;
        if (y1 > y2 && x1 == x2)
            return Direction.BottomToTop;
        if (y1 < y2 && x1 == x2)
            return Direction.TopToBottom;
        throw new Exception("The two objects are not next to each other");
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

    enum Direction { LeftToRight, RightToLeft, TopToBottom, BottomToTop }
}
