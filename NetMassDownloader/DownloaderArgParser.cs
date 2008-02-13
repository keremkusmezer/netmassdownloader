#region Detail Information
/*------------------------------------------------------------------------------
 * SourceCodeDownloader - Kerem Kusmezer (keremskusmezer@gmail.com)
 *                        John Robbins (john@wintellect.com)
 * 
 * Download all the .NET Reference Sources and PDBs to pre-populate your 
 * symbol server! No more waiting as each file downloads individually while
 * debugging.
 * ---------------------------------------------------------------------------*/
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

#region Imported Libraries
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Win32;
using System.Text.RegularExpressions;
#endregion

namespace NetMassDownloader
{
    /// <summary>
    /// Does all the argument parsing and collection for the application.
    /// </summary>
    internal class DownloaderArgParser : ArgParser
    {

        /// <summary>
        /// The dirFiles to process. 
        /// </summary>
        public List<String> Files
        {
            get { return files; }
            set { files = value; }
        }
        private List<String> files;

        /// <summary>
        /// The output directory to use instead of the symbol server cache.
        /// </summary>
        public String Output
        {
            get { return output; }
            set { output = value; }
        }
        private String output;

        private bool useInternalList;

        /// <summary>
        /// If true, turns on download with pe detection from the internal storage , aka assembly version list.
        /// </summary>
        public bool UseInternalList
        {
            get
            {
                return useInternalList;
            }
            set
            {
                useInternalList = value;
            }
        }


        /// <summary>
        /// If true, turns off logo display.
        /// </summary>
        private Boolean NoLogo
        {
            get { return noLogo; }
            set { noLogo = value; }
        }
        private Boolean noLogo;

        /// <summary>
        /// Holds the VS version number.
        /// </summary>
        private String VsVersion
        {
            get { return vsVersion; }
            set { vsVersion = value; }
        }
        private String vsVersion;

        /// <summary>
        /// If true, does verbose output.
        /// </summary>
        public Boolean Verbose
        {
            get { return verbose; }
            set { verbose = value; }
        }
        private Boolean verbose;

        /// <summary>
        /// If true, forces the download of PDB files into the symbol server.
        /// </summary>
        public Boolean Force
        {
            get { return force; }
            set { force = value; }
        }
        private Boolean force;

        /// <summary>
        /// If true, using the symbol server cache.
        /// </summary>
        public Boolean UsingSymbolCache
        {
            get { return usingSymbolCache; }
            set { usingSymbolCache = value; }
        }
        private Boolean usingSymbolCache;

        /// <summary>
        /// Holds the error message generated while parsing.
        /// </summary>
        private String errorMessage;

        /// <summary>
        ///  Set to true if the error was found after parsing.
        /// </summary>
        private Boolean errorAfterParse;

        #region Flag Values
        const String argDirectory = "directory";
        const String argDirectoryShort = "d";
        const String argFile = "file";
        const String argFileShort = "f";
        const String argForce = "force";
        const String argForceShort = "fo";
        const String argHelp = "help";
        const String argHelpQuestion = "?";
        const String argHelpShort = "h";
        const String argNoLogo = "nologo";
        const String argNoLogoShort = "n";
        const String argOutput = "output";
        const String argOutputShort = "o";
        const String argVerbose = "verbose";
        const String argVerboseShort = "v";
        const String argVsVer = "vsver";
        const String argVsVerShort = "vs";
        //BugFix 1133 Define Additional Proxy Properties
        const String argProxy = "proxy";
        const String argProxyShort = "p";
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloaderArgParser"/> 
        /// class.
        /// </summary>
        public DownloaderArgParser ( )
            : base ( new String [] { argHelp  ,
                                        argHelpShort ,
                                        argHelpQuestion , 
                                     argNoLogo ,
                                        argNoLogoShort ,
                                     argVerbose ,
                                        argVerboseShort ,
                                     argForce ,
                                        argForceShort} ,
                     new String [] { argFile ,
                                        argFileShort ,
                                     argDirectory ,
                                        argDirectoryShort ,
                                     argOutput ,
                                        argOutputShort ,
                                     argVsVer ,
                                        argVsVerShort,
                                    argProxy,
                                     argProxyShort})
        {
            this.Files = new List<String> ( );
            this.Output = String.Empty;
            this.VsVersion = String.Empty;

            errorMessage = String.Empty;
        }

        private static Regex m_processproxySwitch =
                    new Regex(@"^""{0,1}(?<proxyAddress>http://[a-zA-Z0-9._]*\:[0-9]{1,5})\|(?<username>[^|]{1,})\|(?<password>[^|]{1,})(\|(?<domain>[^|]*)""{0,1}$|""{0,1}$)", RegexOptions.Singleline 
                                                                                        | RegexOptions.Compiled);
        private Match proxyMatch;
        
        public Match ProxyMatch
        {
            get
            {
                return proxyMatch;
            }
            private set
            {
                proxyMatch = value;
            }
        }

        private SwitchStatus ProcessProxySwitch(string switchValue)
        {
            
            SwitchStatus ss = SwitchStatus.NoError;

            if (!String.IsNullOrEmpty(switchValue))
            {
                Match proxyMatch =
                    m_processproxySwitch.Match(switchValue);
                if (proxyMatch.Success)
                {
                    this.proxyMatch = proxyMatch;
                    ss = SwitchStatus.NoError;
                }
                else
                {
                    errorMessage = Constants.Proxy;
                    ss = SwitchStatus.Error;
                }
            }
            else
            {
                errorMessage = Constants.Proxy;
                ss = SwitchStatus.Error;
            }

            return ss;
        }

        /// <summary>
        /// Called when a switch is parsed out.
        /// </summary>
        /// <param name="switchSymbol">
        /// The switch value parsed out.
        /// </param>
        /// <param name="switchValue">
        /// The value of the switch. For flag switches this is null/Nothing.
        /// </param>
        /// <returns>
        /// One of the <see cref="ArgParser.SwitchStatus"/> values.
        /// </returns>
        protected override SwitchStatus OnSwitch ( string switchSymbol ,
                                                   string switchValue )
        {
            SwitchStatus ss = SwitchStatus.NoError;
            switch ( switchSymbol )
            {
                //BugFix For 1133 Kerem Kusmezer
                case argProxy:
                case argProxyShort:
                    ss =ProcessProxySwitch(switchValue);
                    break;

                case argHelp:
                case argHelpQuestion:
                case argHelpShort:
                    ss = SwitchStatus.ShowUsage;
                    break;

                case argNoLogo:
                case argNoLogoShort:
                    NoLogo = true;
                    break;

                case argVerbose:
                case argVerboseShort:
                    Verbose = true;
                    break;

                case argForce:
                case argForceShort:
                    Force = true;
                    break;

                case argFile:
                case argFileShort:
                    ss = ProcessFileSwitch ( switchValue );
                    break;

                case argDirectory:
                case argDirectoryShort:
                    ss = ProcessDirectorySwitch ( switchValue );
                    break;

                case argOutput:
                case argOutputShort:
                    // Only one -output can be specified.
                    if ( false == String.IsNullOrEmpty ( Output ) )
                    {
                        errorMessage = Constants.ErrorMultipleOutputArgs;
                        ss = SwitchStatus.Error;
                    }
                    else if ( true == String.IsNullOrEmpty ( switchValue ) )
                    {
                        errorMessage = Constants.ErrorEmptyOutputArg;
                        ss = SwitchStatus.Error;
                    }
                    else
                    {
                        if ( false == Directory.Exists ( switchValue ) )
                        {
                            // Just let any problems get tossed right out.
                            Directory.CreateDirectory ( switchValue );
                        }
                        Output = AppendTrailingSlashIfNecessary ( switchValue );
                    }
                    break;

                case argVsVer:
                case argVsVerShort:
                    if ( false == String.IsNullOrEmpty ( VsVersion ) )
                    {
                        errorMessage = Constants.ErrorMultipleVersions;
                        ss = SwitchStatus.Error;
                    }
                    else if ( true == String.IsNullOrEmpty ( switchValue ) )
                    {
                        errorMessage = Constants.ErrorEmptyVsVerArg;
                        ss = SwitchStatus.Error;
                    }
                    else
                    {
                        VsVersion = switchValue;
                    }
                    break;

                // Not a parameter we know about.
                default:
                    {
                        errorMessage = Constants.ErrorUnknownCommandLineOption;
                        ss = SwitchStatus.Error;
                    }
                    break;
            }
            return ( ss );
        }

        /// <summary>
        /// Called when parsing is finished so final sanity checking can be
        /// performed.
        /// </summary>
        /// <returns>
        /// One of the <see cref="ArgParser.SwitchStatus"/> values.
        /// </returns>
        protected override SwitchStatus OnDoneParse ( )
        {
            SwitchStatus retValue = SwitchStatus.Error;
            // If the user specified an output directory, turn on the force
            // switch.
            if ( false == String.IsNullOrEmpty ( Output ) )
            {
                Force = true;
            }
            // If there are no dirFiles, we've got a problem.
            if ( Files.Count == 0 )
            {
                errorMessage = Constants.ErrorNoFiles;
            }
            // If the -vsver switch was not specified, default to VS 2008.
            if ( true == String.IsNullOrEmpty ( VsVersion ) )
            {
                VsVersion = "9.0";
            }
            // If the output directory was not set, pull the cache directory
            // out of the Visual Studio registry keys.
            if ( true == String.IsNullOrEmpty ( Output ) )
            {
                String key = String.Format ( CultureInfo.CurrentCulture ,
                               @"Software\Microsoft\VisualStudio\{0}\Debugger" ,
                               VsVersion );
                RegistryKey vsDbgKey = Registry.CurrentUser.OpenSubKey ( key );
                if ( null == vsDbgKey )
                {
                    errorMessage = Constants.ErrorVSNotInstalled;
                }
                else
                {
                    String cacheDir = (String)vsDbgKey.GetValue (
                                                           "SymbolCacheDir" ,
                                                            String.Empty );
                    if ( true == String.IsNullOrEmpty ( cacheDir ) )
                    {
                        errorMessage = Constants.ErrorVSNoCacheDirSet;
                    }
                    else
                    {
                        if ( false == Directory.Exists ( cacheDir ) )
                        {
                            errorMessage = String.Format (
                                           CultureInfo.CurrentCulture ,
                                           Constants.ErrorVSCacheDirNotExist ,
                                           cacheDir );
                        }
                        else
                        {
                            // Got us a cache.
                            Output = AppendTrailingSlashIfNecessary ( cacheDir );
                            UsingSymbolCache = true;
                            retValue = SwitchStatus.NoError;
                        }
                    }
                }
            }
            else
            {
                // Output was set and checked already.
                retValue = SwitchStatus.NoError;
            }
            errorAfterParse = ( retValue == SwitchStatus.Error );
            return ( retValue );
        }

        private static String AppendTrailingSlashIfNecessary ( String path )
        {
            if ( false == path.EndsWith ( "\\" ,
                                      StringComparison.OrdinalIgnoreCase ) )
            {
                path += "\\";
            }
            return ( path );
        }

        private SwitchStatus ProcessDirectorySwitch ( string directory )
        {
            SwitchStatus retValue = SwitchStatus.NoError;
            if ( false == Directory.Exists ( directory ) )
            {
                retValue = SwitchStatus.Error;
                errorMessage = String.Format ( CultureInfo.CurrentCulture ,
                                         Constants.ErrorDirectoryDoesNotExist ,
                                               directory );
            }
            else
            {
                // Get all the DLLs and EXEs.
                String [] dirFiles = Directory.GetFiles ( directory , "*.EXE" );
                Files.AddRange ( dirFiles );
                dirFiles = Directory.GetFiles ( directory , "*.DLL" );
                Files.AddRange ( dirFiles );
            }
            return ( retValue );
        }

        private SwitchStatus ProcessFileSwitch ( string file )
        {
            SwitchStatus retValue = SwitchStatus.NoError;
            // The file has to exists.
            if ( false == File.Exists ( file ) )
            {
                retValue = SwitchStatus.Error;
                errorMessage = String.Format ( CultureInfo.CurrentCulture ,
                                               Constants.ErrorFileDoesNotExist ,
                                               file );
            }
            else
            {
                Files.Add ( file );
            }
            return ( retValue );
        }

        /// <summary>
        /// Called when a non-switch value is parsed out.
        /// </summary>
        /// <param name="value">The value parsed out.</param>
        /// <returns>
        /// One of the <see cref="ArgParser.SwitchStatus"/> values.
        /// </returns>
        protected override SwitchStatus OnNonSwitch ( string value )
        {
            // All options are covered by switches.
            errorMessage = Constants.ErrorUnknownCommandLineOption;
            return ( SwitchStatus.Error );
        }

        public void Logo ( )
        {
            if ( false == NoLogo )
            {
                ProcessModule exe = Process.GetCurrentProcess ( ).Modules [ 0 ];
                Console.WriteLine ( Constants.LogoString ,
                                    exe.FileVersionInfo.FileVersion );
            }
        }

        /// <summary>
        /// Displays usage and if set, error messages.
        /// </summary>
        /// <param name="errorInfo">
        /// The error. Null for no error.
        /// </param>
        public override void OnUsage ( string errorInfo )
        {
            Logo ( );
            Console.WriteLine ( Constants.UsageString );
            if ( ( false == errorAfterParse ) && 
                 ( false == String.IsNullOrEmpty ( errorInfo ) ) )
            {
                Console.WriteLine ( );
                Console.WriteLine ( Constants.ErrorSwitch , errorInfo );
            }
            if ( false == String.IsNullOrEmpty ( errorMessage ) )
            {
                Console.WriteLine ( );
                Console.WriteLine ( errorMessage );
            }
        }
    }
}
