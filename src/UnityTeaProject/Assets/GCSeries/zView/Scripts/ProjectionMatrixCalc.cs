using UnityEngine;

namespace GCSeries.zView
{
    public class ProjectionMatrixCalc
    {
        public ProjectionMatrixCalc()
        {

        }
        public ProjectionMatrixCalc(Vector3 TL, Vector3 BL, Vector3 BR)
        {
            TopLeft = TL;
            BottomLeft = BL;
            BottomRight = BR;
        }

        public void SetVector(Vector3 TL, Vector3 BL, Vector3 BR)
        {
            TopLeft = TL;
            BottomLeft = BL;
            BottomRight = BR;
        }

        Vector3 TopLeft = Vector3.zero;
        Vector3 BottomLeft = Vector3.zero;
        Vector3 BottomRight = Vector3.zero;

        public Matrix4x4 GeneralizedPerspectiveProjection(Vector3 EyeLocation,
                                                            float near, float far)
        {
            Vector3 vector_right = BottomRight - BottomLeft;
            vector_right.Normalize();
            Vector3 vector_up = TopLeft - BottomLeft;
            vector_up.Normalize();
            Vector3 vector_forward = Vector3.Cross(vector_right, vector_up);
            vector_forward.Normalize();

            Vector3 vector_eyeToBL = BottomLeft - EyeLocation;
            Vector3 vector_eyeToBR = BottomRight - EyeLocation;
            Vector3 vector_eyeToTL = TopLeft - EyeLocation;

            float temp_proportion = near / Vector3.Dot(vector_forward, vector_eyeToBL);

            float left = Vector3.Dot(vector_right, vector_eyeToBL) * temp_proportion;
            float right = Vector3.Dot(vector_right, vector_eyeToBR) * temp_proportion;

            float bottom = Vector3.Dot(vector_up, vector_eyeToBL) * temp_proportion;
            float top = Vector3.Dot(vector_up, vector_eyeToTL) * temp_proportion;

            return PerspectiveOffCenter(left, right, bottom, top, near, far);
        }

        public Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            Matrix4x4 result = Matrix4x4.identity;

            result[0, 0] = 2f * near / (right - left);
            result[0, 1] = 0f;
            result[0, 2] = (right + left) / (right - left);
            result[0, 3] = 0f;

            result[1, 0] = 0f;
            result[1, 1] = 2f * near / (top - bottom);
            result[1, 2] = (top + bottom) / (top - bottom);
            result[1, 3] = 0f;

            result[2, 0] = 0f;
            result[2, 1] = 0f;
            result[2, 2] = -((far + near) / (far - near));
            result[2, 3] = -(2f * far * near) / (far - near);

            result[3, 0] = 0f;
            result[3, 1] = 0f;
            result[3, 2] = -1f;
            result[3, 3] = 0f;

            return result;
        }
    }
}