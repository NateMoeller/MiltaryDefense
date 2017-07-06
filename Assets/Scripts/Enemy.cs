using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Enemy : MonoBehaviour {

    [SerializeField]
    private float speed;
    private Stack<Node> path;
    public Point GridPosition { get; set; }
    private Vector3 destination;
    public bool IsActive { get; set; }

    [SerializeField]
    private GameObject healthBar;
    private float maxHealth = 100f;
    private float curHealth;
    private Animator animator;

    public delegate void EnemyDelegate(GameObject enemy);
    public EnemyDelegate enemyDelegate;

    [SerializeField]
    private int scoreReward;
    [SerializeField]
    private int moneyReward;
    public string enemyType;

    public void TakeDamage(float amount, string bulletType) {
        // TODO: nerf amounts based on bullet and enemy type
        if (enemyType == "soldier") {
            if (bulletType == "electric") {
                amount = .1f;
            }
        }
        else if (enemyType == "dog") {
            if(bulletType == "electric" || bulletType == "rocket") {
                amount = .1f;
            }
        }
        else if(enemyType == "tank") {
            if(bulletType == "torpedo" || bulletType == "flame") {
                amount = .1f;
            }
        }


        curHealth -= amount;
        if(curHealth <= 0f) {
            if (IsActive) {
                GameManager.instance.Score += scoreReward;
                GameManager.instance.Currency += moneyReward;
            }
            Release();
            return;
        }
        float calcHealth = curHealth / maxHealth;
        SetHealthBar(calcHealth);
    }

    private void SetHealthBar(float health) {
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(health, 0f, 1f), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public void Spawn(Point start, Point goal) {
        animator = GetComponent<Animator>();
        curHealth = maxHealth;
        transform.position = LevelManager.Instance.StartPortal.transform.position;
        SetHealthBar(maxHealth);
        // this is where every enemy can generate their own path
        if(enemyType == "airplane") {
            SetPath(AStar.GetAirplanePath(start, goal));
        }
        else {
            SetPath(AStar.GetPath(start, goal));
        }
        


        // TODO: HARD CODED RIGHT
        animator.SetInteger("Horizontal", 1);
        animator.SetInteger("Vertical", 0);
        IsActive = true;
    }

    private void Update() {
        Move();
    }

    private void Move() {
        if (IsActive) {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            if (transform.position == destination) {
                if (path != null && path.Count > 0) {
                    Animate(GridPosition, this.path.Peek().GridPosition);
                    GridPosition = path.Peek().GridPosition;
                    destination = path.Pop().WorldPosition;
                }
            }

        }
        
    }

    public void SetPath(Stack<Node> newPath) {
        if(newPath != null) {
            this.path = newPath;
            Animate(GridPosition, this.path.Peek().GridPosition);
            GridPosition = this.path.Peek().GridPosition;
            destination = this.path.Pop().WorldPosition;
        }
    }

    private void Animate(Point curPosition, Point nextPostion) {
        if(curPosition.Y > nextPostion.Y) {
            // moving up
            animator.SetInteger("Horizontal", 0);
            animator.SetInteger("Vertical", 1);
        }
        else if(curPosition.Y < nextPostion.Y){
            // moving down
            animator.SetInteger("Horizontal", 0);
            animator.SetInteger("Vertical", -1);
        }
        else {
            // curPosition.Y == nextPosition.Y
            if(curPosition.X > nextPostion.X) {
                // moving left
                animator.SetInteger("Horizontal", -1);
                animator.SetInteger("Vertical", 0);
            }
            else if(curPosition.X < nextPostion.X) {
                // moving right
                animator.SetInteger("Horizontal", 1);
                animator.SetInteger("Vertical", 0);
            }
        }
        
    }

    public double distanceToGoal() {
        return Math.Sqrt(Math.Pow(GridPosition.X - LevelManager.Instance.GoalSpawn.X, 2) + Math.Pow(GridPosition.Y - LevelManager.Instance.GoalSpawn.Y, 2));
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "GoalPortal") {
            GameManager.instance.Lives--;
            Release();
        }
    }

    private void Release() {
        if (enemyDelegate != null) {
            enemyDelegate(gameObject);
        }

        IsActive = false;
        // TODO: Grid position of this enemy?! (7.7)
        GameManager.Instance.Pool.ReleaseObject(gameObject);

        // remove itself from the active list
        GameManager.Instance.RemoveEnemy(this);

    }
}
