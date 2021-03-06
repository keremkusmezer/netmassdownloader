﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4200
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetMassDownloader {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Constants {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Constants() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NetMassDownloader.Constants", typeof(Constants).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The array muse not be null or of zero length..
        /// </summary>
        internal static string ArrayMustBeValid {
            get {
                return ResourceManager.GetString("ArrayMustBeValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FileCount : {0}
        ///FileName  : {1}
        ///Directory : {2}.
        /// </summary>
        internal static string DownloadedFileOfFmt {
            get {
                return ResourceManager.GetString("DownloadedFileOfFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading {0} .
        /// </summary>
        internal static string DownloadingPdb {
            get {
                return ResourceManager.GetString("DownloadingPdb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ----------------------------------
        ///FileDownload Result
        ///----------------------------------
        ///.
        /// </summary>
        internal static string DownloadingSourceCode {
            get {
                return ResourceManager.GetString("DownloadingSourceCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Visual Studio cache directory, &apos;{0}&apos;, does not exist..
        /// </summary>
        internal static string ErrorCacheDirNotExist {
            get {
                return ResourceManager.GetString("ErrorCacheDirNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The directory &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string ErrorDirectoryDoesNotExist {
            get {
                return ResourceManager.GetString("ErrorDirectoryDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The -output argument must specify the output directory..
        /// </summary>
        internal static string ErrorEmptyOutputArg {
            get {
                return ResourceManager.GetString("ErrorEmptyOutputArg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If specified, the -vsver argument cannot be empty..
        /// </summary>
        internal static string ErrorEmptyVsVerArg {
            get {
                return ResourceManager.GetString("ErrorEmptyVsVerArg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string ErrorFileDoesNotExist {
            get {
                return ResourceManager.GetString("ErrorFileDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only one -output switch can be specified on the command line..
        /// </summary>
        internal static string ErrorMultipleOutputArgs {
            get {
                return ResourceManager.GetString("ErrorMultipleOutputArgs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only one -vsver switch can be specified on the command line..
        /// </summary>
        internal static string ErrorMultipleVersions {
            get {
                return ResourceManager.GetString("ErrorMultipleVersions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No files or directories to process were specified on the command line..
        /// </summary>
        internal static string ErrorNoFiles {
            get {
                return ResourceManager.GetString("ErrorNoFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The output directory &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string ErrorOutputDoesNotExist {
            get {
                return ResourceManager.GetString("ErrorOutputDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error on the {0} switch..
        /// </summary>
        internal static string ErrorSwitch {
            get {
                return ResourceManager.GetString("ErrorSwitch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown command line argument..
        /// </summary>
        internal static string ErrorUnknownCommandLineOption {
            get {
                return ResourceManager.GetString("ErrorUnknownCommandLineOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The directory in the Visual Studio debugger symbol cache directory, &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string ErrorVSCacheDirNotExist {
            get {
                return ResourceManager.GetString("ErrorVSCacheDirNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no value set for the Visual Studio debugger symbol cache..
        /// </summary>
        internal static string ErrorVSNoCacheDirSet {
            get {
                return ResourceManager.GetString("ErrorVSNoCacheDirSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Visual Studio does not appeared to be installed. Unable to read the 
        ///Debugger key. The default is to look for Visual Studio 2008. If you specified 
        ///a version with a -vsver command line option, make sure to specify the 
        ///complete version. For example, Visual Studio 2005 is 8.0 and Visual Studio 
        ///2008 is 9.0 and Visual Studio 2010 is 10.0.
        /// </summary>
        internal static string ErrorVSNotInstalled {
            get {
                return ResourceManager.GetString("ErrorVSNotInstalled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only {0} Versions Are Supported As VS Version.
        /// </summary>
        internal static string ErrorVSVersion {
            get {
                return ResourceManager.GetString("ErrorVSVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File download failed: {0}.
        /// </summary>
        internal static string FileDownloadFailedFmt {
            get {
                return ResourceManager.GetString("FileDownloadFailedFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hostname Resolve Failed. Please check details:{0}.
        /// </summary>
        internal static string HostNameNotResolved {
            get {
                return ResourceManager.GetString("HostNameNotResolved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An invalid parameter was passed to the method..
        /// </summary>
        internal static string InvalidParameter {
            get {
                return ResourceManager.GetString("InvalidParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .Net Mass Downloader {0} - (c) 2008 to 2010 by Kerem Kusmezer, John Robbins
        ///.
        /// </summary>
        internal static string LogoString {
            get {
                return ResourceManager.GetString("LogoString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading {0}  (No .debug section).
        /// </summary>
        internal static string NoDebugSection {
            get {
                return ResourceManager.GetString("NoDebugSection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No matching PDB file on symbol server for {0}.
        /// </summary>
        internal static string NoPdbFileFmt {
            get {
                return ResourceManager.GetString("NoPdbFileFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You cannot download the files without accepting Microsoft&apos;s EULA..
        /// </summary>
        internal static string NotEulaFile {
            get {
                return ResourceManager.GetString("NotEulaFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File not on symbol server: {0}.
        /// </summary>
        internal static string NotOnSymbolServer {
            get {
                return ResourceManager.GetString("NotOnSymbolServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not a PE file: {0}.
        /// </summary>
        internal static string NotPEFileFmt {
            get {
                return ResourceManager.GetString("NotPEFileFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PDB file already in the symbol server cache for:{0}{1}.
        /// </summary>
        internal static string PdbAlreadyInSymbolServer {
            get {
                return ResourceManager.GetString("PdbAlreadyInSymbolServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing {0}.
        /// </summary>
        internal static string ProcessingFileFmt {
            get {
                return ResourceManager.GetString("ProcessingFileFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing {0}.
        /// </summary>
        internal static string ProcessingPdb {
            get {
                return ResourceManager.GetString("ProcessingPdb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Proxy switch selected , but wrong proxy parameters supplied..
        /// </summary>
        internal static string Proxy {
            get {
                return ResourceManager.GetString("Proxy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to During Http Request, A Proxy Based Error Occurred: {0}.
        /// </summary>
        internal static string ProxyBasedError {
            get {
                return ResourceManager.GetString("ProxyBasedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Processed binaries/PDBs : {0}
        ///Files not processed     : {1}
        ///Downloaded source files : {2}.
        /// </summary>
        internal static string RunStatsFmt {
            get {
                return ResourceManager.GetString("RunStatsFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Batch download the Microsoft .NET Reference Source code.
        ///
        ///Usage: NetMassDownloader [-file &lt;file&gt;] 
        ///                         [-directory &lt;directory]
        ///                         [-output &lt;directory&gt;]
        ///                         [-vsver &lt;version&gt;]
        ///                         [-proxy server|username|password|domainname]
        ///                         [-force] [-nologo] [-verbose] [-?]
        ///
        ///    -file      - Download an individual file&apos;s PDB and source code. You can 
        ///                 specify multiple file parameters. (Sho [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string UsageString {
            get {
                return ResourceManager.GetString("UsageString", resourceCulture);
            }
        }
    }
}
