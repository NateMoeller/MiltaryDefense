using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarDebug : MonoBehaviour {

    [SerializeField]
    private TileScript goal, start;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        /*
        ClickTile();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AStar.GetPath(start.GridPosition, goal.GridPosition);
        }
        */
	}


    private void ClickTile(){
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if(hit.collider != null){
                TileScript tmp = hit.collider.GetComponent<TileScript>();

                if(tmp != null){
                    if(start == null)
                    {
                        start = tmp;
                        start.Debugging = true;
                        start.SpriteRenderer.color = Color.green;
                    }
                    else if(goal == null)
                    {
                        goal = tmp;
                        goal.Debugging = true;
                        goal.SpriteRenderer.color = Color.red;
                    }
                }
            }
        }

    }



    public void DebugPath(HashSet<Node> openList, HashSet<Node> closedList){
        foreach(Node node in openList){
            if(node.TileRef != start && node.TileRef != goal){
                node.TileRef.SpriteRenderer.color = Color.cyan;
            }           
        }


        foreach (Node node in closedList) {
            if (node.TileRef != start && node.TileRef != goal) {
                node.TileRef.SpriteRenderer.color = Color.blue;
            }
        }
    }


}
