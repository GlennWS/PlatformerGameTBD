using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Speed")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("Camera target not set.");
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime * 50f
        );

        transform.position = smoothedPosition;
    }
}