using GHVRQ_Save_Manager.Data.math;
using GHVRQ_Save_Manager.XML;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GHVRQ_Save_Manager.Data
{
    public partial class GHVRObject: ObservableObject
    {
        private string object_id = "";
        public string Object_Id { 
            get => object_id; 
            set {
                SetProperty(ref object_id, value);
            } 
        }

        private EItemID item_id = EItemID.NONE;
        public EItemID ItemID { 
            get => item_id; 
            set{
                SetProperty(ref item_id, value);
            }
        }

        private XElement xElement;
        public XElement XMLElement { 
            get => xElement; 
            set{
                SetProperty(ref xElement, value);
            } 
        }

        private Vector3 position;
        public Vector3 Position { 
            get => position; 
            set{
                SetProperty(ref position, value);
            } 
        }

        private Rotation rotation;
        public Rotation Rotation { 
            get => rotation; 
            set{
                SetProperty(ref rotation, value);
            }
        }

        private BackpackObject? backpackObject;
        public BackpackObject? BackpackObject { 
            get => backpackObject;
            set {
                SetProperty(ref backpackObject, value);
                OnPropertyChanged(nameof(IsInBackpack));
            } 
        }

        public bool IsInBackpack { get => backpackObject != null;  }

        private HolsterObject? holsterObject;
        public HolsterObject? HolsterObject
        {
            get => holsterObject;
            set
            {
                SetProperty(ref holsterObject, value);
                OnPropertyChanged(nameof(IsInHolster));
            }
        }

        public bool IsInHolster { get => holsterObject != null; }

        private List<IObjectComponent> components = [];
        public List<IObjectComponent> Components { get => components; set => SetProperty(ref components, value); }

        public void RemoveFromBackpack()
        {
            XElement? xElement = BackpackObject?.XMLElement;
            XMLReaderSystem.GetCategoryElement(XMLReaderSystem.Category.Items)?.Element("BACKPACK_OBJECTS_LIST")?.Elements("BACKPACK_OBJECT").Where(x => x == xElement).Remove();
            BackpackObject = null;
        }

        public void AddToBackpack(BackpackObject.Category category, int page, int slot)
        {

            XElement backpackElement = new("BACKPACK_OBJECT",
                    new XElement("CATEGORY", category.ToString()),
                    new XElement("PAGE", page.ToString()),
                    new XElement("SLOT", slot.ToString()),
                    new XElement("OBJECT_ID", this.Object_Id)
            );


            XMLReaderSystem.GetCategoryElement(XMLReaderSystem.Category.Items)?.Element("BACKPACK_OBJECTS_LIST")?.Add(backpackElement);

            Debug.WriteLine($"Added {this.Object_Id} to backpack at category {category}, page {page}, slot {slot}");

            BackpackObject backpackObject = new()
            {
                category = category, 
                page = page,
                slot = slot,
                Object_Id = this.Object_Id,
                XMLElement = backpackElement
            };

            BackpackObject = backpackObject;
        }

        public void RemoveFromHolster()
        {
            XElement? xElement = HolsterObject?.XMLElement;
            XMLReaderSystem.GetCategoryElement(XMLReaderSystem.Category.Items)?.Element("HOLSTER_OBJECTS_LIST")?.Elements("HOLSTER_OBJECT").Where(x => x == xElement).Remove();
            HolsterObject = null;
        }

        public void AddToHolster(HolsterObject.SlotType slot)
        {
            XElement holsterElement = new("HOLSTER_OBJECT",
                    new XElement("SLOT", slot.ToString()),
                    new XElement("OBJECT_ID", this.Object_Id)
            );
            XMLReaderSystem.GetCategoryElement(XMLReaderSystem.Category.Items)?.Element("HOLSTER_OBJECTS_LIST")?.Add(holsterElement);
            Debug.WriteLine($"Added {this.Object_Id} to holster at slot {slot}");
            HolsterObject holsterObject = new()
            {
                slot = slot,
                Object_Id = this.Object_Id,
                XMLElement = holsterElement
            };
            HolsterObject = holsterObject;
        }

        public void RemoveComponent(IObjectComponent component)
        {
            XElement? xElement = component.XMLElement;
            XMLElement?.Elements("COMPONENTS")?.Elements("COMPONENT")?.Where(x => x == xElement).Remove();
            Components.Remove(component);
        }

        public void AddComponent(IObjectComponent component)
        {
            XElement componentElement = component.XMLElement;
            XMLElement?.Element("COMPONENTS")?.Add(componentElement);
            Components.Add(component);
        }

        public void RemoveAllComponents()
        {
            XMLElement?.Element("COMPONENTS")?.Elements("COMPONENT")?.Remove();
            Components.Clear();
        }
    }

    public class BackpackObject
    {
        public enum Category
        {
            BACKPACK_TOOLS,
            BACKPACK_MATERIALS,
            BACKPACK_FOOD,
            BACKPACK_WEAPONS,
        }

        public Category category { get; set; }
        public int page { get; set; }
        public int slot { get; set; }
        public string Object_Id { get; set; }

        public XElement? XMLElement { get; set; }
    }

    public class HolsterObject
    {
        public enum SlotType
        {
            LEFT,
            RIGHT,
        }
        public SlotType slot { get; set; }
        public string Object_Id { get; set; }
        public XElement? XMLElement { get; set; }
    }
}
