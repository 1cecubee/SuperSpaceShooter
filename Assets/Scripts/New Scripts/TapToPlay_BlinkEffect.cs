using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TapToPlay_BlinkEffect : MonoBehaviour
{
    [SerializeField] private float blinkInterval;
    [SerializeField] private Image image;


    private void Start()
    {
        image = GetComponent<Image>();

        StartCoroutine(BlinkEffect());
    }

    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            if (image != null)
            {
                image.enabled = !image.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
