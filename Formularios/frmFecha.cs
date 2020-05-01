using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winEntregas.Formularios
{


    public partial class frmFecha : Form
    {
    public string fechaInicio { get; set; }
        public frmFecha()
        {
            InitializeComponent();
        }

        private void frmFecha_Load(object sender, EventArgs e)
        {
            this.lblFecha.Text = DateTime.Now.ToString("yyyyMMdd");
        }

        private void frmFecha_FormClosed(object sender, FormClosedEventArgs e)
        {
            fechaInicio = lblFecha.Text;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            this.lblFecha.Text = dtpFecha.Value.ToString("yyyyMMdd");
        }
    }
}
