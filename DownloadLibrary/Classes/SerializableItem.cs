using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace DownloadLibrary.Classes {

[CLSCompliant(true)]
public abstract class SerializableItem {

    public static object Deserialize( Type type, string data ) {
        try {
            XmlSerializer xmlSerialier = new XmlSerializer(type);
            return xmlSerialier.Deserialize(new XmlTextReader(new StringReader(data)));
        }
        catch( Exception exc ) {
            Debug.WriteLine( exc.ToString() );
            return null;
        }
    }

    public string Serialize() {
        StringBuilder sb = new StringBuilder();
        XmlTextWriter xmlWriter = new XmlTextWriter( new XmlStringWriter(sb, CultureInfo.InvariantCulture) );
        xmlWriter.Formatting = Formatting.Indented;
        xmlWriter.Indentation = 2;
        xmlWriter.IndentChar = ' ';
        
        XmlSerializer xmlSerialier = new XmlSerializer( base.GetType() );
        xmlSerialier.Serialize(xmlWriter, this);
        return sb.ToString();
    }

    public override string ToString() {
        return this.Serialize();
    }

    #region Inner class: XmlStringWriter
    private class XmlStringWriter : StringWriter {
        public XmlStringWriter( StringBuilder sb, IFormatProvider provider ) : base( sb, provider ) {}

        public override Encoding Encoding {
            get {
                return Encoding.UTF8;
            }
        }
    }
    #endregion

} // EndClass
} // namespace
