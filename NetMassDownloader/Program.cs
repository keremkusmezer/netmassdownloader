/*------------------------------------------------------------------------------
 * SourceCodeDownloader - Kerem Kusmezer (keremskusmezer@gmail.com)
 *                        John Robbins (john@wintellect.com)
 * 
 * Download all the .NET Reference Sources and PDBs to pre-populate your 
 * symbol server! No more waiting as each file downloads individually while
 * debugging.
 * ---------------------------------------------------------------------------*/

#region Licence Information
/*       
 * http://www.codeplex.com/NetMassDownloader To Get The Latest Version
 *     
 * Copyright 2008 Kerem Kusmezer(keremskusmezer@gmail.com) And John Robbins (john@wintellect.com)
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

#region Imported Libraries
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using DownloadLibrary.Classes;
using System.Net;
using DownloadLibrary.Classes.Eula;
using System.Windows.Forms;
using DownloadLibrary.PEParsing;
#endregion

#region Licence Information
/*       
 * http://www.codeplex.com/NetMassDownloader To Get The Latest Version
 *     
 * Copyright 2008 Kerem Kusmezer(keremskusmezer@gmail.com) And John Robbins (john@wintellect.com)
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

namespace NetMassDownloader
{
    class Program
    {
        private static DownloaderArgParser argValues;

        // The number of files fully processed.
        private static int numProcessFiles;
        // The number of files where there were errors.
        private static int numNotProcessedFiles;
        // The number of source files downloaded.
        private static int numSourceFiles;

        // The flag to indicate I created the directory in the symbol server.
        private static Boolean createdSymbolDir;

        /// <summary>
        /// The entry point to the whole application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments to the program.
        /// </param>
        static int Main ( string [] args )
        {
            int retValue = 0;

            argValues = new DownloaderArgParser ( );

            // If there are no arguments, just return now.
            if ( args.Length == 0 )
            {
                argValues.OnUsage ( null );
                return ( 1 );
            }

            if ( true == argValues.Parse ( args ) )
            {
                argValues.Logo ( );

                try
                {
                    ProcessFiles ( );
                    ShowRunStats ( );
                }
                catch ( EulaNotAcceptedException )
                {
                    Console.WriteLine ( Constants.NotEulaFile );
                    retValue = 2;
                }
            }
            else
            {
                retValue = 1;
            }
            return ( retValue );
        }

        private static void ShowRunStats ( )
        {
            Console.WriteLine ( Constants.RunStatsFmt ,
                                numProcessFiles ,
                                numNotProcessedFiles ,
                                numSourceFiles );
        }

        static void ProcessFiles ( )
        {
            XmlPEFileList xmlPEFileList = new XmlPEFileList();

            foreach ( String file in argValues.Files )
            {
                // Build up the output path.
                String finalPdbPath = argValues.Output;
                try
                {
                    PEFile peFile = new AppPEFile( file );
                    xmlPEFileList.Items.Add( new XmlPEFileItem(peFile) );

                    if (! AppSettings.DownloadSymbols) {
                        continue;
                    }

                    peFile.CleanupTempCompressedSymbols = AppSettings.CleanupTempCompressedSymbols;
                    peFile.SymbolServerUrl = AppSettings.SymbolServerUrl;

                    bool doTheDownload = true;
                    if ( true == argValues.UsingSymbolCache )
                    {
                        finalPdbPath =
                           CreateSymbolServerDirectory ( finalPdbPath ,
                                                          peFile ,
                                                          argValues.VsVersion );
                    }

                    // The final pdb file.
                    String pdbToProcess = finalPdbPath + peFile.PdbFileName;
                    bool pdbFileAlreadyExists = File.Exists( pdbToProcess );

                    // We're downloading to a symbol server and not forcing, 
                    // look to see if the file already exists. If it does, skip 
                    // it.
                    if ( ( true == argValues.UsingSymbolCache ) &&
                         ( false == argValues.Force ) )
                    {
                        if (pdbFileAlreadyExists)
                        {
                            if (! AppSettings.DownloadSourceCode) {
                                numNotProcessedFiles++;
                                doTheDownload = false;
                                //Console.WriteLine (
                                            //Constants.PdbAlreadyInSymbolServer ,
                                            //Environment.NewLine ,
                                            //file );
                            }
                        }
                    }

                    if ( true == doTheDownload )
                    {
                        if (! pdbFileAlreadyExists) {
                            Console.Write( Constants.DownloadingPdb, peFile.PdbFileName );
                        }
                        else {
                            Console.WriteLine( Constants.ProcessingPdb, peFile.PdbFileName );
                        }

                        peFile.ProxyMatch = argValues.ProxyMatch;

                        // Get the PDB file itself from the server.
                        bool downloadPdbFileOk = true;
                        if (! pdbFileAlreadyExists) {
                            downloadPdbFileOk = peFile.DownloadPDBFromServer( finalPdbPath );
                        }

                        //if ( null != resultStream )
                        if (downloadPdbFileOk && AppSettings.DownloadSourceCode)
                        {
                            try
                            {
                                PdbFileExtractor extract = new AppPdbFileExtractor(pdbToProcess);
                                extract.SkipExistingSourceFiles = AppSettings.SkipExistingSourceFiles;
                                extract.ProxyMatch = argValues.ProxyMatch;
                                // If we are not extracting to a symbol server use
                                // the file paths in the PDB so everything works 
                                // with VS 2005.
                                extract.UseSourceFilePath =
                                                        !argValues.UsingSymbolCache;

                                extract.SourceFileDownloaded += new PdbFileExtractor.
                                         SourceCodeHandler(SourceFileDownloaded);

                                extract.EulaAcceptRequested += new PdbFileExtractor.
                                                EulaHandler(EulaAcceptRequested);

                                String finalSrcPath = argValues.Output;

                                if (true == argValues.UsingSymbolCache)
                                {
                                    /* Visual Studio is hardcoded to look at the 
                                     "src\source\.net\8.0" directory for VS 2008 
                                     RTM and "src\source\dotnetfx35SP1_3053\1.1" 
                                     for VS 2008 SP1. As SP 3.5 SP1 requires VS 
                                     2008 SP1, let's assume that. This will
                                     probably break in a future release.
                                     Bugfix Kerem Kusmezer We Already Have The Version Detection So Why Not Use It
                                     Kerem Kusmezer Information Update
                                     This information is actually wrong, because the srvsrc file already holds this information for each of the pdb's
                                     So we don't need to hardcode this information here 
                                     For Example When I Compare The SrvSrc For mscorlib for SP1 and RTM The Http Alias Is Like This:
                                     RTM HTTP_ALIAS=Http://ReferenceSource.microsoft.com/source/.net/8.0
                                     SP1  HTTP_ALIAS=Http://ReferenceSource.microsoft.com/source/dotnetfx35SP1_3053/1.1
                                     so we can always extract this information from the pdb itself directly instead of hardcoding it directly.
                                     TODO Fix Here In The Main Engine Accordingly Kerem Kusmezer To Support Future Versions
                                    */

                                    if (FrameworkVersionData.RelevantVersionData.SPLevel == 0)
                                    {
                                        finalSrcPath +=
                                                @"src\source\.net\8.0\";
                                    }
                                    else
                                    {
                                        finalSrcPath +=
                                               @"src\source\dotnetfx35SP1_3053\1.1";
                                    }

                                }
                                extract.DownloadWholeFiles(finalSrcPath);
                                numProcessFiles++;
                            }
                            catch (InvalidOperationException invalidException)
                            {
                                //Kerem Kusmezer BugFix 31.08.2008
                                Console.WriteLine(invalidException.Message);
                                numNotProcessedFiles++;
                            }
                            Console.WriteLine();
                        }
                        else
                        {
                            if (! downloadPdbFileOk) {
                                numNotProcessedFiles++;
                            }
                        }
                    }
                }
                catch ( WebException webException)
                {
                    // Couldn't find it on the symbol server.
                    if ( true == argValues.UsingSymbolCache )
                    {
                        DeleteSymbolServerDirectory ( finalPdbPath );
                    }
                    numNotProcessedFiles++;
                    if (webException.Status == WebExceptionStatus.ProxyNameResolutionFailure ||
                        webException.Status == WebExceptionStatus.RequestProhibitedByProxy)
                    {
                        Console.WriteLine(Constants.ProxyBasedError, webException.Message);
                        break;
                        //throw;
                    }
                    else if (webException.Status == WebExceptionStatus.NameResolutionFailure)
                    {
                        Console.WriteLine(Constants.HostNameNotResolved, webException.Message);
                        break;
                        //throw;
                    }
                    else
                    {
                        Console.WriteLine(Constants.NotOnSymbolServer, file);
                    }                    
                }
                catch ( FileLoadException )
                {
                    // This is not a PE file.
                    if ( true == argValues.UsingSymbolCache )
                    {
                        DeleteSymbolServerDirectory ( finalPdbPath );
                    }
                    Console.WriteLine ( Constants.NotPEFileFmt , file );
                    numNotProcessedFiles++;
                }
                catch ( NoDebugSectionException )
                {
                    // There is not a .debug section in the PE file.
                    Console.WriteLine( Constants.NoDebugSection, 
                                       Path.GetFileName (file) );
                    numNotProcessedFiles++;
                }
            }

            //{{HDN==================================================
            if (xmlPEFileList.Items.Count > 0) {
                string peSymbolListFilePath = argValues.Output + "PESymbolList.xml";

                if (File.Exists( peSymbolListFilePath )) {
                    File.Delete( peSymbolListFilePath );
                }
                File.WriteAllText( peSymbolListFilePath, xmlPEFileList.Serialize(), Encoding.UTF8 );
            }
            //}}HDN==================================================
        }

        static void EulaAcceptRequested ( object sender , EulaRequestEvent e )
        {
            EulaDialog dlg = new EulaDialog ( e.EulaContent );
            DialogResult res = dlg.ShowDialog ( );
            e.EulaAccepted = ( res == DialogResult.Yes );
        }

        static void SourceFileDownloaded ( object sender ,
                                           SourceFileLoadEventArg e )
        {
            if ( null != e.OccurredException )
            {
                Console.WriteLine( e.ToString() );
            }
            else
            {
                if ( true == argValues.Verbose )
                {
                    Console.WriteLine ( Constants.DownloadedFileOfFmt ,
                                        e.FileLocationState ,
                                        Path.GetFileName ( e.FileName ) ,
                                        Path.GetDirectoryName ( e.FileName ) );
                }
                numSourceFiles++;
            }
        }

        /// <summary>
        /// Deletes the final symbol server directory for a failed download.
        /// </summary>
        /// <param name="finalPdbDirectory">
        /// The complete directory name including GUID.
        /// </param>
        static void DeleteSymbolServerDirectory ( String finalPdbDirectory )
        {
            if ( true == createdSymbolDir )
            {
                // I need to delete the directory above the final directory and 
                // this allows me to do that. :)
                String parentDir = finalPdbDirectory + "..\\";
                if ( true == Directory.Exists ( finalPdbDirectory ) )
                {
                    // Delete the GUID directory.
                    Directory.Delete ( finalPdbDirectory );
                    // The FOO.PDB directory could already exist from a symbol
                    // download by a debugger. If the directory is not empty
                    // skip it.
                    if ( ( Directory.GetFiles ( parentDir ).Length == 0 ) &&
                         ( Directory.GetDirectories ( parentDir ).Length == 0 ) )
                    {
                        // Delete the FOO.PDB directory.
                        Directory.Delete ( parentDir );
                    }
                }
            }
        }

        /// <summary>
        /// Creates and returns the full symbol path where the PDB file needs 
        /// to be downloaded to.
        /// </summary>
        /// <param name="symbolServer">
        /// The root symbol server cache directory.
        /// </param>
        /// <param name="peFileInfo">
        /// The <see cref="PEFile"/> class describing the loaded PE file.
        /// </param>
        /// <param name="vsVersion">
        /// The version of Visual Studio using the cache.
        /// </param>
        /// <returns>
        /// The full path name for the PDB file location.
        /// </returns>
        static String CreateSymbolServerDirectory ( String symbolServer ,
                                                    PEFile peFileInfo ,
                                                    String vsVersion )
        {
            // Starting with VS 2008 SP1, the PDBs are downloaded to 
            // <symbol cache>\\MicrosoftPublicSymbols and that's the only place
            // where the VS debugger looks at them for source. If we are 
            // running for version 9.0, add the MicrosoftPublicSymbols onto
            // the symbol cache name.
            // BugFix Kerem Kusmezer Check Also The SPLevel So It now works without SP1 also.
            if ( "9.0" == vsVersion  && FrameworkVersionData.RelevantVersionData.SPLevel > 0)
            {
                symbolServer += "MicrosoftPublicSymbols\\";
            }

            // Append on the PDB name and GUID since this is going into the 
            // symbol server. If there's any path information on the PDB file,
            // I'll only do the filename itself.
            String pdbFile = peFileInfo.PdbFileName;
            String currPath = symbolServer + pdbFile + "\\";
            currPath += peFileInfo.PdbVersion;
            currPath += "\\";
            if ( false == Directory.Exists ( currPath ) )
            {
                Directory.CreateDirectory ( currPath );
                createdSymbolDir = true;
            }
            else
            {
                createdSymbolDir = false;
            }
            return ( currPath );
        }
    }
}
