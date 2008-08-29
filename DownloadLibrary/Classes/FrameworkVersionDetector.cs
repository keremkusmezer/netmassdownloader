using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
namespace DownloadLibrary.Classes
{
    public abstract class FrameworkVersionData
    {
        public abstract string GetUserAgentVersion();

        private string m_versionNumber;

        public string VersionNumber
        {
            get
            {
                return m_versionNumber;
            }
            protected set
            {
                m_versionNumber = value;
            }
        }

        private int m_spLevel;

        public int SPLevel
        {
            get
            {
                return m_spLevel;
            }
            protected set
            {
                m_spLevel = value;
            }
        }



        private int m_InstallLevel;

        public int InstallLevel
        {
            get
            {
                return m_InstallLevel;
            }
            protected set
            {
                m_InstallLevel = value;
            }
        }
        private string m_InstallPath;

        public string InstallPath
        {
            get
            {
                return m_InstallPath;
            }
            protected set
            {
                m_InstallPath = value;
            }
        }



        private string m_CBSLevel;

        public string CBSLevel
        {
            get
            {
                return m_CBSLevel;
            }
            protected set
            {
                m_CBSLevel = value;
            }
        }

        private string m_OCMLevel;

        public string OCMLevel
        {
            get
            {
                return m_OCMLevel;
            }
            protected set
            {
                m_OCMLevel = value;
            }
        }





        protected FrameworkVersionData(RegistryKey rootKey)
        {







            this.SPLevel = Convert.ToInt32(rootKey.GetValue("SP"));







            this.InstallLevel = Convert.ToInt32(rootKey.GetValue("Install").ToString());







            this.VersionNumber = rootKey.GetValue("Version").ToString();



        }





        static FrameworkVersionData()
        {

            m_relevantVersionData =



            new Framework35VersionData(FrameworkVersionDetector.V35DirectKey);



        }







        private static FrameworkVersionData m_relevantVersionData;







        public static FrameworkVersionData RelevantVersionData
        {





            get
            {



                //To Do Extended Here Later 







                //return new Framework35VersionData(FrameworkVersionDetector.V35DirectKey); 







                return m_relevantVersionData;



            }

        }

    }





    internal class Framework35VersionData : FrameworkVersionData
    {



        public Framework35VersionData(RegistryKey rootKey)
            : base(rootKey)
        {



        }





        public override string GetUserAgentVersion()
        {





            if (this.SPLevel > 0)
            {







                return "Microsoft-Symbol-Server/6.9.0003.113";



            }





            else
            {







                return "Microsoft-Symbol-Server/6.8.0004.0";



            }

        }

    }





    public static class FrameworkVersionDetector
    {



        internal const string FrameworkSetupBaseRegistryPath =







        @"SOFTWARE\Microsoft\Net Framework Setup\NDP\";







        internal const string Frameworkv20RegistryPathExtension =







        @"v2.0.50727";







        internal const string Frameworkv30RegistryPathExtension =







        @"v3.0";







        internal const string FrameworkV35RegistryPathExtension =







        @"v3.5";







        public static Microsoft.Win32.RegistryKey V20DirectKey
        {





            get
            {



                return







                Registry.LocalMachine.OpenSubKey(FrameworkSetupBaseRegistryPath + Frameworkv20RegistryPathExtension);



            }

        }





        public static Microsoft.Win32.RegistryKey V30DirectKey
        {





            get
            {



                return







                Registry.LocalMachine.OpenSubKey(FrameworkSetupBaseRegistryPath + Frameworkv30RegistryPathExtension);



            }

        }





        public static Microsoft.Win32.RegistryKey V35DirectKey
        {





            get
            {



                return







                Registry.LocalMachine.OpenSubKey(FrameworkSetupBaseRegistryPath + FrameworkV35RegistryPathExtension);



            }

        }

    }

}


