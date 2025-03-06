using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UI_StartGame : MonoBehaviour
{
    [SerializeField] private UnityEvent startGame;
    [SerializeField] private TMP_Text missionScreen;

    private void Awake()
    {
        int level = Level_Manager.instance.currentLevel;

        missionScreen.GetComponent<TMP_Text>().text = ("MISSION " + level);
    }

    private void StartGame()
    {
        startGame.Invoke();
    }

}
