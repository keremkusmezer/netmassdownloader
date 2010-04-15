#region Licence Information
/*       
 * http://www.codeplex.com/NetMassDownloader To Get The Latest Version
 *     
 * Copyright 2008 Kerem Kusmezer(izzetkeremskusmezer@gmail.com)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
#endregion
///*------------------------------------------------------------------------------
// * SourceCodeDownloader - Kerem Kusmezer (izzetkeremskusmezer@gmail.com)
// *                        
// * Download all the .NET Reference Sources and PDBs to pre-populate your 
// * symbol server! No more waiting as each file downloads individually while
// * debugging.
// * ---------------------------------------------------------------------------*/
#region Imported Libraries
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
#endregion

#region Notes
///*
//SRCSRV: ini ------------------------------------------------ 
//VERSION=2 
//INDEXVERSION=2 
//VERCTRL=http
//DATETIME=Wed Oct 24 01:03:24 2007 
//SRCSRV: variables ------------------------------------------ 
//SRCSRVVERCTRL=http
//DEVDIV=DDRTSD:4000 
//HTTP_ALIAS=Http://ReferenceSource.microsoft.com/source/.net/8.0
//HTTP_EXTRACT_TARGET=%HTTP_ALIAS%/%var2%/%var3%/%var4%/%fnfile%(%var1%)
//SRCSRVTRG=%http_extract_target%
//SRCSRVCMD=



//Source Server Data Blocks
//Source server relies on two blocks of data within the PDB file.


//Source file list. Building a module automatically creates a list of fully-qualified paths to the source files used to build the module. 
//Data block. Indexing the source as described previously adds an alternate stream to the PDB file named "srcsrv". The script that inserts this data is dependent on the specific build process and source control system in use. 
//In the language specification version 1, the data block is divided into three sections: ini, variables, and source files. It has the following syntax.


// Copy Code
//SRCSRV: ini ------------------------------------------------ 
//VERSION=1
//VERCTRL=<source_control_str>
//DATETIME=<date_time_str>
//SRCSRV: variables ------------------------------------------ 
//SRCSRVTRG=%sdtrg% 
//SRCSRVCMD=%sdcmd% 
//SRCSRVENV=var1=string1\bvar2=string2 
//DEPOT=//depot 
//SDCMD=sd.exe -p %fnvar%(%var2%) print -o %srcsrvtrg% -q %depot%/%var3%#%var4%
//SDTRG=%targ%\%var2%\%fnbksl%(%var3%)\%var4%\%fnfile%(%var1%) 
//WIN_SDKTOOLS= sserver.microsoft.com:4444 
//SRCSRV: source files --------------------------------------- 
//<path1>*<var2>*<var3>*<var4> 
//<path2>*<var2>*<var3>*<var4> 
//<path3>*<var2>*<var3>*<var4> 
//<path4>*<var2>*<var3>*<var4> 
//SRCSRV: end ------------------------------------------------All text is interpreted literally, except for text enclosed in percent signs (%). Text enclosed in percent signs is treated as a variable name to be resolved recursively, unless it is one of the following functions:


//%fnvar%() 
//The parameter text should be enclosed in percent signs and treated as a variable to be expanded. 

//%fnbksl%() 
//All forward slashes (/) in the parameter text should be replaced with backward slashes (\).

//%fnfile%() 
//All path information in the parameter text should be stripped out, leaving only the file name.

//The ini section contains variables that describe the requirements. The indexing script can add any number of variables to this section. The following are examples:


//VERSION 
//The language specification version. This variable is required.

//VERCTL 
//A string that describes the source control product. This variable is optional.

//DATETIME 
//A string that indicates the date and time the PDB file was processed. This variable is optional.

//The variables section contains variables that describe how to extract a file from source control. It can also be used to define commonly used text as variables to reduce the size of the data block.


//SRCSRVTRG 
//Describes how to build the target path for the extracted file. This is a required variable.

//SRCSRVCMD 
//Describes how to build the command to extract the file from source control. This includes the name of the executable file and its command-line parameters. This is a required variable.

//SRCSRVENV 
//A string that lists environment variables to be created during the file extraction. Separate multiple entries with a backspace character (\b). This is an optional variable.

//The source files section contains an entry for each source file that has been indexed. The contents of each line are interpreted as variables with the names VAR1, VAR2, VAR3, and so on until VAR10. The variables are separated by asterisks. VAR1 must specify the fully-qualified path to the source file as listed elsewhere in the PDB file. For example, the following line: 


// Copy Code
//c:\proj\src\file.cpp*TOOLS_PRJ*tools/mytool/src/file.cpp*3is interpreted as follows:


// Copy Code
//VAR1=c:\proj\src\file.cpp
//VAR2=TOOLS_PRJ
//VAR3=tools/mytool/src/file.cpp
//VAR4=3In this example, VAR4 is a version number. However, most source control systems support labeling files in such a way that the source state for a given build can be restored. Therefore, you could alternately use the label for the build. The sample data block could be modified to contain a variable such as the following:


// Copy Code
// LABEL=BUILD47Then, presuming the source control system uses the at sign (@) to indicate a label, you could modify the SRCSRVCMD variable as follows:

//sd.exe -p %fnvar%(%var2%) print -o %srcsrvtrg% -q %depot%/%var3%@%label%
//*/
#endregion

namespace DownloadLibrary.Classes
{
    public class SrcSrvDownloadAbleFile
    {
        private string m_localFileTarget;

        public string LocalFileTarget
        {
            get { return m_localFileTarget; }
            set { m_localFileTarget = value; }
        }

        private string m_urlToBeRequested;

        public string UrlToBeRequested
        {
            get { return m_urlToBeRequested; }
            set { m_urlToBeRequested = value; }
        }
        private string m_localFileTargetAlternative;

        public string LocalFileTargetAlternative
        {
            get
            {
                return m_localFileTargetAlternative;
            }
            set
            {
                m_localFileTargetAlternative = value;
            }
        }
    }
    public class SrcSrvFile
    {
        #region Constants
        private System.Collections.Hashtable m_tempTable = new System.Collections.Hashtable();

        // The indexes into the srcsrv file info string where the pieces are 
        // located.
        private const int BuildIndex = 0;
        private const int SrcSrvIndex = 1;
        private const int SymbolIndex = 2;
        private const int VersionIndex = 3;

        //Partitions So Far Detected In The SrcSrvFile , which is included to the pdb 
        private const string FileListBegin =
            "SRCSRV: source files ---------------------------------------";

        private const string FileListEnd =
            "SRCSRV: end ------------------------------------------------";

        private const string FileSrcSrv =
            "SRCSRV: ini ------------------------------------------------";

        //The Http_extract_target Uses 3 of them this one is equal to .Net Frameworks System.IO.Path.GetFileName %fnfile%
        private static Regex fnfileCleaner =
            new Regex(@"\%fnfile\%\(\%var(?<filenumber>(10|[1-9]))\%\)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="srcsrvFile"></param>
        /// <returns></returns>
        private SrcSrvDownloadAbleFile
            BuildSrcSrvDownloadFromFile(string targetPath, string srcsrvFile)
        {
            String[] rawData = srcsrvFile.Split(new Char[] { '*' });
            if (rawData.Length != 4)
            {
                return (null);
            }

            // With VS 2008 SP1, some of the files on the reference symbol
            // server don't have the HTTP_ALIAS value set.
            if ( null == Http_extract_target )
            {
                return ( null );
            }

            string tempTarget =
                Http_extract_target;

            tempTarget =
                tempTarget.Replace("%HTTP_ALIAS%", Http_Alias);

            int fnFileIndex = 0;

            while ((fnFileIndex = tempTarget.IndexOf("%fnfile%")) > 0)
            {
                Match tempMatch = fnfileCleaner.Match(tempTarget);
                //tempMatch.Value;
                if (tempMatch.Success)
                {
                    int fileNumber =
                        Convert.ToInt32(tempMatch.Groups["filenumber"].Value) - 1;

                    tempTarget =
                        tempTarget.Replace(tempMatch.Value, System.IO.Path.GetFileName(rawData[fileNumber]));
                }
            }

            for (int i = 0; i < rawData.Length; i++)
            {
                tempTarget =
                    tempTarget.Replace(String.Format("%var{0}%", i + 1), rawData[i]);
            }
            // Build up the output directory.
            StringBuilder targetKey = new StringBuilder();
            targetKey.Append(Utility.CleanupPath(targetPath));
            targetKey.Append(Utility.CleanupPath(rawData[SrcSrvIndex]));
            targetKey.Append(Utility.CleanupPath(rawData[SymbolIndex].
                                                       Replace('/', '\\')));
            targetKey.Append(Utility.CleanupPath(rawData[VersionIndex]));
            targetKey.Append(Path.GetFileName(rawData[BuildIndex]));

            SrcSrvDownloadAbleFile tempFile =
                new SrcSrvDownloadAbleFile();

            tempFile.LocalFileTarget = targetKey.ToString();
            
            tempFile.LocalFileTargetAlternative = 
                Utility.CleanupPath(targetPath) + Utility.PreparePath(rawData[0]);
            tempFile.UrlToBeRequested = tempTarget;

            return tempFile;
        }

        private String[] BuildKeyValuePairFromString(String targetPath, String srcsrvFile)
        {
            // Split the string on the asterisks.
            String[] rawData = srcsrvFile.Split(new Char[] { '*' });
            //Debug.Assert ( null != rawData , "null != rawData" );
            //Debug.Assert ( rawData.Length == 4 , "rawData.Length == 4" );
            try
            {
                if (rawData.Length != 4)
                {
                    return (null);
                }
                // Yes, recombine the symbol and version so we have that for the 
                // value.
                String downloadValue = rawData[SymbolIndex] + "*" +
                                                           rawData[VersionIndex];
                // Build up the output directory.
                String targetKey = Utility.CleanupPath(targetPath);
                targetKey += Utility.CleanupPath(rawData[SrcSrvIndex]);
                targetKey += Utility.CleanupPath(rawData[SymbolIndex].
                                                           Replace('/', '\\'));
                targetKey += Utility.CleanupPath(rawData[VersionIndex]);
                targetKey += Path.GetFileName(rawData[BuildIndex]);
                return (new String[] { targetKey, downloadValue });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return (null);
            }
        }


        private string m_srcSrvBody;

        private string m_version;
        public string Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        private string m_verctrl;
        public string Verctrl
        {
            get { return m_verctrl; }
            set { m_verctrl = value; }
        }

        private string m_http_alias;
        public string Http_Alias
        {
            get { return m_http_alias; }
            set { m_http_alias = value; }
        }

        private string m_http_extract_target;

        public string Http_extract_target
        {
            get { return m_http_extract_target; }
            set { m_http_extract_target = value; }
        }

        private string m_targetPath;

        public SrcSrvFile(string srcSrvBody, string targetPath)
        {
            m_tempTable =
                new System.Collections.Hashtable(StringComparer.InvariantCultureIgnoreCase);

            if (String.IsNullOrEmpty(srcSrvBody))
                throw new ArgumentNullException("srcSrvBody");

            if (!srcSrvBody.StartsWith(FileSrcSrv))
                throw new FormatException("Invalid SRCSRV Format");

            m_targetPath = targetPath;

            m_srcSrvBody = srcSrvBody;

            LoadTypes();

        }

        private void LoadTypes()
        {
            System.Reflection.PropertyInfo[] wholeProperties =
                this.GetType().GetProperties();
            for (int i = 0; i < wholeProperties.Length; i++)
            {
                if (!m_tempTable.ContainsKey(wholeProperties[i].Name))
                {
                    m_tempTable.Add(wholeProperties[i].Name, wholeProperties[i]);
                }
            }
        }


        private List<SrcSrvDownloadAbleFile> m_filesToBeDownloaded =
                new List<SrcSrvDownloadAbleFile>();

        public List<SrcSrvDownloadAbleFile> FilesToBeDownloaded
        {
            get { return m_filesToBeDownloaded; }
            private set { m_filesToBeDownloaded = value; }
        }


        
        public void ParseBody()
        {

            FilesToBeDownloaded.Clear();

            bool insideFileParsing =
                false;

            using (System.IO.StringReader bodyReader =
                   new System.IO.StringReader(m_srcSrvBody))
            {
                string currentLine =
                    bodyReader.ReadLine();
                while ((currentLine = bodyReader.ReadLine()) != null)
                {
                    if (currentLine == FileListBegin)
                    {
                        insideFileParsing = true;
                        currentLine = bodyReader.ReadLine();
                        m_targetPath +=
                            m_http_alias.Replace("http://ReferenceSource.microsoft.com/",string.Empty).Replace("/","\\");
                        m_targetPath = Utility.CleanupPath(m_targetPath);
                    }
                    else if (currentLine == FileListEnd)
                    {
                        insideFileParsing = false;
                    }

                    if (String.IsNullOrEmpty(currentLine))
                        continue;

                    if (insideFileParsing)
                    {
                        SrcSrvDownloadAbleFile file = BuildSrcSrvDownloadFromFile ( m_targetPath , currentLine );
                        if ( null != file )
                        {
                            FilesToBeDownloaded.Add ( file );
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(currentLine) &&
                            !currentLine.StartsWith("SRCSRV"))
                        {
                            string[] parameters =
                                currentLine.Split('=');
                            if (parameters.Length == 2)
                            {
                                try
                                {
                                    if (null != m_tempTable[parameters[0]])
                                    {
                                        ((PropertyInfo)m_tempTable[parameters[0]]).SetValue(this, parameters[1], null);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
