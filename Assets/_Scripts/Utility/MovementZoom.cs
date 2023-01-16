using UnityEngine;
public class MovementZoom : MonoBehaviour
{
    public Transform parentObject;
    public float zoomLevel;
    public float sensitivity = 1;
    public float speed = 30;
    public float maxZoom = 30;
    float zoomPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        zoomLevel += Input.mouseScrollDelta.y * sensitivity;
        zoomLevel = Mathf.Clamp(zoomLevel, 0, maxZoom);
        zoomPosition = Mathf.MoveTowards(zoomPosition, zoomLevel, speed * Time.deltaTime);
        transform.position = parentObject.position + (transform.forward * zoomPosition);
    }
}