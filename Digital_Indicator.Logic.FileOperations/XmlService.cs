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
            foreach(XElement element in settingsDoc.Descendants("settings").Nodes())
            {
                XmlSettings.Add(settingsDoc.Root.Name.LocalName + "." + element.Name.LocalName, element.Value);
            }

        }
    }
}
