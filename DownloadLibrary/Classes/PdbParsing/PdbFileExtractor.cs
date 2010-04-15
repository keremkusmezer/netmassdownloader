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
 * 
 * 
 * Taken From The Following Project Also Written By Kerem Kusmezer 
 * PdbParser in C# http://www.codeplex.com/pdbparser 
 * 
*/
#endregion

#region Imported Libraries
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using DownloadLibrary.Classes.Eula;
using System.Text.RegularExpressions;
#endregion

namespace DownloadLibrary.Classes
{
    //[CLSCompliant(true)]
    public class PdbFileExtractor
    {

        #region Public Events

        public delegate void EulaHandler(Object sender, EulaRequestEvent e);

        public delegate void SourceCodeHandler(Object sender, SourceFileLoadEventArg e);

        /// <summary>
        /// Occurs When A File Is Downloaded
        /// </summary>
        public event SourceCodeHandler SourceFileDownloaded;

        /// <summary>
        /// Occurs During First Time Code Download Requested, 
        /// If Eula Not Accepted The Engine Stops Downloading The Source Code
        /// </summary>
        public event EulaHandler EulaAcceptRequested;

        protected virtual void OnEulaAcceptRequested(EulaRequestEvent e)
        {
            if (EulaAcceptRequested != null)
            {
                EulaAcceptRequested(this, e);
            }
        }

        protected virtual void OnSourceFileDownloaded(SourceFileLoadEventArg e)
        {
            if (SourceFileDownloaded != null)
            {
                SourceFileDownloaded(this, e);
            }
        }


        //{{HDN==================================================
        private bool _skipExistingSourceFiles;
        public bool SkipExistingSourceFiles {
            get { return _skipExistingSourceFiles; }
            set { _skipExistingSourceFiles = value; }
        }

        protected virtual PDBWebClient GetWebClientWithCookie() {
            return Utility.GetWebClientWithCookie( this.m_proxyMatch );
        }

        protected virtual void OnBeginDownloadSourceFile( object sender, DownloadFileEventArgs args ) {}
        protected virtual void OnDownloadSourceFileAfterEulaAccepted(
            object sender, DownloadFileEventArgs args ) {}
        //}}HDN==================================================

        #endregion

        #region Private Variables

        public bool ShowDialog
        {
            get
            {
                return m_showDialog;
            }
            set
            {
                m_showDialog = value;
            }
        }
        // The indexes into the srcsrv file info string where the pieces are 
        // located.
        private const int BuildIndex = 0;
        private const int SrcSrvIndex = 1;
        private const int SymbolIndex = 2;
        private const int VersionIndex = 3;
        private string m_pdbFileName;
        private System.IO.MemoryStream m_pdbStream;
        private bool m_showDialog;
        private PdbParser m_internalPdbParser;
        private bool m_useSourceFilePath;

        /// <summary>
        /// True if the user wants to use the file paths instead of the symbol 
        /// server paths. Set this to true for VS 2005 to use the 
        /// .NET Reference Source Code.
        /// </summary>
        public bool UseSourceFilePath
        {
            get
            {
                return m_useSourceFilePath;
            }
            set
            {
                m_useSourceFilePath = value;
            }
        }

        #endregion
               
        #region Constructors

        /// <summary>
        /// The Generated Pdb Parser Around The Provided Pdb Stream
        /// </summary>
        public PdbParser InternalPdbParser
        {
            get { return m_internalPdbParser; }
            private set { m_internalPdbParser = value; }
        }

        private void CreatePdbParser()
        {
            if (m_pdbStream != null)
            {
                m_internalPdbParser = 
                    new PdbParser(m_pdbStream);
            }
            else
            {
                byte [] data = System.IO.File.ReadAllBytes ( m_pdbFileName );
                if ( 0 == data.Length )
                {
                    String msg = m_pdbFileName + " appears corrupt" ;
                    throw new InvalidOperationException ( msg );
                }
                m_internalPdbParser = 
                    new PdbParser(new System.IO.MemoryStream(data));
            }
        }

        public PdbFileExtractor(string pdbFileName, System.IO.MemoryStream pdbStream)
        {
            m_pdbFileName = pdbFileName;
            if (m_pdbStream == null)
                throw new System.IO.FileNotFoundException("Following File Not Found", pdbFileName);
            m_pdbStream = pdbStream;
            CreatePdbParser();
        }

        public PdbFileExtractor(string pdbFileName)
        {
            if (System.IO.File.Exists(pdbFileName))
            {
                m_pdbFileName = pdbFileName;
            }
            else
            {
                throw new System.IO.FileNotFoundException("Following File Not Found", pdbFileName);
            }
            CreatePdbParser();
        }

        #endregion

        #region Public Instance Methods

        private Match m_proxyMatch;

        public Match ProxyMatch
        {
            protected get 
            {
                return m_proxyMatch;
            }
            set
            {
                m_proxyMatch = value;
            }
        }

        public List<SrcSrvDownloadAbleFile> RetrieveNormal(string targetPath)
        {
            String locatedFileIndex = LocateFileIndex();
            return null;
        }

        public List<SrcSrvDownloadAbleFile> RetrieveFileRelationsExtended(string targetPath)
        {
            targetPath = Utility.CleanupPath(targetPath);
            String locatedSrcSrvBody = LocateFileIndex();
            if ( null == locatedSrcSrvBody )
            {
                return ( new List<SrcSrvDownloadAbleFile>() ) ;
            }
            SrcSrvFile tempSrcSrvFile = new SrcSrvFile(locatedSrcSrvBody, targetPath);
            tempSrcSrvFile.ParseBody();
            return tempSrcSrvFile.FilesToBeDownloaded;
        }

        public bool DownloadWholeFiles(string targetPath)
        {
            List<SrcSrvDownloadAbleFile> pdbResults = 
                RetrieveFileRelationsExtended(targetPath);

            List<SrcSrvDownloadAbleFile>.Enumerator tempEnum =
                pdbResults.GetEnumerator();

            int i = 1;
            
            while (tempEnum.MoveNext())
            {
                
                PDBWebClient tempClient = GetWebClientWithCookie();
                

                string directoryName = String.Empty;
                try
                {
                    if (UseSourceFilePath)
                    {
                        directoryName =
                            System.IO.Path.GetDirectoryName(tempEnum.Current.LocalFileTargetAlternative);
                    }
                    else
                    {
                        directoryName =
                            System.IO.Path.GetDirectoryName(tempEnum.Current.LocalFileTarget);
                    }
                    DirectoryInfo createdDirectory = null;
                    if (!System.IO.Directory.Exists(directoryName))
                    {
                        createdDirectory =
                            Directory.CreateDirectory(directoryName);
                    }
                    else
                    {
                        createdDirectory = new DirectoryInfo(directoryName);
                    }
                    //{{HDN==================================================
                    DownloadFileEventArgs args = new DownloadFileEventArgs();
                    args.TargetFilePath = UseSourceFilePath
                        ? tempEnum.Current.LocalFileTargetAlternative : tempEnum.Current.LocalFileTarget;
                    if (_skipExistingSourceFiles) {
                        if (File.Exists(args.TargetFilePath) || Path.GetExtension(args.TargetFilePath) == ".h") {
                            continue;
                        }
                    }

                    OnBeginDownloadSourceFile( this, args );

                    byte[] downloadedData = new byte[] {};
                    bool downloadOk = tempClient.DownloadDataWithProgress(
                        tempEnum.Current.UrlToBeRequested, out downloadedData );
                    if (! downloadOk) {
                        createdDirectory.Delete(true);
                        continue;
                    }
                    //}}HDN==================================================

                    EulaRequestEvent eulaEventArg = null;

                    if (tempClient.IsEulaResponse)
                    {

                        eulaEventArg =
                            new EulaRequestEvent(new EulaContents(tempClient.EulaBody));

                        OnEulaAcceptRequested(eulaEventArg);

                        if (!eulaEventArg.EulaAccepted)
                        {
                            throw new EulaNotAcceptedException();
                        }
                        else
                        {
                            //{{HDN==================================================
                            OnDownloadSourceFileAfterEulaAccepted( this, args );
                            
                            downloadOk = tempClient.DownloadDataWithProgress(
                                tempEnum.Current.UrlToBeRequested + "?" + eulaEventArg.EulaContent.AcceptCmdKey, out downloadedData );
                            if (! downloadOk) {
                                continue;
                            }
                            //}}HDN==================================================
                        }

                    }

                    if (UseSourceFilePath)
                    {
                        System.IO.File.WriteAllBytes(tempEnum.Current.LocalFileTargetAlternative,
                                                     downloadedData);
                        if (tempClient.HasLastFileWriteTimeOnServer) {
                            File.SetLastAccessTimeUtc( tempEnum.Current.LocalFileTargetAlternative, tempClient.LastFileWriteTimeOnServer );
                        }
                    }
                    else
                    {
                        System.IO.File.WriteAllBytes(tempEnum.Current.LocalFileTarget,
                             downloadedData);
                        if (tempClient.HasLastFileWriteTimeOnServer) {
                            File.SetLastAccessTimeUtc( tempEnum.Current.LocalFileTarget, tempClient.LastFileWriteTimeOnServer );
                        }
                    }
                    OnSourceFileDownloaded(new SourceFileLoadEventArg((UseSourceFilePath?tempEnum.Current.LocalFileTargetAlternative:tempEnum.Current.LocalFileTarget), tempEnum.Current.UrlToBeRequested, i.ToString() + "/" + pdbResults.Count));
                }
                catch (Exception ex)
                {
                    if (ex is EulaNotAcceptedException)
                        throw;
                    OnSourceFileDownloaded(new SourceFileLoadEventArg((UseSourceFilePath ? tempEnum.Current.LocalFileTargetAlternative : tempEnum.Current.LocalFileTarget), tempEnum.Current.LocalFileTarget, i.ToString() + "/" + pdbResults.Count, ex));
                }
                i++;
            }           
            return true;
        }

        public Dictionary<String, String> RetrieveFileRelations(string targetPath)
        {
            targetPath = Utility.CleanupPath(targetPath);
            Dictionary<String, String> fileRelations = new Dictionary<string, string>();
            String locatedFileIndex = LocateFileIndex();
            string[] sourceFiles =
                locatedFileIndex.Split(new string[] { "SRCSRV: source files ---------------------------------------" }, StringSplitOptions.None);
            if (sourceFiles.Length == 2)
            {
                System.IO.StringReader tester = new System.IO.StringReader(sourceFiles[1].Substring(0,sourceFiles[1].IndexOf("SRCSRV: end ------------------------------------------------")).Trim());
                string pdbBodyToParse = null;
                while ((pdbBodyToParse = tester.ReadLine()) != null)
                {
                    // Skip all empty lines.
                    if (true == String.IsNullOrEmpty(pdbBodyToParse))
                    {
                        continue;
                    }

                    String[] processed = BuildKeyValuePairFromString(targetPath,
                                                                       pdbBodyToParse);
                    
                    if (null == processed)
                    {
                        Debug.WriteLine(pdbBodyToParse);
                    }
                    else
                    {
                        fileRelations.Add(processed[0], processed[1]);
                    }                   
                }
            }
            return fileRelations;
        }

        private String[] BuildKeyValuePairFromString(String targetPath, String srcsrvFile)
        {
            // Split the string on the asterisks.
            String[] rawData = srcsrvFile.Split(new Char[] { '*' });
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

        public String LocateFileIndex()
        {
            MemoryStream srvsrcLocation = 
                this.InternalPdbParser.GetStreamBeginsWithBytesArrayASCII(Constants.SRCSRVLocator);
            if ( null == srvsrcLocation )
            {
                return ( null );
            }
            return System.Text.Encoding.ASCII.GetString(srvsrcLocation.ToArray());
        }

        public bool UpdateFilePaths()
        {
            byte[] wholeLines =
                System.IO.File.ReadAllBytes(m_pdbFileName);
            wholeLines =
                Utility.ReplaceInByteArray(wholeLines, Utility.ToByteArray("663a5c"), Utility.ToByteArray("633a5c"));
            System.IO.File.WriteAllBytes(m_pdbFileName, wholeLines);
            return true;
        }

        #endregion

    }

    #region Class: DownloadFileEventArgs
    //[CLSCompliant(true)]
    public class DownloadFileEventArgs : EventArgs {
        private string _targetFilePath;
        private string _targetFileName;

        public string TargetFilePath {
            get { return _targetFilePath; }
            set {
                _targetFilePath = value;

                _targetFileName = null;
                if (_targetFilePath != null) {
                    _targetFileName = Path.GetFileName( _targetFilePath );
                }
            }
        }

        public string TargetFileName {
            get { return _targetFileName; }
        }
    }
    #endregion Class: DownloadFileEventArgs
}