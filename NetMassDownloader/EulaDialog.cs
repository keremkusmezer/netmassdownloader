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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DownloadLibrary.Classes.Eula;


namespace NetMassDownloader
{
    /// <summary>
    /// Handles showing the EULA to the user.
    /// </summary>
    internal partial class EulaDialog : Form
    {
        /// <summary>
        /// Constructs a <see cref="EulaDialog"/> class.
        /// </summary>
        /// <param name="contents">
        /// The <see cref="EulaContents"/> that contains all the pieces for the
        /// dialog.
        /// </param>
        public EulaDialog ( EulaContents contents )
        {
            InitializeComponent ( );

            Text = contents.TitleKey;
            txtEula.Text = contents.EulaText;
            lblReleaseDetails.Text = contents.ButtonDescriptionKey;
            btnAccept.Text = contents.AcceptTextKey;
            btnDecline.Text = contents.DeclineTextKey;

        }

    }
}
