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
/*------------------------------------------------------------------------------
 * SourceCodeDownloader - Kerem Kusmezer (keremskusmezer@gmail.com)
 *                        John Robbins (john@wintellect.com)
 * 
 * Download all the .NET Reference Sources and PDBs to pre-populate your 
 * symbol server! No more waiting as each file downloads individually while
 * debugging.
 * ---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloadLibrary.Classes
{
    /// <summary>
    /// Thrown when there is no .DEBUG section in the PE file.
    /// </summary>
    public class NoDebugSectionException : Exception
    {
        public NoDebugSectionException():base("No Debug Section Can Be Located")
        {

        }
        public NoDebugSectionException(string text)
            : base(text)
        {

        }
    }
    /// <summary>
    /// Thrown when there is no .PDB section in the PE file.
    /// </summary>
    public class NoPdbSectionException : NoDebugSectionException
    {
        public NoPdbSectionException():base("No Pdb Section Can Be Located")
        {

        }
    }
}
