using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    public TowerBtn ClickedBtn { get; set; }
    public ObjectPool Pool { get; set; }
    private int currency;
    private int wave = 0;
    [SerializeField]
    private Text waveText;
    [SerializeField]
    private Text currencyTxt;
    [SerializeField]
    private GameObject waveBtn;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private int lives;
    [SerializeField]
    private Text livesText;
    private int score;
    [SerializeField]
    private Text scoreText;
    private float timeBetweenEnemies;
    private bool gameOver;
    private bool paused;

    [SerializeField]
    private GameObject gameOverMenu;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject towerPanel;
    public bool menuActive;

    private bool hovering;
    private bool newEnemyShown = false;
    private bool showPopUpMessage;


    public bool WaveActive {
        get { return ActiveEnemies.Count > 0; }
    }

    public int Currency
    {
        get
        {
            return currency;
        }

        set
        {
            currency = value;
            this.currencyTxt.text = "<color=lime>$</color>" + value.ToString(); 
        }
    }

    public List<Enemy> ActiveEnemies {
        get {
            return activeEnemies;
        }

        private set {
            activeEnemies = value;
        }
    }

    public int Lives {
        get {
            return lives;
        }

        set {
            lives = value;
            livesText.text = string.Format("{0}", lives);
            if(lives <= 0) {
                this.lives = 0;
                GameOver();
            }
        }
    }

    public int Score {
        get {
            return score;
        }

        set {
            score = value;
            scoreText.text = string.Format("Score: {0}", score);
        }
    }

    private void Awake() {
        Pool = GetComponent<ObjectPool>();
    }

    // Use this for initialization
    void Start () {
        Currency = 25; // 25
        Lives = 10;
        timeBetweenEnemies = 1.2f;
        gameOver = false;
        hovering = false;
        menuActive = false;
        paused = false;
    }
	
	// Update is called once per frame
	void Update () {
        HandleEscape();
	}

    public void PickTower(TowerBtn towerBtn){
        if (Currency >= towerBtn.Price){
            this.ClickedBtn = towerBtn;
            hovering = true;
            Hover.Instance.Activate(towerBtn.Sprite);
        } 
    }

    public void BuyTower(){
        if (Currency >= ClickedBtn.Price) {
            Currency -= ClickedBtn.Price;
            Hover.Instance.Deactivate();
            hovering = false;
            // recalculate the path foreach active enemy
            if (WaveActive) {
                foreach (Enemy enemy in ActiveEnemies) {
                    if(enemy.enemyType != "airplane") {
                        enemy.SetPath(AStar.GetPath(enemy.GridPosition, LevelManager.Instance.GoalSpawn));
                    }                  
                }
            }

            
        }
    }

    private void HandleEscape(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (hovering) {
                Hover.Instance.Deactivate();
                hovering = false;
            }
            else {
                Pause();
            }
        }
    }

    public void Pause() {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        towerPanel.SetActive(false);
    }

    public void Unpause() {
        pauseMenu.SetActive(false);
        towerPanel.SetActive(true);
        Time.timeScale = 1;       
    }


    public void StartWave() {
        wave++;
        if(wave > 5 && timeBetweenEnemies >= .1f) {
            timeBetweenEnemies -= .02f; // make the enemies come out faster
        }
        Debug.Log("Time In Between: " + timeBetweenEnemies);
        waveText.text = string.Format("Wave: <color=lime>{0}</color>", wave);
        StartCoroutine(SpawnWave());
        waveBtn.SetActive(false);
    }

    private IEnumerator SpawnWave() {
        Random rand = new Random();
        int intervalSoldier1 = 100;
        int intervalSoldier2 = 0;
        int intervalDogs = 0;
        int intervalTanks = 0;
        int intervalAirplanes = 0;
        
        if(wave >= 5) {
            intervalSoldier2 = intervalSoldier1;
            intervalSoldier1 -= 20;

            if(wave >= 7) {
                intervalDogs = intervalSoldier2;
                intervalSoldier1 -= 15;
                intervalSoldier2 -= 15;
            }

            if(wave >= 15) {
                intervalTanks = intervalDogs;
                intervalSoldier1 -= 10;
                intervalSoldier2 -= 10;
                intervalDogs -= 10;
            }
            if(wave >= 20) {
                intervalAirplanes = intervalTanks;
                intervalSoldier1 -= 10;
                intervalSoldier2 -= 10;
                intervalDogs -= 10;
                intervalTanks -= 10;
            } 
        }
        int total = wave + (wave / 5) + (wave / 7) + (wave / 10) + (wave / 15);
        /*
        // figure out the total amount of enemies
        int numSoldiers1 = wave;
        int numSoldiers2 = wave / 5;
        int numDogs = wave / 7;
        int numTanks = wave / 10;
        int numAirplanes;
        if(wave < 20) {
            numAirplanes = 0;
        }
        else {
            numAirplanes = wave / 15;
        }
        

        int total;
        if(wave > 5) {
            total = numSoldiers1 + numSoldiers2 + numDogs + numTanks + numAirplanes;
        }
        else {
            total = numSoldiers1;
        }

        List<string> types = new List<string>();
        for(int i = 0; i < numSoldiers1; i++) {
            types.Add("soldier");
        }
        for(int i = 0; i < numSoldiers2; i++) {
            types.Add("soldier2");
        }
        for (int i = 0; i < numDogs; i++) {
            types.Add("Dog");
        }
        for (int i = 0; i < numTanks; i++) {
            // add variablilty between black and red
            float num = Random.Range(0, 2);
            if(num <= .5f) {
                types.Add("tankRed");
            }
            else {
                types.Add("tankBlack");
            }
            
        }
        
        for(int i = 0; i < numAirplanes; i++) {
            types.Add("airplane");
        }
        
        Shuffle(types);
        */
        for (int i = 0; i < total; i++) {
            //string type = types[i];
            string type = getTypeFromDistribution(intervalSoldier1, intervalSoldier2, intervalDogs, intervalTanks, intervalAirplanes);

            // get an enemy from the pool
            Debug.Log("Spawning: " + type);
            Enemy monster = Pool.GetObject(type).GetComponent<Enemy>();
            monster.Spawn(LevelManager.Instance.PortalSpawn, LevelManager.Instance.GoalSpawn);
            ActiveEnemies.Add(monster);
            yield return new WaitForSeconds(timeBetweenEnemies);
        }
        
    }

    private string getTypeFromDistribution(int soldier1, int soldier2, int dog, int tank, int airplane) {
        Random rand = new Random();
        int randNum = Random.Range(0, 100);
        if (randNum >= 0 && randNum <= soldier1) {
            return "soldier";
        }
        else if (randNum > soldier1 && randNum <= soldier2) {
            return "soldier2";
        }
        else if (randNum > soldier2 && randNum <= dog) {
            return "Dog";
        }
        else if (randNum > dog && randNum <= tank) {
            return "tankRed";
        }
        else if(randNum > tank && randNum <= airplane) {
            return "airplane";
        }

        return "soldier";
    }

    public void RemoveEnemy(Enemy enemy) {
        ActiveEnemies.Remove(enemy);
        
        if (!WaveActive && !gameOver) {
            waveBtn.SetActive(true);
        }     
    }


    public bool ValidPath(TileScript hover) {
        // check if a path exists from start to goal, as well as every active enemy to goal
        /*       
        if(activeEnemies.Count > 0) {
            foreach(Enemy enemy in activeEnemies) {
                Stack<Node> path = AStar.GetPath(enemy.GridPosition, LevelManager.Instance.GoalSpawn, hover);
                if(path.Count <= 0) {
                    return false;
                }
            }
        }
        */
        Stack<Node> startPath = AStar.GetPath(LevelManager.Instance.PortalSpawn, LevelManager.Instance.GoalSpawn, hover);
        if(startPath.Count <= 0) {
            return false;
        }

        return true;
    }

    private void Shuffle(List<string> list) {
        int count = list.Count;
        int last = count - 1;
        for (int i = 0; i < last; ++i) {
            int r = Random.Range(i, count);
            string tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public void GameOver() {
        if (!gameOver) {
            gameOver = true;
            gameOverMenu.SetActive(true);
        }
    }
    public void Restart() {
        Time.timeScale = 1; // unpause game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame() {
        Application.Quit();
    }
}
