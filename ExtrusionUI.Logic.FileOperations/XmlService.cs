using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExtrusionUI.Logic.FileOperations
{
    public class XmlService : IXmlService
    {
        IFileService _fileService;

        public Dictionary<string, string> XmlSettings { get; set; }

        private XDocument persistentXml;

        public XmlService(IFileService fileService)
        {
            _fileService = fileService;

            XmlSettings = new Dictionary<string, string>();

            BuildXmlSettings();
        }

        public void SaveSettings()
        {
            if (persistentXml == null)
                return;

            foreach (KeyValuePair<string, string> kvp in XmlSettings) //flat XML for now
            {
                if (kvp.Key.Contains(".") && kvp.Value != null)
                {
                    string parentNode = kvp.Key.Substring(0, kvp.Key.IndexOf("."));
                    XElement element = persistentXml.Element("persistenceData").Element(parentNode).Element(kvp.Key.Replace(parentNode + ".", ""));

                    if (element != null)
                        element.Value = kvp.Value;
                }

            }
            _fileService.WriteFile(_fileService.EnvironmentDirectory + @"\persistence.xml", GetXmlData());

        }

        private void BuildXmlSettings()
        {
            string settingsData = _fileService.ReadFile(_fileService.EnvironmentDirectory + @"\persistence.xml");
            persistentXml = XDocument.Parse(settingsData);

            foreach (XElement element in persistentXml.Nodes())
            {
                Process(element, 0); // recurse to get all xml document elements
            }
        }

        private void Process(XElement element, int depth)
        {
            if (!element.HasElements)
            {
                // element is child with no descendants
                // ignore element
            }
            else
            {
                // element is parent with children

                depth++;

                foreach (var child in element.Elements())
                {
                    if (child.HasElements)
                    {
                        Process(child, depth);
                        continue;
                    }

                    string dictKey = element.Name.LocalName + "." + child.Name.LocalName;
                    if (!XmlSettings.ContainsKey(dictKey))
                        XmlSettings.Add(dictKey, child.Value);
                    Process(child, depth);
                }

                depth--;
            }
        }

        private string GetXmlData()
        {
            return persistentXml.Declaration.ToString() + "\r\n" + persistentXml.ToString();
        }
    }
}
