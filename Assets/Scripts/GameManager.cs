using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake()
    {
        if( instance == null ) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {

    }
}