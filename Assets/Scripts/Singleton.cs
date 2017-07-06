using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

    public static T instance;


    public static T Instance{
        get{
            if(instance == null){
                instance = FindObjectOfType<T>();
            }
            return instance;
        }
    }
}
