using System;
using System.Collections.Generic;
using System.Text;

namespace GHVRQ_Save_Manager.Data
{
    public class SaveFileBindableData : UraniumUI.UraniumBindableObject, ISaveFileBindableData
    {
        private float3 position;
        public float3 Position { get => position; set => SetProperty(ref position, value); }
    }

    public struct float3
    {
        public float x;
        public float y;
        public float z;
        public float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public struct float4
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public float3 eulerAngles;
        public float4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}