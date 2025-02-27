using TMPro;
using UnityEngine;

public class FPS_Display : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText; // Assign this in the Inspector
    private float deltaTime;

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
    }
}
