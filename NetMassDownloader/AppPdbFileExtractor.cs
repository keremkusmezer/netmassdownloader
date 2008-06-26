using System;
using System.IO;
using System.Text.RegularExpressions;

using DownloadLibrary.Classes;
using DownloadLibrary.Classes.Eula;


namespace NetMassDownloader {

/// <summary>
/// </summary>
public class AppPdbFileExtractor : PdbFileExtractor {

    #region Instance Constructors
    ///<summary>
    ///</summary>
    ///<param name="pdbFileName"></param>
    ///<param name="pdbStream"></param>
    public AppPdbFileExtractor(string pdbFileName, MemoryStream pdbStream)
        : base(pdbFileName, pdbStream) {}

    ///<summary>
    ///</summary>
    ///<param name="pdbFileName"></param>
    public AppPdbFileExtractor(string pdbFileName) : base(pdbFileName) {}
    #endregion Instance Constructors


    #region Instance Methods
    #region Overrides
    /// <summary>
    /// </summary>
    /// <returns></returns>
    protected override PDBWebClient GetWebClientWithCookie() {
        PDBWebClient wc;

        Match m = base.ProxyMatch;
        if (m != null) {
            wc = new ConsoleWebClient(
                Utility.GetProxyAddressFrom(m), Utility.GetUserNameFrom(m),
                Utility.GetPasswordFrom(m), Utility.GetDomainFrom(m) );
        }
        else {
            wc = new ConsoleWebClient();
        }

        Utility.AddHeadersIfNecessary( wc );
        return wc;
    }


    /// <summary>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void OnBeginDownloadSourceFile( object sender, DownloadFileEventArgs args ) {
        Console.Write( "{0}Downloading {1} ", AppSettings.Indent, args.TargetFileName );
    }

    /// <summary>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void OnDownloadSourceFileAfterEulaAccepted( object sender, DownloadFileEventArgs args ) {
        Console.Write( "{0}EULA accepted. Downloading {1} ", AppSettings.Indent, args.TargetFileName );
    }
    #endregion Overrides
    #endregion Instance Methods

} // EndClass
} // namespace