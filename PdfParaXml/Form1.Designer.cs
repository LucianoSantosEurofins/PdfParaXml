
namespace PdfParaXml
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_LocationMendelics = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_LocationSollutio = new System.Windows.Forms.TextBox();
            this.btn_SelectFolderMendelics = new System.Windows.Forms.Button();
            this.btn_SelectFolderSollutio = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_XMLLocation = new System.Windows.Forms.TextBox();
            this.btn_SelectXMLFolder = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.FlatAppearance.BorderSize = 2;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox_LocationMendelics
            // 
            resources.ApplyResources(this.textBox_LocationMendelics, "textBox_LocationMendelics");
            this.textBox_LocationMendelics.Name = "textBox_LocationMendelics";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBox_LocationSollutio
            // 
            resources.ApplyResources(this.textBox_LocationSollutio, "textBox_LocationSollutio");
            this.textBox_LocationSollutio.Name = "textBox_LocationSollutio";
            // 
            // btn_SelectFolderMendelics
            // 
            this.btn_SelectFolderMendelics.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btn_SelectFolderMendelics, "btn_SelectFolderMendelics");
            this.btn_SelectFolderMendelics.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SelectFolderMendelics.FlatAppearance.BorderSize = 0;
            this.btn_SelectFolderMendelics.Name = "btn_SelectFolderMendelics";
            this.btn_SelectFolderMendelics.UseVisualStyleBackColor = false;
            this.btn_SelectFolderMendelics.Click += new System.EventHandler(this.btn_SelectFolderMendelics_Click);
            // 
            // btn_SelectFolderSollutio
            // 
            this.btn_SelectFolderSollutio.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btn_SelectFolderSollutio, "btn_SelectFolderSollutio");
            this.btn_SelectFolderSollutio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SelectFolderSollutio.FlatAppearance.BorderSize = 0;
            this.btn_SelectFolderSollutio.Name = "btn_SelectFolderSollutio";
            this.btn_SelectFolderSollutio.UseVisualStyleBackColor = false;
            this.btn_SelectFolderSollutio.Click += new System.EventHandler(this.btn_SelectFolderSollutio_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBox_XMLLocation
            // 
            resources.ApplyResources(this.textBox_XMLLocation, "textBox_XMLLocation");
            this.textBox_XMLLocation.Name = "textBox_XMLLocation";
            // 
            // btn_SelectXMLFolder
            // 
            this.btn_SelectXMLFolder.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btn_SelectXMLFolder, "btn_SelectXMLFolder");
            this.btn_SelectXMLFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_SelectXMLFolder.FlatAppearance.BorderSize = 0;
            this.btn_SelectXMLFolder.Name = "btn_SelectXMLFolder";
            this.btn_SelectXMLFolder.UseVisualStyleBackColor = false;
            this.btn_SelectXMLFolder.Click += new System.EventHandler(this.btn_SelectXMLFolder_Click);
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_SelectXMLFolder);
            this.Controls.Add(this.btn_SelectFolderSollutio);
            this.Controls.Add(this.btn_SelectFolderMendelics);
            this.Controls.Add(this.textBox_XMLLocation);
            this.Controls.Add(this.textBox_LocationSollutio);
            this.Controls.Add(this.textBox_LocationMendelics);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_LocationMendelics;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_LocationSollutio;
        private System.Windows.Forms.Button btn_SelectFolderMendelics;
        private System.Windows.Forms.Button btn_SelectFolderSollutio;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_XMLLocation;
        private System.Windows.Forms.Button btn_SelectXMLFolder;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button2;
    }
}

