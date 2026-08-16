using UnityEngine;

public class SkriptaProtivUnistenja : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
