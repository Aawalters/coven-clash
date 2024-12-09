using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        // if there's no instance, set this instance as the singleton
        if (Instance == null)
        {
            Instance = this as T;
            // optionally, make this object persistent across scenes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // if there's already an instance, destroy this one
            Destroy(gameObject);
        }
    }
}
