using UnityEngine;
using CustomMath;

public class CustomTransformSync : MonoBehaviour
{
    public CustomMath.Transform customTransform = new CustomMath.Transform();

    void Awake()
    {
        customTransform.localPosition = new Vec3(transform.localPosition);
        customTransform.localRotation = new Quat(transform.localRotation);
        customTransform.localScale = new Vec3(transform.localScale);
    }

    void LateUpdate()
    {
        transform.localPosition = customTransform.localPosition;
        transform.localRotation = customTransform.localRotation;
        transform.localScale = customTransform.localScale;
    }
}