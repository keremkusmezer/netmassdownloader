#region Licence Information
/*       
 * http://www.codeplex.com/NetMassDownloader To Get The Latest Version
 *     
 * Copyright 2008 Kerem Kusmezer(keremskusmezer@gmail.com)
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
 * 
*/
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloadLibrary.Classes.Eula
{
    public class PDBWebClient : System.Net.WebClient
    {
        private bool m_is210Requested;

        public bool IsEulaResponse
        {
            get { 
                return m_is210Requested; 
            }
            set { 
                m_is210Requested = value; 
            }
        }

        private string m_eulaBody;

        /// <summary>
        /// Returns The Eula Body Returned By The MS Reference Server 
        /// </summary>
        public string EulaBody
        {
            get { return m_eulaBody; }
            private set { m_eulaBody = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxyUri"></param>
        /// <param name="proxyCredential"></param>        
        public PDBWebClient(System.Uri proxyUri, System.Net.NetworkCredential proxyCredential):base()
        {
            //BugFix 1113 Kerem Kusmezer 12.02.2008
            base.Proxy = new System.Net.WebProxy(proxyUri);
            base.Proxy.Credentials = proxyCredential;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxyUrl"></param>
        /// <param name="proxyUsername"></param>
        /// <param name="proxyPassword"></param>
        /// <param name="proxyDomain"></param>
        public PDBWebClient(string proxyUrl,string proxyUsername,string proxyPassword,string proxyDomain):base()
        {
            //BugFix 1113 Kerem Kusmezer 12.02.2008
            base.Proxy = new System.Net.WebProxy(new System.Uri(proxyUrl));
            System.Net.NetworkCredential tempCredential = null;
            if (String.IsNullOrEmpty(proxyDomain))
            {
                tempCredential =
                    new System.Net.NetworkCredential(proxyUsername, proxyPassword);
            }
            else
            {
                tempCredential = 
                    new System.Net.NetworkCredential(proxyUsername,proxyPassword,proxyDomain);
            }
            base.Proxy.Credentials = tempCredential;
        }
        /// <summary>
        /// 
        /// </summary>
        public PDBWebClient():base()
        {
        }

        public new void DownloadFile(Uri address, string fileName)
        {
            if (this.Headers["User-Agent"] == null)
            {
                this.Headers.Add("User-Agent", Constants.userAgentHeader);
            }
            base.DownloadFile(address, fileName);
        }

        public new void DownloadFile(string address, string fileName)
        {
            if (this.Headers["User-Agent"] == null)
            {
                this.Headers.Add("User-Agent", Constants.userAgentHeader);
            }
            base.DownloadFile(address, fileName);
        }
        public new byte[] DownloadData(string url)
        {
            if (this.Headers["User-Agent"] == null)
            {
                this.Headers.Add("User-Agent", Constants.userAgentHeader);
            }
            byte[] resultBytes = base.DownloadData(url);
            if (m_is210Requested)
            {
                m_eulaBody = System.Text.Encoding.Unicode.GetString(resultBytes);
            }
            return resultBytes;
        }
        protected override System.Net.WebRequest GetWebRequest(Uri address)
        {
            System.Net.WebRequest tempRequest =
                base.GetWebRequest(address);
            return tempRequest;
        }
        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request)
        {            
            System.Net.WebResponse requestResponse = 
            base.GetWebResponse(request);
            if (requestResponse is System.Net.HttpWebResponse)
            {
                System.Net.HttpWebResponse httpResponse =
                    (System.Net.HttpWebResponse)requestResponse;
                
                if (m_is210Requested 
                    && 
                    httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Eula.EulaContents.SaveEulaCookie(httpResponse);
                }
                
                //BUGFIX 1128 08.02.2008 Kerem Kusmezer It seems the Status Description 
                //Has Been Stripped Or Replaced With Unknown In Some Proxy Servers
                //Thanks Florin
                //(() 
                //&& (httpResponse.StatusDescription == "End User License Agreement");
                m_is210Requested = ((int)httpResponse.StatusCode == 210);
                    
                
                httpResponse = null;

            }
            return requestResponse;
        }
    }
}
