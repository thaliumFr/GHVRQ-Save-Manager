using GHVRQ_Save_Manager.Data.math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Xml.Linq;

namespace GHVRQ_Save_Manager.Data
{
    public struct GHVRObject: INotifyPropertyChanged
    {
        public string Object_Id { get; set; }
        public EItemID type { get; set; }
        public XElement XMLElement { get; set; }

        public Vector3 position { get; set; }
        public Rotation rotation { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
