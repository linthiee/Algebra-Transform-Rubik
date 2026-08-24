using System;
using UnityEngine;
using System.ComponentModel;
using CustomMath;
using Transform = CustomMath.Transform;

public struct Quat
{
    const float epsilon = 1e-5f;

#pragma region Variables
    public float x;
    public float y;
    public float z;
    public float w;
#pragma endregion

#pragma region Properties
    public static Quat identity => new Quat(0f, 0f, 0f, 1f);
    public Vec3 eulerAngles => ToEulerAngles(this);
    public Quat normalized => Normalize(this);
#pragma endregion

#pragma region Constructors
    public Quat(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Quat(Quat other)
    {
        this.x = other.x;
        this.y = other.y;
        this.z = other.z;
        this.w = other.w;
    }

    public Quat(Quaternion unityQuat)
    {
        this.x = unityQuat.x;
        this.y = unityQuat.y;
        this.z = unityQuat.z;
        this.w = unityQuat.w;
    }
#pragma endregion

#pragma region Operators
    public static bool operator ==(Quat lhs, Quat rhs)
    {
        return Math.Abs(lhs.x - rhs.x) < epsilon &&
               Math.Abs(lhs.y - rhs.y) < epsilon &&
               Math.Abs(lhs.z - rhs.z) < epsilon &&
               Math.Abs(lhs.w - rhs.w) < epsilon;
    }

    public static bool operator !=(Quat lhs, Quat rhs) => !(lhs == rhs);

    public static Quat operator *(Quat lhs, Quat rhs)
    {
        return new Quat
        (
            lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
            lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
            lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
            lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z
        );
    }

    public static Vec3 operator *(Quat rotation, Vec3 point)
    {
        Vec3 temp = 2 * Vec3.Cross(new Vec3(rotation.x, rotation.y, rotation.z), point);

        return new Vec3(point + rotation.w * temp + Vec3.Cross(new Vec3(rotation.x, rotation.y, rotation.z), temp));
    }

    public static implicit operator Quat(Quaternion q) => new Quat(q);
    public static implicit operator Quaternion(Quat q) => new Quaternion(q.x, q.y, q.z, q.w);

#pragma endregion
#pragma region Static

    static float Angle(Quat a, Quat b)
    {
        return 2 * Mathf.Acos(Dot(a, b));
    }

   public static Quat AngleAxis(float angle, Vec3 axis)
    {
        Vec3 normAxis = axis.normalized;
        float halfAngleRad = angle * Mathf.Deg2Rad / 2;

        float sin = (float)Math.Sin(halfAngleRad);
        float cos = (float)Math.Cos(halfAngleRad);

        return new Quat
        (
            normAxis.x * sin,
            normAxis.y * sin,
            normAxis.z * sin,
            cos
        );
    }

    public static float Dot(Quat a, Quat b) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

    static Quat Euler(Vec3 euler)
    {
        float cx = (float)Math.Cos(euler.x * Mathf.Deg2Rad / 2);
        float sx = (float)Math.Sin(euler.x * Mathf.Deg2Rad / 2);

        float cy = (float)Math.Cos(euler.y * Mathf.Deg2Rad / 2);
        float sy = (float)Math.Sin(euler.y * Mathf.Deg2Rad / 2);

        float cz = (float)Math.Cos(euler.z * Mathf.Deg2Rad / 2);
        float sz = (float)Math.Sin(euler.z * Mathf.Deg2Rad / 2);

        return new Quat
        (
            sx * cy * cz + cx * sy * sz, // x
            cx * sy * cz - sx * cy * sz, // y
            cx * cy * sz - sx * sy * cz, // z
            cx * cy * cz + sx * sy * sz // w
        );
    }

    static Quat EulerAngles(Vec3 euler) => Euler(euler);

    static Quat EulerRotation(Vec3 euler) => Euler(euler);

    static Quat FromToRotation(Vec3 fromDirection, Vec3 toDirection)
    {
        Vec3 from = fromDirection.normalized;
        Vec3 to = toDirection.normalized;

        float dot = Vec3.Dot(from, to);

        if (dot < -0.999999f) //if vectors are exactly opposites (180 deg) chose an arbitrary axis
        {
            //random perpendicular vec
            Vec3 perpendicular = (Math.Abs(from.z) < 0.9999f) ? new Vec3(0, 0, 1) : new Vec3(1, 0, 0);
            Vec3 axis = Vec3.Cross(from, perpendicular).normalized;

            return new Quat(axis.x, axis.y, axis.z, 0f);
        }
        else if (dot > 0.999999f) //if vectors already point to the same direction
        {
            return new Quat(identity);
        }

        Vec3 cross = Vec3.Cross(from, to);

        Quat q = new Quat
        (
            cross.x,
            cross.y,
            cross.z,
            1f + dot
        );

        return q.normalized;
    }

    public static Quat Inverse(Quat rotation) => new Quat(-rotation.x, -rotation.y, -rotation.z, rotation.w);

    static Quat Lerp(Quat a, Quat b, float t)
    {
        return LerpUnclamped(a, b, Mathf.Clamp01(t));
    }

    static Quat LerpUnclamped(Quat a, Quat b, float t)
    {
        float dot = Dot(a, b);
        float sign = (dot < 0f) ? -1f : 1f;

        Quat q = new Quat
        (
            a.x + (b.x * sign - a.x) * t,
            a.y + (b.y * sign - a.y) * t,
            a.z + (b.z * sign - a.z) * t,
            a.w + (b.w * sign - a.w) * t
        );

        return Normalize(q);
    }

    static Quat LookRotation(Vec3 forward)
    {
        return LookRotation(forward, new Vec3(0, 1, 0));
    }

    static Quat LookRotation(Vec3 forward, [DefaultValue("Vec3.up")] Vec3 upwards)
    {
        Vec3 fwd = forward.normalized;
        
        if (fwd.x == 0 && fwd.y == 0 && fwd.z == 0)
            return identity;

        Vec3 right = Vec3.Cross(upwards, fwd).normalized;
        Vec3 up = Vec3.Cross(fwd, right);

        if (right.x == 0 && right.y == 0 && right.z == 0)
            return identity;

        // extract values from imaginary 3x3 rot mat
        float m00 = right.x;
        float m01 = up.x;
        float m02 = fwd.x;
        
        float m10 = right.y;
        float m11 = up.y;
        float m12 = fwd.y;
        
        float m20 = right.z;
        float m21 = up.z; 
        float m22 = fwd.z;
        
        float trace = m00 + m11 + m22;
        Quat quat = new Quat();

        if (trace > epsilon)
        {
            float s = Mathf.Sqrt(trace + 1f);
            quat.w = s * 0.5f;
            s = 0.5f / s;
            quat.x = (m21 - m12) * s;
            quat.y = (m02 - m20) * s;
            quat.z = (m10 - m01) * s;
        }
        else if ((m00 >= m11) && (m00 >= m22))
        {
            float s = Mathf.Sqrt(((1f + m00) - m11) - m22);
            float invS = 0.5f / s;
            quat.x = 0.5f * s;
            quat.y = (m01 + m10) * invS;
            quat.z = (m02 + m20) * invS;
            quat.w = (m21 - m12) * invS;
        }
        else if (m11 > m22)
        {
            float s = Mathf.Sqrt(((1f + m11) - m00) - m22);
            float invS = 0.5f / s;
            quat.x = (m10 + m01) * invS;
            quat.y = 0.5f * s;
            quat.z = (m21 + m12) * invS;
            quat.w = (m02 - m20) * invS;
        }
        else
        {
            float s = Mathf.Sqrt(((1f + m22) - m00) - m11);
            float invS = 0.5f / s;
            quat.x = (m20 + m02) * invS;
            quat.y = (m21 + m12) * invS;
            quat.z = 0.5f * s;
            quat.w = (m10 - m01) * invS;
        }

        return quat;
        
    }

    public static Quat Normalize(Quat q)
    {
        float mag = Mathf.Sqrt(Dot(q, q));

        if (mag < epsilon)
            return identity;

        return new Quat(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
    }

    static Quat RotateTowards(Quat from, Quat to, float maxDegreesDelta)
    {
        float dot = Dot(from, to);

        float angle = Mathf.Acos(Mathf.Min(Mathf.Abs(dot), 1f)) * 2f * Mathf.Rad2Deg;

        if (angle == 0f)
            return to;

        float t = Mathf.Min(1f, maxDegreesDelta / angle);
        return SlerpUnclamped(from, to, t);
    }

    static Quat Slerp(Quat a, Quat b, float t)
    {
        return SlerpUnclamped(a, b, Mathf.Clamp01(t));
    }

    static Quat SlerpUnclamped(Quat a, Quat b, float t)
    {
        float dot = Dot(a, b);

        if (dot < 0.0f) //shorter path
        {
            dot = -dot;
            b = new Quat(-b.x, -b.y, -b.z, -b.w);
        }

        if (dot > 0.9995f)
        {
            return LerpUnclamped(a, b, t);
        }

        float theta0 = Mathf.Acos(dot);
        float theta = theta0 * t;

        float sinTheta = Mathf.Sin(theta);
        float sinTheta0 = Mathf.Sin(theta0);

        float s0 = Mathf.Cos(theta) - dot * sinTheta / sinTheta0;
        float s1 = sinTheta / sinTheta0;

        return new Quat
        (
            (s0 * a.x) + (s1 * b.x),
            (s0 * a.y) + (s1 * b.y),
            (s0 * a.z) + (s1 * b.z),
            (s0 * a.w) + (s1 * b.w)
        );
    }

    static Vec3 ToEulerAngles(Quat rotation)
    {
        Quat normalizedQuat = Normalize(rotation);

        float sinr_cosp = 2f * (normalizedQuat.w * normalizedQuat.x + normalizedQuat.y * normalizedQuat.z);
        float cosr_cosp = 1f - 2f * (normalizedQuat.x * normalizedQuat.x + normalizedQuat.y * normalizedQuat.y);
        float x = Mathf.Atan2(sinr_cosp, cosr_cosp);

        float sinp = 2f * (normalizedQuat.w * normalizedQuat.y - normalizedQuat.z * normalizedQuat.x);
        float y = 0.0f;
        
        if (Mathf.Abs(sinp) >= 1f)
            y = Mathf.Sign(sinp) * Mathf.PI / 2f; 
        else
            y = Mathf.Asin(sinp);

        float siny_cosp = 2f * (normalizedQuat.w * normalizedQuat.z + normalizedQuat.x * normalizedQuat.y);
        float cosy_cosp = 1f - 2f * (normalizedQuat.y * normalizedQuat.y + normalizedQuat.z * normalizedQuat.z);
        float z = Mathf.Atan2(siny_cosp, cosy_cosp);

        return new Vec3(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg);
        
    }

#pragma endregion
#pragma region Instance

    public void Normalize()
    {
        this = this.normalized;
    }

    public void Set(float newX, float newY, float newZ, float newW)
    {
        x = newX;
        y = newY;
        z = newZ;
        w = newW;
    }

    public void SetAxisAngle(Vec3 axis, float angle)
    {
        this = AngleAxis(angle, axis);
    }

    public void SetEulerAngles(Vec3 euler)
    {
        this = Euler(euler);
    }

    public void SetEulerRotation(Vec3 euler)
    {
        this = Euler(euler);
    }

    public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
    {
        this = FromToRotation(fromDirection, toDirection);
    }

    public void SetLookRotation(Vec3 view, Vec3 up)
    {
        this = LookRotation(view, up);
    }

    public Vec3 ToEuler()
    {
        return ToEulerAngles(this);
    }

#pragma endregion
}