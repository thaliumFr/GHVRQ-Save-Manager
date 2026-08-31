using System.Numerics;

namespace GHVRQ_Save_Manager.Data.math
{
    public struct Rotation
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector3 EulerAngles
        {
            get
            {
                // Convert quaternion to Euler angles (in degrees)
                float ysqr = y * y;
                // roll (x-axis rotation)
                float t0 = +2.0f * (w * x + y * z);
                float t1 = +1.0f - 2.0f * (x * x + ysqr);
                float roll = (float)Math.Atan2(t0, t1);
                // pitch (y-axis rotation)
                float t2 = +2.0f * (w * y - z * x);
                t2 = t2 > 1.0f ? 1.0f : t2;
                t2 = t2 < -1.0f ? -1.0f : t2;
                float pitch = (float)Math.Asin(t2);
                // yaw (z-axis rotation)
                float t3 = +2.0f * (w * z + x * y);
                float t4 = +1.0f - 2.0f * (ysqr + z * z);
                float yaw = (float)Math.Atan2(t3, t4);
                return new Vector3(roll, pitch, yaw);
            }
            set {
                // Convert Euler angles to quaternion
                float cy = (float)Math.Cos(value.Z * 0.5f);
                float sy = (float)Math.Sin(value.Z * 0.5f);
                float cp = (float)Math.Cos(value.Y * 0.5f);
                float sp = (float)Math.Sin(value.Y * 0.5f);
                float cr = (float)Math.Cos(value.X * 0.5f);
                float sr = (float)Math.Sin(value.X * 0.5f);

                w = cr * cp * cy + sr * sp * sy;
                x = sr * cp * cy - cr * sp * sy;
                y = cr * sp * cy + sr * cp * sy;
                z = cr * cp * sy - sr * sp * cy;
            }
        }
    }
}
