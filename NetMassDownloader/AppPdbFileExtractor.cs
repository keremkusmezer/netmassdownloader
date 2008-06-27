#region Imported Libraries
using System;
using System.IO;
using System.Text.RegularExpressions;
using DownloadLibrary.Classes;
using DownloadLibrary.Classes.Eula;
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
        Console.Write(Constants.DownloadingSourceCode);
        Console.Write("{0}Downloading {1}", String.Empty, args.TargetFileName);
        //Console.Write("{0}Downloading {1}", AppSettings.Indent, args.TargetFileName);
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