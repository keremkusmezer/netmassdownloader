#region Imported Libraries
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
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
public class ConsoleWebClient : PDBWebClient {

    #region Constants
    private const string PROGRESS_PLACEHOLDER = AppSettings.Indent;
    #endregion Constants


    #region Instance Constructors
    /// <summary>
    /// </summary>
    /// <param name="proxyUri"></param>
    /// <param name="proxyCredential"></param>
    public ConsoleWebClient(Uri proxyUri, NetworkCredential proxyCredential)
        : base( proxyUri, proxyCredential ) {}

    /// <summary>
    /// </summary>
    /// <param name="proxyUrl"></param>
    /// <param name="proxyUsername"></param>
    /// <param name="proxyPassword"></param>
    /// <param name="proxyDomain"></param>
    public ConsoleWebClient(string proxyUrl, string proxyUsername, string proxyPassword, string proxyDomain)
        : base( proxyUrl, proxyUsername, proxyPassword, proxyDomain ) {}

    /// <summary>
    /// </summary>
    public ConsoleWebClient() {
    }
    #endregion Instance Constructors


    #region Static Methods
    private static int PrintBackspaceIfNecessary( DownloadInfo di ) {
        int n = -1;
        if (di.LastToken != null) {
            n = di.LastToken.Length;
            Console.Write( new string('\b', n) );
            di.LastToken = null;
        }
        return n;
    }

    private static void PrintPlaceHolder( string placeHolder, DownloadInfo di ) {
        Console.Write( placeHolder );
        di.LastToken = placeHolder;
    }

    private static string BuildFileSizeWithUnitString( long fileSize ) {
        string unit = "bytes";
        if (fileSize > 1024) {
            fileSize /= 1024;
            unit = "KB";
        }
        return string.Format( "{0:N0} {1}", fileSize, unit );
    }
    #endregion Static Helpers


    #region Instance Methods
    #region Overrides
    /// <summary>
    /// </summary>
    /// <param name="e"></param>
    protected override void OnDownloadProgressChanged( DownloadProgressChangedEventArgs e ) {
        lock( base.SyncRoot ) {
            DownloadInfo di = e.UserState as DownloadInfo;
            if (di.ProgressDone) { // This can happen!
                return;
            }

            di.HasProgress = true;

            bool firstTime = di.FirstTime;
            if (firstTime) {
                Console.Write( "...\t| " );
                di.FirstTime = false;
            }

            if (e.TotalBytesToReceive > -1) {
                if (! di.PrintedTotal) {
                    PrintBackspaceIfNecessary( di );
                    Console.Write( "{0,12}  ", BuildFileSizeWithUnitString(e.TotalBytesToReceive) );
                    di.PrintedTotal = true;

                    PrintPlaceHolder( PROGRESS_PLACEHOLDER, di );
                }

                PrintBackspaceIfNecessary( di );
                PrintPlaceHolder( string.Format( "{0,3}%", e.ProgressPercentage.ToString() ), di );

                if (e.ProgressPercentage == 100) {
                    di.ProgressDone = true;
                }
            }
            else {
                if (e.BytesReceived > 0) {
                    int n = PrintBackspaceIfNecessary( di );

                    string str = string.Format( "{0,12}", BuildFileSizeWithUnitString(e.BytesReceived) );
                    if (str.Length < n) {
                        str += new string( ' ', n - str.Length );
                    }
                    PrintPlaceHolder( str, di );
                }
                else {
                    if (firstTime) {
                        PrintPlaceHolder( PROGRESS_PLACEHOLDER, di );
                    }
                }
            }
        } // lock
    }

    /// <summary>
    /// </summary>
    /// <param name="e"></param>
    protected override void OnDownloadFileCompleted( AsyncCompletedEventArgs e ) {
        this.OnDownloadAnyComplete( e );
    }
    /// <summary>
    /// </summary>
    /// <param name="e"></param>
    protected override void OnDownloadDataCompleted( DownloadDataCompletedEventArgs e ) {
        this.OnDownloadAnyComplete( e );
    }
    #endregion Overrides

    #region Helpers
    private void OnDownloadAnyComplete( AsyncCompletedEventArgs e ) {
        DownloadInfo di = e.UserState as DownloadInfo;

        bool hasProgress = false;
        lock( base.SyncRoot ) {
            hasProgress = di.HasProgress;
        }

        if (hasProgress) { // Wait until the last output by OnDownloadProgressChanged is done
            while (true) {
                lock( base.SyncRoot ) {
                    if (di.ProgressDone) {
                        break;
                    }
                }
                Thread.Sleep( 500 );
            }
        }

        // Note:
        // di.Result must be set in the last order to avoid the next download jumps in before newline is printed

        lock( base.SyncRoot ) {
            Exception exc = e.Error;
            if (exc != null) {
                string excMessage = null;

                if (exc is WebException) {
                    WebResponse response = ((WebException) exc).Response;
                    if (response is HttpWebResponse) {
                        HttpStatusCode statusCode = ((HttpWebResponse) response).StatusCode;
                        if (statusCode == HttpStatusCode.NotFound) {
                            excMessage = "...\tNot available";
                        }
                        else {
                            excMessage = string.Format( "...\t{0}: {1}", (int) statusCode, statusCode );
                        }
                    }
                }
                if (excMessage != null) {
                    Console.WriteLine( excMessage );
                }
                else {
                    Console.WriteLine( "...\n{0}{0}{1}", AppSettings.Indent, e.Error.Message );
                }
                Debug.WriteLine( exc.ToString() );

                di.Result = e.Error;
            }
            else {
                Console.WriteLine();

                if (e is DownloadDataCompletedEventArgs) {
                    di.Result = ((DownloadDataCompletedEventArgs) e).Result;
                }
                else {
                    di.Result = "Done";
                }
            }
        } // lock
    }
    #endregion Helpers
    #endregion Instance Methods

} // EndClass    
} // namespace
