/*------------------------------------------------------------------------------
 * SourceCodeDownloader - Kerem Kusmezer (keremskusmezer@gmail.com)
 *                        John Robbins (john@wintellect.com)
 * 
 * Download all the .NET Reference Sources and PDBs to pre-populate your 
 * symbol server! No more waiting as each file downloads individually while
 * debugging.
 * ---------------------------------------------------------------------------*/
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
 * 
 * 
 * Taken From The Following Project Also Written By Kerem Kusmezer 
 * PdbParser in C# http://www.codeplex.com/pdbparser 
 * 
*/
#endregion
#region Imported Libraries
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion
namespace DownloadLibrary.Classes
{
    internal static class Constants
    {
        //Regular Expression For Eula Dialog Parsing
        private const string ResponseRegEx = 
                @"SYMSRVCOMMAND:\s*YESNODIALOG:\s*IDYES:(?<a_txt>(\w|\'| )+):(?<a_cmd>(\w|=)+)\s*IDNO:(?<d_txt>(\w|\'| )+):(?<d_cmd>(\w|=)+)\s*TITLE:(?<title>.+)[\r\n]+\nBUTTONDESCRIPTION:(?<buttondesc>.+)[\r\n]+TEXT:(?<eula>.+)$";
        
        private static Regex m_EulaResponseParsingRegex;

        public static Regex EulaResponseParsingRegex
        {
            get
            {
                if (m_EulaResponseParsingRegex == null)
                {
                    m_EulaResponseParsingRegex = 
                        new Regex(ResponseRegEx,RegexOptions.Compiled | RegexOptions.Singleline);
                }
                return m_EulaResponseParsingRegex;
            }
        }

        public const string SRCSRVLocator = "SRCSRV: ini --------------------------------------------";
      
        public const string VERCTRL = "VERCTRL=http";

        public const string VersionLocator = "52534453"; //RSDS

        public const string rootHeader = "http://ReferenceSource.microsoft.com/source/.net/8.0/DEVDIV/";
        //This will be changed with dynamic version
        public const string userAgentHeader = "Microsoft-Symbol-Server/6.8.0004.0";
        public const string symbolsLocation = "http://referencesource.microsoft.com/symbols/";
    }
}
