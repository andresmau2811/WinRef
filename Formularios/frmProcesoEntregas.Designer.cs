namespace winRef.Formularios
{
    partial class frmProcesoEntregas
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProcesoEntregas));
            this.bwkProcesamientoEntregas = new System.ComponentModel.BackgroundWorker();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnBaseDatos = new System.Windows.Forms.ToolStripButton();
            this.txtLogProcesos = new System.Windows.Forms.RichTextBox();
            this.tmrIniciar = new System.Windows.Forms.Timer(this.components);
            this.tmrTiempo = new System.Windows.Forms.Timer(this.components);
            this.bwkGenEcollect = new System.ComponentModel.BackgroundWorker();
            this.bwkConEcollect = new System.ComponentModel.BackgroundWorker();
            this.bwkSMS = new System.ComponentModel.BackgroundWorker();
            this.bwkMail = new System.ComponentModel.BackgroundWorker();
            this.bwkSeteo = new System.ComponentModel.BackgroundWorker();
            this.btnActivar = new System.Windows.Forms.Button();
            this.btnConciliacion = new System.Windows.Forms.Button();
            this.btnDetener = new System.Windows.Forms.Button();
            this.txtLogBoton = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMailInt = new System.Windows.Forms.RichTextBox();
            this.chkinter = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkMailRechazos = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMailRech = new System.Windows.Forms.RichTextBox();
            this.chkSeteo = new System.Windows.Forms.CheckBox();
            this.chkEmail = new System.Windows.Forms.CheckBox();
            this.chkSms = new System.Windows.Forms.CheckBox();
            this.chkConPse = new System.Windows.Forms.CheckBox();
            this.chkgenPse = new System.Windows.Forms.CheckBox();
            this.txtSeteo = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMail = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSMS = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEcollectCon = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEcollectUrl = new System.Windows.Forms.RichTextBox();
            this.tmrConcil = new System.Windows.Forms.Timer(this.components);
            this.bkwMailRechazos = new System.ComponentModel.BackgroundWorker();
            this.bkwMaillInt = new System.ComponentModel.BackgroundWorker();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bwkProcesamientoEntregas
            // 
            this.bwkProcesamientoEntregas.WorkerReportsProgress = true;
            this.bwkProcesamientoEntregas.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwkProcesamientoEntregas_DoWork);
            this.bwkProcesamientoEntregas.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwkProcesamientoExtractos_ProgressChanged);
            this.bwkProcesamientoEntregas.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwkProcesamientoEntregas_RunWorkerCompleted);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBaseDatos});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(996, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnBaseDatos
            // 
            this.btnBaseDatos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBaseDatos.Image = ((System.Drawing.Image)(resources.GetObject("btnBaseDatos.Image")));
            this.btnBaseDatos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBaseDatos.Name = "btnBaseDatos";
            this.btnBaseDatos.Size = new System.Drawing.Size(23, 22);
            this.btnBaseDatos.Text = "Configurar base de datos";
            this.btnBaseDatos.Click += new System.EventHandler(this.btnBaseDatos_Click);
            // 
            // txtLogProcesos
            // 
            this.txtLogProcesos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtLogProcesos.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogProcesos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.txtLogProcesos.Location = new System.Drawing.Point(203, 30);
            this.txtLogProcesos.Name = "txtLogProcesos";
            this.txtLogProcesos.Size = new System.Drawing.Size(692, 100);
            this.txtLogProcesos.TabIndex = 4;
            this.txtLogProcesos.Text = "";
            this.txtLogProcesos.TextChanged += new System.EventHandler(this.txtLogProcesos_TextChanged);
            // 
            // tmrIniciar
            // 
            this.tmrIniciar.Interval = 10000;
            // 
            // tmrTiempo
            // 
            this.tmrTiempo.Interval = 1000;
            // 
            // bwkGenEcollect
            // 
            this.bwkGenEcollect.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwkGenEcollect_DoWork);
            this.bwkGenEcollect.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwkGenEcollect_RunWorkerCompleted);
            // 
            // bwkConEcollect
            // 
            this.bwkConEcollect.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwkConEcollect_DoWork);
            this.bwkConEcollect.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwkConEcollect_RunWorkerCompleted);
            // 
            // bwkSMS
            // 
            this.bwkSMS.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwkSMS_DoWork);
            this.bwkSMS.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwkSMS_RunWorkerCompleted);
            // 
            // bwkMail
            // 
            this.bwkMail.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwkMail_DoWork);
            this.bwkMail.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwkMail_RunWorkerCompleted);
            // 
            // bwkSeteo
            // 
            this.bwkSeteo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwkSeteo_DoWork);
            this.bwkSeteo.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwkSeteo_RunWorkerCompleted);
            // 
            // btnActivar
            // 
            this.btnActivar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnActivar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActivar.Location = new System.Drawing.Point(45, 29);
            this.btnActivar.Name = "btnActivar";
            this.btnActivar.Size = new System.Drawing.Size(101, 50);
            this.btnActivar.TabIndex = 23;
            this.btnActivar.Text = "Activar Procesos";
            this.btnActivar.UseVisualStyleBackColor = false;
            this.btnActivar.Click += new System.EventHandler(this.btnActivar_Click);
            // 
            // btnConciliacion
            // 
            this.btnConciliacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btnConciliacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConciliacion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnConciliacion.Location = new System.Drawing.Point(45, 48);
            this.btnConciliacion.Name = "btnConciliacion";
            this.btnConciliacion.Size = new System.Drawing.Size(120, 61);
            this.btnConciliacion.TabIndex = 24;
            this.btnConciliacion.Text = "Conciliación Credipoliza";
            this.btnConciliacion.UseVisualStyleBackColor = false;
            this.btnConciliacion.Click += new System.EventHandler(this.btnConciliacion_Click);
            // 
            // btnDetener
            // 
            this.btnDetener.BackColor = System.Drawing.Color.Red;
            this.btnDetener.Enabled = false;
            this.btnDetener.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDetener.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDetener.Location = new System.Drawing.Point(45, 85);
            this.btnDetener.Name = "btnDetener";
            this.btnDetener.Size = new System.Drawing.Size(101, 50);
            this.btnDetener.TabIndex = 25;
            this.btnDetener.Text = "Detener Procesos";
            this.btnDetener.UseVisualStyleBackColor = false;
            this.btnDetener.Click += new System.EventHandler(this.btnDetener_Click);
            // 
            // txtLogBoton
            // 
            this.txtLogBoton.BackColor = System.Drawing.Color.Silver;
            this.txtLogBoton.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLogBoton.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtLogBoton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.txtLogBoton.Location = new System.Drawing.Point(12, 156);
            this.txtLogBoton.Name = "txtLogBoton";
            this.txtLogBoton.ReadOnly = true;
            this.txtLogBoton.Size = new System.Drawing.Size(188, 45);
            this.txtLogBoton.TabIndex = 26;
            this.txtLogBoton.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMailInt);
            this.groupBox1.Controls.Add(this.chkinter);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.chkMailRechazos);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtMailRech);
            this.groupBox1.Controls.Add(this.chkSeteo);
            this.groupBox1.Controls.Add(this.chkEmail);
            this.groupBox1.Controls.Add(this.chkSms);
            this.groupBox1.Controls.Add(this.chkConPse);
            this.groupBox1.Controls.Add(this.chkgenPse);
            this.groupBox1.Controls.Add(this.txtSeteo);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtMail);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtSMS);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtEcollectCon);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtEcollectUrl);
            this.groupBox1.Controls.Add(this.btnActivar);
            this.groupBox1.Controls.Add(this.txtLogBoton);
            this.groupBox1.Controls.Add(this.btnDetener);
            this.groupBox1.Location = new System.Drawing.Point(12, 130);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(937, 373);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Procesos Pin de Pago";
            // 
            // txtMailInt
            // 
            this.txtMailInt.BackColor = System.Drawing.SystemColors.GrayText;
            this.txtMailInt.ForeColor = System.Drawing.Color.Yellow;
            this.txtMailInt.Location = new System.Drawing.Point(598, 301);
            this.txtMailInt.Name = "txtMailInt";
            this.txtMailInt.Size = new System.Drawing.Size(315, 72);
            this.txtMailInt.TabIndex = 47;
            this.txtMailInt.Text = "";
            this.txtMailInt.TextChanged += new System.EventHandler(this.txtMailInt_TextChanged);
            // 
            // chkinter
            // 
            this.chkinter.AutoSize = true;
            this.chkinter.Checked = true;
            this.chkinter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkinter.Location = new System.Drawing.Point(853, 278);
            this.chkinter.Name = "chkinter";
            this.chkinter.Size = new System.Drawing.Size(15, 14);
            this.chkinter.TabIndex = 46;
            this.chkinter.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(595, 278);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(256, 16);
            this.label7.TabIndex = 45;
            this.label7.Text = "Generación Email de Intermediarios";
            // 
            // chkMailRechazos
            // 
            this.chkMailRechazos.AutoSize = true;
            this.chkMailRechazos.Checked = true;
            this.chkMailRechazos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMailRechazos.Location = new System.Drawing.Point(808, 194);
            this.chkMailRechazos.Name = "chkMailRechazos";
            this.chkMailRechazos.Size = new System.Drawing.Size(15, 14);
            this.chkMailRechazos.TabIndex = 44;
            this.chkMailRechazos.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(598, 192);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(204, 16);
            this.label5.TabIndex = 43;
            this.label5.Text = "Generación Email Rechazos";
            // 
            // txtMailRech
            // 
            this.txtMailRech.BackColor = System.Drawing.SystemColors.GrayText;
            this.txtMailRech.ForeColor = System.Drawing.Color.Yellow;
            this.txtMailRech.Location = new System.Drawing.Point(598, 211);
            this.txtMailRech.Name = "txtMailRech";
            this.txtMailRech.Size = new System.Drawing.Size(315, 64);
            this.txtMailRech.TabIndex = 42;
            this.txtMailRech.Text = "";
            this.txtMailRech.TextChanged += new System.EventHandler(this.txtMailRech_TextChanged);
            // 
            // chkSeteo
            // 
            this.chkSeteo.AutoSize = true;
            this.chkSeteo.Checked = true;
            this.chkSeteo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSeteo.Location = new System.Drawing.Point(411, 229);
            this.chkSeteo.Name = "chkSeteo";
            this.chkSeteo.Size = new System.Drawing.Size(15, 14);
            this.chkSeteo.TabIndex = 41;
            this.chkSeteo.UseVisualStyleBackColor = true;
            // 
            // chkEmail
            // 
            this.chkEmail.AutoSize = true;
            this.chkEmail.Checked = true;
            this.chkEmail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEmail.Location = new System.Drawing.Point(728, 96);
            this.chkEmail.Name = "chkEmail";
            this.chkEmail.Size = new System.Drawing.Size(15, 14);
            this.chkEmail.TabIndex = 40;
            this.chkEmail.UseVisualStyleBackColor = true;
            // 
            // chkSms
            // 
            this.chkSms.AutoSize = true;
            this.chkSms.Checked = true;
            this.chkSms.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSms.Location = new System.Drawing.Point(725, 8);
            this.chkSms.Name = "chkSms";
            this.chkSms.Size = new System.Drawing.Size(15, 14);
            this.chkSms.TabIndex = 39;
            this.chkSms.UseVisualStyleBackColor = true;
            // 
            // chkConPse
            // 
            this.chkConPse.AutoSize = true;
            this.chkConPse.Checked = true;
            this.chkConPse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkConPse.Location = new System.Drawing.Point(416, 119);
            this.chkConPse.Name = "chkConPse";
            this.chkConPse.Size = new System.Drawing.Size(15, 14);
            this.chkConPse.TabIndex = 38;
            this.chkConPse.UseVisualStyleBackColor = true;
            // 
            // chkgenPse
            // 
            this.chkgenPse.AutoSize = true;
            this.chkgenPse.Checked = true;
            this.chkgenPse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkgenPse.Location = new System.Drawing.Point(411, 8);
            this.chkgenPse.Name = "chkgenPse";
            this.chkgenPse.Size = new System.Drawing.Size(15, 14);
            this.chkgenPse.TabIndex = 37;
            this.chkgenPse.UseVisualStyleBackColor = true;
            // 
            // txtSeteo
            // 
            this.txtSeteo.BackColor = System.Drawing.SystemColors.GrayText;
            this.txtSeteo.ForeColor = System.Drawing.Color.Yellow;
            this.txtSeteo.Location = new System.Drawing.Point(222, 246);
            this.txtSeteo.Name = "txtSeteo";
            this.txtSeteo.Size = new System.Drawing.Size(344, 76);
            this.txtSeteo.TabIndex = 36;
            this.txtSeteo.Text = "";
            this.txtSeteo.TextChanged += new System.EventHandler(this.txtSeteo_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(219, 227);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(186, 16);
            this.label6.TabIndex = 35;
            this.label6.Text = "Seteos Pendientes Bizagi";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(595, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 16);
            this.label4.TabIndex = 34;
            this.label4.Text = "Generación Email";
            // 
            // txtMail
            // 
            this.txtMail.BackColor = System.Drawing.SystemColors.GrayText;
            this.txtMail.ForeColor = System.Drawing.Color.Yellow;
            this.txtMail.Location = new System.Drawing.Point(598, 116);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(315, 72);
            this.txtMail.TabIndex = 33;
            this.txtMail.Text = "";
            this.txtMail.TextChanged += new System.EventHandler(this.txtMail_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(595, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 16);
            this.label3.TabIndex = 32;
            this.label3.Text = "Generación SMS";
            // 
            // txtSMS
            // 
            this.txtSMS.BackColor = System.Drawing.SystemColors.GrayText;
            this.txtSMS.ForeColor = System.Drawing.Color.Yellow;
            this.txtSMS.Location = new System.Drawing.Point(598, 24);
            this.txtSMS.Name = "txtSMS";
            this.txtSMS.Size = new System.Drawing.Size(312, 70);
            this.txtSMS.TabIndex = 31;
            this.txtSMS.Text = "";
            this.txtSMS.TextChanged += new System.EventHandler(this.txtSMS_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(218, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "Conciliación PSE E-collect";
            // 
            // txtEcollectCon
            // 
            this.txtEcollectCon.BackColor = System.Drawing.SystemColors.GrayText;
            this.txtEcollectCon.ForeColor = System.Drawing.Color.Yellow;
            this.txtEcollectCon.Location = new System.Drawing.Point(221, 139);
            this.txtEcollectCon.Name = "txtEcollectCon";
            this.txtEcollectCon.Size = new System.Drawing.Size(344, 74);
            this.txtEcollectCon.TabIndex = 29;
            this.txtEcollectCon.Text = "";
            this.txtEcollectCon.TextChanged += new System.EventHandler(this.txtEcollectCon_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(218, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 16);
            this.label1.TabIndex = 28;
            this.label1.Text = "Generación PSE E-collect";
            // 
            // txtEcollectUrl
            // 
            this.txtEcollectUrl.BackColor = System.Drawing.SystemColors.GrayText;
            this.txtEcollectUrl.ForeColor = System.Drawing.Color.Yellow;
            this.txtEcollectUrl.Location = new System.Drawing.Point(221, 24);
            this.txtEcollectUrl.Name = "txtEcollectUrl";
            this.txtEcollectUrl.Size = new System.Drawing.Size(347, 89);
            this.txtEcollectUrl.TabIndex = 27;
            this.txtEcollectUrl.Text = "";
            this.txtEcollectUrl.TextChanged += new System.EventHandler(this.txtEcollectUrl_TextChanged);
            // 
            // tmrConcil
            // 
            this.tmrConcil.Enabled = true;
            this.tmrConcil.Interval = 60000;
            this.tmrConcil.Tick += new System.EventHandler(this.tmrConcil_Tick);
            // 
            // bkwMailRechazos
            // 
            this.bkwMailRechazos.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bkwMailRechazos_DoWork);
            this.bkwMailRechazos.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bkwMailRechazos_RunWorkerCompleted);
            // 
            // bkwMaillInt
            // 
            this.bkwMaillInt.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bkwMaillInt_DoWork);
            this.bkwMaillInt.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bkwMailIntermediario_RunWorkerCompleted);
            // 
            // frmProcesoEntregas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(996, 515);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnConciliacion);
            this.Controls.Add(this.txtLogProcesos);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmProcesoEntregas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Procesamiento  Pagos Referenciados --";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProcesoEntregas_FormClosing);
            this.Load += new System.EventHandler(this.frmProcesoEntregas_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker bwkProcesamientoEntregas;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnBaseDatos;
        private System.Windows.Forms.RichTextBox txtLogProcesos;
        private System.Windows.Forms.Timer tmrIniciar;
        private System.Windows.Forms.Timer tmrTiempo;
        private System.ComponentModel.BackgroundWorker bwkGenEcollect;
        private System.ComponentModel.BackgroundWorker bwkConEcollect;
        private System.ComponentModel.BackgroundWorker bwkSMS;
        private System.ComponentModel.BackgroundWorker bwkMail;
        private System.ComponentModel.BackgroundWorker bwkSeteo;
        private System.Windows.Forms.Button btnActivar;
        private System.Windows.Forms.Button btnConciliacion;
        private System.Windows.Forms.Button btnDetener;
        private System.Windows.Forms.RichTextBox txtLogBoton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox txtSeteo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox txtMail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox txtSMS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtEcollectCon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtEcollectUrl;
        private System.Windows.Forms.CheckBox chkSeteo;
        private System.Windows.Forms.CheckBox chkEmail;
        private System.Windows.Forms.CheckBox chkSms;
        private System.Windows.Forms.CheckBox chkConPse;
        private System.Windows.Forms.CheckBox chkgenPse;
        private System.Windows.Forms.Timer tmrConcil;
        private System.Windows.Forms.CheckBox chkMailRechazos;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox txtMailRech;
        private System.ComponentModel.BackgroundWorker bkwMailRechazos;
        private System.Windows.Forms.RichTextBox txtMailInt;
        private System.Windows.Forms.CheckBox chkinter;
        private System.Windows.Forms.Label label7;
        private System.ComponentModel.BackgroundWorker bkwMaillInt;
    }
}