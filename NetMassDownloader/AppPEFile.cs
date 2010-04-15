#region Imported Libraries
using System.Text.RegularExpressions;
using DownloadLibrary.Classes;
using DownloadLibrary.Classes.Eula;
using DownloadLibrary.PEParsing;
#endregion

#region Licence Information
/*       
 * http://www.codeplex.com/NetMassDownloader To Get The Latest Version
 *     
 * Copyright 2008 Kerem Kusmezer(izzetkeremskusmezer@gmail.com) And John Robbins (john@wintellect.com)
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