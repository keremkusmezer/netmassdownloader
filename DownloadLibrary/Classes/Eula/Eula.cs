/*------------------------------------------------------------------------------
 * SourceCodeDownloader - Kerem Kusmezer (keremskusmezer@gmail.com)
 *                        John Robbins (john@wintellect.com)
 * 
 * Download all the .NET Reference Sources and PDBs to pre-populate your 
 * symbol server! No more waiting as each file downloads individually while
 * debugging.
 * ---------------------------------------------------------------------------*/
#region Imported Libraries
using DownloadLibrary.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
#endregion

namespace DownloadLibrary.Classes.Eula
{
    public class EulaRequestEvent : EventArgs
    {
        private EulaContents m_eulaContent;

        private bool m_eulaAccepted;

        public EulaContents EulaContent
        {
            get
            {
                return m_eulaContent;
            }
        }

        public bool EulaAccepted
        {
            get
            {
                return m_eulaAccepted;
            }
            set
            {
                m_eulaAccepted = value;
            }
        }

        public EulaRequestEvent(EulaContents EulaContent)
        {
            m_eulaContent = EulaContent;
        }
    }
    
    public class EulaContents
    {
        public static string GetEulaCookie()
        {
            return Settings.Default.EULACookie;
        }
        public static bool SaveEulaCookie(HttpWebResponse httpResponse)
        {
            //Settings.Default.
            Settings.Default.EULACookie = httpResponse.Headers["Set-Cookie"];
            Settings.Default.Save();
            return true;
        }

        private void ParseBody()
        {
            Match bodyMatch = ResponseParser.Match(m_FullEulaBody);
            if (bodyMatch.Success)
            {
                m_EulaText = bodyMatch.Groups["eula"].Value;
                m_AcceptTextKey = bodyMatch.Groups["a_txt"].Value;
                m_AcceptCmdKey = bodyMatch.Groups["a_cmd"].Value;
                m_DeclineTextKey = bodyMatch.Groups["d_txt"].Value;
                m_DeclineCmdKey = bodyMatch.Groups["d_cmd"].Value;
                m_TitleKey = bodyMatch.Groups["title"].Value;
                m_ButtonDescriptionKey = bodyMatch.Groups["buttondesc"].Value;
            }
            else
            {
                //TODO Throw Invalid Key Body Exception Here;
            }
        }

        #region Private Variables
        private string m_FullEulaBody;

        public string FullEulaBody
        {
            get { return m_FullEulaBody; }
            private set { m_FullEulaBody = value; }
        }

        private string m_EulaText;

        public string EulaText
        {
            get { return m_EulaText; }
            private set { m_EulaText = value; }
        }
        private string m_AcceptTextKey;

        public string AcceptTextKey
        {
            get { return m_AcceptTextKey; }
            private set { m_AcceptTextKey = value; }
        }
        private string m_AcceptCmdKey;

        public string AcceptCmdKey
        {
            get { return m_AcceptCmdKey; }
            private set { m_AcceptCmdKey = value; }
        }
        private string m_DeclineTextKey;

        public string DeclineTextKey
        {
            get { return m_DeclineTextKey; }
            private set { m_DeclineTextKey = value; }
        }
        private string m_DeclineCmdKey;

        public string DeclineCmdKey
        {
            get { return m_DeclineCmdKey; }
            private set { m_DeclineCmdKey = value; }
        }
        private string m_TitleKey;

        public string TitleKey
        {
            get { return m_TitleKey; }
            private set { m_TitleKey = value; }
        }
        private string m_ButtonDescriptionKey;

        public string ButtonDescriptionKey
        {
            get { return m_ButtonDescriptionKey; }
            private set { m_ButtonDescriptionKey = value; }
        }
        #endregion

        #region Constants

        private const string ResponseRegEx = @"SYMSRVCOMMAND:\s*YESNODIALOG:\s*IDYES:(?<a_txt>(\w|\'| )+):(?<a_cmd>(\w|=)+)\s*IDNO:(?<d_txt>(\w|\'| )+):(?<d_cmd>(\w|=)+)\s*TITLE:(?<title>.+)[\r\n]+\nBUTTONDESCRIPTION:(?<buttondesc>.+)[\r\n]+TEXT:(?<eula>.+)$";

        private static Regex ResponseParser = new Regex(ResponseRegEx,RegexOptions.Compiled | RegexOptions.Singleline);

        #endregion

        public EulaContents(string EulaBodyToBeParsed)
        {
            m_FullEulaBody = EulaBodyToBeParsed;
            ParseBody();
        }

       
    }
}