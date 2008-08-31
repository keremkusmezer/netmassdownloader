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
using DownloadLibrary.Classes;
using System.IO;
#endregion

namespace SrvSrcExtractor
{
    class Program
    {
        const string SRCSRVLocator = "SRCSRV: ini --------------------------------------------";
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayUsage();
            }
            else
            {
                WriteHeader();
                foreach (string file in args)
                {
                    if (System.IO.File.Exists(file)){
                        SrvSrcFileExtractor(file);
                    }
                    else if (Directory.Exists(file))
                    {
                        ProcessDirectory(file);
                    }
                    else
                    {
                        Console.WriteLine("Given File Not Found");
                    }
                }
            }
        }

        private static void ProcessDirectory(string file)
        {
            string[] files =
                Directory.GetFiles(file, "*.pdb");
            if (files != null)
            {
                foreach (string infile in files)
                {
                    SrvSrcFileExtractor(infile);
                }
            }
            string[] subDirectories =
                Directory.GetDirectories(file);
            if (subDirectories != null)
            {
                foreach (string subDirectory in subDirectories)
                {
                    ProcessDirectory(subDirectory);
                }
            }
        }

        private static void SrvSrcFileExtractor(string file)
        {
            string targetPath =
                Utility.CleanupPath(Path.GetDirectoryName(file));

            targetPath += System.IO.Path.GetFileName(file) + ".txt";

            try
            {
                using (PdbParser tempParser =
                     new PdbParser(file))
                {
                    MemoryStream resultStream =
                        tempParser.GetStreamBeginsWithBytesArrayASCII(SRCSRVLocator);

                    if (resultStream != null)
                    {
                        Console.WriteLine("Writing File To:" + targetPath);
                        using (System.IO.FileStream outputStream = new System.IO.FileStream(targetPath, FileMode.Create))
                        {
                            resultStream.WriteTo(outputStream);
                            outputStream.Flush();
                            resultStream.Dispose();
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (System.IO.IOException ioException)
            {
                Console.WriteLine("An IO Exception Occurred While Extracting The Pdb File:" + ioException.Message);
            }
        }
        private static void WriteHeader()
        {
            Console.WriteLine("SrvSrc Extractor v0.1");
            Console.WriteLine("---------------------------------------------");
        }
        private static void DisplayUsage()
        {
            Console.WriteLine("Usage Is: SrvSrcExtractor nameofthepdbfile.pdb nameofthesecondpdbfile.pdb directory containing pdb's works also recursive");
            Console.WriteLine("The srcsrv will be extracted with the same name with txt extension appended back of them");
        }
    }
}