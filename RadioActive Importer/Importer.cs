using System;
using System.IO;
using System.Net;
using System.Xml;
using MeediOS;

namespace RadioActive
{


    public class Importer : IMLImportPlugin
    {


        public bool Import(IMLSection Section, IMLImportProgress Progress)
        {


            XmlDocument IceCastDirectoryXml = new XmlDocument();

            WebClient client = new WebClient();

            const string iceCastDirectoryURL = "http://dir.xiph.org/yp.xml";

            Console.WriteLine("Downloading xml data...");

            Progress.Progress(0, "Loading IceCast station directory...");

            byte[] data = client.DownloadData(iceCastDirectoryURL);

            Stream stream = new MemoryStream(data);

            IceCastDirectoryXml.Load(stream);

            XmlNode directoryNode = IceCastDirectoryXml.ChildNodes[1];

            Console.WriteLine(directoryNode.Name);


            XmlNodeList directoryEntries = directoryNode.ChildNodes;



            Section.BeginUpdate();


            foreach (XmlNode radioStation in directoryEntries)
            {

                XmlNodeList stationProperties = radioStation.ChildNodes;


                string itemName = String.Empty;
                string itemLocation = String.Empty;
                string itemBitrate = String.Empty;
                string itemGenre = String.Empty;


                foreach (XmlNode stationProperty in stationProperties)
                {


                    if (stationProperty.Name == "server_name")
                    {
                        itemName = stationProperty.InnerText;

                    }


                    if (stationProperty.Name == "listen_url")
                    {
                        itemLocation = stationProperty.InnerText;

                    }


                    if (stationProperty.Name == "bitrate")
                    {
                        itemBitrate = stationProperty.InnerText;

                    }

                    if (stationProperty.Name == "genre")
                    {
                        itemGenre = stationProperty.InnerText;

                    }

               


                }


                if (String.IsNullOrEmpty(itemName))
                    continue;

                if (String.IsNullOrEmpty(itemLocation))
                    continue;


                IMLItem item
                    = Section.AddNewItem
                    (itemName, itemLocation);


                item.Tags["BitRate"] = itemBitrate;
                item.Tags["Genre"] = itemGenre;

                item.SaveTags();
                




            }

            Section.EndUpdate();



            Progress.Progress(100, "RadioActive completed succesfully!");

            return true;
        }








        public bool EditCustomProperty(IntPtr Window, string PropertyName, ref string Value)
        {



            return false;
        }


        public bool SetProperties(IMeedioItem Properties, out string ErrorText)
        {
            ErrorText = String.Empty;
            return true;
        }


        public bool GetProperty(int Index, IMeedioPluginProperty Prop)
        {



            return false;
        }



    }
     






}
