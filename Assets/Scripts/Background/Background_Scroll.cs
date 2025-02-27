using UnityEngine;

public class Background_Scroll : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Material mat;

    private float offset;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        offset += (Time.deltaTime * speed) / 10F;
        mat.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }
}
