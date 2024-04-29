using UnityEngine;

public class WorldSpaceUI : MonoBehaviour
{
    public GameObject uiElement;
    public float scale = 0.07f;
    private Transform mainCameraTransform;

    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Update the position to follow the unit
        uiElement.transform.position = transform.position;

        // Ensure the UI element always faces the camera
        uiElement.transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);

        // Update the scale based on the distance from the camera
        UpdateScale();
    }

    void UpdateScale()
    {
        // Calculate the distance between the unit and the camera
        float distanceToCamera = Vector3.Distance(transform.position, mainCameraTransform.position);

        // Adjust the scale based on the distance (customize the formula as needed)
        float scaleFactor = (distanceToCamera * scale); // Example formula, adjust as needed

        // Set the new scale for the UI element
        uiElement.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}