using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour {

    public Point GridPosition { get; private set; }
    public bool IsEmpty { get; set; }

    public Vector2 WorldPosition {
        get { return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 2)); }
        set { }
   }

    private Color32 fullColor = new Color32(255, 118, 118, 255);
    private Color32 emptyColor = new Color32(96, 255, 90, 255);

    public SpriteRenderer SpriteRenderer { get; set; }
    public bool Walkable { get; set; }
    public bool HasEnemy { get; set; }
    public bool Debugging { get; set; }
    public bool HasTower { get; set; }

    private bool menuOn = false;
    private TowerData towerData;
    private GameObject tower;

    // Use this for initialization
    void Start () {
        SpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SetUp(Point gridPos, Vector3 worldPos, Transform parent){
        IsEmpty = true;
        if (gameObject.tag == "BushTile") {
            IsEmpty = false;
            Walkable = false;
        }
        else {
            Walkable = true;
        }

        
        this.GridPosition = gridPos;
        transform.position = worldPos;

        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver(){
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null) {
            bool valid = true;
            if (IsEmpty && Walkable && NoEnemies() && GameManager.Instance.ValidPath(this) && !Debugging) {
                ColorTile(emptyColor);
            }
            else {
                valid = false;
                ColorTile(fullColor);
            }

            if (valid && Input.GetMouseButtonDown(0)) {
                PlaceTower();
            }
        }
        else if (HasTower && Input.GetMouseButtonDown(0) && !GameManager.Instance.menuActive) {
            // get the sell and upgrade price of the tower
            towerData = GetComponentInChildren<TowerData>();
            menuOn = true;
            GameManager.Instance.menuActive = true;
        }


    }

    private void OnMouseExit(){
        if (!Debugging)
        {
            ColorTile(Color.white);
        }
        
    }

    private void PlaceTower(){
        // calculate angle to begining portal       
        Point vectorToTarget = GridPosition - LevelManager.Instance.PortalSpawn;
        float angle = Mathf.Atan2(vectorToTarget.Y, vectorToTarget.X) * Mathf.Rad2Deg;
        
        // place the tower in the middle of the tile
        Vector3 positionToPlace = new Vector3(transform.position.x + (transform.localScale.x/ 2), transform.position.y - (transform.localScale.y / 2));
        tower = (GameObject)Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, positionToPlace, Quaternion.identity);
        tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y; // fix overlapping issue

        //tower.transform.Rotate(new Vector3(0, 0, 1), 90f - angle);
        tower.transform.rotation = Quaternion.AngleAxis(90f - angle, new Vector3(0, 0, 1));

        // set the tower as a child object of the tile
        tower.transform.SetParent(transform);

        IsEmpty = false;
        ColorTile(Color.white);
        Walkable = false;
        HasTower = true;
        GameManager.Instance.BuyTower();
        
    }

    private void SellTower() {
        IsEmpty = true;
        Walkable = true;
        HasTower = false;
        Destroy(tower);
    }


    private void ColorTile(Color newColor){
        SpriteRenderer.color = newColor;
    }


    private bool NoEnemies() {
        // loop through the active enemies and make sure none of them have the same grid postion as this tile
        foreach(Enemy enemy in GameManager.Instance.ActiveEnemies) {
            if(enemy.GridPosition == GridPosition) {
                return false;
            }
        }
        return true;
    }


    private void OnGUI() {
        // TODO: the rectangle values are fixed pixel values. Change to be based on screen?

        if (menuOn) {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 150, Screen.height / 2 - (Screen.height / 4), Screen.width / 2, Screen.height / 2));

            //the menu background box
            GUI.Box(new Rect(0, 0, 300, 200), "");

            if (GUI.Button(new Rect(10, 10, 130, 80), "Sell Tower for: $" + towerData.sellPrice)) {
                // do something here
                SellTower();
                GameManager.Instance.Currency += towerData.sellPrice;
                menuOn = false;
                GameManager.Instance.menuActive = false;
            }

            if(GameManager.Instance.Currency < towerData.upgradePrice) {
                GUI.color = new Color(1, 1, 1, 0.5f); // set transparency
            }
            else {
                GUI.color = new Color(1, 1, 1, 1);
            }

            if(towerData.level == 0) {
                if (GUI.Button(new Rect(150, 10, 130, 80), towerData.upgradeName + " \nfor: $" + towerData.upgradePrice) &&
                GameManager.Instance.Currency >= towerData.upgradePrice) {
                    GameManager.Instance.Currency -= towerData.upgradePrice;
                    towerData.UpgradeTower();
                    menuOn = false;
                    GameManager.Instance.menuActive = false;
                }
            }
            

            GUI.color = new Color(1, 1, 1, 1);
            if (GUI.Button(new Rect(150, 100, 130, 80), "Close")) {
                menuOn = false;
                GameManager.Instance.menuActive = false;
            }

            GUI.EndGroup();
        }
    }
}
