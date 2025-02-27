using UnityEngine;

public class ScaleBackgroundToScreen : MonoBehaviour
{
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Material material;

    [SerializeField] private float scrollSpeed;


    private static readonly int speedHash = Shader.PropertyToID("_Speed");
    public float Speed { get => material.GetFloat(speedHash); private set => material.SetFloat(speedHash, value); }


    private void Reset()
    {
        if (TryGetComponent(out renderer))
        {
            material = renderer.sharedMaterial;
        }
    }

    private void Awake()
    {
        float screenWidthInWorldUnits = 2.0f * Camera.main.orthographicSize * Screen.width / Screen.height;
        float spriteWidth = renderer.sprite.bounds.size.x;

        transform.localScale = (screenWidthInWorldUnits / spriteWidth) * Vector3.one;
    }
    private void OnEnable()
    {
        Speed = scrollSpeed;
    }
    private void OnDisable()
    {
        Speed = 0F;
    }
}
