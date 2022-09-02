using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace TerminalRbkm.Config
{
    public class XmlHistory
    {
        public static string FilenameWindow = "\\Window.xml";


        public static object GetXmlData(string filename)
        {
            String path = Properties.Settings.Default.PathSaveSetting;

            if (!Directory.Exists(path))
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                dir.Create();
            }

            if (filename == FilenameWindow)
            {
                try
                {
                    XmlSerializer reader = new XmlSerializer(typeof(List<string>));
                    StreamReader file = new StreamReader(path + filename);
                    List<string> result = new List<string>();
                    result = (List<string>)reader.Deserialize(file);
                    return result;
                }
                catch (Exception)
                {
                    return new List<string>();
                }
            }




            return null;
        }

        public static void SetXmlData(string filename, object list)
        {
            String path = Properties.Settings.Default.PathSaveSetting;

            if (!Directory.Exists(path))
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                dir.Create();
            }


            if (filename == FilenameWindow)
            {
                try
                {
                    XmlSerializer write = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                    using (StreamWriter file = new System.IO.StreamWriter(path + filename))
                        write.Serialize(file, list);
                }
                catch (Exception ex)
                {
                    MainWindow.Log.Error(DateTime.Now + "   " + "Ошибка. Сохранение: " + ex.Message);
                }
            }



        }

    }
}