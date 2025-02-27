using UnityEngine;
using UnityEngine.Events;

public class UI_StartGame : MonoBehaviour
{
    [SerializeField] private UnityEvent startGame;


    private void StartGame()
    {
        startGame.Invoke();
    }

}
