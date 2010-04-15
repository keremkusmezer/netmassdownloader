#region Imported Libraries
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#endregion

#region Implementation Notes
/*------------------------------------------------------------------------------
 * SourceCodeDownloader - Kerem Kusmezer (izzetkeremskusmezer@gmail.com)
 *                        
 * 
 * Download all the .NET Reference Sources and PDBs to pre-populate your 
 * symbol server! No more waiting as each file downloads individually while
 * debugging.
 * ---------------------------------------------------------------------------*/
// Thanks To Brendan Dolan-Gavitt For Pdb Stream Parsing And 
// Detail Stream Parsing Algorithms And Structures :)
// Most of the code has been ported 
// from his phyton implementation in http://pdbparse.googlecode.com/svn/pdbparse/ 	
#endregion

#region Licence Information
/*       
 * http://www.codeplex.com/NetMassDownloader To Get The Latest Version
 *     
 * Copyright 2008 Kerem Kusmezer(izzetkeremskusmezer@gmail.com)
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
    /// Pdb Partial Stream Which Holds One Of The Streams Contained In The Pdb Stream Directory
    /// </summary>
    public class PdbPartialStream
    {
        public static Int32 CalculatePageCount(UInt32 ByteStreamLength, UInt32 PageSize)
        {
            Int32 numberOfPages = (int)(ByteStreamLength / PageSize);
            numberOfPages += (ByteStreamLength % PageSize != 0 ? 1 : 0);
            return numberOfPages;
        }

        private UInt32 m_streamLocation;

        public UInt32 StreamLocation
        {
            get { return m_streamLocation; }
            set { m_streamLocation = value; }
        }

        private System.IO.MemoryStream ReadBlockBytesFromStream(System.IO.Stream sourceStream,
                                                                List<uint> adIndexPages,
                                                                uint sizeForRootIndexBytes)
        {

            //TODO Add Range Checking And Exceptions Here!!!
            uint controlSize = sizeForRootIndexBytes;

            System.IO.MemoryStream rootStream =
                  new System.IO.MemoryStream();

            for (int i = 0; i < adIndexPages.Count; i++)
            {
                //Here We Get The Bytes That Construct The Base Stream
                byte[] firstPageData =
                    new byte[m_pageSize];

                sourceStream.Seek(((adIndexPages[i]) * m_pageSize), SeekOrigin.Begin);

                int readedAmount =
                    sourceStream.Read(firstPageData, 0, (int)m_pageSize);

                if (readedAmount > sizeForRootIndexBytes)
                {
                    readedAmount = (int)sizeForRootIndexBytes;
                }

                rootStream.Write(firstPageData, 0, readedAmount);

                sizeForRootIndexBytes -= (uint)readedAmount;

            }

            rootStream.Position = 0;

            if (rootStream.Length != controlSize)
            {

                System.Diagnostics.Debug.Assert(rootStream.Length ==
                                                sizeForRootIndexBytes);

            }
            return rootStream;
        }

        public void BuildUpDecomposedPartialStream(System.IO.Stream sourceStream)
        {
            if (null == m_decompossedPartialStream &&
                this.m_InitialSize != PdbParser.pdbEmptyStreamHeader)
            {
                sourceStream.Position = 0;
                m_decompossedPartialStream =
                    ReadBlockBytesFromStream(sourceStream, m_pagesUsedByStream, m_InitialSize);
                m_decompossedPartialStream.Position = 0;
            }
            else
            {
                m_decompossedPartialStream = new MemoryStream();
            }
        }

        private UInt32 m_pageSize;

        public UInt32 PageCount
        {
            get
            {
                return (uint)CalculatePageCount((uint)m_decompossedPartialStream.Length, m_pageSize);
            }
        }

        private System.IO.MemoryStream m_decompossedPartialStream;

        public System.IO.MemoryStream DecompossedPartialStream
        {
            get { return m_decompossedPartialStream; }
            set { m_decompossedPartialStream = value; }
        }

        private System.UInt32 m_RootStreamBeginIndex;

        public System.UInt32 RootStreamBeginIndex
        {
            get { return m_RootStreamBeginIndex; }
            set { m_RootStreamBeginIndex = value; }
        }

        private List<System.UInt32> m_pagesUsedByStream;

        public List<System.UInt32> PagesUsedByStream
        {
            get { return m_pagesUsedByStream; }
        }

        private System.UInt32 m_InitialSize;

        public System.UInt32 InitialSize
        {
            get { return m_InitialSize; }
            set { m_InitialSize = value; }
        }

        public PdbPartialStream(UInt32 PageSize)
        {
            m_pagesUsedByStream = new List<System.UInt32>();
            m_pageSize = PageSize;
        }

    }

    public abstract class PdbBaseStreamReader : IDisposable
    {

        MemoryStream m_streamToBeParsed;
        BinaryReader m_streamReader;

        protected void TruncateStream()
        {
            long oldLength = StreamToBeParsed.Length;

            StreamToBeParsed.SetLength(1);
            StreamToBeParsed.SetLength(oldLength);
            StreamToBeParsed.Position = 0;
        }

        protected BinaryReader StreamReader
        {
            get { return m_streamReader; }
            private set { m_streamReader = value; }
        }

        protected MemoryStream StreamToBeParsed
        {
            get { return m_streamToBeParsed; }
            private set { m_streamToBeParsed = value; }
        }

        public PdbBaseStreamReader(MemoryStream StreamToBeParsed)
        {

            if (null == StreamToBeParsed)
                throw new ArgumentNullException("StreamToBeParsed", "Given Stream cannot be null");

            this.StreamToBeParsed = StreamToBeParsed;
            this.StreamReader = new BinaryReader(StreamToBeParsed);
        }
        public bool SaveStream()
        {
            return SaveStreamInternal();
        }
        public bool ParseStream()
        {
            return ParseStreamInternal();
        }

        protected abstract bool ParseStreamInternal();
        protected abstract bool SaveStreamInternal();

        #region IDisposable Members

        public void Dispose()
        {
            if (null != m_streamToBeParsed)
                m_streamToBeParsed.Dispose();
            m_streamToBeParsed = null;
        }

        #endregion
    }
    public class PdbDebugInfoStreamReader : PdbBaseStreamReader
    {
        public PdbDebugInfoStreamReader(MemoryStream streamToBeParsed)
            : base(streamToBeParsed)
        {

        }
        protected override bool ParseStreamInternal()
        {
            return true;
            //throw new Exception("The method or operation is not implemented.");
        }

        protected override bool SaveStreamInternal()
        {
            return true;
            //throw new Exception("The method or operation is not implemented.");
        }
    }
    /// <summary>
    /// Encapsulates The Pdb Info Stream Which Contains The Information About The Guid, Version , Name Of The Pdb
    /// </summary>
    public class PdbInfoStreamReader : PdbBaseStreamReader
    {

        private UInt32 m_version;

        public UInt32 Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        private UInt32 m_timeDateStamp;

        public UInt32 TimeDateStamp
        {
            get { return m_timeDateStamp; }
            set { m_timeDateStamp = value; }
        }

        private UInt32 m_age;

        public UInt32 Age
        {
            get { return m_age; }
            set { m_age = value; }
        }

        private System.Guid m_guid;

        public System.Guid Guid
        {
            get { return m_guid; }
            set { m_guid = value; }
        }

        //Guid Is Always A 16 Byte Structure
        private byte[] m_guidArray;

        private byte[] pdbFileNamesByteContainer;

        //Number Of Bytes For The Names String
        private UInt32 m_cbNames;

        private String m_pdbFileNames;

        public String PdbFileNames
        {
            get
            {
                return m_pdbFileNames;
            }
            set
            {

                if (((uint)value.Length) > UInt32.MaxValue)
                    throw new InvalidDataException();

                m_pdbFileNames =
                    value;

                m_cbNames =
                    (uint)m_pdbFileNames.Length;

                pdbFileNamesByteContainer =
                    System.Text.Encoding.ASCII.GetBytes(m_pdbFileNames);

            }
        }



        public PdbInfoStreamReader(MemoryStream StreamToBeParsed)
            : base(StreamToBeParsed)
        {

        }

        protected override bool ParseStreamInternal()
        {
            m_version = StreamReader.ReadUInt32();
            m_timeDateStamp = StreamReader.ReadUInt32();
            m_age = StreamReader.ReadUInt32();
            m_guidArray = StreamReader.ReadBytes(16);
            m_cbNames = StreamReader.ReadUInt32();

            byte[] finalGuid = new byte[16];

            //Make The Byte Order Correct So We Can Construct The Right Guid Out Of It

            //Guid Buildup Begin
            finalGuid[0] = m_guidArray[3];
            finalGuid[1] = m_guidArray[2];
            finalGuid[2] = m_guidArray[1];
            finalGuid[3] = m_guidArray[0];
            //c section
            finalGuid[4] = m_guidArray[5];
            finalGuid[5] = m_guidArray[4];
            //d relocation
            finalGuid[6] = m_guidArray[7];
            finalGuid[7] = m_guidArray[6];

            for (int guidCount = 8; guidCount < 16; guidCount++)
            {
                finalGuid[guidCount] = m_guidArray[guidCount];
            }

            m_guid = new Guid(finalGuid);

            if (m_cbNames > StreamReader.BaseStream.Length)
                throw new InvalidDataException("m_cbNames");

            pdbFileNamesByteContainer =
                StreamReader.ReadBytes((int)m_cbNames);

            for (int i = 0; i < pdbFileNamesByteContainer.Length; i++)
            {
                if (pdbFileNamesByteContainer[i] == 0)
                {
                    pdbFileNamesByteContainer[i] = (byte)'\r';
                }
            }

            m_pdbFileNames =
                System.Text.Encoding.ASCII.GetString(pdbFileNamesByteContainer);

            return true;

        }

        protected override bool SaveStreamInternal()
        {
            TruncateStream();
            return true;
        }
    }
}