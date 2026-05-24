using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; protected set;}

    public virtual void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            
            if(Instance != this as T)
            {
               // Debug.Log("Destroying GameObject From Singleton : " + gameObject.transform.name);
                Destroy(gameObject);
            }
            else
            {
              //  Debug.Log("Singleton already exists : " + gameObject.transform.name);
            }
         
        }
    }

  
}

public class DontDestroySingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; protected set; }

    public virtual void OnEnable()
    {
        transform.parent = null;
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this as T)
            {
            //    Debug.Log("Destroying GameObject From Singleton : " + gameObject.transform.name);
                Destroy(gameObject);
            }
            else
            {
              //  Debug.Log("Singleton already exists : " + gameObject.transform.name);
            }
        }
    }
}
