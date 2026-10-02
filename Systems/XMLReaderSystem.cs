using GHVRQ_Save_Manager.Data;
using System.Xml.Linq;
using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.XML
{
    public static class XMLReaderSystem
    {
        public static XDocument? CurrentDoc;

        public static event EventHandler? OnXMLDocumentLoaded;

        public static Dictionary<TextField, XElement> FieldLinks = [];

        public static ref XDocument? Load(string xmlFilePath)
        {
            if (String.IsNullOrEmpty(xmlFilePath) || !File.Exists(xmlFilePath)) return ref CurrentDoc;

            CurrentDoc = XDocument.Load(xmlFilePath);

            FieldLinks.Clear();
            foreach (var (textField, path) in MainPage.TextFieldsList)
            {
                AddElementLink(textField, path);
            }

            OnXMLDocumentLoaded?.Invoke(null, EventArgs.Empty);
            return ref CurrentDoc;
        }

        public static ref XDocument? LoadFromString(string xmlString)
        {
            CurrentDoc = XDocument.Parse(xmlString);

            FieldLinks.Clear();
            foreach (var (textField, path) in MainPage.TextFieldsList)
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

        public static XElement? AddElementLink(TextField textField, string path)
        {
            XElement? node = CurrentDoc?.Root;

            path.Split(".").ToList().ForEach(name =>
            {
                node = node?.Element(name);
            });
            textField.Text = node.Value;

            textField.TextChanged += (sender, e) =>
            {
                node.Value = textField.Text;
            };
            FieldLinks.Add(textField, node);

            return node;
        }

        public static XElement? AddElementLink(TextField textField, XElement element)
        {
            if (!FieldLinks.ContainsKey(textField))
            {
                textField.Text = element.Value;

                textField.TextChanged += (sender, e) =>
                {
                    element.Value = textField.Text;
                };
                FieldLinks.Add(textField, element);
            }

            return element;
        }

        public static List<GHVRObject> GetAllGHVRObjects()
        {
            List<GHVRObject> objects = [];
            XElement? itemsElement = GetCategoryElement(Category.Items)?.Element("OBJECTS_LIST");
            if (itemsElement == null) return objects;
            foreach (XElement itemElement in itemsElement.Elements("OBJECT"))
            {
                XElement? pos = itemElement.Element("POSITION");
                XElement? rot = itemElement.Element("ROTATION");

                GHVRObject obj = new()
                {
                    XMLElement = itemElement,
                    Object_Id = itemElement.Element("OBJECT_ID")?.Value ?? "",
                    type = Enum.TryParse(itemElement.Element("TYPE")?.Value, out EItemID itemType) ? itemType : EItemID.NONE,
                    position = new System.Numerics.Vector3(
                        float.Parse(pos?.Attribute("x")?.Value ?? "0"),
                        float.Parse(pos?.Attribute("y")?.Value ?? "0"),
                        float.Parse(pos?.Attribute("z")?.Value ?? "0")
                    ),
                    rotation = new Data.math.Rotation
                    {
                        x = float.Parse(rot?.Attribute("x")?.Value ?? "0"),
                        y = float.Parse(rot?.Attribute("y")?.Value ?? "0"),
                        z = float.Parse(rot?.Attribute("z")?.Value ?? "0"),
                        w = float.Parse(rot?.Attribute("w")?.Value ?? "1")
                    }
                };
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
