using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform followTarget;
    public Transform lookTarget;

    public Vector3 offset = new Vector3(0f, 2.5f, -5f);
    public float smoothTime = 0.2f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        Quaternion yawRotaion = Quaternion.Euler(0f, followTarget.eulerAngles.y, 0f);

        Vector3 desirePosition = followTarget.position + yawRotaion * offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desirePosition,
            ref velocity,
            smoothTime
            );

        transform.LookAt(lookTarget);
    }
}
