using System;
using System.Configuration;
using System.Collections.ObjectModel;


namespace NetMassDownloader {

/// <summary>
/// </summary>
public static class AppSettings {

    /// <summary>
    /// </summary>
    public const string Indent = " "; 

    /// <summary>
    /// </summary>
    public static bool ProcessInputDirRecursively {
        get {
            return string.Equals( ConfigurationManager.AppSettings["ProcessInputDirRecursively"],
                bool.TrueString, StringComparison.OrdinalIgnoreCase );
        }
    }

    ///<summary>
    ///</summary>
    public static bool CleanupTempCompressedSymbols {
        get {
            return string.Equals( ConfigurationManager.AppSettings["CleanupTempCompressedSymbols"],
                bool.TrueString, StringComparison.OrdinalIgnoreCase );
        }
    }
    
    private static ReadOnlyCollection<string> _supportedVSVersions;
    /// <summary>
    /// Supported Visual Studio Versions Returned By The Config
    /// </summary>
    public static ReadOnlyCollection<string> SupportedVSVersions
    {
        get {

            if (_supportedVSVersions == null)
            {
                string supportedVersions =
                    ConfigurationManager.AppSettings["SupportedVersions"];
                if (!String.IsNullOrEmpty(supportedVersions))
                {
                    System.Collections.Generic.List<string> resultList =
                        new System.Collections.Generic.List<string>();
                    resultList.AddRange(supportedVersions.Split(';'));
                    _supportedVSVersions = 
                        new ReadOnlyCollection<string>(resultList);
                }
                else
                {
                    _supportedVSVersions = 
                        new ReadOnlyCollection<string>(new System.Collections.Generic.List<String>());
                }
            }
            return _supportedVSVersions;
        }
    }
    ///<summary>
    ///</summary>
    public static bool UseReferenceSourceServer {
        get {
            return string.Equals( ConfigurationManager.AppSettings["UseReferenceSourceServer"],
                bool.TrueString, StringComparison.OrdinalIgnoreCase );
        }
    }

    ///<summary>
    ///</summary>
    public static bool DownloadSymbols {
        get {
            return string.Equals(ConfigurationManager.AppSettings["DownloadSymbols"],
                bool.TrueString, StringComparison.OrdinalIgnoreCase);
        }
    }

    
    ///<summary>
    ///</summary>
    public static bool DownloadSourceCode {
        get {
            return string.Equals(ConfigurationManager.AppSettings["DownloadSourceCode"],
                bool.TrueString, StringComparison.OrdinalIgnoreCase);
        }
    }

    ///<summary>
    ///</summary>
    public static bool SkipExistingSourceFiles {
        get {
            return string.Equals(ConfigurationManager.AppSettings["SkipExistingSourceFiles"],
                bool.TrueString, StringComparison.OrdinalIgnoreCase);
        }
    }

    ///<summary>
    ///</summary>
    public static string DefaultSymbolServerUrl {
        get { return ConfigurationManager.AppSettings["DefaultSymbolServerUrl"]; }
    }
    ///<summary>
    ///</summary>
    public static string ReferenceSourceServerUrl {
        get { return ConfigurationManager.AppSettings["ReferenceSourceServerUrl"]; }
    }
    ///<summary>
    ///</summary>
    public static string SymbolServerUrl {
        get { return UseReferenceSourceServer ? ReferenceSourceServerUrl : DefaultSymbolServerUrl; }
    }

} // EndClass
} // namespace