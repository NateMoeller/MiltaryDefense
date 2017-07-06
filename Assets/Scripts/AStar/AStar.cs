using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public static class AStar {

    private static Dictionary<Point, Node> nodes;

    private static void CreateNodes(){
        nodes = new Dictionary<Point, Node>();

        foreach (TileScript tile in LevelManager.Instance.Tiles.Values){
            nodes.Add(tile.GridPosition, new Node(tile));
        }
    }

    public static Stack<Node> GetPath(Point start, Point goal, TileScript hover = null){
        if (nodes == null) CreateNodes();

        // PART OF THIS IS HARD CODED
        if(hover != null && (LevelManager.Instance.PortalSpawn == hover.GridPosition || (LevelManager.Instance.PortalSpawn.X + 1 == hover.GridPosition.X 
            && LevelManager.Instance.PortalSpawn.Y == hover.GridPosition.Y))) {
            return new Stack<Node>();
        }
        

        HashSet<Node> openList = new HashSet<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        Stack<Node> path = new Stack<Node>();

        Node currentNode = nodes[start];
        Node goalNode = nodes[goal];
        Node hoverNode = null;
        if(hover != null) {
            hoverNode = nodes[hover.GridPosition];
        }

        openList.Add(currentNode);


        while(openList.Count > 0) {
            // go through all neighbors
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    Point neighborPosition = new Point(currentNode.GridPosition.X - x, currentNode.GridPosition.Y - y);

                    // check if a neighbor is walkable, in bounds, and not itself (and not being hovered upon?)
                    if (LevelManager.Instance.InBounds(neighborPosition) && LevelManager.Instance.Tiles[neighborPosition].Walkable &&
                        neighborPosition != currentNode.GridPosition && (hover == null || hover.GridPosition != neighborPosition)) {

                        Node neighbor = nodes[neighborPosition];

                        // diagonals cost 14, vertical and horizontal cost 10
                        int gCost = 0;
                        if (Math.Abs(x - y) == 1) {
                            gCost = 10;
                        }
                        else {

                            // fix corner cutting                           
                            if (!ConnectedLeftDiagonally(currentNode, neighbor, hoverNode)) {
                                continue;
                            }
                            
                            gCost = 14;
                        }                

                        if (openList.Contains(neighbor)) {
                            if (currentNode.G + gCost < neighbor.G) {
                                neighbor.CalcValues(currentNode, gCost, goalNode);
                            }
                        }
                        else if (!closedList.Contains(neighbor)) {
                            openList.Add(neighbor);
                            neighbor.CalcValues(currentNode, gCost, goalNode);
                        }

                    }

                }

            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // LOOK into optimizing this use PRIORITY QUEUE
            if (openList.Count > 0) {
                // sorting open list by F value, get first
                currentNode = openList.OrderBy(n => n.F).First();
            }

            if (currentNode == goalNode) {
                while(currentNode.GridPosition != start) {
                    path.Push(currentNode);
                    currentNode = currentNode.Parent;
                }
                
                break;
            }
        }

        return path;

        // debugging code
        //GameObject.Find("Debugger").GetComponent<AStarDebug>().DebugPath(openList, closedList); 
    }

    public static Stack<Node> GetAirplanePath(Point start, Point goal) {
        if (nodes == null) CreateNodes();

        // go horizontal until you get to the start
        Stack<Node> output = new Stack<Node>();
        Node startNode = nodes[start];
        Node goalNode = nodes[goal];

        Node currentNode = goalNode;
        while(currentNode != startNode) {
            output.Push(currentNode);

            currentNode = nodes[new Point(currentNode.GridPosition.X - 1, currentNode.GridPosition.Y)];
        }

        return output;
    }

    private static bool ConnectedLeftDiagonally(Node currentNode, Node neighbor, Node hoverNode) {
        Point direction = neighbor.GridPosition - currentNode.GridPosition;
        Point first = new Point(currentNode.GridPosition.X + direction.X, currentNode.GridPosition.Y);
        Point second = new Point(currentNode.GridPosition.X + direction.X, currentNode.GridPosition.Y + direction.Y);

        if (LevelManager.Instance.InBounds(first) && (!LevelManager.Instance.Tiles[first].Walkable || (hoverNode != null && first == hoverNode.GridPosition))) {
            return false;
        }

        if (LevelManager.Instance.InBounds(second) && (!LevelManager.Instance.Tiles[second].Walkable || (hoverNode != null && second == hoverNode.GridPosition))) {
            return false;
        }

        return true;
    }

}
