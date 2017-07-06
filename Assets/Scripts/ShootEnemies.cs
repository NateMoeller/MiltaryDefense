using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootEnemies : MonoBehaviour {

    private List<GameObject> enemiesInRange;
    private float lastShotTime;
    private TowerData towerData;

	// Use this for initialization
	void Start () {
	    enemiesInRange = new List<GameObject>();
        lastShotTime = Time.time;
        towerData = GetComponent<TowerData>();
    }

    void OnEnemyRelease(GameObject enemy) {
        enemiesInRange.Remove(enemy);
    }
	
    void OnTriggerEnter2D(Collider2D other) {
        if (towerData.type == "missle") {
            if (other.gameObject.tag.Equals("airplane")) {
                enemiesInRange.Add(other.gameObject);
                Enemy del = other.gameObject.GetComponent<Enemy>();
                del.enemyDelegate += OnEnemyRelease;
            }          
        }
        else {
            if (other.gameObject.tag.Equals("Enemy")){
                enemiesInRange.Add(other.gameObject);
                Enemy del = other.gameObject.GetComponent<Enemy>();
                del.enemyDelegate += OnEnemyRelease;
            }          
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(towerData.type == "missle") {
            if (other.gameObject.tag.Equals("airplane")) {
                enemiesInRange.Remove(other.gameObject);
                Enemy del = other.gameObject.GetComponent<Enemy>();
                del.enemyDelegate -= OnEnemyRelease;
            }          
        }
        else {
            if (other.gameObject.tag.Equals("Enemy")) {
                enemiesInRange.Remove(other.gameObject);
                Enemy del = other.gameObject.GetComponent<Enemy>();
                del.enemyDelegate -= OnEnemyRelease;
            }           
        }
    }

    private void Shoot(Collider2D target) {
        GameObject bulletPrefab = towerData.bullet;

        Vector3 startPosition = gameObject.transform.position;
        Vector3 targetPosition = target.transform.position;
        startPosition.z = bulletPrefab.transform.position.z;
        targetPosition.z = bulletPrefab.transform.position.z;

        // make a new bullet, set the start and target position
        GameObject newBullet = (GameObject)Instantiate(bulletPrefab);
        newBullet.transform.position = startPosition;
        Bullet bulletComp = newBullet.GetComponent<Bullet>();
        bulletComp.target = target.gameObject;
        bulletComp.startPosition = startPosition;
        bulletComp.targetPosition = targetPosition;

    }


    // Update is called once per frame
    void Update () {
	    double minimalEnemyDistance = double.MaxValue;
        GameObject target = null;

        // now find the target closest to the goal
        foreach (GameObject enemy in enemiesInRange) {
            double distanceToGoal = enemy.GetComponent<Enemy>().distanceToGoal();
            if (distanceToGoal < minimalEnemyDistance) {
                target = enemy;
                minimalEnemyDistance = distanceToGoal;
            }
        }

        if(target != null) {
            // rotate the tower towards the target
            Vector3 vectorToTarget = gameObject.transform.position - target.transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

            //gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), speed * Time.deltaTime);
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
            
            
            // shoot the target
            if (Time.time - lastShotTime > towerData.fireRate) {
                Shoot(target.GetComponent<Collider2D>());
                lastShotTime = Time.time;
            }
        }
        


    }
}
