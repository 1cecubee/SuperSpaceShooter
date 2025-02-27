using UnityEngine;

public class TEMP_SCRIPT : MonoBehaviour
{
    [SerializeField] private GameObject targetObject, popupText, anim;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 offset;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(anim, targetObject.transform.position, default);

            HAHA();
        }

        //transform.Translate(Vector2.down * speed * Time.deltaTime);
    }


    private void HAHA()
    {
        Vector3 targetPosition = targetObject.transform.position;

        // Step 2: Optionally, add an offset (e.g., spawn slightly above or beside the target)
        targetPosition += offset;

        // Step 3: Instantiate the object at the target's position
        GameObject spawnedObject = Instantiate(popupText, targetPosition, Quaternion.identity);

        // Optionally, adjust the rotation of the spawned object (if needed)

    }
}
