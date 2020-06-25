using UnityEngine;

public class CS_SphereController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {}

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 5.0f));

            targetPosition.z = 561.0f;
            transform.position = targetPosition;
        }
    }
}
