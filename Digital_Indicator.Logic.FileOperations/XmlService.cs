using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Digital_Indicator.Logic.FileOperations
{
    public class XmlService : IXmlService
    {
        IFileService _fileService;

        public Dictionary<string, string> XmlSettings { get; set; }
        

        public XmlService(IFileService fileService)
        {
            _fileService = fileService;

            XmlSettings = new Dictionary<string, string>();

            BuildXmlSettings();
        }

        public void SaveSettings()
        {

        }

        private void BuildXmlSettings()
        {
            string settingsData = _fileService.ReadFile(_fileService.EnvironmentDirectory + @"\persistence.xml");
            XDocument settingsDoc = XDocument.Parse(settingsData);

            foreach (XElement element in settingsDoc.Nodes())
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
                    string dictKey = element.Name.LocalName + "." + child.Name.LocalName;
                    if (!XmlSettings.ContainsKey(dictKey))
                        XmlSettings.Add(dictKey, child.Value);
                    Process(child, depth);
                }

                depth--;
            }
        }
    }
}
