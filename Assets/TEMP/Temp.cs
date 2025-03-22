using UnityEngine;

public class Temp : MonoBehaviour
{
    [SerializeField] private Vector3 touchLocation;
    [SerializeField] private Camera mainCamera;


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchLocation = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));
                touchLocation.z = 0;
            }

            transform.position = touchLocation;
        }
    }
}
