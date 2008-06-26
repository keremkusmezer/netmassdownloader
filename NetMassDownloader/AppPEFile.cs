using System.Text.RegularExpressions;

using DownloadLibrary.Classes;
using DownloadLibrary.Classes.Eula;
using DownloadLibrary.PEParsing;


namespace NetMassDownloader {

/// <summary>
/// </summary>
public class AppPEFile : PEFile {

    ///<summary>
    ///</summary>
    ///<param name="path"></param>
    public AppPEFile(string path) : base(path) {}


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


} // EndClass
} // namespace