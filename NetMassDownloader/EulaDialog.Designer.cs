namespace NetMassDownloader
{
   partial class EulaDialog
   {
       /// <summary>
       /// Required designer variable.
       /// </summary>
       private System.ComponentModel.IContainer components = null;

       /// <summary>
       /// Clean up any resources being used.
       /// </summary>
       /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
       protected override void Dispose(bool disposing)
       {
           if (disposing && (components != null))
           {
               components.Dispose();
           }
           base.Dispose(disposing);
       }

       #region Windows Form Designer generated code

       /// <summary>
       /// Required method for Designer support - do not modify
       /// the contents of this method with the code editor.
       /// </summary>
       private void InitializeComponent()
       {
           this.txtEula = new System.Windows.Forms.TextBox();
           this.lblReleaseDetails = new System.Windows.Forms.Label();
           this.btnAccept = new System.Windows.Forms.Button();
           this.btnDecline = new System.Windows.Forms.Button();
           this.SuspendLayout();
           // 
           // txtEula
           // 
           this.txtEula.Location = new System.Drawing.Point(7, 6);
           this.txtEula.Multiline = true;
           this.txtEula.Name = "txtEula";
           this.txtEula.ReadOnly = true;
           this.txtEula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
           this.txtEula.Size = new System.Drawing.Size(360, 299);
           this.txtEula.TabIndex = 2;
           // 
           // lblReleaseDetails
           // 
           this.lblReleaseDetails.Location = new System.Drawing.Point(5, 308);
           this.lblReleaseDetails.Name = "lblReleaseDetails";
           this.lblReleaseDetails.Size = new System.Drawing.Size(362, 52);
           this.lblReleaseDetails.TabIndex = 1;
           this.lblReleaseDetails.Text = "Please read carefully and understand the licence agreement above. If you\r\nwant to" +
               " accept the licence agreement, please click the \"Accept\" button.\r\n\r\n\r\n";
           // 
           // btnAccept
           // 
           this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.Yes;
           this.btnAccept.Location = new System.Drawing.Point(200, 354);
           this.btnAccept.Name = "btnAccept";
           this.btnAccept.Size = new System.Drawing.Size(75, 23);
           this.btnAccept.TabIndex = 0;
           this.btnAccept.Text = "Accept";
           this.btnAccept.UseVisualStyleBackColor = true;
           // 
           // btnDecline
           // 
           this.btnDecline.DialogResult = System.Windows.Forms.DialogResult.No;
           this.btnDecline.Location = new System.Drawing.Point(292, 354);
           this.btnDecline.Name = "btnDecline";
           this.btnDecline.Size = new System.Drawing.Size(75, 23);
           this.btnDecline.TabIndex = 1;
           this.btnDecline.Text = "Decline";
           this.btnDecline.UseVisualStyleBackColor = true;
           // 
           // EulaDialog
           // 
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.ClientSize = new System.Drawing.Size(379, 389);
           this.Controls.Add(this.btnDecline);
           this.Controls.Add(this.btnAccept);
           this.Controls.Add(this.lblReleaseDetails);
           this.Controls.Add(this.txtEula);
           this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
           this.MaximizeBox = false;
           this.MinimizeBox = false;
           this.Name = "EulaDialog";
           this.Text = "End User Licence Agreement";
           this.ResumeLayout(false);
           this.PerformLayout();

       }

       #endregion

       private System.Windows.Forms.TextBox txtEula;
       private System.Windows.Forms.Label lblReleaseDetails;
       private System.Windows.Forms.Button btnAccept;
       private System.Windows.Forms.Button btnDecline;
   }
}