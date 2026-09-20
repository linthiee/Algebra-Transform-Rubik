using System;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

namespace CustomMath
{
    public class Transform
    {
        private Transform _parent;
        private readonly List<Transform> _children = new List<Transform>();
        private bool _hasChanged = true;

        private Vec3 _localPosition;
        private Quat _localRotation;
        private Vec3 _localScale;

        public Vec3 localPosition
        {
            get => _localPosition;
            set
            {
                _localPosition = value;
                hasChanged = true;
            }
        }

        public Quat localRotation
        {
            get => _localRotation;
            set
            {
                _localRotation = value;
                hasChanged = true;
            }
        }

        public Vec3 localScale
        {
            get => _localScale;
            set
            {
                _localScale = value;
                hasChanged = true;
            }
        }

        public Vec3 position
        {
            get
            {
                if (parent == null)
                    return localPosition;

                return parent.localToWorldMatrix.MultiplyPoint3x4(localPosition);
            }
            set
            {
                if (parent == null)
                {
                    localPosition = value;
                }
                else
                {
                    localPosition = parent.worldToLocalMatrix.MultiplyPoint3x4(value);
                }
            }
        }

        public Quat rotation
        {
            get
            {
                if (parent == null)
                    return localRotation;

                return parent.rotation * localRotation;
            }
            set
            {
                if (parent == null)
                    localRotation = value;
                else
                    localRotation = Quat.Inverse(parent.rotation) * value;
            }
        }

        public Vec3 lossyScale
        {
            get
            {
                if (parent == null)
                    return localScale;

                Mat4x4 scale = this.localToWorldMatrix;

                return scale.lossyScale;
            }
        }

        public Vec3 right
        {
            get => this.rotation * Vec3.Right;
            set => this.rotation = Quat.FromToRotation(this.right, value) * this.rotation;
        }

        public Vec3 up
        {
            get => this.rotation * Vec3.Up;
            set => this.rotation = Quat.FromToRotation(this.up, value) * this.rotation;
        }

        public Vec3 forward
        {
            get => this.rotation * Vec3.Forward;
            set => this.rotation = Quat.LookRotation(value);
        }

        public Vec3 eulerAngles
        {
            get => this.rotation.eulerAngles;
            set => this.rotation = Quat.Euler(value);
        }

        public Vec3 localEulerAngles
        {
            get => this.localRotation.eulerAngles;
            set => this.localRotation = Quat.Euler(value);
        }

        public Transform parent
        {
            get => this.GetParent();
            set => this.SetParent(value);
        }

        private Transform GetParent() => _parent;

        public void SetParent(Transform p) => this.SetParent(p, true);

        public void SetParent(Transform parent, bool worldPositionStays)
        {
            if (_parent == parent)
                return;

            Vec3 worldPos = this.position;
            Quat worldRot = this.rotation;

            if (_parent != null)
                _parent._children.Remove(this);

            _parent = parent;
            if (_parent != null)
                _parent._children.Add(this);

            if (worldPositionStays)
            {
                this.position = worldPos;
                this.rotation = worldRot;
            }
        }

        public Mat4x4 worldToLocalMatrix
        {
            get { return localToWorldMatrix.inverse; }
        }

        public Mat4x4 localToWorldMatrix
        {
            get
            {
                Mat4x4 trs = Mat4x4.TRS(localPosition, localRotation, localScale);

                if (parent != null)
                {
                    return parent.localToWorldMatrix * trs;
                }

                return trs;
            }
        }

        public void SetPositionAndRotation(Vec3 position, Quat rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public void SetLocalPositionAndRotation(Vec3 localPosition, Quat localRotation)
        {
            this.localPosition = localPosition;
            this.localRotation = localRotation;
        }

        public void GetPositionAndRotation(out Vec3 position, out Quat rotation)
        {
            position = this.position;
            rotation = this.rotation;
        }

        public void GetLocalPositionAndRotation(out Vec3 localPosition, out Quat localRotation)
        {
            localPosition = this.localPosition;
            localRotation = this.localRotation;
        }

        public void Translate(Vec3 translation, [DefaultValue("Space.Self")] Space relativeTo)
        {
            if (relativeTo == Space.World)
                this.position += translation;
            else
                this.position += this.TransformDirection(translation);
        }

        public void Translate(Vec3 translation) => this.Translate(translation, Space.Self);

        public void Translate(float x, float y, float z, [DefaultValue("Space.Self")] Space relativeTo)
        {
            this.Translate(new Vec3(x, y, z), relativeTo);
        }

        public void Translate(float x, float y, float z)
        {
            this.Translate(new Vec3(x, y, z), Space.Self);
        }

        public void Translate(Vec3 translation, Transform relativeTo)
        {
            if (relativeTo != null)
                this.position += relativeTo.TransformDirection(translation);
            else
                this.position += translation;
        }

        public void Translate(float x, float y, float z, Transform relativeTo)
        {
            this.Translate(new Vec3(x, y, z), relativeTo);
        }

        public void Rotate(Vec3 eulers, [DefaultValue("Space.Self")] Space relativeTo)
        {
            Quat quaternion = Quat.Euler(new Vec3(eulers.x, eulers.y, eulers.z));
            if (relativeTo == Space.Self)
                this.localRotation *= quaternion;
            else
                this.rotation = quaternion * this.rotation;
        }

        public void Rotate(Vec3 eulers) => this.Rotate(eulers, Space.Self);

        public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            this.Rotate(new Vec3(xAngle, yAngle, zAngle), relativeTo);
        }

        public void Rotate(float xAngle, float yAngle, float zAngle)
        {
            this.Rotate(new Vec3(xAngle, yAngle, zAngle), Space.Self);
        }

        public void Rotate(Vec3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            Quat quaternion = Quat.AngleAxis(angle, axis);
            if (relativeTo == Space.Self)
                this.localRotation *= quaternion;
            else
                this.rotation = quaternion * this.rotation;
        }

        public void Rotate(Vec3 axis, float angle) => this.Rotate(axis, angle, Space.Self);

        public void RotateAround(Vec3 point, Vec3 axis, float angle)
        {
            Vec3 direction = this.position - point;

            Quat quaternion = Quat.AngleAxis(angle, axis);
            direction = quaternion * direction;

            this.position = point + direction;
            this.Rotate(axis, angle, Space.World);
        }

        public void LookAt(Transform target, [DefaultValue("Vector3.up")] Vec3 worldUp)
        {
            LookAt(target.position, worldUp);
        }

        public void LookAt(Transform target)
        {
            LookAt(target.position, Vec3.Up);
        }

        public void LookAt(Vec3 worldPosition, [DefaultValue("Vector3.up")] Vec3 worldUp)
        {
            Vec3 forward = worldPosition - this.position;
            const float epsilon = 1e-5f;

            if (forward.sqrMagnitude > epsilon)
            {
                this.rotation = Quat.LookRotation(forward, worldUp);
            }
        }

        public void LookAt(Vec3 worldPosition)
        {
            LookAt(worldPosition, Vec3.Up);
        }

        public Vec3 TransformDirection(Vec3 direction) => this.rotation * direction;

        public Vec3 TransformDirection(float x, float y, float z)
        {
            return this.TransformDirection(new Vec3(x, y, z));
        }

        public void TransformDirections(
            ReadOnlySpan<Vec3> directions,
            Span<Vec3> transformedDirections)
        {
            for (int i = 0; i < directions.Length; i++)
                transformedDirections[i] = TransformDirection(directions[i]);
        }
        
        public void TransformDirections(Span<Vec3> directions) => TransformDirections(directions, directions);

        public Vec3 InverseTransformDirection(Vec3 direction) => Quat.Inverse(this.rotation) * direction;

        public Vec3 InverseTransformDirection(float x, float y, float z)
        {
            return this.InverseTransformDirection(new Vec3(x, y, z));
        }

        public void InverseTransformDirections(
            ReadOnlySpan<Vec3> directions,
            Span<Vec3> transformedDirections)
        {
            for (int i = 0; i < directions.Length; i++)
                transformedDirections[i] = InverseTransformDirection(directions[i]);
        }
        
        public void InverseTransformDirections(Span<Vec3> directions) => InverseTransformDirections(directions, directions);

        public Vec3 TransformVector(Vec3 vector) => this.localToWorldMatrix.MultiplyVector(vector);

        public Vec3 TransformVector(float x, float y, float z)
        {
            return this.TransformVector(new Vec3(x, y, z));
        }

        public Vec3 InverseTransformVector(Vec3 vector) => this.worldToLocalMatrix.MultiplyVector(vector);

        public Vec3 InverseTransformVector(float x, float y, float z)
        {
            return this.InverseTransformVector(new Vec3(x, y, z));
        }

        public void InverseTransformVectors(
            ReadOnlySpan<Vec3> vectors,
            Span<Vec3> transformedVectors)
        {
            for (int i = 0; i < vectors.Length; i++)
                transformedVectors[i] = InverseTransformVector(vectors[i]);
        }
        
        public void InverseTransformVectors(Span<Vec3> vectors) => InverseTransformVectors(vectors, vectors);

        public Vec3 TransformPoint(Vec3 position) => this.localToWorldMatrix.MultiplyPoint3x4(position);

        public Vec3 TransformPoint(float x, float y, float z)
        {
            return this.TransformPoint(new Vec3(x, y, z));
        }

        public void TransformPoints(ReadOnlySpan<Vec3> positions, Span<Vec3> transformedPositions)
        {
            for (int i = 0; i < positions.Length; i++)
                transformedPositions[i] = TransformPoint(positions[i]);
        }
        
        public void TransformPoints(Span<Vec3> positions)
        {
            this.TransformPoints((ReadOnlySpan<Vec3>)positions, positions);
        }

        public Vec3 InverseTransformPoint(Vec3 position) => this.worldToLocalMatrix.MultiplyPoint3x4(position);

        public Vec3 InverseTransformPoint(float x, float y, float z)
        {
            return this.InverseTransformPoint(new Vec3(x, y, z));
        }

        public void InverseTransformPoints(
            ReadOnlySpan<Vec3> positions,
            Span<Vec3> transformedPositions)
        {
            for (int i = 0; i < positions.Length; i++)
                transformedPositions[i] = InverseTransformPoint(positions[i]);
        }
        
        public void InverseTransformPoints(Span<Vec3> positions)
        {
            this.InverseTransformPoints((ReadOnlySpan<Vec3>)positions, positions);
        }

        public Transform root => this.GetRoot();

        private Transform GetRoot()
        {
            Transform current = this;
            while (current.parent != null)
            {
                current = current.parent;
            }

            return current;
        }

        public void SetAsFirstSibling()
        {
            SetSiblingIndex(0);
        }

        public void SetAsLastSibling()
        {
            if (_parent != null)
                SetSiblingIndex(_parent._children.Count - 1);
        }

        public void SetSiblingIndex(int index)
        {
            if (_parent == null)
                return;

            _parent._children.Remove(this);
            index = Math.Clamp(index, 0, _parent._children.Count);
            _parent._children.Insert(index, this);
        }

        public int GetSiblingIndex()
        {
            return _parent != null ? _parent._children.IndexOf(this) : 0;
        }

        public bool hasChanged
        {
            get => _hasChanged;
            set => _hasChanged = value;
        }

        public Transform GetChild(int index) => _children[index];

        public int GetChildCount() => _children.Count;
        
        public bool IsChildOf(Transform parent)
        {
            if (parent == null)
                return false;
            
            Transform current = this.parent;
            while (current != null)
            {
                if (current == parent) 
                    return true;
                
                current = current.parent;
            }
            return false;
        }
    }
}