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
        private bool m_is201Requested;

        public bool IsEulaResponse
        {
            get { 
                return m_is201Requested; 
            }
            set { 
                m_is201Requested = value; 
            }
        }

        private string m_eulaBody;

        public string EulaBody
        {
            get { return m_eulaBody; }
            private set { m_eulaBody = value; }
        }
        public PDBWebClient()
        {
            //if (this.Headers["User-Agent"] == null)
            //{
            //    this.Headers.Add("User-Agent", Constants.userAgentHeader);
            //
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
            if (m_is201Requested)
            {
                m_eulaBody = System.Text.Encoding.Unicode.GetString(resultBytes);
            }
            return resultBytes;
        }
        protected override System.Net.WebRequest GetWebRequest(Uri address)
        {
            System.Net.WebRequest tempRequest =
                base.GetWebRequest(address);
            //tempRequest.Timeout = 5 * 60 * 1000;
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
                
                if (m_is201Requested 
                    && 
                    httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Eula.EulaContents.SaveEulaCookie(httpResponse);
                }

                m_is201Requested = 
                    (((int)httpResponse.StatusCode) == 210) && (httpResponse.StatusDescription == "End User License Agreement");

                httpResponse = null;

            }
            return requestResponse;
        }
    }
}
