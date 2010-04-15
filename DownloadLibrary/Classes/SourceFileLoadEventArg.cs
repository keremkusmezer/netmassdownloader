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
 * 
 * 
 * Taken From The Following Project Also Written By Kerem Kusmezer 
 * PdbParser in C# http://www.codeplex.com/pdbparser 
 * 
*/
#endregion
/*------------------------------------------------------------------------------
 * SourceCodeDownloader - Kerem Kusmezer (izzetkeremskusmezer@gmail.com)
 *                        
 * 
 * Download all the .NET Reference Sources and PDBs to pre-populate your 
 * symbol server! No more waiting as each file downloads individually while
 * debugging.
 * ---------------------------------------------------------------------------*/
#region Licence Information
/*   
   Copyright 2008 Kerem Kusmezer(izzetkeremskusmezer@gmail.com) And John Robbins(john@wintellect.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion

#region Imported Libraries
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace DownloadLibrary.Classes
{
    public class SourceFileLoadEventArg : EventArgs
    {
        private string m_fileName;

        public string FileName
        {
            get { return m_fileName; }
        }
        private Exception m_occurredException;

        public Exception OccurredException
        {
            get { return m_occurredException; }
        }
        private string m_fileUrl;

        public string FileUrl
        {
            get { return m_fileUrl; }
        }
        public override string ToString()
        {
            if (m_occurredException != null)
            {
                //return String.Format("{0} {1} {2}", FileName, FileUrl,m_occurredException.Message);
                return string.Format( "{0}{0}{1}", "    ", m_occurredException.Message );
            }
            else
            {
                return String.Format("{0} {1}", FileName, FileUrl);
            }
        }

        private string m_fileLocationState;

        public string FileLocationState
        {
            get { return m_fileLocationState; }
        }

        public SourceFileLoadEventArg(string FileName,string FileUrl,string FileLocationState)
        {
            m_fileName = FileName;
            m_fileUrl = FileUrl;
            m_fileLocationState = FileLocationState;
        }
        public SourceFileLoadEventArg(string FileName, string FileUrl, string FileLocationState, Exception OccurredException)
            : this(FileName, FileUrl, FileLocationState)
        {
            m_occurredException = OccurredException;
        }
            
    }
}
