using System;
using System.Collections;
using UnityEngine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CustomMath
{
    public struct Transform
    {
        public Vec3 position { get; }
        public Vec3 localPosition { get; }

        public Vec3 right { get; }

        public Vec3 up { get; }

        public Vec3 forward { get; }

        public Quat rotation { get; }

        public Quat localRotation { get; }

        public Vec3 eulerAngles { get; }

        public Vec3 localEulerAngles { get; }

        public Vec3 scale { get; }
        public Vec3 lossyScale { get; }
        public Vec3 localScale { get; }

        public Transform parent
        {
            set { }
        }

        private Transform GetParent()
        {
            throw new NotImplementedException();
        }

        public void SetParent(Transform p)
        {
            throw new NotImplementedException();
        }

        public Mat4x4 worldToLocalMatrix { get; }
        public Mat4x4 localToWorldMatrix { get; }

        public void SetPositionAndRotation(Vec3 position, Quat rotation)
        {
            throw new NotImplementedException();
        }

        public void SetLocalPositionAndRotation(Vec3 localPosition, Quat localRotation)
        {
            throw new NotImplementedException();
        }

        public void GetPositionAndRotation(out Vec3 position, out Quat rotation)
        {
            throw new NotImplementedException();
        }

        public void GetLocalPositionAndRotation(out Vec3 localPosition, out Quat localRotation)
        {
            throw new NotImplementedException();
        }

        public void Translate(Vec3 translation, [DefaultValue("Space.Self")] Space relativeTo)
        {
            throw new NotImplementedException();
        }

        public void Translate(Vec3 translation)
        {
            throw new NotImplementedException();
        }

        public void Translate(float x, float y, float z, [DefaultValue("Space.Self")] Space relativeTo)
        {
            throw new NotImplementedException();
        }

        public void Translate(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public void Translate(Vec3 translation, Transform relativeTo)
        {
            throw new NotImplementedException();
        }

        public void Translate(float x, float y, float z, Transform relativeTo)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Vec3 eulers, [DefaultValue("Space.Self")] Space relativeTo)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Vec3 eulers)
        {
            throw new NotImplementedException();
        }

        public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            throw new NotImplementedException();
        }

        public void Rotate(float xAngle, float yAngle, float zAngle)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Vec3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            throw new NotImplementedException();
        }

        public void Rotate(Vec3 axis, float angle)
        {
            throw new NotImplementedException();
        }

        public void RotateAround(Vec3 point, Vec3 axis, float angle)
        {
            throw new NotImplementedException();
        }

        public void LookAt(Transform target, [DefaultValue("Vector3.up")] Vec3 worldUp)
        {
            throw new NotImplementedException();
        }

        public void LookAt(Transform target)
        {
            throw new NotImplementedException();
        }

        public void LookAt(Vec3 worldPosition, [DefaultValue("Vector3.up")] Vec3 worldUp)
        {
            throw new NotImplementedException();
        }

        public void LookAt(Vec3 worldPosition)
        {
            throw new NotImplementedException();
        }

        public Vec3 TransformDirection(Vec3 direction)
        {
            throw new NotImplementedException();
        }

        public Vec3 TransformDirection(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public void TransformDirections(
            ReadOnlySpan<Vec3> directions,
            Span<Vec3> transformedDirections)
        {
            throw new NotImplementedException();
        }

        public void TransformDirections(Span<Vec3> directions)
        {
            throw new NotImplementedException();
        }

        public Vec3 InverseTransformDirection(Vec3 direction)
        {
            throw new NotImplementedException();
        }

        public Vec3 InverseTransformDirection(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public void InverseTransformDirections(
            ReadOnlySpan<Vec3> directions,
            Span<Vec3> transformedDirections)
        {
            throw new NotImplementedException();
        }

        public void InverseTransformDirections(Span<Vec3> directions)
        {
            throw new NotImplementedException();
        }

        public Vec3 TransformVector(Vec3 vector)
        {
            throw new NotImplementedException();
        }

        public Vec3 TransformVector(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public void TransformVectors(ReadOnlySpan<Vec3> vectors, Span<Vec3> transformedVectors)
        {
            throw new NotImplementedException();
        }

        public void TransformVectors(Span<Vec3> vectors)
        {
            throw new NotImplementedException();
        }

        public Vec3 InverseTransformVector(Vec3 vector)
        {
            throw new NotImplementedException();
        }

        public Vec3 InverseTransformVector(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public void InverseTransformVectors(
            ReadOnlySpan<Vec3> vectors,
            Span<Vec3> transformedVectors)
        {
            throw new NotImplementedException();
        }

        public void InverseTransformVectors(Span<Vec3> vectors)
        {
            throw new NotImplementedException();
        }

        public Vec3 TransformPoint(Vec3 position)
        {
            throw new NotImplementedException();
        }

        public Vec3 TransformPoint(float x, float y, float z)
        {
            return this.TransformPoint(new Vec3(x, y, z));
        }

        public void TransformPoints(ReadOnlySpan<Vec3> positions, Span<Vec3> transformedPositions)
        {
            throw new NotImplementedException();
        }

        public void TransformPoints(Span<Vec3> positions)
        {
            this.TransformPoints((ReadOnlySpan<Vec3>)positions, positions);
        }

        public Vec3 InverseTransformPoint(Vec3 position)
        {
            throw new NotImplementedException();
        }

        public Vec3 InverseTransformPoint(float x, float y, float z)
        {
            return this.InverseTransformPoint(new Vec3(x, y, z));
        }

        public void InverseTransformPoints(
            ReadOnlySpan<Vec3> positions,
            Span<Vec3> transformedPositions)
        {
            throw new NotImplementedException();
        }

        public void InverseTransformPoints(Span<Vec3> positions)
        {
            this.InverseTransformPoints((ReadOnlySpan<Vec3>)positions, positions);
        }

        public Transform root => this.GetRoot();
        private Transform GetRoot()
        {
            throw new NotImplementedException();
        }
        
        public void SetAsFirstSibling()
        {
            throw new NotImplementedException();
        }
        
        public void SetAsLastSibling()
        {
            throw new NotImplementedException();
        }
        public void SetSiblingIndex(int index)
        {
            throw new NotImplementedException();
        }
        public int GetSiblingIndex()
        {
            throw new NotImplementedException();
        }
        public Transform Find(string n)
        {
            throw new NotImplementedException();
        }
        public bool IsChildOf([NotNull] Transform parent)
        {
            throw new NotImplementedException();
        }
        
        public bool hasChanged
        {
            get { throw new NotImplementedException(); }
            set  { throw new NotImplementedException(); }
        }
        
        public Transform GetChild(int index)
        {
            throw new NotImplementedException();
        }
        public int GetChildCount()
        {
            throw new NotImplementedException();
        }
    }
}