using System;
using UnityEngine;
using CustomMath;

public struct Mat4x4
{
    const float epsilon = 1e-5f;
    
#pragma region Variables
    private float m00;
    private float m01;
    private float m02;
    private float m03;

    private float m10;
    private float m11;
    private float m12;
    private float m13;

    private float m20;
    private float m21;
    private float m22;
    private float m23;

    private float m30;
    private float m31;
    private float m32;
    private float m33;
#pragma endregion

#pragma region Constructors

    public Mat4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
    {
        m00 = column0.x;
        m01 = column1.x;
        m02 = column2.x;
        m03 = column3.x;

        m10 = column0.y;
        m11 = column1.y;
        m12 = column2.y;
        m13 = column3.y;

        m20 = column0.z;
        m21 = column1.z;
        m22 = column2.z;
        m23 = column3.z;

        m30 = column0.w;
        m31 = column1.w;
        m32 = column2.w;
        m33 = column3.w;
    }

    public Mat4x4(Matrix4x4 unityMatrix)
    {
        m00 = unityMatrix.m00;
        m01 = unityMatrix.m01;
        m02 = unityMatrix.m02;
        m03 = unityMatrix.m03;

        m10 = unityMatrix.m10;
        m11 = unityMatrix.m11;
        m12 = unityMatrix.m12;
        m13 = unityMatrix.m13;

        m20 = unityMatrix.m20;
        m21 = unityMatrix.m21;
        m22 = unityMatrix.m22;
        m23 = unityMatrix.m23;

        m30 = unityMatrix.m30;
        m31 = unityMatrix.m31;
        m32 = unityMatrix.m32;
        m33 = unityMatrix.m33;
    }

#pragma endregion

#pragma region Properties
    public static Mat4x4 zero => new Mat4x4(Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero);

    public static Mat4x4 identity
    {
        get
        {
            Mat4x4 m = new Mat4x4();
            m.m00 = 1f;
            m.m11 = 1f;
            m.m22 = 1f;
            m.m33 = 1f;
            return m;
        }
    }

    public bool isIdentity =>
        m00 == 1f && m01 == 0f && m02 == 0f && m03 == 0f &&
        m10 == 0f && m11 == 1f && m12 == 0f && m13 == 0f &&
        m20 == 0f && m21 == 0f && m22 == 1f && m23 == 0f &&
        m30 == 0f && m31 == 0f && m32 == 0f && m33 == 1f;

    public float determinant => Determinant(this);
    public Mat4x4 inverse => Inverse(this);

    public Quat rotation
    {
        get
        {
            Vec3 scale = lossyScale;

            float sX = (Mathf.Abs(scale.x) > epsilon) ? scale.x : 1f;
            float sY = (Mathf.Abs(scale.y) > epsilon) ? scale.y : 1f;
            float sZ = (Mathf.Abs(scale.z) > epsilon) ? scale.z : 1f;

            Mat4x4 pureRotationMat = identity;

            pureRotationMat.m00 = m00 / sX;
            pureRotationMat.m10 = m10 / sX;
            pureRotationMat.m20 = m20 / sX;

            pureRotationMat.m01 = m01 / sY;
            pureRotationMat.m11 = m11 / sY;
            pureRotationMat.m21 = m21 / sY;

            pureRotationMat.m02 = m02 / sZ;
            pureRotationMat.m12 = m12 / sZ;
            pureRotationMat.m22 = m22 / sZ;

            return pureRotationMat.ExtractRotation();
        }
    }

    public Vec3 lossyScale
    {
        get
        {
            return new Vec3
            (
                Mathf.Sqrt(m00 * m00 + m10 * m10 + m20 * m20), // x
                Mathf.Sqrt(m01 * m01 + m11 * m11 + m21 * m21), // y
                Mathf.Sqrt(m02 * m02 + m12 * m12 + m22 * m22) // z
            );
        }
    }

    public Mat4x4 transpose => Transpose(this);

#pragma endregion
#pragma region Operators
    public static Vector4 operator *(Mat4x4 lhs, Vector4 vector)
    {
        float resultX = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
        float resultY = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
        float resultZ = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
        float resultW = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;

        return new Vector4(resultX, resultY, resultZ, resultW);
    }

    public static Mat4x4 operator *(Mat4x4 lhs, Mat4x4 rhs)
    {
        Mat4x4 res = new Mat4x4();

        res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
        res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
        res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
        res.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;

        res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
        res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
        res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
        res.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;

        res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
        res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
        res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
        res.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;

        res.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
        res.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
        res.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
        res.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;

        return res;
    }

    public static bool operator ==(Mat4x4 lhs, Mat4x4 rhs)
    {
        const float epsilon = 1e-5f;

        return Mathf.Abs(lhs.m00 - rhs.m00) < epsilon && Mathf.Abs(lhs.m01 - rhs.m01) < epsilon &&
               Mathf.Abs(lhs.m02 - rhs.m02) < epsilon && Mathf.Abs(lhs.m03 - rhs.m03) < epsilon &&
               Mathf.Abs(lhs.m10 - rhs.m10) < epsilon && Mathf.Abs(lhs.m11 - rhs.m11) < epsilon &&
               Mathf.Abs(lhs.m12 - rhs.m12) < epsilon && Mathf.Abs(lhs.m13 - rhs.m13) < epsilon &&
               Mathf.Abs(lhs.m20 - rhs.m20) < epsilon && Mathf.Abs(lhs.m21 - rhs.m21) < epsilon &&
               Mathf.Abs(lhs.m22 - rhs.m22) < epsilon && Mathf.Abs(lhs.m23 - rhs.m23) < epsilon &&
               Mathf.Abs(lhs.m30 - rhs.m30) < epsilon && Mathf.Abs(lhs.m31 - rhs.m31) < epsilon &&
               Mathf.Abs(lhs.m32 - rhs.m32) < epsilon && Mathf.Abs(lhs.m33 - rhs.m33) < epsilon;
    }

    public static bool operator !=(Mat4x4 lhs, Mat4x4 rhs)
    {
        return !(lhs == rhs);
    }

    public static implicit operator Matrix4x4(Mat4x4 m)
    {
        Matrix4x4 unityMat = new Matrix4x4();
        unityMat.m00 = m.m00;
        unityMat.m01 = m.m01;
        unityMat.m02 = m.m02;
        unityMat.m03 = m.m03;

        unityMat.m10 = m.m10;
        unityMat.m11 = m.m11;
        unityMat.m12 = m.m12;
        unityMat.m13 = m.m13;

        unityMat.m20 = m.m20;
        unityMat.m21 = m.m21;
        unityMat.m22 = m.m22;
        unityMat.m23 = m.m23;

        unityMat.m30 = m.m30;
        unityMat.m31 = m.m31;
        unityMat.m32 = m.m32;
        unityMat.m33 = m.m33;

        return unityMat;
    }

#pragma endregion
#pragma region Statics

    public static Mat4x4 Translate(Vec3 vector)
    {
        Mat4x4 m = identity;

        m.m03 = vector.x;
        m.m13 = vector.y;
        m.m23 = vector.z;

        return m;
    }

    public static Mat4x4 Scale(Vec3 vector)
    {
        Mat4x4 m = identity;

        m.m00 = vector.x;
        m.m11 = vector.y;
        m.m22 = vector.z;

        return m;
    }

    public static Mat4x4 Transpose(Mat4x4 m)
    {
        Mat4x4 res = new Mat4x4();

        res.m00 = m.m00;
        res.m01 = m.m10;
        res.m02 = m.m20;
        res.m03 = m.m30;

        res.m10 = m.m01;
        res.m11 = m.m11;
        res.m12 = m.m21;
        res.m13 = m.m31;

        res.m20 = m.m02;
        res.m21 = m.m12;
        res.m22 = m.m22;
        res.m23 = m.m32;

        res.m30 = m.m03;
        res.m31 = m.m13;
        res.m32 = m.m23;
        res.m33 = m.m33;

        return res;
    }

    public static Mat4x4 Rotate(Quat q)
    {
        float xx = q.x * q.x;
        float yy = q.y * q.y;
        float zz = q.z * q.z;

        float xy = q.x * q.y;
        float xz = q.x * q.z;
        float yz = q.y * q.z;

        float wx = q.w * q.x;
        float wy = q.w * q.y;
        float wz = q.w * q.z;

        Mat4x4 m = identity;

        m.m00 = 1.0f - 2.0f * (yy + zz);
        m.m10 = 2.0f * (xy + wz);
        m.m20 = 2.0f * (xz - wy);

        m.m01 = 2.0f * (xy - wz);
        m.m11 = 1.0f - 2.0f * (xx + zz);
        m.m21 = 2.0f * (yz + wx);

        m.m02 = 2.0f * (xz + wy);
        m.m12 = 2.0f * (yz - wx);
        m.m22 = 1.0f - 2.0f * (xx + yy);

        return m;
    }

    public static Mat4x4 TRS(Vec3 pos, Quat q, Vec3 s)
    {
        Mat4x4 m = Rotate(q);

        m.m00 *= s.x;
        m.m10 *= s.x;
        m.m20 *= s.x; //x

        m.m01 *= s.y;
        m.m11 *= s.y;
        m.m21 *= s.y; //y

        m.m02 *= s.z;
        m.m12 *= s.z;
        m.m22 *= s.z; //z

        m.m03 = pos.x;
        m.m13 = pos.y;
        m.m23 = pos.z;

        return m;
    }

    public static Mat4x4 LookAt(Vec3 from, Vec3 to, Vec3 up)
    {
        Vec3 z = (to - from).normalized;

        //calculate x axis
        Vec3 x = Vec3.Cross(up, z).normalized;

        //recalculate real y axis
        Vec3 y = Vec3.Cross(z, x);

        Mat4x4 m = identity;

        m.m00 = x.x;
        m.m01 = y.x;
        m.m02 = z.x;

        m.m10 = x.y;
        m.m11 = y.y;
        m.m12 = z.y;

        m.m20 = x.z;
        m.m21 = y.z;
        m.m22 = z.z;

        m.m03 = from.x;
        m.m13 = from.y;
        m.m23 = from.z;

        return m;
    }

    public static float Determinant(Mat4x4 m)
    {
        float a0 = m.m00 * m.m11 - m.m01 * m.m10;
        float a1 = m.m00 * m.m12 - m.m02 * m.m10;
        float a2 = m.m00 * m.m13 - m.m03 * m.m10;
        float a3 = m.m01 * m.m12 - m.m02 * m.m11;
        float a4 = m.m01 * m.m13 - m.m03 * m.m11;
        float a5 = m.m02 * m.m13 - m.m03 * m.m12;

        float b0 = m.m20 * m.m31 - m.m21 * m.m30;
        float b1 = m.m20 * m.m32 - m.m22 * m.m30;
        float b2 = m.m20 * m.m33 - m.m23 * m.m30;
        float b3 = m.m21 * m.m32 - m.m22 * m.m31;
        float b4 = m.m21 * m.m33 - m.m23 * m.m31;
        float b5 = m.m22 * m.m33 - m.m23 * m.m32;

        return a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0;
    }

    public Quat ExtractRotation()
    {
        float trace = m00 + m11 + m22;
        float qx;
        float qy;
        float qz;
        float qw;

        if (trace > epsilon)
        {
            float S = Mathf.Sqrt(trace + 1.0f) * 2f;
            qw = 0.25f * S;
            qx = (m21 - m12) / S;
            qy = (m02 - m20) / S;
            qz = (m10 - m01) / S;
        }
        else if ((m00 > m11) && (m00 > m22))
        {
            float S = Mathf.Sqrt(1.0f + m00 - m11 - m22) * 2f;
            qw = (m21 - m12) / S;
            qx = 0.25f * S;
            qy = (m01 + m10) / S;
            qz = (m02 + m20) / S;
        }
        else if (m11 > m22)
        {
            float S = Mathf.Sqrt(1.0f + m11 - m00 - m22) * 2f;
            qw = (m02 - m20) / S;
            qx = (m01 + m10) / S;
            qy = 0.25f * S;
            qz = (m12 + m21) / S;
        }
        else
        {
            float S = Mathf.Sqrt(1.0f + m22 - m00 - m11) * 2f;
            qw = (m10 - m01) / S;
            qx = (m02 + m20) / S;
            qy = (m12 + m21) / S;
            qz = 0.25f * S;
        }

        return new Quat(qx, qy, qz, qw);
    }

    public static Mat4x4 Inverse(Mat4x4 m)
    {
        float a0 = m.m00 * m.m11 - m.m01 * m.m10;
        float a1 = m.m00 * m.m12 - m.m02 * m.m10;
        float a2 = m.m00 * m.m13 - m.m03 * m.m10;
        float a3 = m.m01 * m.m12 - m.m02 * m.m11;
        float a4 = m.m01 * m.m13 - m.m03 * m.m11;
        float a5 = m.m02 * m.m13 - m.m03 * m.m12;

        float b0 = m.m20 * m.m31 - m.m21 * m.m30;
        float b1 = m.m20 * m.m32 - m.m22 * m.m30;
        float b2 = m.m20 * m.m33 - m.m23 * m.m30;
        float b3 = m.m21 * m.m32 - m.m22 * m.m31;
        float b4 = m.m21 * m.m33 - m.m23 * m.m31;
        float b5 = m.m22 * m.m33 - m.m23 * m.m32;

        float det = a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0;

        if (Mathf.Abs(det) <= 1e-6f)
        {
            return identity;
        }

        float invDet = 1.0f / det;
        Mat4x4 res = new Mat4x4();

        res.m00 = (m.m11 * b5 - m.m12 * b4 + m.m13 * b3) * invDet;
        res.m01 = (-m.m01 * b5 + m.m02 * b4 - m.m03 * b3) * invDet;
        res.m02 = (m.m31 * a5 - m.m32 * a4 + m.m33 * a3) * invDet;
        res.m03 = (-m.m21 * a5 + m.m22 * a4 - m.m23 * a3) * invDet;

        res.m10 = (-m.m10 * b5 + m.m12 * b2 - m.m13 * b1) * invDet;
        res.m11 = (m.m00 * b5 - m.m02 * b2 + m.m03 * b1) * invDet;
        res.m12 = (-m.m30 * a5 + m.m32 * a2 - m.m33 * a1) * invDet;
        res.m13 = (m.m20 * a5 - m.m22 * a2 + m.m23 * a1) * invDet;

        res.m20 = (m.m10 * b4 - m.m11 * b2 + m.m13 * b0) * invDet;
        res.m21 = (-m.m00 * b4 + m.m01 * b2 - m.m03 * b0) * invDet;
        res.m22 = (m.m30 * a4 - m.m31 * a2 + m.m33 * a0) * invDet;
        res.m23 = (-m.m20 * a4 + m.m21 * a2 - m.m23 * a0) * invDet;

        res.m30 = (-m.m10 * b3 + m.m11 * b1 - m.m12 * b0) * invDet;
        res.m31 = (m.m00 * b3 - m.m01 * b1 + m.m02 * b0) * invDet;
        res.m32 = (-m.m30 * a3 + m.m31 * a1 - m.m32 * a0) * invDet;
        res.m33 = (m.m20 * a3 - m.m21 * a1 + m.m22 * a0) * invDet;

        return res;
    }
#pragma endregion

    public Vec3 GetPosition()
    {
        return new Vec3(m03, m13, m23);
    }

    public Vector4 GetRow(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector4(m00, m01, m02, m03);
            case 1:
                return new Vector4(m10, m11, m12, m13);
            case 2:
                return new Vector4(m20, m21, m22, m23);
            case 3:
                return new Vector4(m30, m31, m32, m33);
            default:
                throw new System.IndexOutOfRangeException("Invalid index");
        }
    }

    public Vec3 MultiplyPoint(Vec3 point)
    {
        float resX = m00 * point.x + m01 * point.y + m02 * point.z + m03;
        float resY = m10 * point.x + m11 * point.y + m12 * point.z + m13;
        float resZ = m20 * point.x + m21 * point.y + m22 * point.z + m23;

        float numW = m30 * point.x + m31 * point.y + m32 * point.z + m33;

        float invW = 1f / numW;

        return new Vec3(resX * invW, resY * invW, resZ * invW);
    }

    private Vec3 MultiplyPoint3x4(Vec3 point)
    {
        return new Vec3
        (
            m00 * point.x + m01 * point.y + m02 * point.z + m03,
            m10 * point.x + m11 * point.y + m12 * point.z + m13,
            m20 * point.x + m21 * point.y + m22 * point.z + m23
        );
    }

    private Vec3 MultiplyVector(Vec3 vector)
    {
        return new Vec3
        (
            m00 * vector.x + m01 * vector.y + m02 * vector.z,
            m10 * vector.x + m11 * vector.y + m12 * vector.z,
            m20 * vector.x + m21 * vector.y + m22 * vector.z
        );
    }

    private void SetColumn(int index, Vector4 column)
    {
        switch (index)
        {
            case 0:
                m00 = column.x;
                m10 = column.y;
                m20 = column.z;
                m30 = column.w;
                break;
            case 1:
                m01 = column.x;
                m11 = column.y;
                m21 = column.z;
                m31 = column.w;
                break;
            case 2:
                m02 = column.x;
                m12 = column.y;
                m22 = column.z;
                m32 = column.w;
                break;
            case 3:
                m03 = column.x;
                m13 = column.y;
                m23 = column.z;
                m33 = column.w;
                break;
            default:
                throw new System.IndexOutOfRangeException("Invalid index");
        }
    }

    private void SetRow(int index, Vector4 row)
    {
        switch (index)
        {
            case 0:
                m00 = row.x;
                m01 = row.y;
                m02 = row.z;
                m03 = row.w;
                break;
            case 1:
                m10 = row.x;
                m11 = row.y;
                m12 = row.z;
                m13 = row.w;
                break;
            case 2:
                m20 = row.x;
                m21 = row.y;
                m22 = row.z;
                m23 = row.w;
                break;
            case 3:
                m30 = row.x;
                m31 = row.y;
                m32 = row.z;
                m33 = row.w;
                break;
            default:
                throw new System.IndexOutOfRangeException("Invalid index");
        }
    }

    private void SetTRS(Vec3 pos, Quat q, Vec3 s)
    {
        this = TRS(pos, q, s);
    }

    private bool ValidTRS()
    {
        return m30 == 0f && m31 == 0f && m32 == 0f && m33 == 1f;
    }

    public override string ToString()
    {
        return $"[{m00:F5}\t{m01:F5}\t{m02:F5}\t{m03:F5}]\n" +
               $"[{m10:F5}\t{m11:F5}\t{m12:F5}\t{m13:F5}]\n" +
               $"[{m20:F5}\t{m21:F5}\t{m22:F5}\t{m23:F5}]\n" +
               $"[{m30:F5}\t{m31:F5}\t{m32:F5}\t{m33:F5}]";
    }
}