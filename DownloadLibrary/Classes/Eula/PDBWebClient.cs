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

#region Imported Libraries
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
#endregion

namespace DownloadLibrary.Classes.Eula
{
    //[CLSCompliant(true)]
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
                this.Headers.Add("User-Agent", FrameworkVersionData.RelevantVersionData.GetUserAgentVersion());
            }
            base.DownloadFile(address, fileName);
        }

        public new void DownloadFile(string address, string fileName)
        {
            if (this.Headers["User-Agent"] == null)
            {
                this.Headers.Add("User-Agent", FrameworkVersionData.RelevantVersionData.GetUserAgentVersion());
            }
            base.DownloadFile(address, fileName);
        }
        public new byte[] DownloadData(string url)
        {
            if (this.Headers["User-Agent"] == null)
            {
                this.Headers.Add("User-Agent", FrameworkVersionData.RelevantVersionData.GetUserAgentVersion());
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
            
            this.CheckResponse( requestResponse );
            return requestResponse;
        }



        #region [HDN] Additional Code
        #region Instance Properties
        private readonly object m_syncRoot = new object();
        protected object SyncRoot {
            get {
                return m_syncRoot;
            }
        }

        private DateTime m_lastFileWriteTimeOnServer;
        public DateTime LastFileWriteTimeOnServer {
            get { return m_lastFileWriteTimeOnServer; }
        }

        private bool m_hasLastFileWriteTimeOnServer = false;
        public bool HasLastFileWriteTimeOnServer {
            get { return m_hasLastFileWriteTimeOnServer; }
        }
        #endregion Instance Properties

        #region Instance Methods
        #region Public
        public bool DownloadDataWithProgress( string url, out byte[] downloadData ) {
            downloadData = new byte[] {};

            Utility.AddUserAgentHeaderIfNecessary( this );

            DownloadInfo di = new DownloadInfo();
            base.DownloadDataAsync( new Uri(url), di );

            WaitUntilDownloadCompleted( di );
            bool ok = di.Successful;

            if (ok) {
                downloadData = (byte[]) di.Result;
                if (m_is210Requested) {
                    m_eulaBody = System.Text.Encoding.Unicode.GetString(downloadData);
                }
            }
            return ok;
        }

        public bool DownloadFileWithProgress(string address, string fileName) {
            Utility.AddUserAgentHeaderIfNecessary( this );

            DownloadInfo di = new DownloadInfo();
            base.DownloadFileAsync(new Uri(address), fileName, di);

            WaitUntilDownloadCompleted( di );

            if (di.Successful) {
                if (m_hasLastFileWriteTimeOnServer) {
                    File.SetLastAccessTimeUtc( fileName, m_lastFileWriteTimeOnServer );
                }
            }
            else
            {

                // The WebClient base class can create a zero byte file so
                // make sure to delete the file and directory so there's not
                // bad PDB files left around.  I would rather not do this here
                // but the problem is in WebClient, not this application.
                // BugFix 2623 it doesn't work with -o switch instead deleting the above folder , it deletes the entire folder.
                if ( true == File.Exists ( fileName )  && new FileInfo(fileName).Length == 0)
                {
                    File.Delete ( fileName );
                    String file = Path.GetFileName ( fileName );
                    String path = Path.GetDirectoryName ( fileName );
                    // If the filename is in the path, this file was
                    // going into the symbol server.
                    int i = path.IndexOf ( file );
                    if ( -1 != i )
                    {
                        path = path.Substring ( 0 , i + file.Length );
                        //Bugfix 2623 So Moving The Folder Delete Here Solves The Bug
                        Directory.Delete(path, true);
                    }
                }
            }

            return di.Successful;
        }
        #endregion Public

        #region Overrides
        protected override WebResponse GetWebResponse( WebRequest request, IAsyncResult result ) {
            WebResponse response = base.GetWebResponse( request, result );
            this.CheckResponse( response );
            return response;
        }
        #endregion Overrides

        #region Helpers
        private void WaitUntilDownloadCompleted( DownloadInfo di ) {
            while (true) {
                object result = null;
                lock( m_syncRoot ) {
                    result = di.Result;
                }

                if (result != null) {
                    break;
                }

                Thread.Sleep( 500 );
            } // while
        }

        private void CheckResponse( WebResponse requestResponse ) {
            m_hasLastFileWriteTimeOnServer = false;

            string str = requestResponse.Headers["Last-Modified"];
            if (!string.IsNullOrEmpty( str )) {
                m_lastFileWriteTimeOnServer = DateTime.Parse( str );
                m_hasLastFileWriteTimeOnServer = true;
            }

            if (requestResponse is HttpWebResponse)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)requestResponse;
                
                if (m_is210Requested && (httpResponse.StatusCode == HttpStatusCode.OK)) {
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
        }
        #endregion
        #endregion Instance Methods
        #endregion [HDN] Additional Code

    }

    #region Class: DownloadInfo
    public class DownloadInfo {
        private object _result = null;
        private bool _firstTime = true;
        private bool _printedTotal = false;
        private string _lastToken = null;

        private bool _hasProgress = false;
        private bool _progressDone = false;

        public object Result {
            get { return _result; }
            set { _result = value; }
        }
        public bool Successful {
            get { return (!(_result is Exception)); }
        }

        public bool FirstTime {
            get { return _firstTime; }
            set { _firstTime = value; }
        }

        public bool PrintedTotal {
            get { return _printedTotal; }
            set { _printedTotal = value; }
        }

        public string LastToken {
            get { return _lastToken; }
            set { _lastToken = value; }
        }

        public bool HasProgress {
            get { return _hasProgress; }
            set { _hasProgress = value; }
        }
        public bool ProgressDone {
            get { return _progressDone; }
            set { _progressDone = value; }
        }
    }
    #endregion Class : DownloadInfo
}
