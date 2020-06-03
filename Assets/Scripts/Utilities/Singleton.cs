using UnityEngine;

/// <summary>
/// Singleton class is the utility base class to define a Singleton
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // A Singleton is defined by its instance
    private static T instance;

    /// <summary>
    /// Instance properties access returns the instance of the Singleton
    /// </summary>
    public static T Instance
    {
        get { return instance; }
    }

    /// <summary>
    /// IsInitialized returns if the Singleton has an instance or not
    /// </summary>
    /// <returns>Is initialized or not</returns>
    public static bool IsInitialized()
    {
        return (instance != null);
    }

    /// <summary>
    /// On Awake, checks if the Singleton is unique and sets the instance as this element
    /// </summary>
    protected virtual void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("[Singleton] Trying to instanciate a second instance of a Singleton class");
        }
        else
        {
            instance = (T)this;
        }
    }

    /// <summary>
    /// OnDestroy, clears the instance if it was the Singleton instance
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
