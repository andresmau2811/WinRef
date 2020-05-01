namespace winRef.Formularios
{
    partial class frmConfiguracionBaseDatos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfiguracionBaseDatos));
            this.BtnGuardar = new System.Windows.Forms.Button();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbTipo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.LblUsuario = new System.Windows.Forms.Label();
            this.LblPass = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.LblBD = new System.Windows.Forms.Label();
            this.txtBaseDatos = new System.Windows.Forms.TextBox();
            this.LblServidor = new System.Windows.Forms.Label();
            this.btnProbar = new System.Windows.Forms.Button();
            this.lblEstado = new System.Windows.Forms.Label();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnGuardar
            // 
            this.BtnGuardar.BackColor = System.Drawing.Color.Transparent;
            this.BtnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnGuardar.ForeColor = System.Drawing.Color.SteelBlue;
            this.BtnGuardar.Location = new System.Drawing.Point(294, 208);
            this.BtnGuardar.Name = "BtnGuardar";
            this.BtnGuardar.Size = new System.Drawing.Size(81, 22);
            this.BtnGuardar.TabIndex = 12;
            this.BtnGuardar.Text = "&Guardar";
            this.BtnGuardar.UseVisualStyleBackColor = false;
            this.BtnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.cmbTipo);
            this.GroupBox3.Controls.Add(this.label3);
            this.GroupBox3.Controls.Add(this.txtServidor);
            this.GroupBox3.Controls.Add(this.txtUsuario);
            this.GroupBox3.Controls.Add(this.LblUsuario);
            this.GroupBox3.Controls.Add(this.LblPass);
            this.GroupBox3.Controls.Add(this.txtClave);
            this.GroupBox3.Controls.Add(this.LblBD);
            this.GroupBox3.Controls.Add(this.txtBaseDatos);
            this.GroupBox3.Controls.Add(this.LblServidor);
            this.GroupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox3.ForeColor = System.Drawing.Color.SteelBlue;
            this.GroupBox3.Location = new System.Drawing.Point(11, 9);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(363, 179);
            this.GroupBox3.TabIndex = 11;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Conexión BD";
            // 
            // cmbTipo
            // 
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.FormattingEnabled = true;
            this.cmbTipo.Items.AddRange(new object[] {
            "RECAUDOS",
            "CREDIPOLIZA"});
            this.cmbTipo.Location = new System.Drawing.Point(87, 17);
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Size = new System.Drawing.Size(270, 21);
            this.cmbTipo.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(51, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Tipo:";
            // 
            // txtServidor
            // 
            this.txtServidor.Location = new System.Drawing.Point(87, 123);
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(270, 20);
            this.txtServidor.TabIndex = 19;
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(87, 44);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(270, 20);
            this.txtUsuario.TabIndex = 0;
            // 
            // LblUsuario
            // 
            this.LblUsuario.AutoSize = true;
            this.LblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblUsuario.Location = new System.Drawing.Point(36, 48);
            this.LblUsuario.Name = "LblUsuario";
            this.LblUsuario.Size = new System.Drawing.Size(46, 13);
            this.LblUsuario.TabIndex = 12;
            this.LblUsuario.Text = "Usuario:";
            // 
            // LblPass
            // 
            this.LblPass.AutoSize = true;
            this.LblPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblPass.Location = new System.Drawing.Point(45, 71);
            this.LblPass.Name = "LblPass";
            this.LblPass.Size = new System.Drawing.Size(37, 13);
            this.LblPass.TabIndex = 14;
            this.LblPass.Text = "Clave:";
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(87, 67);
            this.txtClave.Name = "txtClave";
            this.txtClave.Size = new System.Drawing.Size(270, 20);
            this.txtClave.TabIndex = 1;
            this.txtClave.UseSystemPasswordChar = true;
            // 
            // LblBD
            // 
            this.LblBD.AutoSize = true;
            this.LblBD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblBD.Location = new System.Drawing.Point(4, 97);
            this.LblBD.Name = "LblBD";
            this.LblBD.Size = new System.Drawing.Size(78, 13);
            this.LblBD.TabIndex = 16;
            this.LblBD.Text = "Base de datos:";
            // 
            // txtBaseDatos
            // 
            this.txtBaseDatos.Location = new System.Drawing.Point(87, 93);
            this.txtBaseDatos.Name = "txtBaseDatos";
            this.txtBaseDatos.Size = new System.Drawing.Size(270, 20);
            this.txtBaseDatos.TabIndex = 2;
            // 
            // LblServidor
            // 
            this.LblServidor.AutoSize = true;
            this.LblServidor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblServidor.Location = new System.Drawing.Point(33, 123);
            this.LblServidor.Name = "LblServidor";
            this.LblServidor.Size = new System.Drawing.Size(49, 13);
            this.LblServidor.TabIndex = 18;
            this.LblServidor.Text = "Servidor:";
            // 
            // btnProbar
            // 
            this.btnProbar.BackColor = System.Drawing.Color.Transparent;
            this.btnProbar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProbar.ForeColor = System.Drawing.Color.SteelBlue;
            this.btnProbar.Location = new System.Drawing.Point(12, 208);
            this.btnProbar.Name = "btnProbar";
            this.btnProbar.Size = new System.Drawing.Size(81, 22);
            this.btnProbar.TabIndex = 13;
            this.btnProbar.Text = "&Probar";
            this.btnProbar.UseVisualStyleBackColor = false;
            this.btnProbar.Click += new System.EventHandler(this.btnProbar_Click);
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstado.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblEstado.Location = new System.Drawing.Point(171, 213);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(0, 13);
            this.lblEstado.TabIndex = 20;
            // 
            // frmConfiguracionBaseDatos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(384, 237);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.btnProbar);
            this.Controls.Add(this.BtnGuardar);
            this.Controls.Add(this.GroupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmConfiguracionBaseDatos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de Base de Datos";
            this.Load += new System.EventHandler(this.frmConfiguracionBaseDatos_Load);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button BtnGuardar;
        internal System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.TextBox txtServidor;
        internal System.Windows.Forms.TextBox txtUsuario;
        internal System.Windows.Forms.Label LblUsuario;
        internal System.Windows.Forms.Label LblPass;
        internal System.Windows.Forms.TextBox txtClave;
        internal System.Windows.Forms.Label LblServidor;
        internal System.Windows.Forms.Label LblBD;
        internal System.Windows.Forms.TextBox txtBaseDatos;
        internal System.Windows.Forms.Button btnProbar;
        internal System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cmbTipo;
        internal System.Windows.Forms.Label label3;
    }
}