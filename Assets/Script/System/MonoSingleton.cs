using UnityEngine;

public abstract class MonoSingleton : MonoBehaviour
{
    protected static MonoSingleton _instance = null;

    void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
