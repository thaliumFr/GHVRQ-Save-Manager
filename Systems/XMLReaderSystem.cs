using GHVRQ_Save_Manager.Data;
using System.Diagnostics;
using System.Xml.Linq;
using UraniumUI.Material.Controls;
using CheckBox = UraniumUI.Material.Controls.CheckBox;

namespace GHVRQ_Save_Manager.XML
{
    public static class XMLReaderSystem
    {
        public static XDocument? CurrentDoc;

        public static event EventHandler? OnXMLDocumentLoaded;

        public static ref XDocument? Load(string xmlFilePath)
        {
            if (String.IsNullOrEmpty(xmlFilePath) || !File.Exists(xmlFilePath)) return ref CurrentDoc;

            CurrentDoc = XDocument.Load(xmlFilePath);

            OnXMLDocumentLoaded?.Invoke(null, EventArgs.Empty);
            return ref CurrentDoc;
        }

        public static ref XDocument? LoadFromString(string xmlString)
        {
            CurrentDoc = XDocument.Parse(xmlString);

            OnXMLDocumentLoaded?.Invoke(null, EventArgs.Empty);
            return ref CurrentDoc;
        }

        public static void Save(string xmlFilePath)
        {
            CurrentDoc?.Save(xmlFilePath);
        }

        public static XElement? GetCategoryElement(Category category)
        {
            string CategoryName = "";

            switch (category)
            {
                case Category.Status:
                    CategoryName = "playerStatus";
                    break;
                case Category.Injury:
                    CategoryName = "playerInjury";
                    break;
                case Category.World:
                    CategoryName = "world";
                    break;
                case Category.Items:
                    CategoryName = "items";
                    break;
                case Category.Notebook:
                    CategoryName = "tutorialNotebook";
                    break;
                case Category.Map:
                    CategoryName = "mapNotebook";
                    break;
                default: return null;
            }

            return CurrentDoc?.Root?.Element(CategoryName);
        }

        public static XElement? AddElementLink(IUniversalInputFieldData field, XElement element)
        {
            if (field is TextFieldData textFieldData)
            {
                textFieldData.Field.TextChanged += (sender, e) =>
                {
                    element.Value = textFieldData.Field.Text;
                };

                Debug.WriteLine($"Text element: {element.Value}");
                field.Value = element.Value;
            }
            else if (field is ToggleFieldData checkBox)
            {
                checkBox.Field.CheckChanged += (sender, e) =>
                {
                    element.Value = checkBox.Field.IsChecked.ToString();
                };
                checkBox.Field.IsChecked = bool.Parse(element.Value);

                Debug.WriteLine($"Toggled element: {element.Value}");
            }
            MainPage.FieldLinks.TryAdd(field, element);

            return element;
        }

        public static XElement? AddElementLink(IUniversalInputFieldData field, string path)
        {
            return AddElementLink(field, GetElementFromPath(path));
        }

        public static XElement GetElementFromPath(string path)
        {
            XElement element = CurrentDoc?.Root!;
            path.Split(".").ToList().ForEach(name =>
            {
                element = element?.Element(name)!;
            });
            if (element == null)
            {
                Debug.WriteLine($"Could not find element with UPS \"{path}\"");
            }
            return element!;
        }

        static List<BackpackObject> GetAllObjectsInBackpack()
        {
            List<BackpackObject> objects = [];
            XElement? itemsElement = GetCategoryElement(Category.Items)?.Element("BACKPACK_OBJECTS_LIST");

            if (itemsElement == null) return [];

            foreach (XElement itemElement in itemsElement.Elements("BACKPACK_OBJECT"))
            {
                BackpackObject obj = new()
                {
                    XMLElement = itemElement,
                    Object_Id = itemElement.Element("OBJECT_ID")?.Value ?? "",
                    category = Enum.TryParse(itemElement.Element("CATEGORY")?.Value, out BackpackObject.Category category) ? category : BackpackObject.Category.BACKPACK_TOOLS,
                    page = int.TryParse(itemElement.Element("PAGE")?.Value, out int page) ? page : 0,
                    slot = int.TryParse(itemElement.Element("SLOT")?.Value, out int slot) ? slot : 0
                };
                objects.Add(obj);
            }

            return objects;
        }

        static List<IObjectComponent> GetAllComponents()
        {
            List<IObjectComponent> components = [];
            XElement? consumables = GetCategoryElement(Category.Items)?.Element("CONSUMABLES_LIST");
            XElement? drinkables = GetCategoryElement(Category.Items)?.Element("DRINKABLES_LIST");
            XElement? damagables = GetCategoryElement(Category.Items)?.Element("DAMAGABLES_LIST");

            foreach (var consumable in consumables?.Elements("CONSUMABLE"))
            {
                components.Add(new ConsumableComponent()
                {
                    COMPONENT_ID = consumable.Element("COMPONENT_ID")?.Value ?? "",
                    OBJECT_ID = consumable.Element("OBJECT_ID")?.Value ?? "",
                    XMLElement = consumable,
                    STATE = Enum.TryParse(consumable.Element("STATE")?.Value, out ConsumableComponent.ConsumableState state) ? state : ConsumableComponent.ConsumableState.RAW,
                    PROGRESS_ROAST = float.TryParse(consumable.Element("PROGRESS_ROAST")?.Value, out float roast) ? roast : 0,
                    PROGRESS_SMOKE = float.TryParse(consumable.Element("PROGRESS_SMOKE")?.Value, out float smoke) ? smoke : 0,
                    PROGRESS_DRY = float.TryParse(consumable.Element("PROGRESS_DRY")?.Value, out float dry) ? dry : 0,
                    PROGRESS_SPOIL = float.TryParse(consumable.Element("PROGRESS_SPOIL")?.Value, out float spoil) ? spoil : 0
                });
            }
            foreach (var drinkable in drinkables?.Elements("DRINKABLE"))
            {
                List<Liquid> mixture = [];
                foreach (var liquid in drinkable.Element("MIXTURE")?.Elements("LIQUID"))
                {
                    mixture.Add(new Liquid()
                    {
                        liquidID = Enum.TryParse(liquid.Element("LIQUID_ID")?.Value, out ELiquids liquidID) ? liquidID : ELiquids.CLEAN_WATER,
                        concentration = float.TryParse(liquid.Element("CONCENTRATION")?.Value, out float concentration) ? concentration : 0,
                        amount = float.TryParse(liquid.Element("AMOUNT")?.Value, out float amount) ? amount : 0
                    });
                }
                components.Add(new DrinkableComponent()
                {
                    COMPONENT_ID = drinkable.Element("COMPONENT_ID")?.Value ?? "",
                    OBJECT_ID = drinkable.Element("OBJECT_ID")?.Value ?? "",
                    XMLElement = drinkable,
                    MIXTURE = mixture
                });
            }
            foreach (var damagable in damagables?.Elements("DAMAGABLE"))
            {
                components.Add(new DamagableComponent()
                {
                    COMPONENT_ID = damagable.Element("COMPONENT_ID")?.Value ?? "",
                    OBJECT_ID = damagable.Element("OBJECT_ID")?.Value ?? "",
                    XMLElement = damagable,
                    HEALTH = int.TryParse(damagable.Element("HEALTH")?.Value, out int health) ? health : 0
                });
            }
            return components;
        }

        static List<HolsterObject> GetAllObjectsInHolsters()
        {
            List<HolsterObject> objects = [];
            XElement? itemsElement = GetCategoryElement(Category.Items)?.Element("HOLSTERS_LIST");

            if (itemsElement == null) return [];

            foreach (XElement itemElement in itemsElement.Elements("HOLSTERS"))
            {
                HolsterObject obj = new()
                {
                    XMLElement = itemElement,
                    Object_Id = itemElement.Element("OBJECT_ID")?.Value ?? "",
                    slot = Enum.TryParse(itemElement.Element("SLOT")?.Value, out HolsterObject.SlotType slot) ? slot : HolsterObject.SlotType.LEFT
                };
                objects.Add(obj);
            }

            return objects;
        }

        public static List<GHVRObject> GetAllGHVRObjects()
        {
            List<GHVRObject> objects = [];
            XElement? itemsElement = GetCategoryElement(Category.Items)?.Element("OBJECTS_LIST");

            if (itemsElement == null) return [];

            List<BackpackObject> backpackObjects = GetAllObjectsInBackpack();
            List<HolsterObject> holsterObjects = GetAllObjectsInHolsters();
            List<IObjectComponent> components = GetAllComponents();

            foreach (XElement itemElement in itemsElement.Elements("OBJECT"))
            {
                XElement? pos = itemElement.Element("POSITION");
                XElement? rot = itemElement.Element("ROTATION");

                GHVRObject obj = new()
                {
                    XMLElement = itemElement,
                    Object_Id = itemElement.Element("OBJECT_ID")?.Value ?? "",
                    ItemID = Enum.TryParse(itemElement.Element("TYPE")?.Value, out EItemID itemType) ? itemType : EItemID.NONE,
                    Position = new System.Numerics.Vector3(
                        Single.TryParse(pos?.Element("x")?.Value.Replace(".", ",") ?? "0", out float x) ? x : 0,
                        Single.TryParse(pos?.Element("y")?.Value.Replace(".", ",") ?? "0", out float y) ? y : 0,
                        Single.TryParse(pos?.Element("z")?.Value.Replace(".", ",") ?? "0", out float z) ? z : 0
                    ),
                    Rotation = new Data.math.Rotation
                    {
                        x = Single.TryParse(rot?.Element("x")?.Value.Replace(".", ",") ?? "0", out float rx) ? rx : 0,
                        y = Single.TryParse(rot?.Element("y")?.Value.Replace(".", ",") ?? "0", out float ry) ? ry : 0,
                        z = Single.TryParse(rot?.Element("z")?.Value.Replace(".", ",") ?? "0", out float rz) ? rz : 0,
                        w = Single.TryParse(rot?.Element("w")?.Value.Replace(".", ",") ?? "1", out float rw) ? rw : 1
                    }
                };

                BackpackObject? backpackObj = backpackObjects.Find(BObj => BObj.Object_Id == obj.Object_Id);
                HolsterObject? holsterObj = holsterObjects.Find(HObj => HObj.Object_Id == obj.Object_Id);
                List<IObjectComponent>? componentsObj = components.FindAll(C => C.OBJECT_ID == obj.Object_Id);

                if (backpackObj != null)
                {
                    obj.BackpackObject = backpackObj;
                }
                if (holsterObj != null)
                {
                    obj.HolsterObject = holsterObj;
                }
                if (componentsObj != null)
                {
                    obj.Components = componentsObj;
                }

                objects.Add(obj);
            }
            return objects;
        }

        public enum Category
        {
            Status,
            Injury,
            World,
            Items,
            Notebook,
            Map,
        }
    }
}
