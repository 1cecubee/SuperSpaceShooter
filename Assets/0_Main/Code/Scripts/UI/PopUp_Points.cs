using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PopUp_Points : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float speed;


    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
   
        Destroy(gameObject, 1F);
    }


    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
}
