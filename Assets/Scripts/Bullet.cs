using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private float speed, damage;
    [SerializeField]
    private string type;

    public GameObject target;
    public Vector3 startPosition;
    public Vector3 targetPosition;

    private float distance;
    private float startTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        distance = Vector3.Distance(startPosition, targetPosition);


	}
	
	// Update is called once per frame
	void Update () {
        float timeInterval = Time.time - startTime;
        gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, timeInterval * speed / distance);

        // rotate the bullet to face the target
        Vector3 vectorToTarget = gameObject.transform.position - target.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));

        if (gameObject.transform.position.Equals(targetPosition)) {
            target.GetComponent<Enemy>().TakeDamage(damage, type);

            Destroy(gameObject); // destroy the bullet
        }

    }
}
