using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using UraniumUI.Material.Controls;

namespace GHVRQ_Save_Manager.XML
{
    public static class XMLReaderSystem
    {
        public static XDocument? CurrentDoc;

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
