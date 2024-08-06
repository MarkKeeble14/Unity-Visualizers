using UnityEngine;

public class FollowActiveCamera : MonoBehaviour
{
    [SerializeField] private float followSpeed = 1;
    [SerializeField] private MathHelper.AlterationMethod mathMethod = MathHelper.AlterationMethod.LERP;
    [SerializeField] private Vector3 offset = Vector3.zero;
    private Vector3 currentValue;

    private void Update()
    {
        currentValue = MathHelper.GetNextValue(currentValue, Camera.main.transform.position + offset, followSpeed, mathMethod, true);
        transform.position = currentValue;
    }
}
