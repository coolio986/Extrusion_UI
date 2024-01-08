using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace ExtrusionUI.Logic.FileOperations
{
    public class FileService : IFileService
    {
        public string EnvironmentDirectory { get; private set; }
        public FileService()
        {
            //EnvironmentDirectory = Path.Combine(Environment.CurrentDirectory, "spooldata");

            EnvironmentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            //EnvironmentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            EnvironmentDirectory = Path.Combine(EnvironmentDirectory, "ExtrusionUI");
            EnvironmentDirectory = Path.Combine(EnvironmentDirectory, "spooldata");

            CheckEnvironment();
        }
        public string ReadFile(string filename)
        {
            string contents = string.Empty;
            using (StreamReader fileContents = new StreamReader(filename))
            {
                contents = fileContents.ReadToEnd();
            }
            return contents;
        }

        public void WriteFile(string filename, string filedata)
        {
            File.WriteAllText(filename, filedata);
        }

        public void AppendFile(string filename, string filedata)
        {
            File.AppendAllText(filename, filedata);
        }

        public void AppendLog(string filedata)
        {
            lock (this)
            {
                DateTime dateTime = DateTime.Now;
                string datetime = string.Empty;
                datetime = dateTime.ToString();
                datetime = datetime.Substring(0, datetime.Length - 3);
                filedata = datetime + "." + dateTime.Millisecond.ToString() + "-> " + filedata + "\r\n";

                AppendFile(EnvironmentDirectory + "\\log.txt", filedata);
            }
        }

        private void CheckEnvironment()
        {
            if (!Directory.Exists(EnvironmentDirectory))
                Directory.CreateDirectory(EnvironmentDirectory);

            if (!File.Exists(EnvironmentDirectory + @"\persistence.xml"))
                File.Create(EnvironmentDirectory + @"\persistence.xml").Close();
            

            try
            {
                XDocument xdoc = XDocument.Load(EnvironmentDirectory + @"\persistence.xml");
                var version = (string)xdoc.Declaration.Version;
            }
            catch (Exception oe)
            {
                if (oe.Message.ToLower().Contains("root element is missing."))
                {
                    var doc = new XDocument(new XDeclaration("1.0", "", ""), new XElement("persistenceData"));
                    doc.Element("persistenceData").Add(new XElement("filamentData"));
                    XElement settings = doc.Element("persistenceData").Element("filamentData");
                    settings.Add(new XElement("previousBatchNumber", "0"));
                    settings.Add(new XElement("materialDescription", ""));
                    settings.Add(new XElement("filamentNominalDiameter", "1.75"));
                    settings.Add(new XElement("upperLimit", "1.80"));
                    settings.Add(new XElement("lowerLimit", "1.70"));
                    settings.Add(new XElement("spoolNumber", "0"));
                    settings.Add(new XElement("batchNumber", "0"));
                    doc.Save(EnvironmentDirectory + @"\persistence.xml");
                }
                else
                {
                    MessageBox.Show(oe.Message);
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
