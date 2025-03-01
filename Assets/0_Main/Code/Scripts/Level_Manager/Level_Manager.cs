using UnityEngine;

public class Level_Manager : MonoBehaviour
{
    public static Level_Manager instance;
   
    [Header("LEVEL SETTINGS")]
    public int currentLevel = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AdvanceLevel()
    {
        currentLevel++;
        Debug.Log("CURRENT LEVEL IS " + currentLevel);
    }
}
