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

        public static Dictionary<IUniversalInputFieldData, XElement> FieldLinks = [];

        public static ref XDocument? Load(string xmlFilePath)
        {
            if (String.IsNullOrEmpty(xmlFilePath) || !File.Exists(xmlFilePath)) return ref CurrentDoc;

            CurrentDoc = XDocument.Load(xmlFilePath);

            FieldLinks.Clear();
            foreach (var (field, path) in MainPage.InputFieldPathDict)
            {
                AddElementLink(field, path);
            }

            OnXMLDocumentLoaded?.Invoke(null, EventArgs.Empty);
            return ref CurrentDoc;
        }

        public static ref XDocument? LoadFromString(string xmlString)
        {
            CurrentDoc = XDocument.Parse(xmlString);

            FieldLinks.Clear();
            foreach (var (textField, path) in MainPage.InputFieldPathDict)
            {
                AddElementLink(textField, path);
            }

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

                Debug.WriteLine(element);
                field.Value = element.Value;
            }
            else if (field is ToggleFieldData checkBox)
            {
                checkBox.Field.CheckChanged += (sender, e) =>
                {
                    element.Value = checkBox.Field.IsChecked.ToString();
                };
                checkBox.Field.IsChecked = bool.Parse(element.Value);
            }
            FieldLinks.TryAdd(field, element);

            return element;
        }

        public static XElement? AddElementLink(IUniversalInputFieldData field, string path)
        {
            XElement? element = CurrentDoc?.Root;

            path.Split(".").ToList().ForEach(name =>
            {
                element = element?.Element(name);
            });

            if (element == null)
            {
                Debug.WriteLine($"Could not find element with UPS \"{path}\"");
            }

            return AddElementLink(field, element);
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
                    category = Enum.TryParse(itemElement.Element("CATEGORY")?.Value, out BackpackObject.Category category) ? category : BackpackObject.Category.None,
                    page = int.TryParse(itemElement.Element("PAGE")?.Value, out int page) ? page : 0,
                    slot = int.TryParse(itemElement.Element("SLOT")?.Value, out int slot) ? slot : 0
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
                if (backpackObj != null)
                {
                    obj.BackpackObject = backpackObj;
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
