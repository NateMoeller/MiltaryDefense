using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

    [SerializeField]
    private GameObject[] objectPrefabs;
    private List<GameObject> pooledObjects = new List<GameObject>();


    public GameObject GetObject(string type) {
        // check to re-use gameobjects
        foreach (GameObject go in pooledObjects) {
            if(go.name == type && !go.active) {
                go.SetActive(true);
                return go;
            }
        }

        // need to make a new game object
        for (int i = 0; i < objectPrefabs.Length; i++) {
            if (objectPrefabs[i].name == type) {
                GameObject newObject = Instantiate(objectPrefabs[i]);
                newObject.name = type;

                pooledObjects.Add(newObject);
                return newObject;
            }
            
        }
        return null;
    }

    public void ReleaseObject(GameObject gameObject) {
        gameObject.SetActive(false);
    }

}
