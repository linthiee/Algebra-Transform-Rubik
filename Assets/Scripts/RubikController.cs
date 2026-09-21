using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using CustomMath;

public class RubikController : MonoBehaviour
{
    public CustomTransformSync root;

    [Header("Axis")] public CustomTransformSync axisRight;
    public CustomTransformSync axisLeft;
    public CustomTransformSync axisUp;
    public CustomTransformSync axisDown;
    public CustomTransformSync axisFront;
    public CustomTransformSync axisBack;

    [Header("Free pieces")] public List<CustomTransformSync> freeCubes = new List<CustomTransformSync>();

    private bool isRotating = false;

    void Start()
    {
        axisRight.customTransform.SetParent(root.customTransform, false);
        axisLeft.customTransform.SetParent(root.customTransform, false);
        axisUp.customTransform.SetParent(root.customTransform, false);
        axisDown.customTransform.SetParent(root.customTransform, false);
        axisFront.customTransform.SetParent(root.customTransform, false);
        axisBack.customTransform.SetParent(root.customTransform, false);

        foreach (CustomTransformSync cube in freeCubes)
        {
            cube.customTransform.SetParent(root.customTransform, true);
            cube.transform.SetParent(root.transform, true);
        }
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.rightArrowKey.isPressed)
            root.customTransform.Translate(new Vec3(3, 0, 0) * Time.deltaTime, Space.World);
        if (Keyboard.current.leftArrowKey.isPressed)
            root.customTransform.Translate(new Vec3(-3, 0, 0) * Time.deltaTime, Space.World);
        if (Keyboard.current.upArrowKey.isPressed)
            root.customTransform.Translate(new Vec3(0, 3, 0) * Time.deltaTime, Space.World);
        if (Keyboard.current.downArrowKey.isPressed)
            root.customTransform.Translate(new Vec3(0, -3, 0) * Time.deltaTime, Space.World);

        if (Keyboard.current.spaceKey.isPressed)
            root.customTransform.Rotate(new Vec3(0, 1, 0), 90f * Time.deltaTime, Space.World);

        if (Keyboard.current.pKey.isPressed)
            root.customTransform.localScale += new Vec3(1, 1, 1) * Time.deltaTime;
        if (Keyboard.current.mKey.isPressed)
            root.customTransform.localScale -= new Vec3(1, 1, 1) * Time.deltaTime;

        if (isRotating)
            return;

        if (Keyboard.current.wKey.wasPressedThisFrame)
            StartCoroutine(RotateFaceRoutine(axisUp, Vec3.Up, 90f, 0.5f));
        if (Keyboard.current.sKey.wasPressedThisFrame)
            StartCoroutine(RotateFaceRoutine(axisDown, Vec3.Down, 90f, 0.5f));
        if (Keyboard.current.dKey.wasPressedThisFrame)
            StartCoroutine(RotateFaceRoutine(axisRight, Vec3.Right, 90f, 0.5f));
        if (Keyboard.current.aKey.wasPressedThisFrame)
            StartCoroutine(RotateFaceRoutine(axisLeft, Vec3.Left, 90f, 0.5f));
        if (Keyboard.current.qKey.wasPressedThisFrame)
            StartCoroutine(RotateFaceRoutine(axisFront, Vec3.Forward, 90f, 0.5f));
        if (Keyboard.current.eKey.wasPressedThisFrame)
            StartCoroutine(RotateFaceRoutine(axisBack, Vec3.Back, 90f, 0.5f));
    }

    private IEnumerator RotateFaceRoutine(CustomTransformSync axisSync, Vec3 rootLocalDirection, float angle,
        float duration)
    {
        isRotating = true;
        CustomMath.Transform axis = axisSync.customTransform;

        List<CustomTransformSync> activeCubes = GetCubesOnFace(rootLocalDirection);

        foreach (CustomTransformSync cube in activeCubes)
        {
            cube.customTransform.SetParent(axis, true);
            cube.transform.SetParent(axisSync.transform, true);
        }

        float elapsed = 0f;

        Quat startLocalRot = axis.localRotation;
        Quat endLocalRot = Quat.AngleAxis(angle, rootLocalDirection) * startLocalRot;

        while (elapsed < duration)
        {
            axis.localRotation = Quat.Slerp(startLocalRot, endLocalRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        axis.localRotation = endLocalRot;

        foreach (CustomTransformSync cube in activeCubes)
        {
            cube.customTransform.SetParent(root.customTransform, true);
            cube.transform.SetParent(root.transform, true);
        }

        isRotating = false;
    }

    private List<CustomTransformSync> GetCubesOnFace(Vec3 expectedLocalDirection)
    {
        List<CustomTransformSync> result = new List<CustomTransformSync>();

        foreach (CustomTransformSync cubeSync in freeCubes)
        {
            Vec3 localPosRelativeToRoot = root.customTransform.worldToLocalMatrix.MultiplyPoint3x4(cubeSync.customTransform.position);

            if (Vec3.Dot(localPosRelativeToRoot, expectedLocalDirection) > 0.5f)
            {
                result.Add(cubeSync);
            }
        }

        //Debug.Log("Size " + result.Count);

        return result;
    }
}