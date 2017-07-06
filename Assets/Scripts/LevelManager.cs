using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelManager : Singleton<LevelManager> {

    [SerializeField]
    private GameObject[] tilePrefabs;
    [SerializeField]
    private CameraMovement cameraMovement;
    [SerializeField]
    private Transform map;

    private Point portalSpawn;
    private Point goalSpawn;

    public Portal StartPortal { get; set; }

    [SerializeField]
    private GameObject portal;
    [SerializeField]
    private GameObject goal;


    public Dictionary<Point, TileScript> Tiles { get; set; }
    private Point mapSize;

    public float TileSize {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    public Point PortalSpawn {
        get {
            return portalSpawn;
        }

        private set {
            portalSpawn = value;
        }
    }

    public Point GoalSpawn {
        get {
            return goalSpawn;
        }

        private set {
            goalSpawn = value;
        }
    }


    // Use this for initialization
    void Start () {
        CreateLevel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void CreateLevel(){
        Tiles = new Dictionary<Point, TileScript>();

        string[] mapData = ReadLevelText();

        // bottom right corner of the map
        

        int mapX = mapData[0].ToCharArray().Length;
        int mapY = mapData.Length;

        mapSize = new Point(mapX, mapY);

        Vector3 maxTile = Vector3.zero;

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));

        for(int y = 0; y < mapY; y++){
            char[] newTiles = mapData[y].ToCharArray();
            for(int x = 0; x < mapX; x++){
                PlaceTile(newTiles[x].ToString(), x, y, worldStart); 
            }
        }

        maxTile = Tiles[new Point(mapX - 1, mapY - 1)].transform.position;

        // set limits on camera
        cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));

        // spawn portals
        SpawnPortals();
    }

    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart){
        int tileIndex = int.Parse(tileType);
        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();

        // change position of the tile
        newTile.SetUp(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), map);

    }

    private string[] ReadLevelText(){
        TextAsset bindData = Resources.Load("Level") as TextAsset;
        string data = bindData.text.Replace(Environment.NewLine, string.Empty);
        return data.Split('-');
    }

    private void SpawnPortals()
    {
        // make start portal
        PortalSpawn = new Point(0, 4);
        GameObject tmp = (GameObject)Instantiate(portal, Tiles[PortalSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
        StartPortal = tmp.GetComponent<Portal>();
        StartPortal.name = "StartPortal";

        // make end portal
        GoalSpawn = new Point(17, 4);
        Instantiate(goal, Tiles[GoalSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
    }

    public bool InBounds(Point position){
        return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y < mapSize.Y;
    }

}
