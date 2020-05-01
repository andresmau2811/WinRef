using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dllRecaudos;
using System.Configuration;

namespace winRef.Formularios
{
    public partial class frmConfiguracionBaseDatos : Form
    {
        public frmConfiguracionBaseDatos()
        {
            InitializeComponent();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                clsGeneral cifrar = new clsGeneral();
                string cadenaConexion = cifrar.cifrarDatos(generaCadenaConexion());
                try
                {
                    var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var settings = configFile.AppSettings.Settings;
                    String origen = "";
                    if(cmbTipo.Text.Trim().Equals("RECAUDOS"))
                    {
                        origen = "origen1";
                    }
                    else
                    {
                        origen = "origen2";
                    }

                    if (settings[origen] == null)
                    {
                        settings.Add(origen, cadenaConexion);
                    }
                    else
                    {
                        settings[origen].Value = cadenaConexion;
                    }
                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                    MessageBox.Show("Conexión configurada, la aplicación se cerrara para aplicar los cambios.");
                    Application.Restart();
                }
                catch (ConfigurationErrorsException)
                {
                    MessageBox.Show("Error escribiendo la configuración");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Configuración de Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }
        private string generaCadenaConexion()
        {
            if (this.cmbTipo.Text.Trim().Length == 0)
            {
                throw new Exception("Falta el parámetro Tipo de cadena de conexión");
            }
            if (this.txtServidor.Text.Trim().Length == 0)
            {
                throw new Exception("Falta el parámetro Servidor");
            }
            if (this.txtBaseDatos.Text.Trim().Length == 0)
            {
                throw new Exception("Falta el nombre de la base de datos");
            }
            if (this.txtUsuario.Text.Trim().Length == 0)
            {
                throw new Exception("Falta el usuario de base de datos");
            }
            if (this.txtClave.Text.Trim().Length == 0)
            {
                throw new Exception("Falta la clave de conexión a la base de datos");
            }
            return "Data source = " + this.txtServidor.Text +
                   ";initial catalog = " + this.txtBaseDatos.Text +
                   ";user id = " + this.txtUsuario.Text +
                   ";password = " + this.txtClave.Text +
                   ";persist security info = False;" +
                   "packet size = 4096;" +
                   "Connect Timeout = 1800;" +
                   "application name =" + Application.ProductName;
        }

        private void frmConfiguracionBaseDatos_Load(object sender, EventArgs e)
        {

        }

        private void btnProbar_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblEstado.Text = "Conectando ...";
                this.lblEstado.Refresh();
                using (clsDatos dt = new clsDatos(generaCadenaConexion()))
                {
                    MessageBox.Show("Estado de la conexión: " + dt.estadoConexion());
                }
                this.lblEstado.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
