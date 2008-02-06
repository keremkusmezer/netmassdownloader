#region Implementation Notes
/*------------------------------------------------------------------------------
 * PdbParser for Version 7.0 CopyRight - Kerem Kusmezer (keremskusmezer@gmail.com)
 *                        
 * Parses Pdb Files For Streams And Extract Whole Found Streams From Pdb Files.
 * 
 * http://moyix.blogspot.com/search/label/debugging used as description reference
 * 
 * ---------------------------------------------------------------------------*/
//Thanks To Brendan Dolan-Gavitt For Pdb Stream Parsing And  Detail Stream Parsing Algorithms And Structures :) 
//in http://pdbparse.googlecode.com/svn/pdbparse/ 
//Thanks To Sven B. Schreiber http://www.rawol.com/ for the initial 
//PDB Hacking And Reverse Engineering, without him none of this were possible :)
//Code Partially Taken From His win_pdbx implementation.
//Great Rocker Great Programmer :) 
#endregion
#region Imported Libraries
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using DownloadLibrary.Classes;
#endregion
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
namespace DownloadLibrary.Classes
{
    /// <summary>
    /// PdbParser Which Extracts The Streams From A PDB File. 
    /// It is a direct implementation in C# By Kerem Kusmezer
    /// </summary>
    public class PdbParser : IDisposable
    {

        ///Empty Stream Indicator In Pdb Files
        internal const UInt32 pdbEmptyStreamHeader =
                                0xffffffff;

        private List<UInt32> m_rootStreamTargetPages =
            new List<UInt32>();

        private MemoryStream m_rootStreamContainer;

        private Dictionary<uint, PdbPartialStream> m_partialStreamDirectory =
                        new Dictionary<uint, PdbPartialStream>();

        /// <summary>
        /// Standart PDB 7.0 Version Header
        /// </summary>
        private static char[] PDBHeader = new char[] { 'M','i','c','r','o','s','o','f','t',
                                                       ' ','C','/','C','+','+',' ','M','S','F',' ','7','.',
                                                       '0','0','\r','\n','\x1A','D','S','\0','\0','\0'};
        #region Pdb Header Formats

        private System.UInt32 m_streamCount;

        /// <summary>
        /// Returns The Total Number Of Streams In The Pdb File
        /// </summary>
        public System.UInt32 StreamCount
        {
            get { return m_streamCount; }
            private set { m_streamCount = value; }
        }

        /// <summary>
        /// Bytes Count Of Each Pages In Pdb File
        /// </summary>
        private System.UInt32 m_dPageBytes;

        /// <summary>
        /// Flag Pages Used By The Pdb Parser
        /// </summary>
        private System.UInt32 m_dFlagPage;

        /// <summary>
        /// The number of pages in PDB File. 
        /// Multiplying this with dPageBytes Gives Out The Length Of The File
        /// </summary>
        private System.UInt32 m_dFilePages;

        /// <summary>
        /// Size Of Root Structure As Byte
        /// </summary>
        private System.UInt32 m_dRootBytes;

        /// <summary>
        /// This is reserved and always 0
        /// </summary>
        private System.UInt32 m_dReserved;

        /// <summary>
        /// Holder For PdbStream
        /// </summary>
        private System.IO.MemoryStream m_pdbStream;

        private string m_pdbPath;

        /// <summary>
        /// Path Of The Pdb
        /// </summary>
        public string PdbPath
        {
            get
            {
                return m_pdbPath;
            }
        }

        #endregion

        #region Position Calculation Methods

        /// <summary>
        /// Calculates The Stream Index Location For The Given Root Bytes
        /// </summary>
        /// <param name="dRootBytes"></param>
        /// <returns></returns>
        private UInt32 CalculateStreamIndexLocation(uint dRootBytes)
        {
            return ((((dRootBytes - 1) / m_dPageBytes) * 4) / m_dPageBytes) + 1;
        }

        /// <summary>
        /// Calculates How Much The Given Streams Size Will Take In The Pdb File
        /// </summary>
        /// <param name="ByteStreamLength"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        private static UInt32 CalculatePageCount(UInt32 ByteStreamLength, UInt32 PageSize)
        {
            UInt32 numberOfPages = ByteStreamLength / PageSize;
            numberOfPages += (uint)(ByteStreamLength % PageSize != (uint)0 ? 1 : 0);
            return numberOfPages;
        }

        private UInt32 CalculateDWORDCountForRootIndex(uint dRootBytes)
        {
            return CalculatePageCount(dRootBytes, m_dPageBytes);//((dRootBytes - 1) / m_dPageBytes) + 1;
        }

        private UInt32 CalculateDWORDToByteCountForRootIndex(uint dRootBytes)
        {
            return CalculateDWORDCountForRootIndex(dRootBytes) * 4;
        }

        #endregion


        /// <summary>
        /// Main Constructor For Pdb
        /// </summary>
        /// <param name="PdbFileToLoad"></param>
        public PdbParser(string PdbFileToLoad)
        {

            if (!System.IO.File.Exists(PdbFileToLoad))
                throw new FileNotFoundException("File Not Found", PdbFileToLoad);

            m_pdbPath = PdbFileToLoad;

            ParsePdbBody(System.IO.File.ReadAllBytes(PdbFileToLoad));

        }

        /// <summary>
        /// Main Constructor For Pdb
        /// </summary>
        /// <param name="PdbFileToLoad"></param>
        public PdbParser(System.IO.MemoryStream PdbStream)
        {
            ParsePdbBody(PdbStream);
        }

        /// <summary>
        /// Main Constructor For Pdb
        /// </summary>
        /// <param name="PdbFileToLoad"></param>
        private void ParsePdbBody(byte[] PdbBody)
        {
            System.IO.MemoryStream PdbStream = new System.IO.MemoryStream(PdbBody);
            ParsePdbBody(PdbStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PdbStream"></param>
        private void ParsePdbBody(System.IO.MemoryStream PdbStream)
        {
            if (PdbStream == null)
                throw new ArgumentNullException("PdbStream");

            m_pdbStream =
                new MemoryStream();

            using (BinaryReader br = new BinaryReader(PdbStream))
            {
                //Check The File Head For The PDB Header String
                for (int i = 0; i < PDBHeader.Length; i++)
                {
                    if (PDBHeader[i] != br.ReadByte())
                    {
                        throw new InvalidDataException();
                    }
                }

                //Size Of Page How Much Bytes It Takes
                m_dPageBytes = br.ReadUInt32();

                //The Flag Page
                m_dFlagPage = br.ReadUInt32();

                //Count Of Pages In File 
                m_dFilePages = br.ReadUInt32();

                //Size Of Root Structure As Byte
                m_dRootBytes = br.ReadUInt32();

                //This is reserved and always 0
                m_dReserved = br.ReadUInt32();

                if (PdbStream.Length !=
                    (m_dPageBytes * m_dFilePages))
                {
                    throw new InvalidDataException();
                }

                // We must divide rootBytes To PageBytes Multiply With 4 And divide again to pageBytes
                // This gives us the bytes required to store the RootBytes byte locations in a byte 
                // array.
                UInt32 locationForRootIndex =
                    ((((m_dRootBytes - 1) / m_dPageBytes) * 4) / m_dPageBytes) + 1;

                //Calculate The Bytes Required For The RootIndex
                UInt32 sizeForRootIndexBytes =
                     ((m_dRootBytes - 1) / m_dPageBytes) + 1;

                sizeForRootIndexBytes =
                    sizeForRootIndexBytes * 4;

                List<uint> adIndexPagesList =
                    new List<uint>();

                for (uint locationCounter = 0;
                     locationCounter < locationForRootIndex;
                     locationCounter++)
                {
                    adIndexPagesList.Add(br.ReadUInt32());
                }

                MemoryStream rootStream = new MemoryStream();

                PdbStream.WriteTo(m_pdbStream);

                //Here We Get The Bytes That Construct The Base Stream
                BuildUpRootStream(PdbStream, adIndexPagesList, sizeForRootIndexBytes);

            }
        }

        /// <summary>
        /// Reads The Given Byte Range From The Provided Stream Array Using Page Calculations
        /// </summary>
        /// <param name="sourceStream">The Main Pdb Stream To Be Searched For</param>
        /// <param name="adIndexPages">Pages That Holds The Contents Of The Stream</param>
        /// <param name="sizeForRootIndexBytes">The Length Of The Stream To Be Readed</param>
        /// <returns></returns>
        private System.IO.MemoryStream ReadBlockBytesFromStream(System.IO.Stream sourceStream,
                                                                List<uint> adIndexPages,
                                                                uint sizeForRootIndexBytes)
        {

            //TODO Add Range Checking And Exceptions Here!!!

            System.IO.MemoryStream rootStream =
                  new System.IO.MemoryStream();

            for (int i = 0; i < adIndexPages.Count; i++)
            {
                //Here We Get The Bytes That Construct The Base Stream
                byte[] firstPageData =
                    new byte[m_dPageBytes];

                m_pdbStream.Seek(((adIndexPages[i]) * m_dPageBytes), SeekOrigin.Begin);

                int readedAmount =
                    m_pdbStream.Read(firstPageData, 0, (int)m_dPageBytes);

                if (readedAmount > sizeForRootIndexBytes)
                {
                    readedAmount = (int)sizeForRootIndexBytes;
                }

                rootStream.Write(firstPageData, 0, readedAmount);

                sizeForRootIndexBytes -= (uint)readedAmount;

            }

            rootStream.Position = 0;

            return rootStream;
        }

        private MemoryStream m_rootStream;

        /// <summary>
        /// This Method Reads The Root Byte Pages, Composes The Root Stream, Extracts The Streams From The RootStream And Composes And Exposes Them Out.
        /// </summary>
        /// <param name="mainStream">The PdbStream That Will Be Extracted</param>
        /// <param name="adIndexPages"></param>
        /// <param name="sizeForRootIndexBytes"></param>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        private bool BuildUpRootStream(System.IO.Stream mainStream,
                                                         List<uint> adIndexPages,
                                                         uint sizeForRootIndexBytes)
        {
            // There seems to be a bug, will be fixed now
            //Reads Whole Bytes That Buildup The Root Stream
            System.IO.MemoryStream rootStream =
                   ReadBlockBytesFromStream(mainStream,
                                            adIndexPages,
                                            sizeForRootIndexBytes);

            System.IO.MemoryStream rootStreamComposed =
                    new System.IO.MemoryStream();

            UInt32 totalSizes = this.m_dRootBytes;

            int totalCount = 0;

            using (BinaryReader rootStreamReader =
                    new BinaryReader(rootStream))
            {

                //Composes The Root Stream From The Found Out Pages
                while ((rootStream.Position + 1) < rootStream.Length)
                {
                    totalCount++;
                    byte[] firstPageData2 =
                           new byte[m_dPageBytes];

                    uint targetPage =
                        rootStreamReader.ReadUInt32();

                    m_rootStreamTargetPages.Add(targetPage * m_dPageBytes);

                    mainStream.Seek((targetPage * m_dPageBytes), SeekOrigin.Begin);

                    int tempReadedAmount =
                        mainStream.Read(firstPageData2, 0, (int)m_dPageBytes);

                    uint currentPosition =
                        BitConverter.ToUInt32(firstPageData2, 0);

                    if (totalSizes < tempReadedAmount)
                        tempReadedAmount = (int)totalSizes;
                    else
                        totalSizes -= (uint)tempReadedAmount;

                    rootStreamComposed.Write(firstPageData2, 0, tempReadedAmount);


                }
            }

            rootStreamComposed.Position = 0;

            m_rootStream =
                new MemoryStream(rootStreamComposed.ToArray());

            m_rootStreamContainer =
                new MemoryStream();

            rootStreamComposed.WriteTo(m_rootStreamContainer);

            Debug.Assert(m_dRootBytes == rootStreamComposed.Length, "Base Transfer Failed");

            //   Ok Now We have the root directory stream
            //   Go Read Each Of The ChildStreams And Compose Them To MemoryStreams :)
            using (BinaryReader testReader = new BinaryReader(rootStreamComposed))
            {
                //Ok First DWORD In The Stream Is The Count Of The Whole Streams Including BaseStream
                StreamCount = testReader.ReadUInt32();

                Debug.WriteLine("Total Streams Contained:", StreamCount.ToString());

                ///Here are the sizes for each of the separated stream sizes
                for (uint streamCounter = 0;
                        streamCounter < StreamCount;
                        streamCounter++)
                {

                    PdbPartialStream partialStream =
                        new PdbPartialStream(this.m_dPageBytes);

                    partialStream.InitialSize =
                        testReader.ReadUInt32();

                    partialStream.StreamLocation =
                        streamCounter;

                    StreamLengths.Add(streamCounter,
                         partialStream.InitialSize);

                    m_partialStreamDirectory.Add(streamCounter,
                        partialStream);

                }
                ///Build up the streams using the pages located here
                for (uint subDetailCounter = 0;
                        subDetailCounter < StreamCount;
                        subDetailCounter++)
                {

                    uint streamLength =
                        StreamLengths[subDetailCounter];

                    PdbPartialStream partialStream =
                        m_partialStreamDirectory[subDetailCounter];

                    uint pageCountToBeReaded =
                        CalculateDWORDCountForRootIndex(streamLength);

                    List<uint> wholeAddresses =
                        new List<uint>();

                    System.IO.MemoryStream finalStream =
                        new System.IO.MemoryStream();

                    if (streamLength == 0 || streamLength == pdbEmptyStreamHeader)
                        pageCountToBeReaded = 0;

                    partialStream.RootStreamBeginIndex =
                        (uint)testReader.BaseStream.Position;

                    bool shouldExit = false;

                    if ((testReader.BaseStream.Position + 1) < testReader.BaseStream.Length)
                    {

                        for (int pageCounter = 0;
                             pageCounter < pageCountToBeReaded;
                             pageCounter++)
                        {

                            byte[] finalStreamDataContainer =
                               new byte[m_dPageBytes];

                            uint currentStreamSourcePage =
                                    testReader.ReadUInt32();

                            partialStream.PagesUsedByStream.Add(currentStreamSourcePage);

                            mainStream.Seek((currentStreamSourcePage * m_dPageBytes),
                                            SeekOrigin.Begin);

                            int streamSourcePagesRead =
                                mainStream.Read(finalStreamDataContainer, 0, (int)m_dPageBytes);

                            uint currentPositionStreamBegin =
                                    BitConverter.ToUInt32(finalStreamDataContainer, 0);

                            if (streamLength < streamSourcePagesRead)
                            {
                                streamSourcePagesRead = (int)streamLength;
                                shouldExit = true;
                            }
                            else
                            {
                                streamLength -= (uint)streamSourcePagesRead;
                            }

                            finalStream.Write(finalStreamDataContainer, 0, streamSourcePagesRead);

                            if (shouldExit)
                                break;

                        }
                    }
                    partialStream.BuildUpDecomposedPartialStream(mainStream);
                    if (InternalStreams.Count == 1)
                    {
                        m_Reader =
                            new PdbInfoStreamReader(partialStream.DecompossedPartialStream);
                        m_Reader.ParseStream();
                    }
                    InternalStreams.Add(partialStream.DecompossedPartialStream);

                    StreamDictionary.Add(subDetailCounter, partialStream.DecompossedPartialStream);

                }
            }

            return true;

        }

        private PdbInfoStreamReader m_Reader;

        public PdbInfoStreamReader PdbStreamHeaderReader
        {
            get { return m_Reader; }
            set { m_Reader = value; }
        }

        Dictionary<uint, uint> StreamLengths =
            new Dictionary<uint, uint>();

        public List<MemoryStream> InternalStore
        {
            get
            {
                return InternalStreams;
            }
        }

        /// <summary>
        /// Extracts Whole Detected Streams To The Target Path
        /// </summary>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public bool SaveToPath(string targetPath)
        {
            for (int i = 0; i < InternalStreams.Count; i++)
            {

                System.IO.File.WriteAllBytes(targetPath + "\\" + System.IO.Path.GetFileName(m_pdbPath) + "." + String.Format("{0:000}", i), InternalStreams[i].ToArray());
            }
            return true;
        }

        List<MemoryStream> InternalStreams =
                new List<MemoryStream>();

        Dictionary<uint, MemoryStream> StreamDictionary =
            new Dictionary<uint, MemoryStream>();

        private static int OrderStreamsByLength(MemoryStream leftStream, MemoryStream rightStream)
        {
            //TODO Make A Static Sorter Class And Move Into It
            return rightStream.Length.CompareTo(leftStream.Length);
        }
        private static int OrderStreamsByLengthForPdbPartial(PdbPartialStream leftStream, PdbPartialStream rightStream)
        {
            //TODO Make A Static Sorter Class And Move Into It
            return rightStream.DecompossedPartialStream.Length.CompareTo(leftStream.DecompossedPartialStream.Length);
        }
        [Obsolete("Will Be Removed Dont Use It",true)]
        public PdbPartialStream GetStreamBeginsWithASCII(string textToBeLocated)
        {
            //TODO This is a very bad algorithm we should improve it later
            byte[] asciiArrayToBeSearched =
                System.Text.Encoding.ASCII.GetBytes(textToBeLocated);

            string arrayToBeLocated =
               Convert.ToBase64String(tempMD5.ComputeHash(asciiArrayToBeSearched));

            List<PdbPartialStream> tempStream = new List<PdbPartialStream>();
            tempStream.AddRange(m_partialStreamDirectory.Values);
            tempStream.Sort(OrderStreamsByLengthForPdbPartial);
            for (int i = 0; i < tempStream.Count; i++)
            {
                if (tempStream[i].DecompossedPartialStream.Length >=
                    asciiArrayToBeSearched.Length)
                {
                    byte[] beginArray =
                        new byte[asciiArrayToBeSearched.Length];
                    tempStream[i].DecompossedPartialStream.Read(beginArray, 0, asciiArrayToBeSearched.Length);
                    string locator = Convert.ToBase64String(tempMD5.ComputeHash(beginArray));

                    if (locator == arrayToBeLocated)
                    {
                        //MemoryStream foundStream = new MemoryStream(tempStream[i].DecompossedPartialStream.ToArray());
                        return tempStream[i];
                    }
                }
            }
            return null;
        }

        public MemoryStream GetStreamBeginsWithBytesArrayASCII(string textToBeLocated)
        {
            return GetStreamBeginsWithBytesArrayASCII(System.Text.Encoding.ASCII.GetBytes(textToBeLocated));
        }

        private static MD5 tempMD5 = MD5.Create();

        public MemoryStream GetStreamBeginsWithBytesArrayASCII(byte[] asciiArrayToBeSearched)
        {
            InternalStreams.Sort(OrderStreamsByLength);

            string arrayToBeLocated =
                Convert.ToBase64String(tempMD5.ComputeHash(asciiArrayToBeSearched));

            for (int i = 0; i < InternalStreams.Count; i++)
            {
                if (InternalStreams[i].Length >=
                    asciiArrayToBeSearched.Length)
                {
                    byte[] beginArray =
                        new byte[asciiArrayToBeSearched.Length];
                    InternalStreams[i].Read(beginArray, 0, asciiArrayToBeSearched.Length);
                    string locator = Convert.ToBase64String(tempMD5.ComputeHash(beginArray));

                    if (locator == arrayToBeLocated)
                    {
                        MemoryStream foundStream = new MemoryStream(InternalStreams[i].ToArray());
                        return foundStream;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns The Target Stream From A Pdb File
        /// </summary>
        /// <param name="StreamIndex">0 Based Index For PdbStream in Pdb File</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception> 
        /// <returns></returns>
        public MemoryStream GetTargetStream(uint StreamIndex)
        {
            if ((InternalStreams.Count == 0)
                ||
                (StreamIndex > (InternalStreams.Count - 1)))
            {
                throw new ArgumentOutOfRangeException("StreamIndex");
            }

            return InternalStreams[(int)StreamIndex];
        }

        public PdbPartialStream GetPartialStream(uint StreamIndex)
        {

            if ((InternalStreams.Count == 0)
                ||
                (StreamIndex > (InternalStreams.Count - 1)))
            {
                throw new ArgumentOutOfRangeException("StreamIndex");
            }

            return m_partialStreamDirectory[StreamIndex];
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                lock (InternalStreams)
                {
                    for (int i = 0; i < InternalStreams.Count; i++)
                    {
                        InternalStreams[i].Dispose();
                    }
                    InternalStreams.Clear();
                }
                m_rootStream.Dispose();
            }
            catch
            {

            }
        }

        #endregion
    }

}