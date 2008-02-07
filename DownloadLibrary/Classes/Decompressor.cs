/*------------------------------------------------------------------------------
 * SourceCodeDownloader - Kerem Kusmezer (keremskusmezer@gmail.com)
 *                        
 * 
 * Download all the .NET Reference Sources and PDBs to pre-populate your 
 * symbol server! No more waiting as each file downloads individually while
 * debugging.
 * ---------------------------------------------------------------------------*/
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
 * Taken From The Following Project Also Written By Kerem Kusmezer 
 * PdbParser in C# http://www.codeplex.com/pdbparser 
 * 
*/
#endregion
#region Imported Libraries
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
#endregion
namespace DownloadLibrary.Classes
{
    /// <summary>
    /// We need this for pdb download, pdbs are stored as compressor compressed files, 
    /// so we need to extract them.
    /// </summary>
    internal static class Decompressor
    {
        #region DllImports Obsolete 
        //[DllImport("lz32.dll", EntryPoint = "LZOpenFile", SetLastError = true,CharSet = CharSet.Ansi)]
        //public static extern int LZOpenFileA(
        //    string lpszFile,
        //    [Out, MarshalAs(UnmanagedType.LPStruct)] OFSTRUCT lpOf,
        //    int mode);

        //[DllImport("lz32.dll")]
        //public static extern int LZCopy(int hfSource, int hfDest);

        //[DllImport("lz32.dll")]
        //public static extern int LZClose(int hfFile);
        
        //[StructLayout(LayoutKind.Sequential)]
        //public class OFSTRUCT
        //{
        //    public const int OFS_MAXPATHNAME = 128;
        //    public byte cBytes;
        //    public byte fFixedDisc;
        //    public UInt16 nErrCode;
        //    public UInt16 Reserved1;
        //    public UInt16 Reserved2;
        //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = OFS_MAXPATHNAME)]
        //    public string szPathName;
        //}
        
        //private const int OF_READ = 0x0;
        //private const int OF_CREATE = 0x1000;

        //public static bool ExpandSourceFileToTarget(string sourceFile, string targetFile)
        //{            
        //    OFSTRUCT openedFileStruct = new OFSTRUCT();

        //    OFSTRUCT targetFileStruct = new OFSTRUCT();

        //    int openedFile = 0;

        //    int targetFiles = 0;

        //    try
        //    {
                
        //        openedFile =
        //            LZOpenFileA(sourceFile, openedFileStruct, OF_READ);
                
        //        targetFiles =
        //            LZOpenFileA(targetFile, targetFileStruct, OF_CREATE);

        //        LZCopy(openedFile, targetFiles);

        //    }
        //    catch(Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        LZClose(openedFile);
        //        LZClose(targetFiles);
        //    }
        //    return true;
        //}
         #endregion

        public static bool ExpandSourceFileToTarget(string sourceFile, string targetFile)
        {
            System.Diagnostics.Process tempProcess = new System.Diagnostics.Process();


           // Environment.GetEnvironmentVariable("SystemRoot");
            string windowsLocation = Environment.GetEnvironmentVariable("windir");

            windowsLocation = windowsLocation + @"\system32\expand.exe";

            
        
            //BUGFIX 1118 08.02.2008 Kerem Kusmezer Stupid Me I forgot to put the target directory 
            //string in "\ "\'s so directories including spaces caused problems.
            targetFile = System.IO.Path.GetDirectoryName(targetFile);
            System.Diagnostics.ProcessStartInfo tempStart = 
                new System.Diagnostics.ProcessStartInfo(windowsLocation,
                        " -r \"" + sourceFile + "\" \"" + targetFile + "\"");

            tempProcess.StartInfo = tempStart;
            tempProcess.StartInfo.UseShellExecute = true;
            tempProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            tempProcess.Start();
            tempProcess.WaitForExit();
            return (tempProcess.ExitCode == 0);
        }

        public static MemoryStream ExpandSourceFileToTargets(string sourceFile, string targetFile)
        {
            MemoryStream resultStream = null;
            if (ExpandSourceFileToTarget(sourceFile, targetFile))
            {
                resultStream = 
                    new MemoryStream(System.IO.File.ReadAllBytes(targetFile));
            }
            return resultStream;
        }
    }
}
       