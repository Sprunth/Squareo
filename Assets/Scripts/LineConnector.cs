using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class LineConnector : MonoBehaviour
{

    private List<GameObject> squaresConnected;
    public List<GameObject> SelectedSquares { get { return squaresConnected; } }

    private LineRenderer lineRenderer;

    public Material LineMat;

	// Use this for initialization
	void Start ()
	{
        squaresConnected = new List<GameObject>();


	    lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetWidth(0.1f, 0.1f);
	    lineRenderer.material = LineMat;
	    //lineRenderer.castShadows = false;
	    //lineRenderer.receiveShadows = false;


	}
	
	// Update is called once per frame
	void Update () {
	
	}


    /// <summary>
    /// Adds the square to the list of connected squares
    /// </summary>
    /// <param name="obj">Square to add</param>
    /// <returns>True if square is valid to add</returns>
    public bool AddSquareToConnect(GameObject obj)
    {
        var x = obj.GetComponent<SquareControl>().gridX;
        var y = obj.GetComponent<SquareControl>().gridY;

        // if not first square to be selected, make sure it is one horiz/vert square away
        if (squaresConnected.Count != 0)
        {
            var prevX = squaresConnected.Last().GetComponent<SquareControl>().gridX;
            var prevY = squaresConnected.Last().GetComponent<SquareControl>().gridY;

            // must be neighbor to most recently connected square
            if (!(
                (Math.Abs(prevX - x) == 1 && prevY == y) ||
                (Math.Abs(prevY - y) == 1 && prevX == x)
                ))
            {
                return false;
            }

            // must be same color as most recently connected square
            if (squaresConnected.Last().GetComponent<SquareControl>().Color != obj.GetComponent<SquareControl>().Color)
                return false;


        }
        if (squaresConnected.Contains(obj))
        {
            // keep popping off objects till we're back at obj
            while (squaresConnected.Last() != obj)
            {
                squaresConnected.RemoveAt(squaresConnected.Count - 1);
                lineRenderer.SetVertexCount(squaresConnected.Count);
            }
            // then remove obj too, as we will add it right afterward.
            squaresConnected.RemoveAt(squaresConnected.Count - 1);
            lineRenderer.SetVertexCount(squaresConnected.Count);
        }

        obj.GetComponent<Animator>().SetBool("Selected", true);
        squaresConnected.Add(obj);
        //Debug.Log(obj.transform.position);
        lineRenderer.SetVertexCount(squaresConnected.Count);
        lineRenderer.SetPosition(squaresConnected.Count-1, obj.transform.position + new Vector3(0,0,-10));
        
        return true;
    }

    public void ClearSquareConnection()
    {
        squaresConnected.Clear();
        lineRenderer.SetVertexCount(0);
    }

    
}
