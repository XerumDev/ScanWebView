using Java.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ScanWebView
{
    public class ConfigFile : IConfigFile
    {
        public string Url { get; set; }

        public ConfigFile()
        {
            //default values
            Url = "file:///android_asset/Test.html";
        }

        public static ConfigFile InitializeConfig()
        {
            ConfigFile configFile = new ConfigFile();

            var dirPath = "/sdcard/Android/data/ScanWebView";
            var filePath = dirPath + "/config.json";
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                        var newfile = new Java.IO.File(dirPath, "config.json");
                        using (FileOutputStream outfile = new FileOutputStream(newfile))
                        {
                            string line = JsonConvert.SerializeObject(configFile);
                            outfile.Write(System.Text.Encoding.ASCII.GetBytes(line));
                            outfile.Close();
                        }
                    }
                }
                else
                {
                    //read file config
                    using (StreamReader streamRdr = new StreamReader(filePath))
                    {
                        string content = streamRdr.ReadToEnd();
                        configFile = JsonConvert.DeserializeObject<ConfigFile>(content);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return configFile;
        }
    }
    public interface IConfigFile
    {
        string Url { get; set; }
    }
}