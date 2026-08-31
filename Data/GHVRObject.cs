using GHVRQ_Save_Manager.Data.math;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GHVRQ_Save_Manager.Data
{
    public struct GHVRObject
    {
        public string Object_Id;
        public EItemID type;

        public Vector3 position;
        public Rotation rotation;


    }
}
