using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace GHVRQ_Save_Manager.Data
{
    public interface IObjectComponent
    {
        public string OBJECT_ID { get; set; }
        public string COMPONENT_ID { get; set; }
        public XElement XMLElement { get; set; }
    }

    public struct ConsumableComponent : IObjectComponent
    {
        public enum ConsumableState
        {
            RAW,
            COOKED,
            SPOILED
        }

        public string OBJECT_ID { get; set; }
        public string COMPONENT_ID { get; set; }
        public XElement XMLElement { get; set; }
        public ConsumableState STATE { get; set; }
        public float PROGRESS_ROAST { get; set; }
        public float PROGRESS_SMOKE { get; set; }
        public float PROGRESS_DRY { get; set; }
        public float PROGRESS_SPOIL { get; set; }
    }

    public struct DrinkableComponent : IObjectComponent
    {
        public string OBJECT_ID { get; set; }
        public string COMPONENT_ID { get; set; }
        public XElement XMLElement { get; set; }
        public List<Liquid> MIXTURE { get; set; }
        
    }
    public struct Liquid
    {
        public ELiquids liquidID { get; set; }
        public float concentration { get; set; }
        public float amount { get; set; }
    }

    struct DamagableComponent : IObjectComponent
    {
        public string OBJECT_ID { get; set; }
        public string COMPONENT_ID { get; set; }
        public XElement XMLElement { get; set; }
        public int HEALTH { get; set; }
    }
}
