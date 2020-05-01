using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dllRecaudos;
using System.IO;
using System.Net;
using winRef.Clases;
using System.Xml.Serialization;
using System.Xml;

namespace winRef.Formularios
{
    public partial class frmProcesoEntregas : Form
    {
        string cConexionRecaudos = string.Empty;
       string cConexionCredipolizaSql = string.Empty;
        clsGeneral clsgeneral = new clsGeneral();
        bool bolProcesos = false;
        int horas = 0;

        public frmProcesoEntregas()
        {
            InitializeComponent();
        }
        private void frmProcesoEntregas_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.Text = "winRef -- " + Application.ProductVersion.ToString();        

            //this.txtProceso.Text = "Detenido.";
            
        }

        private bool iniciaProceso()
        {
            var appSettings = ConfigurationManager.AppSettings;
            cConexionRecaudos = appSettings["origen1"];
            cConexionCredipolizaSql = appSettings["origen2"];
            if (cConexionRecaudos == null || cConexionCredipolizaSql == null)
            {
                MessageBox.Show("La conexión a la base de datos no esta configurada");
                return false;
            }
            else
            {
                try
                {
                    cConexionRecaudos = clsgeneral.descifradoDatos(cConexionRecaudos);
                    cConexionCredipolizaSql = clsgeneral.descifradoDatos(cConexionCredipolizaSql);

                    DataTable dtConf = new DataTable();
                    using (clsDatos dt = new clsDatos(cConexionRecaudos))
                    {
                        dt.nuevoParametro("@id_conf", 1, ParameterDirection.Input);
                        dtConf = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIGURACION_ENVIOS").Tables[0];
                    }
                    if (dtConf.Rows.Count > 0)
                    {
                        horas = int.Parse(dtConf.Rows[0]["hora_concil"].ToString());
                    }
                    return true;
                }
                catch(Exception e)
                {
                    MessageBox.Show("La conexión a la base de datos es erronea, se debe ajustar: " + e.Message);
                    return false;
                }
                
            }
        }        

        private void validacionOpenCard(object sender, DoWorkEventArgs e)  
        {
            try
            {
                DataTable dtValidacionOpenCard = new DataTable();
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dtValidacionOpenCard = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIRMA_APLICACION_OPENCARD").Tables[0];
                }

                if (dtValidacionOpenCard.Rows.Count > 0)
                {
                    //clsReferenciador transferencia = new clsReferenciador(cConexionReferenciador);
                    Int32 cont = 0;
                    foreach (DataRow fila in dtValidacionOpenCard.Rows)
                    {
                        //Consulta Opencard
                        //--------------------------------------------------------
                        DataTable dtconsultaOpenCard = new DataTable();
                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                        {
                            dt.nuevoParametro("@nume_doc", fila["nume_doc"], ParameterDirection.Input);
                            dt.nuevoParametro("@fech_mov", fila["fech_mov"], ParameterDirection.Input);
                            dtconsultaOpenCard = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_OPENCARD").Tables[0];
                        }
                        //--------------------------------------------------------
                        if (dtconsultaOpenCard.Rows.Count > 0)
                        {
                            DataTable dtDetalleOpenCard = new DataTable();
                            using (clsDatos dt = new clsDatos(cConexionRecaudos))
                            {
                                dt.nuevoParametro("@nume_doc", dtconsultaOpenCard.Rows[0]["cedula"], ParameterDirection.Input);
                                dt.nuevoParametro("@nume_cre", dtconsultaOpenCard.Rows[0]["credito_OC"], ParameterDirection.Input);
                                dt.nuevoParametro("@fech_mov", dtconsultaOpenCard.Rows[0]["fecha_recaudo"], ParameterDirection.Input);
                                dt.nuevoParametro("@valo_rec", dtconsultaOpenCard.Rows[0]["valor_recaudo"], ParameterDirection.Input);
                                dtDetalleOpenCard = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_MOVIMIENTO_OPENCARD").Tables[0];
                            }
                            if (dtDetalleOpenCard.Rows.Count > 0)
                            {
                                cambiarEstadoDetalle(dtDetalleOpenCard.Rows[0]["id_prc_det"].ToString(), 8);
                            }
                        }
                        else
                        {
                            using (clsDatos dt = new clsDatos(cConexionRecaudos))
                            {
                                dt.nuevoParametro("@id_prc_det", fila["id_prc_det"], ParameterDirection.Input);
                                dt.ejecutar(CommandType.StoredProcedure, "REGISTRA_VERIFICACION_FALLIDA_OPENCARD");
                            }
                        }
                        cont++;
                    }
                }
                this.txtLogProcesos.Text += "Confirmación de pagos aplicados en OpenCard finalizado." + " -- Fecha: " + DateTime.Now.ToLongDateString() + Environment.NewLine;
            }
            catch (Exception ex)
            {
                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, ex.Message, Application.ProductName, "validacionOpenCard", ex.TargetSite.Name, Application.ProductVersion, "");
                MessageBox.Show("Error en el proceso de entregas:" + Environment.NewLine +
                                ex.Message, "Proceso de entregas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void validacionTuCredito(object sender, DoWorkEventArgs e)
        {
            try
            {
                DataTable dtValidacionTuCredito = new DataTable();
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dtValidacionTuCredito = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_REGISTROS_PARA_TUCREDITO").Tables[0];//Consulta registros de detalle en estado 1
                }

                if (dtValidacionTuCredito.Rows.Count > 0)
                {
                    Int32 cont = 0;
                    foreach (DataRow fila in dtValidacionTuCredito.Rows)
                    {
                        //Consulta SIIF
                        //--------------------------------------------------------
                        DataTable dtconsultaSIIF = new DataTable();
                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                        {
                            dt.nuevoParametro("@NIT", DBNull.Value, ParameterDirection.Input);
                            dt.nuevoParametro("@CODIGO_EMPRESA", DBNull.Value, ParameterDirection.Input);
                            dt.nuevoParametro("@NRO_CREDITO", fila["nume_ref_pri"], ParameterDirection.Input);
                            dt.nuevoParametro("@NRO_IDENTIFICACION", DBNull.Value, ParameterDirection.Input);
                            dtconsultaSIIF = dt.ejecutar(CommandType.StoredProcedure, "SP_CarteraLibranzaCredivaloresSIIF").Tables[0];
                        }
                        //--------------------------------------------------------
                        if (dtconsultaSIIF.Rows.Count > 0)
                        {
                            if (!dtconsultaSIIF.Rows[0]["ESTADO_CREDITO"].ToString().Equals("CANCELADO"))
                            {
                                if (Convert.ToDouble(dtconsultaSIIF.Rows[0]["VALOR_CUOTA"].ToString()) >= Convert.ToDouble(fila["valo_rec"].ToString()))
                                {
                                    cambiarEstadoDetalleTuCredito(fila["id_prc_det"].ToString(), 2);// marca los registros de detalle en 2 para que se asignen para conciliación
                                }
                            }
                        }
                        //else
                        //{
                        //    using (clsDatos dt = new clsDatos(cConexionRecaudos))
                        //    {
                        //        dt.nuevoParametro("@id_prc_det", fila["id_prc_det"], ParameterDirection.Input);
                        //        dt.ejecutar(CommandType.StoredProcedure, "REGISTRA_VERIFICACION_FALLIDA_OPENCARD");
                        //    }
                        //}
                        cont++;
                    }
                }
                asignarRegistrosConciliacion();//Deja en 9 los registros para conciliar y asigna el usuario de conciliacion
                this.txtLogProcesos.Text += "Confirmación de pagos validados de TuCredito finalizado." + " -- Fecha: " + DateTime.Now.ToLongDateString() + Environment.NewLine;
            }
            catch (Exception ex)
            {
                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, ex.Message, Application.ProductName, "validacionTuCredito", ex.TargetSite.Name, Application.ProductVersion, "");
                MessageBox.Show("Error en el proceso de entregas:" + Environment.NewLine +
                                ex.Message, "Proceso de entregas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void asignarRegistrosConciliacion()
        {
            try
            {
                //Asignación de registros para TUCREDITO
                //__________________________________________________________________________________________________________________________
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                    dt.ejecutar(CommandType.StoredProcedure, "BLOQUEA_MOVIMIENTOS_CONCILIACION_TUCREDITO");
                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                    {
                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, "Proceso de entregas", "asignarRegistrosConciliacion", Application.ProductVersion, "");
                    }
                }
                //__________________________________________________________________________________________________________________________
            }
            catch (Exception ex)
            {
                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, ex.Message, Application.ProductName, "Proceso de entregas", ex.TargetSite.Name, Application.ProductVersion, "");
                throw new Exception(ex.Message);
            }
        }

        public void asignarRegistrosConciliacionEmpresarial()
        {
            try
            {
                //Asignación de registros para TUCREDITO
                //__________________________________________________________________________________________________________________________
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                    dt.ejecutar(CommandType.StoredProcedure, "BLOQUEA_MOVIMIENTOS_CONCILIACION_TUCREDITO_EMPR");
                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                    {
                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, "Proceso de entregas", "asignarRegistrosConciliacion", Application.ProductVersion, "");
                    }
                }
                //__________________________________________________________________________________________________________________________
            }
            catch (Exception ex)
            {
                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, ex.Message, Application.ProductName, "Proceso de entregas", ex.TargetSite.Name, Application.ProductVersion, "");
                throw new Exception(ex.Message);
            }
        }

        private void generacionOrigen9(object sender, DoWorkEventArgs e)
        {
            DataTable dtEncabezadoEfectivo = new DataTable();
            DataTable dtDetalleEfectivo = new DataTable();
            DataTable dtEncabezadoCheque = new DataTable();
            DataTable dtDetalleCheque = new DataTable();
            DataSet dtsOrigen9 = new DataSet();
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dtsOrigen9 = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_ORIGEN9");
            }
            dtEncabezadoEfectivo = dtsOrigen9.Tables[0];
            dtDetalleEfectivo = dtsOrigen9.Tables[1];
            dtEncabezadoCheque = dtsOrigen9.Tables[2];
            dtDetalleCheque = dtsOrigen9.Tables[3];

            if (dtDetalleEfectivo.Rows.Count > 0)
            {
                string nombreArchivoEfectivo = "ArchPagosEfectivo.txt";
                StreamWriter archivoEfectivo = new StreamWriter(nombreArchivoEfectivo, false);
                string linea = "";
                for (int i = 0; i < dtEncabezadoEfectivo.Columns.Count - 1; i++)
                {
                    if (!Convert.IsDBNull(dtEncabezadoEfectivo.Rows[0][i]))
                    {
                        linea += dtEncabezadoEfectivo.Rows[0][i].ToString();
                    }
                }
                archivoEfectivo.Write(linea.PadRight(150, ' '));
                archivoEfectivo.Write(archivoEfectivo.NewLine);
                foreach (DataRow dr in dtDetalleEfectivo.Rows)
                {
                    linea = "";
                    for (int i = 0; i < dtDetalleEfectivo.Columns.Count - 2; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            linea += dr[i].ToString();
                        }
                    }
                    archivoEfectivo.Write(linea.PadRight(150, ' '));
                    archivoEfectivo.Write(archivoEfectivo.NewLine);
                }
                archivoEfectivo.Close();

                if (new FileInfo(nombreArchivoEfectivo).Exists)
                {
                    foreach (DataRow dr in dtDetalleEfectivo.Rows)
                    {
                        cambiarEstadoDetalle(dr["id_prc_det"].ToString(), 7);
                    }
                }
            }

            if (dtDetalleCheque.Rows.Count > 0)
            {
                string nombreArchivoCheque = "ArchPagosCheque.txt";
                StreamWriter archivoCheque = new StreamWriter(nombreArchivoCheque, false);
                string linea = "";
                for (int i = 0; i < dtEncabezadoCheque.Columns.Count - 1; i++)
                {
                    if (!Convert.IsDBNull(dtEncabezadoCheque.Rows[0][i]))
                    {
                        linea += dtEncabezadoCheque.Rows[0][i].ToString();
                    }
                }
                archivoCheque.Write(linea.PadRight(150, ' '));
                archivoCheque.Write(archivoCheque.NewLine);
                foreach (DataRow dr in dtDetalleCheque.Rows)
                {
                    linea = "";
                    for (int i = 0; i < dtDetalleCheque.Columns.Count - 2; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            linea += dr[i].ToString();
                        }
                    }
                    archivoCheque.Write(linea.PadRight(150, ' '));
                    archivoCheque.Write(archivoCheque.NewLine);
                }
                archivoCheque.Close();
                if (new FileInfo(nombreArchivoCheque).Exists)
                {
                    foreach (DataRow dr in dtDetalleCheque.Rows)
                    {
                        cambiarEstadoDetalle(dr["id_prc_det"].ToString(), 7);
                    }
                }
            }
            this.txtLogProcesos.Text += "Generación del Origen 9 finalizado." + " -- Fecha: " + DateTime.Now.ToLongDateString() + Environment.NewLine;
        }

        private void generacionAsobancariaLibranzas(object sender, DoWorkEventArgs e)
        {
            DataTable dtAsobancariaLib = new DataTable();
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dtAsobancariaLib = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_ASOBANCARIA_LIBRANZAS").Tables[0];
            }

            if (dtAsobancariaLib.Rows.Count > 0)
            {
                string nombreArchivo = "ASOB_CP.txt";
                StreamWriter archivo = new StreamWriter(nombreArchivo, false);
                string linea = "";
                foreach (DataRow dr in dtAsobancariaLib.Rows)
                {
                    linea = "";
                    linea += dr[0].ToString();
                    archivo.Write(linea);
                    archivo.Write(archivo.NewLine);
                }
                archivo.Close();
            }

            this.txtLogProcesos.Text += "Generación del Asobancaria finalizado." + " -- Fecha: " + DateTime.Now.ToLongDateString() + Environment.NewLine;
        }


        private void cambiarEstadoDetalle(string id_prc_det, Int32 codi_est_det)
        {
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@id_prc_det", id_prc_det, ParameterDirection.Input);
                dt.nuevoParametro("@codi_est_det", codi_est_det, ParameterDirection.Input);
                dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                dt.ejecutar(CommandType.StoredProcedure, "ACTUALIZAR_ESTADO_DETALLE_EXTRACTO");
                if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                {
                    clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "envioReferenciador", Application.ProductVersion, id_prc_det);
                }
            }
        }

        private void cambiarEstadoDetalleTuCredito(string id_prc_det, Int32 codi_est_det)
        {
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@id_prc_det", id_prc_det, ParameterDirection.Input);
                dt.nuevoParametro("@codi_est_det", codi_est_det, ParameterDirection.Input);
                dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                dt.ejecutar(CommandType.StoredProcedure, "ACTUALIZAR_ESTADO_DETALLE_EXTRACTO_TUCREDITO");
                if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                {
                    clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "envioReferenciador", Application.ProductVersion, id_prc_det);
                }
            }
        }

        private void ExtractoBancario_pasarTraza(string traza)
        {
            this.txtLogProcesos.Text += traza + Environment.NewLine;
        }


        private void btnBaseDatos_Click(object sender, EventArgs e)
        {
            frmConfiguracionBaseDatos frDB = new frmConfiguracionBaseDatos();
            frDB.ShowDialog();
        }

        private void bwkProcesamientoExtractos_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //frmConfiguracionFTP frFTP = new frmConfiguracionFTP();
            //frFTP.ShowDialog();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if ((MessageBox.Show("Se iniciara el procesamiento de transferencia de información. ¿Desea continuar?", "Recaudos", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)) == DialogResult.OK)
            {
                //if (validarHorario())
                //{
                iniciaProceso();
                //}       
            }
        }


        #region Procesos
        private void envioSMS(object sender, DoWorkEventArgs e)
        {
            int Estadosms = 0;
            string rtaSMS = "";
            string errWs = "";

            DataTable dtSMS = new DataTable();
            DataTable dtConf = new DataTable();
            clsEnviosWs claseEnvios = new clsEnviosWs();
            mensajeProcesos(this.txtSMS, "Conectando...", false);
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dtSMS = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PINES_SMS").Tables[0];
            }
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@id_conf", 1, ParameterDirection.Input);
                dtConf = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIGURACION_ENVIOS").Tables[0];
            }

            if (dtSMS != null)
            {
                if (dtSMS.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtSMS.Rows)
                    {
                        errWs = "";
                        mensajeProcesos(this.txtSMS, "Consumiendo servicio SMS PIN " + dr["Numero_Referencia"].ToString(), true);
                        rtaSMS = claseEnvios.consumirSMS(dr["Numero_Documento"].ToString(), dr["Celular"].ToString(), dr["Apellidos"].ToString(), dr["Numero_Referencia"].ToString(), dr["valor_cuota"].ToString(), dr["fecha_vencimiento"].ToString(), dr["Ecollect_url"].ToString(), int.Parse(dr["intentos_pse"].ToString()), cConexionRecaudos, dtConf, out errWs);

                        mensajeProcesos(this.txtSMS, "Respuesta SMS: " + rtaSMS, true);
                        if (rtaSMS == "true")
                        {
                            Estadosms = 1;
                        }
                        else
                        {
                            //reintentos
                            switch (int.Parse(dr["Estado_sms"].ToString()))
                            {
                                case 0:
                                    Estadosms = 95;
                                    break;
                                case 95:
                                    Estadosms = 96;
                                    break;
                                case 96:
                                    Estadosms = 97;
                                    break;
                                case 97:
                                    Estadosms = 98;
                                    break;
                                case 98:
                                    Estadosms = 99;
                                    break;
                            }
                            if (errWs != "")
                            {
                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, errWs, Application.ProductName, this.Name, "envioSMS", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                            }
                        }

                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                        {
                            dt.nuevoParametro("@Estado_sms", Estadosms, ParameterDirection.Input);
                            dt.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                            dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                            dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                            dt.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_ESTADO_SMS");
                            if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                            {
                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "envioSMS", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                            }
                        }
      
                    }


                }
            }
            Esperar(60000 * int.Parse(dtConf.Rows[0]["Time_sms"].ToString()), this.txtSMS);
            mensajeProcesos(this.txtSMS, "", false);
        }
        private void envioMail(object sender, DoWorkEventArgs e)
        {
            int Estadomail = 0;
            string rtaMail = "";
            string errWs = "";
            string errPDF = "";
            string base64 = "";
            string gs1 = "";

            DataTable dtMail = new DataTable();
            DataTable dtConf = new DataTable();
            clsEnviosWs claseEnvios = new clsEnviosWs();
            clsPDF clasePDF = new clsPDF();
            mensajeProcesos(this.txtMail, "Conectando...", false);
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {

                dtMail = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PINES_MAIL").Tables[0];
            }
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@id_conf", 1, ParameterDirection.Input);
                dtConf = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIGURACION_ENVIOS").Tables[0];
            }

            if (dtMail != null)
            {
                if (dtMail.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtMail.Rows)
                    {
                        errWs = "";
                        mensajeProcesos(this.txtMail, "Consumiendo servicio Mail PIN " + dr["Numero_Referencia"].ToString(), true);
                        //genera archivo PDF
                        errPDF = "";
                        if (dtConf.Rows.Count > 0)
                        {
                            gs1 = dtConf.Rows[0]["Gs1CodeBar"].ToString();
                        }

                        clasePDF.generarCuponAlternativo(dr, gs1, out errPDF);

                        
                        base64 = clasePDF.generarBase64(dr["Numero_Referencia"].ToString());

                        if (errPDF == "" && base64 != "")
                        {
                            //consume Ws
                            rtaMail = claseEnvios.consumirMail(dr["Numero_Documento"].ToString(), dr["Email1"].ToString(), dr["Numero_Referencia"].ToString(), dr["Numero_Referencia"].ToString() + ".pdf", base64, dr["apellidos"].ToString(), dr["Valor_cuota"].ToString(), dr["Fecha_Vencimiento_Pago"].ToString(), dr["Ecollect_url"].ToString(), int.Parse(dr["intentos_pse"].ToString()), cConexionRecaudos, dtConf, out errWs);
                        }
                        else
                        {
                            rtaMail = "Error al generar el archivo PDF = " + errPDF + " " + base64;
                        }

                        mensajeProcesos(this.txtMail, "Respuesta Mail: " + rtaMail, true);
                        if (rtaMail == "true")
                        {
                            Estadomail = 1;
                        }
                        else
                        {
							claseEnvios.registraLogSMSMail(cConexionRecaudos, 1, dtConf.Rows[0]["sender"].ToString(), dr["Email1"].ToString(), dtConf.Rows[0]["asunto"].ToString(), "", "", dtConf.Rows[0]["host_mail"].ToString(), rtaMail, "", "", "", "", "");
                            //reintentos
                            if (dr["Estado_mail"] != null)
                            {
                                Estadomail = 95;
                            }
                            else
                            {
                                switch (int.Parse(dr["Estado_mail"].ToString()))
                                {
                                    case 0:
                                        Estadomail = 95;
                                        break;
                                    case 95:
                                        Estadomail = 96;
                                        break;
                                    case 96:
                                        Estadomail = 97;
                                        break;
                                    case 97:
                                        Estadomail = 98;
                                        break;
                                    case 98:
                                        Estadomail = 99;
                                        break;
                                }
                            }

                            if (errWs != "")
                            {
                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, errWs, Application.ProductName, this.Name, "envioMail", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                            }
                        }

                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                        {
                            dt.nuevoParametro("@Estado_mail", Estadomail, ParameterDirection.Input);
                            dt.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                            dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                            dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                            dt.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_ESTADO_MAIL");
                            if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                            {
                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "envioSMS", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                            }
                        }   
                        //borra PDF
                        clasePDF.borrarCuponTemporal(dr["Numero_Referencia"].ToString());
                    }


                }
            }
            Esperar(60000 * int.Parse(dtConf.Rows[0]["Time_Email"].ToString()), this.txtMail);
            mensajeProcesos(this.txtMail, "", false);
        }

        private void envioMailRechazado(object sender, DoWorkEventArgs e)
        {
            int EstadomailRechazo = 0;
            string rtaMail = "";
            string errWs = "";
            string errPDF = "";
            string base64 = "";
            string gs1 = "";

            DataTable dtMail = new DataTable();
            DataTable dtConf = new DataTable();
            clsEnviosWs claseEnvios = new clsEnviosWs();
            clsPDF clasePDF = new clsPDF();
            mensajeProcesos(this.txtMailRech, "Conectando...", false);
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {

                dtMail = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PINES_MAIL_RECHAZO").Tables[0];
            }
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@id_conf", 1, ParameterDirection.Input);
                dtConf = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIGURACION_ENVIOS").Tables[0];
            }

            if (dtMail != null)
            {
                if (dtMail.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtMail.Rows)
                    {
                        errWs = "";
                        mensajeProcesos(this.txtMailRech, "Consumiendo servicio Mail Rechazos PSE PIN " + dr["Numero_Referencia"].ToString(), true);
                        //genera archivo PDF
                        errPDF = "";
                        
                        rtaMail = claseEnvios.consumirMailRechazoEcollect(dr["Numero_Documento"].ToString(), dr["Email1"].ToString(), dr["Numero_Referencia"].ToString(), dr["nombres"].ToString() + " " +dr["apellidos"].ToString(), dr["Valor_cuota"].ToString(), dr["Fecha_Vencimiento_Pago"].ToString(), cConexionRecaudos, dtConf, out errWs);
                        
                        mensajeProcesos(this.txtMailRech, "Respuesta Mail de Rechazo: " + rtaMail, true);
                        if (rtaMail == "true")
                        {
                            EstadomailRechazo = 1;
                        }
                        else
                        {
                            //reintentos
                            if (dr["Estado_Rechazo"] != null)
                            {
                                EstadomailRechazo = 95;
                            }
                            else
                            {
                                switch (int.Parse(dr["Estado_Rechazo"].ToString()))
                                {
                                    case 0:
                                        EstadomailRechazo = 95;
                                        break;
                                    case 95:
                                        EstadomailRechazo = 96;
                                        break;
                                    case 96:
                                        EstadomailRechazo = 97;
                                        break;
                                    case 97:
                                        EstadomailRechazo = 98;
                                        break;
                                    case 98:
                                        EstadomailRechazo = 99;
                                        break;
                                }
                            }

                            if (errWs != "")
                            {
                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, errWs, Application.ProductName, this.Name, "envioMail", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                            }
                        }

                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                        {
                            dt.nuevoParametro("@Estado_mail", EstadomailRechazo, ParameterDirection.Input);
                            dt.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                            dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                            dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                            dt.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_ESTADO_MAIL_RECHAZO");
                            if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                            {
                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "envioMailRechazado", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                            }
                        }
                        //borra PDF
                    }
                }
            }
            Esperar(60000 * int.Parse(dtConf.Rows[0]["Time_Email"].ToString()), this.txtMailRech);
            mensajeProcesos(this.txtMailRech, "", false);
        }
        private void envioMailIntermediario(object sender, DoWorkEventArgs e)
        {
            int EstadomailRechazo = 0;
            string rtaMail = "";
            string errWs = "";
            string errPDF = "";
            string base64 = "";
            string gs1 = "";

            DataTable dtMail = new DataTable();
            DataTable dtConf = new DataTable();
            DataTable dtVista = new DataTable();
            clsEnviosWs claseEnvios = new clsEnviosWs();
            clsPDF clasePDF = new clsPDF();
            mensajeProcesos(this.txtMailInt, "Conectando...", false);
            string destinatario = "";
            string nombres = "";
            try
            {
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dtMail = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PINES_MAIL_INTERMEDIARIO").Tables[0];
                }
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dt.nuevoParametro("@id_conf", 1, ParameterDirection.Input);
                    dtConf = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIGURACION_ENVIOS").Tables[0];
                }

                if (dtMail != null)
                {
                    if (dtMail.Rows.Count > 0)
                    {
                        destinatario = "";
                        foreach (DataRow dr in dtMail.Rows)
                        {
                            errWs = "";
                            mensajeProcesos(this.txtMailInt, "Consumiendo servicio Mail intermediarios PSE PIN " + dr["Numero_Referencia"].ToString(), true);
                            //genera archivo PDF
                            errPDF = "";
                            using (clsDatos dt = new clsDatos(cConexionCredipolizaSql))
                            {
                                dt.nuevoParametro("@id_caso", decimal.Parse(dr["Id_case_bzg"].ToString()), ParameterDirection.Input);
                                dtVista = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_VISTA_INTERMEDIARIO").Tables[0];
                            }

                            if (dtVista.Rows.Count > 0)
                            {
                                destinatario = dtVista.Rows[0]["Correo_Intermediario"].ToString();
                                nombres = dtVista.Rows[0]["Nombre_Intermediario"].ToString();

                                if (dtConf.Rows.Count > 0)
                                {
                                    gs1 = dtConf.Rows[0]["Gs1CodeBar"].ToString();
                                }
                                clasePDF.generarCuponAlternativo(dr, gs1, out errPDF);
                                base64 = clasePDF.generarBase64(dr["Numero_Referencia"].ToString());


                                rtaMail = claseEnvios.consumirMailIntermediario(destinatario, dr["Numero_Referencia"].ToString() + ".pdf", base64, nombres, dr["cuerpo"].ToString(), cConexionRecaudos, dtConf, out errWs);
                                mensajeProcesos(this.txtMailInt, "Respuesta Mail de Intermediario: " + rtaMail, true);
                                if (rtaMail == "true")
                                {
                                    EstadomailRechazo = 1;
                                }
                                else
                                {
                                    if (dr["estado"] != null)
                                    {
                                        EstadomailRechazo = 95;
                                    }
                                    else
                                    {
                                        switch (int.Parse(dr["estado"].ToString()))
                                        {
                                            case 0:
                                                EstadomailRechazo = 95;
                                                break;
                                            case 95:
                                                EstadomailRechazo = 96;
                                                break;
                                            case 96:
                                                EstadomailRechazo = 97;
                                                break;
                                            case 97:
                                                EstadomailRechazo = 98;
                                                break;
                                            case 98:
                                                EstadomailRechazo = 99;
                                                break;
                                        }
                                    }

                                    if (errWs != "")
                                    {
                                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, errWs, Application.ProductName, this.Name, "envioMailIntermediario", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                    }
                                }

                                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                {
                                    dt.nuevoParametro("@Estado_mail", EstadomailRechazo, ParameterDirection.Input);
                                    dt.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                                    dt.nuevoParametro("@Destinatario", destinatario, ParameterDirection.Input);
                                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                                    dt.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_ESTADO_MAIL_INTER");
                                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                                    {
                                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "envioMailIntermediario", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                    }
                                }
                                //borra PDF
                                clasePDF.borrarCuponTemporal(dr["Numero_Referencia"].ToString());
                            }
                            else
                            {
                                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                {
                                    dt.nuevoParametro("@Estado_mail", 2, ParameterDirection.Input);
                                    dt.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                                    dt.nuevoParametro("@Destinatario", "", ParameterDirection.Input);
                                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                                    dt.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_ESTADO_MAIL_INTER");
                                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                                    {
                                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@envioMailIntermediario").ToString(), Application.ProductName, this.Name, "envioMailIntermediario", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch(Exception ex)
            {
                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, ex.ToString(), Application.ProductName, this.Name, "envioMailIntermediario", Application.ProductVersion, "0");
            }

            
            Esperar(60000 * int.Parse(dtConf.Rows[0]["Time_Email"].ToString()), this.txtMailInt);
            mensajeProcesos(this.txtMailInt, "", false);
        }
        /// <summary>
        /// Inicia el servicio de PSE para un PIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generarUrlPSE(object sender, DoWorkEventArgs e)
        {
            string errWs = "";
            DataTable dtPines = new DataTable();
            DataTable dtConf = new DataTable();
            clsEnviosWs claseEnvios = new clsEnviosWs();
            clsPDF clasePDF = new clsPDF();
            DataRow drConf;
            WsEcollect.createTransactionResponseType rtaWs = new WsEcollect.createTransactionResponseType();
            string sign = "";

            try
            {

                mensajeProcesos(this.txtEcollectUrl, "Conectando...", false);
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {

                    dtPines = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PINES_URL").Tables[0];
                }
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dt.nuevoParametro("@id_conf", 1, ParameterDirection.Input);
                    dtConf = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIGURACION_ENVIOS").Tables[0];
                }

                if (dtPines != null)
                {
                    if (dtPines.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtPines.Rows)
                        {
                            if (dtConf.Rows.Count > 0)
                            {
                                drConf = dtConf.Rows[0];
                                errWs = "";
                                mensajeProcesos(this.txtEcollectUrl, "Consumiendo Web service E-collect " + dr["Numero_Referencia"].ToString(), true);

                                //consume Ws
                                rtaWs = claseEnvios.consumirWsEcollect(dr, drConf, out errWs, out sign);
                                if (rtaWs != null)
                                {
                                    if (rtaWs.eCollectUrl == "" || rtaWs.TicketId == "")
                                    {
                                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                        {
                                            dt.nuevoParametro("@Ticket_id", DBNull.Value, ParameterDirection.Input);
                                            dt.nuevoParametro("@url", DBNull.Value, ParameterDirection.Input);
                                            dt.nuevoParametro("@retorno", rtaWs.ReturnCode, ParameterDirection.Input);
                                            dt.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                                            dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                            dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                                            dt.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_TICKET_PSE");
                                            if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                                            {
                                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "generarUrlPSE", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                            }
                                        }
                                        mensajeProcesos(this.txtEcollectUrl, "Respuesta Ws: " + rtaWs.ReturnCode, true);

                                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, rtaWs.ReturnCode, Application.ProductName, this.Name, "generarUrlPSE", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                    }
                                    else
                                    {
                                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                        {
                                            dt.nuevoParametro("@Ticket_id", rtaWs.TicketId, ParameterDirection.Input);
                                            dt.nuevoParametro("@url", rtaWs.eCollectUrl, ParameterDirection.Input);
                                            dt.nuevoParametro("@retorno", rtaWs.ReturnCode, ParameterDirection.Input);
                                            dt.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                                            dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                            dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                                            dt.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_TICKET_PSE");
                                            if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                                            {
                                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "generarUrlPSE", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                            }
                                        }
                                    }
                                    //log de transaccion
                                    guardarLogPSECrearTransaccion(dr["Numero_Referencia"].ToString(), drConf["UrlECollectWs"].ToString(), drConf["EntityCodePSE"].ToString(), drConf["SrvCodePSE"].ToString(), decimal.Parse(dr["Valor_Cuota"].ToString()),
                                                       0, drConf["UrlRedirectPSE"].ToString(), drConf["PaymentSystem"].ToString(), sign, "Referencia",
                                                       dr["Tipo_documento"].ToString(), dr["Numero_Documento"].ToString(), dr["Nombres"].ToString() + " " + dr["Apellidos"].ToString(), dr["Direccion_Ubicacion"].ToString(),
                                                       dr["Telefono1"].ToString(), dr["Email1"].ToString(), rtaWs.TicketId, rtaWs.eCollectUrl, rtaWs.ReturnCode);

                                }
                                else
                                {
                                    using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                    {
                                        dt.nuevoParametro("@Ticket_id", DBNull.Value, ParameterDirection.Input);
                                        dt.nuevoParametro("@url", DBNull.Value, ParameterDirection.Input);
                                        dt.nuevoParametro("@retorno", errWs, ParameterDirection.Input);
                                        dt.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                                        dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                        dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                                        dt.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_TICKET_PSE");
                                        if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                                        {
                                            clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "generarUrlPSE", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                        }
                                    }
                                    mensajeProcesos(this.txtEcollectUrl, "Respuesta Ws: " + errWs, true);
                                    clsgeneral.registraErroresAplicaciones(cConexionRecaudos, errWs, Application.ProductName, this.Name, "generarUrlPSE", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                }
                            }
                            mensajeProcesos(this.txtEcollectUrl, "Finalizado consumo Ws para PIN " + dr["Numero_Referencia"].ToString(), true);

                        }
                    }
                }
                Esperar(60000 * int.Parse(dtConf.Rows[0]["Time_Ecollect_url"].ToString()), this.txtEcollectUrl);
                mensajeProcesos(this.txtEcollectUrl, "", false);
            }
            catch (Exception ex)
            {
                this.txtEcollectUrl.Text += Environment.NewLine + "Error de Proceso: " + ex.ToString();
            }


        }

        /// <summary>
        /// Concilia los pines que se pagaron por PSE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void conciliarPSECredipoliza(object sender, DoWorkEventArgs e)
        {
            string errWs = "";
            DataTable dtPines = new DataTable();
            DataTable dtConf = new DataTable();
            clsEnviosWs claseEnvios = new clsEnviosWs();
            clsPDF clasePDF = new clsPDF();
            DataRow drConf;
            string sign = "";
            string aditionalInfoArray = "";
            string AuthReferenceArray = "";
            string OperationArray = "";
            string estadoReferencia = "";
            string rtaSeteo = "";
            string errorWs = "";
            int estadoSeteo = 0;
            clsEnviosWs enviosWs = new clsEnviosWs();
            WsEcollect.getTransactionInformationResponseType rtaWs = new WsEcollect.getTransactionInformationResponseType();
            mensajeProcesos(this.txtEcollectCon, "Conectando...", false);
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {

                dtPines = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PINES_CONCIL_PSE").Tables[0];
            }
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@id_conf", 1, ParameterDirection.Input);
                dtConf = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIGURACION_ENVIOS").Tables[0];
            }

            if (dtPines != null)
            {
                if (dtPines.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtPines.Rows)
                    {
                        if (dtConf.Rows.Count > 0)
                        {
                            drConf = dtConf.Rows[0];
                            errWs = "";
                            mensajeProcesos(this.txtEcollectCon, "Consumiendo Web service de información E-collect " + dr["Numero_Referencia"].ToString(), true);
                            //consume Ws
                            rtaWs = claseEnvios.infoWsEcollect(dr, drConf, out errWs);
                            if (rtaWs != null)
                            {

                                mensajeProcesos(this.txtEcollectCon, "Respuesta Estado de Pago " + rtaWs.TranState, true);
                                DataTable dtRespuesta = new DataTable();
                                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                {
                                    dt.nuevoParametro("@codi_ban", 7, ParameterDirection.Input);
                                    dt.nuevoParametro("@nume_ref", dr["Numero_Referencia"].ToString(), ParameterDirection.Input);
                                    dt.nuevoParametro("@estado_pse", rtaWs.TranState, ParameterDirection.Input);
                                    dt.nuevoParametro("@fecha_pago", rtaWs.BankProcessDate, ParameterDirection.Input);
                                    dt.nuevoParametro("@valor", rtaWs.TransValue, ParameterDirection.Input);
                                    dt.nuevoParametro("@estado", 0, ParameterDirection.Output);
                                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                                    dtRespuesta = dt.ejecutar(CommandType.StoredProcedure, "ACTUALIZAR_PAGOREF_PSE").Tables[0];
                                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                                    {
                                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtRespuesta.Rows[0][1].ToString(), Application.ProductName, this.Name, "conciliarPSECredipoliza", Application.ProductVersion, dr["Numero_Referencia"].ToString());
                                    }
                                    else
                                    {
                                        if (rtaWs.TranState == "OK")
                                        {
                                            estadoReferencia = dt.retornaParametro("@estado").ToString();

                                            this.txtEcollectCon.Text += Environment.NewLine + "Seteo de evento en Bizagi...";
                                            rtaSeteo = enviosWs.seteoBizagi(dr["Id_case_bzg"].ToString(), estadoReferencia, dr["Tipo_pago_ci"].ToString(), 0, out errorWs);
                                            if (rtaSeteo == "false" || errorWs != "")
                                            {
                                                estadoSeteo = 0;
                                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, rtaSeteo + " " + errorWs, Application.ProductName, this.Name, "conciliarPSECredipoliza", Application.ProductVersion, dr["Id_case_bzg"].ToString());
                                            }
                                            else
                                            {
                                                estadoSeteo = 1;
                                            }

                                            using (clsDatos dtSeteo = new clsDatos(cConexionRecaudos))
                                            {
                                                dtSeteo.nuevoParametro("@Estado_seteo", estadoSeteo, ParameterDirection.Input);
                                                dtSeteo.nuevoParametro("@Mensaje_Seteo", errorWs, ParameterDirection.Input);
                                                dtSeteo.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                                                dtSeteo.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                                dtSeteo.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                                                dtSeteo.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_ESTADO_SETEO");
                                                if (dtSeteo.retornaParametro("@db_codi_err").ToString() != "0")
                                                {
                                                    clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtSeteo.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "conciliarPSECredipoliza", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                                                }
                                            }
                                        }

                                    }
                                }

                                aditionalInfoArray = rtaWs.AdditionalInfoArray == null ? "" : rtaWs.AdditionalInfoArray.ToString();
                                AuthReferenceArray = rtaWs.AuthReferenceArray == null ? "" : rtaWs.AuthReferenceArray.ToString();
                                OperationArray = rtaWs.OperationArray == null ? "" : rtaWs.OperationArray.ToString();

                                //
                                guardarLogPSEInfoTransaccion(rtaWs.TicketId, dr["numero_referencia"].ToString(), aditionalInfoArray, AuthReferenceArray,
                                                  rtaWs.BankName, rtaWs.BankProcessDate, rtaWs.CurrencyRate, rtaWs.EntityCode, rtaWs.FICode,
                                                  rtaWs.Invoice, OperationArray, rtaWs.PayCurrency, rtaWs.PaymentSystem, rtaWs.ReferenceArray == null ? "" : rtaWs.ReferenceArray[0],
                                                  rtaWs.ReferenceArray == null ? "" : rtaWs.ReferenceArray[1], rtaWs.ReferenceArray == null ? "" : rtaWs.ReferenceArray[2], rtaWs.ReferenceArray == null ? "" : rtaWs.ReferenceArray[3], rtaWs.ReferenceArray == null ? "" : rtaWs.ReferenceArray[4], rtaWs.ReferenceArray == null ? "" : rtaWs.ReferenceArray[5],
                                                  null, rtaWs.TranState, rtaWs.TransCycle, rtaWs.TransValue, rtaWs.TransVatValue,
                                                  rtaWs.TrazabilityCode);



                            }
                            else
                            {
                                mensajeProcesos(this.txtEcollectCon, "Respuesta Web service: " + errWs, true);
                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, errWs, Application.ProductName, this.Name, "generarUrlPSE", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                            }
                        }
                    }

                }
            }
            Esperar(60000 * int.Parse(dtConf.Rows[0]["Time_Ecollect_con"].ToString()), this.txtEcollectCon);
            mensajeProcesos(this.txtEcollectCon, "", false);
        }

        private void resolverPinesBizagi(object sender, DoWorkEventArgs e)
        {
            string estadoReferencia = "";
            string rtaSeteo = "";
            string errorWs = "";
            int estadoSeteo = 0;
            clsEnviosWs enviosWs = new clsEnviosWs();

            DataTable dtPines = new DataTable();
            DataTable dtConf = new DataTable();
            clsEnviosWs claseEnvios = new clsEnviosWs();
            mensajeProcesos(this.txtSeteo, "Conectando...", false);
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dtPines = dt.ejecutar(CommandType.StoredProcedure, "PA_PINES_SIN_SETEO_BZG").Tables[0];
            }
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@id_conf", 1, ParameterDirection.Input);
                dtConf = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_CONFIGURACION_ENVIOS").Tables[0];
            }

            if (dtPines != null)
            {
                if (dtPines.Rows.Count > 0)
                {
                    mensajeProcesos(this.txtSeteo, "Seteo de evento en Bizagi masivo...", true);
                    foreach (DataRow dr in dtPines.Rows)
                    {
                        estadoReferencia = dr["Estado_PagoRef"].ToString();

                        rtaSeteo = enviosWs.seteoBizagi(dr["Id_case_bzg"].ToString(), estadoReferencia, dr["Tipo_pago_ci"].ToString(), decimal.Parse(dr["valor_extractos"].ToString()), out errorWs);
                        if (rtaSeteo == "false" || errorWs != "")
                        {
                            estadoSeteo = 0;
                            clsgeneral.registraErroresAplicaciones(cConexionRecaudos, rtaSeteo + " " + errorWs, Application.ProductName, this.Name, "resolverPinesBizagi", Application.ProductVersion, dr["Id_case_bzg"].ToString());
                        }
                        else
                        {
                            estadoSeteo = 1;
                        }
                        mensajeProcesos(this.txtSeteo, "Registrando estado del Id_case:" + dr["Id_case_bzg"].ToString() + ", estado seteo:" + estadoSeteo, true);

                        using (clsDatos dtSeteo = new clsDatos(cConexionRecaudos))
                        {
                            dtSeteo.nuevoParametro("@Estado_seteo", estadoSeteo, ParameterDirection.Input);
                            dtSeteo.nuevoParametro("@Mensaje_Seteo", errorWs, ParameterDirection.Input);
                            dtSeteo.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dr["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                            dtSeteo.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                            dtSeteo.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                            dtSeteo.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_ESTADO_SETEO");
                            if (dtSeteo.retornaParametro("@db_codi_err").ToString() != "0")
                            {
                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtSeteo.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "resolverPinesBizagi", Application.ProductVersion, dr["Id_Row_CodigoPago"].ToString());
                            }
                        }

                    }

                }
            }
            Esperar(60000 * int.Parse(dtConf.Rows[0]["Time_seteo"].ToString()), this.txtSeteo);
            mensajeProcesos(this.txtSeteo, "", false);
        }
        /// <summary>
        /// Proceso de conciliar Credipoliza, se integra la logica de pagos iniciales
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void conciliarCredipoliza(object sender, DoWorkEventArgs e)
        {
            try
            {

                DataTable dtDetallesExtractos = new DataTable();
                DataTable dtconsultaCartera = new DataTable();
                DataTable dtconsultaTarjeta = new DataTable();
                string cedula = "";
                int tipoRecaudo = 0;
                string nume_ref_pri = "";
                string nume_ref_sec = "";
                bool procesaSecs = false;
                string estadoReferencia = "0";
                string valorPagado = "0";
                string errorWs = "";
                string rtaSeteo = "";
                int estadoSeteo = 0;
                decimal numeroTarjetaPin = 0;
                clsEnviosWs enviosWs = new clsEnviosWs();
                string excepcion = "";
                ///Obtiene los extractos que seran procesados para credipoliza
                mensajeProcesos(this.txtLogProcesos, "Conectando...", false);

                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dtDetallesExtractos = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_REGISTROS_CONCILIAR_CP").Tables[0];
                    mensajeProcesos(this.txtLogProcesos, "Conectado", true);
                }
                if (dtDetallesExtractos.Rows.Count > 0)
                {

                    foreach (DataRow drExt in dtDetallesExtractos.Rows)
                    {
                        try
                        {
                            if (decimal.Parse(drExt["valo_rec"].ToString()) < 0)
                            {
                                procesarCredipoliza(drExt, 11, "");
                            }
                            else
                            {
                                mensajeProcesos(this.txtLogProcesos, "Procesando extracto id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                nume_ref_pri = drExt["nume_ref_pri"].ToString();
                                nume_ref_sec = drExt["nume_ref_sec"].ToString();
                                if (nume_ref_pri != null)
                                {
                                    if (nume_ref_pri.Length > 18)
                                    {
                                        if (nume_ref_pri.IndexOf("R") != -1)
                                        {
                                            if (nume_ref_pri.Substring(nume_ref_pri.IndexOf("R") - 2).Length >= 18)
                                            {
                                                nume_ref_pri = nume_ref_pri.Substring(nume_ref_pri.IndexOf("R") - 2, 18);
                                            }
                                        }

                                    }
                                    if (nume_ref_pri.IndexOf("R") == -1)//si vienen referencias con 16 caracteres sin la R
                                    {
                                        if (nume_ref_pri.Length > 16)
                                        {
                                            if (nume_ref_pri.IndexOf("5") != -1)
                                            {
                                                if (nume_ref_pri.Substring(nume_ref_pri.IndexOf("5")).Length >= 16)
                                                {
                                                    nume_ref_pri = nume_ref_pri.Substring(nume_ref_pri.IndexOf("5"), 16);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (nume_ref_sec != null)
                                {
                                    if (nume_ref_sec.Length > 18)
                                    {
                                        if (nume_ref_sec.IndexOf("R") != -1)
                                        {
                                            if (nume_ref_sec.Substring(nume_ref_sec.IndexOf("R") - 2).Length >= 18)
                                            {
                                                nume_ref_sec = nume_ref_sec.Substring(nume_ref_sec.IndexOf("R") - 2, 18);
                                            }
                                        }

                                    }
                                    if (nume_ref_sec.IndexOf("R") == -1)//si vienen referencias con 16 caracteres sin la R
                                    {
                                        if (nume_ref_sec.Length > 16)
                                        {
                                            if (nume_ref_sec.IndexOf("5") != -1)
                                            {
                                                if (nume_ref_sec.Substring(nume_ref_sec.IndexOf("5")).Length >= 16)
                                                {
                                                    nume_ref_sec = nume_ref_sec.Substring(nume_ref_sec.IndexOf("5"), 16);
                                                }
                                            }
                                        }
                                    }
                                }
                                DataTable dtRevPinUnicoPri = new DataTable();
                                DataTable dtRevPinUnicoSec = new DataTable();
                                procesaSecs = false;
                                //Valida del primer pago
                                mensajeProcesos(this.txtLogProcesos, "Busqueda Referencia de PIN Unico primer pago...", true);
                                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                {
                                    dt.nuevoParametro("@Numero_referencia", nume_ref_sec, ParameterDirection.Input);
                                    dtRevPinUnicoPri = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PIN_UNICO").Tables[0];
                                }
                                if (dtRevPinUnicoPri.Rows.Count == 0)
                                {
                                    using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                    {
                                        //busca si la referencia se introdujo manual
                                        dt.nuevoParametro("@Numero_referencia", drExt["nume_ref_usu"].ToString(), ParameterDirection.Input);
                                        dtRevPinUnicoPri = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PIN_UNICO").Tables[0];
                                    }
                                }
                                if (dtRevPinUnicoPri.Rows.Count == 0)
                                {
                                    using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                    {
                                        //busca si por la referencia 1
                                        dt.nuevoParametro("@Numero_referencia", nume_ref_pri, ParameterDirection.Input);
                                        dtRevPinUnicoPri = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PIN_UNICO").Tables[0];
                                    }
                                }
                                if (dtRevPinUnicoPri.Rows.Count == 0)
                                {
                                    procesaSecs = true;
                                }
                                else
                                {
                                    if (int.Parse(dtRevPinUnicoPri.Rows[0]["totalDetalle"].ToString()) == 1)
                                    {
                                        mensajeProcesos(this.txtLogProcesos, "Conciliando primer pago..." + dtRevPinUnicoPri.Rows[0]["Numero_Referencia"].ToString(), true);
                                        DataTable dtRespuesta = new DataTable();
                                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                        {
                                            dt.nuevoParametro("@id_prc_cab", decimal.Parse(drExt["id_prc_cab"].ToString()), ParameterDirection.Input);
                                            dt.nuevoParametro("@id_prc_det", decimal.Parse(drExt["id_prc_det"].ToString()), ParameterDirection.Input);
                                            dt.nuevoParametro("@codi_ban", int.Parse(drExt["codigoEntidadFinanciera"].ToString()).ToString(), ParameterDirection.Input);
                                            dt.nuevoParametro("@nume_ref", dtRevPinUnicoPri.Rows[0]["Numero_Referencia"].ToString(), ParameterDirection.Input);
                                            dt.nuevoParametro("@band_ref", 1, ParameterDirection.Input);
                                            dt.nuevoParametro("@estado", 0, ParameterDirection.Output);
                                            dt.nuevoParametro("@valor_ext", 0, ParameterDirection.Output);
                                            dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                            dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                                            dtRespuesta = dt.ejecutar(CommandType.StoredProcedure, "ACTUALIZAR_PAGOREF_EXTRACTO").Tables[0];
                                            if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                                            {
                                                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtRespuesta.Rows[0][1].ToString(), Application.ProductName, this.Name, "envioReferenciador", Application.ProductVersion, drExt["id_prc_det"].ToString());
                                            }
                                            else
                                            {
                                                estadoReferencia = dt.retornaParametro("@estado").ToString();
                                                valorPagado = dt.retornaParametro("@valor_ext").ToString();
                                                if (estadoReferencia == "5" || estadoReferencia == "6")
                                                {
                                                    mensajeProcesos(this.txtLogProcesos, "Seteo de evento en Bizagi...", true);
                                                    rtaSeteo = enviosWs.seteoBizagi(dtRevPinUnicoPri.Rows[0]["Id_case_bzg"].ToString(), estadoReferencia, dtRevPinUnicoPri.Rows[0]["Tipo_pago_ci"].ToString(), decimal.Parse(valorPagado), out errorWs);
                                                    if (rtaSeteo == "false" || errorWs != "")
                                                    {
                                                        estadoSeteo = 0;
                                                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, rtaSeteo + " " + errorWs, Application.ProductName, this.Name, "conciliacionCrediPoliza", Application.ProductVersion, dtRevPinUnicoPri.Rows[0]["Id_case_bzg"].ToString());
                                                    }
                                                    else
                                                    {
                                                        estadoSeteo = 1;
                                                    }                                                    
                                                }
                                                else
                                                {
                                                    if (estadoReferencia != "4")
                                                    {
                                                        estadoSeteo = 0;
                                                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, "El estado que retorna ACTUALIZAR_PAGOREF_EXTRACTO, no es valido: " + estadoReferencia, Application.ProductName, this.Name, "conciliacionCrediPoliza", Application.ProductVersion, dtRevPinUnicoPri.Rows[0]["Id_Row_CodigoPago"].ToString());
                                                    }
                                                    else
                                                    {
                                                        estadoSeteo = 0;
                                                    }
                                                }

                                                if (estadoReferencia != "4")
                                                {
                                                    using (clsDatos dtSeteo = new clsDatos(cConexionRecaudos))
                                                    {
                                                        dtSeteo.nuevoParametro("@Estado_seteo", estadoSeteo, ParameterDirection.Input);
                                                        dtSeteo.nuevoParametro("@Mensaje_Seteo", errorWs, ParameterDirection.Input);
                                                        dtSeteo.nuevoParametro("@Id_Row_CodigoPago", decimal.Parse(dtRevPinUnicoPri.Rows[0]["Id_Row_CodigoPago"].ToString()), ParameterDirection.Input);
                                                        dtSeteo.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                                                        dtSeteo.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                                                        dtSeteo.ejecutar(CommandType.StoredProcedure, "PA_ACTUALIZA_ESTADO_SETEO");
                                                        if (dtSeteo.retornaParametro("@db_codi_err").ToString() != "0")
                                                        {
                                                            clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtSeteo.retornaParametro("@db_desc_err").ToString(), Application.ProductName, this.Name, "conciliacionCredipoliza", Application.ProductVersion, dtRevPinUnicoPri.Rows[0]["Id_Row_CodigoPago"].ToString());
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        procesaSecs = true;
                                    }
                                }
                                if (procesaSecs)
                                {
                                    //Validación de pagos siguientes
                                    mensajeProcesos(this.txtLogProcesos, "Busqueda Referencia de PIN Unico pagos siguientes...id_prc_det " + drExt["id_prc_det"].ToString(), true);
                                    using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                    {
                                        dt.nuevoParametro("@Numero_referencia", nume_ref_sec, ParameterDirection.Input);
                                        dtRevPinUnicoSec = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PIN_UNICO_EXTR").Tables[0];
                                    }
                                    if (dtRevPinUnicoSec.Rows.Count == 0)
                                    {
                                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                        {
                                            //busca si la referencia se introdujo manual
                                            dt.nuevoParametro("@Numero_referencia", drExt["nume_ref_usu"].ToString(), ParameterDirection.Input);
                                            dtRevPinUnicoSec = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PIN_UNICO_EXTR").Tables[0];
                                        }
                                    }
                                    if (dtRevPinUnicoSec.Rows.Count == 0)
                                    {
                                        using (clsDatos dt = new clsDatos(cConexionRecaudos))
                                        {
                                            //busca si por la referencia 1
                                            dt.nuevoParametro("@Numero_referencia", nume_ref_pri, ParameterDirection.Input);
                                            dtRevPinUnicoSec = dt.ejecutar(CommandType.StoredProcedure, "PA_CONSULTA_PIN_UNICO_EXTR").Tables[0];
                                        }
                                    }
                                    if (dtRevPinUnicoSec.Rows.Count > 0)
                                    {
                                        //Inicia conciliacion por PIN Unico
                                        cedula = dtRevPinUnicoSec.Rows[0]["Numero_Documento"].ToString();
                                        tipoRecaudo = int.Parse(dtRevPinUnicoSec.Rows[0]["Tipo_pago_ci"].ToString());
                                        //consulta tbl_cartera con PIN


                                        //Logica punto 2 cartera Consulta Cartera con PIN
                                        //--------------------------------------------------------
                                        mensajeProcesos(this.txtLogProcesos, "Busqueda pago con PIN Unico en Tbl_cartera...id_prc_det " + drExt["id_prc_det"].ToString(), true);
                                        dtconsultaCartera = new DataTable();
                                        dtconsultaCartera = consultaTblCartera(dtRevPinUnicoSec.Rows[0]["Numero_referencia"].ToString(), "", 1);

                                        if (dtconsultaCartera.Rows.Count > 0)
                                        {
                                            //revisar nro_cuenta
                                            conciliarCredipoliza(drExt, tipoRecaudo, "", dtconsultaCartera.Rows[0]["Estado_credito"].ToString(),
                                                dtRevPinUnicoSec.Rows[0]["Numero_referencia"].ToString(), dtconsultaCartera.Rows[0]["Numero_Credito"].ToString());
                                            mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO con PIN id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                        }
                                        else
                                        {

                                            marcarSinReferenciaConciliacionPIN(drExt);
                                            mensajeProcesos(this.txtLogProcesos, "Extracto no conciliado con PIN id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                        }
                                    }
                                    else
                                    {
                                        //punto 3 tbl cartera
                                        mensajeProcesos(this.txtLogProcesos, "Busqueda pago en tbl_cartera con nro credito...id_prc_det " + drExt["id_prc_det"].ToString(), true);
                                        dtconsultaCartera = new DataTable();
                                        dtconsultaCartera = consultaTblCartera("", drExt["nume_ref_pri"].ToString(), 2);
                                        if (dtconsultaCartera.Rows.Count > 0)
                                        {
                                            procesarCredipoliza(drExt, 4, dtconsultaCartera.Rows[0]["Numero_Credito"].ToString());
                                            mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                        }
                                        else
                                        {
                                            dtconsultaCartera = new DataTable();
                                            dtconsultaCartera = consultaTblCartera("", drExt["nume_ref_sec"].ToString(), 2);
                                            if (dtconsultaCartera.Rows.Count > 0)
                                            {
                                                procesarCredipoliza(drExt, 4, dtconsultaCartera.Rows[0]["Numero_Credito"].ToString());
                                                mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                            }
                                            else
                                            {
                                                dtconsultaCartera = new DataTable();
                                                dtconsultaCartera = consultaTblCartera("", drExt["nume_ref_usu"].ToString(), 2);
                                                if (dtconsultaCartera.Rows.Count > 0)
                                                {
                                                    procesarCredipoliza(drExt, 4, dtconsultaCartera.Rows[0]["Numero_Credito"].ToString());
                                                    mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                }
                                                else
                                                {
                                                    //Punto 4 validacion de nro de tarjeta
                                                    mensajeProcesos(this.txtLogProcesos, "Busqueda pago en tbl_cartera con numero de tarjeta campo PIN...id_prc_det " + drExt["id_prc_det"].ToString(), true);
                                                    dtconsultaCartera = new DataTable();
                                                    if (!decimal.TryParse(drExt["nume_ref_pri"].ToString(), out numeroTarjetaPin))
                                                    {
                                                        numeroTarjetaPin = 0;
                                                    }
                                                    dtconsultaCartera = consultaTblCartera(numeroTarjetaPin.ToString(), "", 1);
                                                    if (dtconsultaCartera.Rows.Count > 0)
                                                    {
                                                        mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                        procesarCredipoliza(drExt, 4, dtconsultaCartera.Rows[0]["Numero_Credito"].ToString());
                                                    }
                                                    else
                                                    {
                                                        dtconsultaCartera = new DataTable();
                                                        if (!decimal.TryParse(drExt["nume_ref_sec"].ToString(), out numeroTarjetaPin))
                                                        {
                                                            numeroTarjetaPin = 0;
                                                        }
                                                        dtconsultaCartera = consultaTblCartera(numeroTarjetaPin.ToString(), "", 1);
                                                        if (dtconsultaCartera.Rows.Count > 0)
                                                        {
                                                            mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                            procesarCredipoliza(drExt, 4, dtconsultaCartera.Rows[0]["Numero_Credito"].ToString());
                                                        }
                                                        else
                                                        {
                                                            dtconsultaCartera = new DataTable();
                                                            if (!decimal.TryParse(drExt["nume_ref_usu"].ToString(), out numeroTarjetaPin))
                                                            {
                                                                numeroTarjetaPin = 0;
                                                            }
                                                            dtconsultaCartera = consultaTblCartera(numeroTarjetaPin.ToString(), "", 1);
                                                            if (dtconsultaCartera.Rows.Count > 0)
                                                            {
                                                                mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                                procesarCredipoliza(drExt, 4, dtconsultaCartera.Rows[0]["Numero_Credito"].ToString());
                                                            }
                                                            else
                                                            {
                                                                //logica punto 5
                                                                mensajeProcesos(this.txtLogProcesos, "Busqueda pago en tarjetas cpms con nro tarjeta...id_prc_det" + drExt["id_prc_det"].ToString(), true);
                                                                dtconsultaTarjeta = new DataTable();
                                                                dtconsultaTarjeta = consultaTblTarjetasCPMS(drExt["nume_ref_pri"].ToString());
                                                                if (dtconsultaTarjeta.Rows.Count > 0)
                                                                {
                                                                    //validacion de obligación
                                                                    dtconsultaCartera = new DataTable();
                                                                    dtconsultaCartera = consultaTblCartera("", dtconsultaTarjeta.Rows[0]["Obligacion"].ToString(), 2);
                                                                    if (dtconsultaCartera.Rows.Count > 0)
                                                                    {
                                                                        mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                                        procesarCredipoliza(drExt, 4, dtconsultaTarjeta.Rows[0]["Obligacion"].ToString());
                                                                    }
                                                                    else
                                                                    {
                                                                        //marca registro por conciliar, pendiente
                                                                        procesarCredipoliza(drExt, 3, "");
                                                                        mensajeProcesos(this.txtLogProcesos, "Extracto no conciliado id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    dtconsultaTarjeta = new DataTable();
                                                                    dtconsultaTarjeta = consultaTblTarjetasCPMS(drExt["nume_ref_sec"].ToString());
                                                                    if (dtconsultaTarjeta.Rows.Count > 0)
                                                                    {
                                                                        //validacion de obligación en cartera campo nro de credito
                                                                        dtconsultaCartera = new DataTable();
                                                                        dtconsultaCartera = consultaTblCartera("", dtconsultaTarjeta.Rows[0]["Obligacion"].ToString(), 2);
                                                                        if (dtconsultaCartera.Rows.Count > 0)
                                                                        {
                                                                            procesarCredipoliza(drExt, 4, dtconsultaTarjeta.Rows[0]["Obligacion"].ToString());
                                                                            mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);

                                                                        }
                                                                        else
                                                                        {
                                                                            //marca registro por conciliar, pendiente
                                                                            mensajeProcesos(this.txtLogProcesos, "Extracto no conciliado id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                                            procesarCredipoliza(drExt, 3, "");
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        dtconsultaTarjeta = new DataTable();
                                                                        dtconsultaTarjeta = consultaTblTarjetasCPMS(drExt["nume_ref_usu"].ToString());
                                                                        if (dtconsultaTarjeta.Rows.Count > 0)
                                                                        {
                                                                            //validacion de obligación en cartera campo nro de credito
                                                                            dtconsultaCartera = new DataTable();
                                                                            dtconsultaCartera = consultaTblCartera("", dtconsultaTarjeta.Rows[0]["Obligacion"].ToString(), 2);
                                                                            if (dtconsultaCartera.Rows.Count > 0)
                                                                            {
                                                                                procesarCredipoliza(drExt, 4, dtconsultaTarjeta.Rows[0]["Obligacion"].ToString());
                                                                                mensajeProcesos(this.txtLogProcesos, "Extracto CONCILIADO id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);

                                                                            }
                                                                            else
                                                                            {
                                                                                //marca registro por conciliar, pendiente
                                                                                mensajeProcesos(this.txtLogProcesos, "Extracto no conciliado id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                                                procesarCredipoliza(drExt, 3, "");
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            //marca registro por referenciar
                                                                            mensajeProcesos(this.txtLogProcesos, "Extracto no conciliado id_prc_det:" + drExt["id_prc_det"].ToString() + "...", true);
                                                                            procesarCredipoliza(drExt, 3, "");
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }
                                                }


                                            }
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            excepcion = "";
                            excepcion = ex.ToString();
                            if (excepcion.Length >= 500)
                            {
                                excepcion = excepcion.Substring(0, 499);
                            }
                            procesarCredipoliza(drExt, 99, "");
                            clsgeneral.registraErroresAplicaciones(cConexionRecaudos, excepcion, Application.ProductName, this.Name, "conciliarCredipoliza", Application.ProductVersion, "0");
                        }                       
                                             
                        //FIN FOR
                    }
                }
                mensajeProcesos(this.txtLogProcesos, "---Proceso de Conciliación Credipoliza FINALIZADO---", true);

            }
            catch (Exception ex)
            {
                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, ex.Message, Application.ProductName, this.Name, "conciliarCredipoliza", Application.ProductVersion, "0");
            }
        }
        /// <summary>
        /// Metodo de consulta de Tbl_cartera
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="busqueda"></param>
        /// <returns></returns>
        private DataTable consultaTblCartera(string pin, string nro_credito, int busqueda)
        {
            DataTable dtconsultaCartera = new DataTable();
            decimal decCredito = 0;
            string lcNroCredito = "";
            clsPDF clspdf = new clsPDF();
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@opc_busqueda", busqueda, ParameterDirection.Input);
                switch (busqueda)
                {
                    case 1:
                        dt.nuevoParametro("@pin", pin, ParameterDirection.Input);
                        break;
                    case 2:
                        if (!decimal.TryParse(nro_credito, out decCredito))
                        {
                            decCredito = -1;
                        }
                        dt.nuevoParametro("@nro_credito", decCredito, ParameterDirection.Input);
                        break;
                }
                dtconsultaCartera = dt.ejecutar(CommandType.StoredProcedure, "CONSULTAR_TBL_CARTERA").Tables[0];
            }

            return dtconsultaCartera;
        }
        /// <summary>
        /// Consulta la table de tarjetas
        /// </summary>
        /// <param name="nro_tarjeta"></param>
        /// <returns></returns>
        private DataTable consultaTblTarjetasCPMS(string nro_tarjeta)
        {
            decimal decTarjeta = 0;
            if (!decimal.TryParse(nro_tarjeta, out decTarjeta))
            {
                decTarjeta = -1;
            }
            DataTable dtconsultaCartera = new DataTable();
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@Numero_tarjeta", decTarjeta, ParameterDirection.Input);
                dtconsultaCartera = dt.ejecutar(CommandType.StoredProcedure, "CONSULTAR_TARJETAS_CPMS").Tables[0];
            }
            return dtconsultaCartera;


        }
        /// <summary>
        /// Concilia un extracto con información valida de credipoliza
        /// </summary>
        /// <param name="drExt"></param>
        /// <param name="tipoRecaudo"></param>
        /// <param name="nroCuenta"></param>
        /// <param name="estadoCredito"></param>
        /// <param name="pin"></param>
        private void conciliarCredipoliza(DataRow drExt, int tipoRecaudo, string nroCuenta, string estadoCredito, string pin, string nume_ref_usu)
        {
            bool rtaValidador = true;
            DataTable dtRespuesta = new DataTable();
            if (rtaValidador)
            {
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dt.nuevoParametro("@id_prc_cab", decimal.Parse(drExt["id_prc_cab"].ToString()), ParameterDirection.Input);
                    dt.nuevoParametro("@id_prc_det", decimal.Parse(drExt["id_prc_det"].ToString()), ParameterDirection.Input);
                    dt.nuevoParametro("@nume_cue", nroCuenta, ParameterDirection.Input);
                    dt.nuevoParametro("@nume_ref", pin, ParameterDirection.Input);
                    dt.nuevoParametro("@Estado_credito", estadoCredito, ParameterDirection.Input);
                    dt.nuevoParametro("@nume_ref_usu", nume_ref_usu, ParameterDirection.Input);
                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                    dtRespuesta = dt.ejecutar(CommandType.StoredProcedure, "CONCILIAR_CREDIPOLIZA_PIN").Tables[0];
                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                    {
                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtRespuesta.Rows[0][1].ToString(), Application.ProductName, this.Name, "conciliarBDCredipolizaPIN", Application.ProductVersion, drExt["id_prc_cab"].ToString());
                    }
                    else
                    {
                        mensajeProcesos(this.txtLogProcesos, "PIN " + drExt["nume_ref_sec"].ToString() + " proceso finalizado...Extracto: " + drExt["id_prc_det"].ToString(), true);
                    }

                }

            }
            else
            {
                marcarSinReferenciaConciliacionPIN(drExt);
            }

        }
        private void procesarCredipoliza(DataRow drExt, int estado, string nume_ref_usu)
        {

            DataTable dtRespuesta = new DataTable();
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@id_prc_det", decimal.Parse(drExt["id_prc_det"].ToString()), ParameterDirection.Input);
                dt.nuevoParametro("@codi_est_prc", estado, ParameterDirection.Input);
                dt.nuevoParametro("@nume_ref_usu", nume_ref_usu, ParameterDirection.Input);
                dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                dtRespuesta = dt.ejecutar(CommandType.StoredProcedure, "PROCESAR_CREDIPOLIZA_OTROS").Tables[0];
                if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                {
                    clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtRespuesta.Rows[0][1].ToString(), Application.ProductName, this.Name, "procesarCredipoliza", Application.ProductVersion, drExt["id_prc_cab"].ToString());
                }
                else
                {
                    mensajeProcesos(this.txtLogProcesos,  "id_prc_det " + drExt["nume_ref_sec"].ToString() + " proceso finalizado " + drExt["id_prc_det"].ToString(), true);
                }

            }

        }

        private void guardarLogPSECrearTransaccion(string Numero_Referencia, string DireccionWs, string EntityCode, string SrvCode, decimal TransValue,
                                                   decimal TransVatValue, string urlRedirect, string PaymentSystem, string Sign, string singField,
                                                   string ReferenceArray1, string ReferenceArray2, string ReferenceArray3, string ReferenceArray4,
                                                   string ReferenceArray5, string ReferenceArray6, string Ticket_id, string eCollectUrl, string ReturnCode)
        {
            DataTable dtRespuesta = new DataTable();
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@Numero_Referencia", Numero_Referencia, ParameterDirection.Input);
                dt.nuevoParametro("@DireccionWs", DireccionWs, ParameterDirection.Input);
                dt.nuevoParametro("@EntityCode", EntityCode, ParameterDirection.Input);
                dt.nuevoParametro("@SrvCode", SrvCode, ParameterDirection.Input);
                dt.nuevoParametro("@TransValue", TransValue, ParameterDirection.Input);
                dt.nuevoParametro("@TransVatValue", TransVatValue, ParameterDirection.Input);
                dt.nuevoParametro("@URLRedirect", urlRedirect, ParameterDirection.Input);
                dt.nuevoParametro("@PaymentSystem", PaymentSystem, ParameterDirection.Input);
                dt.nuevoParametro("@Sign", Sign, ParameterDirection.Input);
                dt.nuevoParametro("@SignFields", singField, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray1", ReferenceArray1, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray2", ReferenceArray2, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray3", ReferenceArray3, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray4", ReferenceArray4, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray5", ReferenceArray5, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray6", ReferenceArray6, ParameterDirection.Input);
                dt.nuevoParametro("@Ticket_id", Ticket_id, ParameterDirection.Input);
                dt.nuevoParametro("@eColectUrl", eCollectUrl, ParameterDirection.Input);
                dt.nuevoParametro("@ReturnCode", ReturnCode, ParameterDirection.Input);
                dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                dtRespuesta = dt.ejecutar(CommandType.StoredProcedure, "REGISTRA_PSE_CREAR_TRANSACCION").Tables[0];
                if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                {
                    clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtRespuesta.Rows[0][1].ToString(), Application.ProductName, this.Name, "guardarLogPSECrearTransaccion", Application.ProductVersion, Numero_Referencia);
                }
                else
                {
                    mensajeProcesos(this.txtEcollectUrl, "numero referencia " + Numero_Referencia + " registrado en Log PSE", true);
                }
            }
        }

        private void guardarLogPSEInfoTransaccion(string TicketId, string Numero_referencia, string AdditionalInfoArray, string AuthReferenceArray,
                                                  string BankName, DateTime BankProcessDate, decimal CurrencyRate, string EntityCode, string FlCode,
                                                  string Invoice, string OperationArray, string PayCurrency, string PaymentSystem, string ReferenceArray1,
                                                  string ReferenceArray2, string ReferenceArray3, string ReferenceArray4, string ReferenceArray5, string ReferenceArray6,
                                                  string RetriesTicketId, string TransState, string TransCycle, decimal TransValue, decimal TrasVatValue,
                                                  string TrazabitilityCode)
        {
            DataTable dtRespuesta = new DataTable();
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@TicketId", TicketId, ParameterDirection.Input);
                dt.nuevoParametro("@Numero_referencia", Numero_referencia, ParameterDirection.Input);
                dt.nuevoParametro("@AdditionalInfoArray", AdditionalInfoArray, ParameterDirection.Input);
                dt.nuevoParametro("@AuthReferenceArray", AuthReferenceArray, ParameterDirection.Input);
                dt.nuevoParametro("@BankName", BankName, ParameterDirection.Input);
                dt.nuevoParametro("@BankProcessDate", BankProcessDate, ParameterDirection.Input);
                dt.nuevoParametro("@CurrencyRate", CurrencyRate, ParameterDirection.Input);
                dt.nuevoParametro("@EntityCode", EntityCode, ParameterDirection.Input);
                dt.nuevoParametro("@FlCode", FlCode, ParameterDirection.Input);
                dt.nuevoParametro("@Invoice", Invoice, ParameterDirection.Input);
                dt.nuevoParametro("@OperationArray", OperationArray, ParameterDirection.Input);
                dt.nuevoParametro("@PayCurrency", PayCurrency, ParameterDirection.Input);
                dt.nuevoParametro("@PaymentSystem", PaymentSystem, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray1", ReferenceArray1, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray2", ReferenceArray2, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray3", ReferenceArray3, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray4", ReferenceArray4, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray5", ReferenceArray5, ParameterDirection.Input);
                dt.nuevoParametro("@ReferenceArray6", ReferenceArray6, ParameterDirection.Input);
                dt.nuevoParametro("@RetriesTicketId", RetriesTicketId, ParameterDirection.Input);
                dt.nuevoParametro("@TransState", TransState, ParameterDirection.Input);
                dt.nuevoParametro("@TransCycle", TransCycle, ParameterDirection.Input);
                dt.nuevoParametro("@TransValue", TransValue, ParameterDirection.Input);
                dt.nuevoParametro("@TrasVatValue", TrasVatValue, ParameterDirection.Input);
                dt.nuevoParametro("@TrazabitilityCode", TrazabitilityCode, ParameterDirection.Input);

                dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                dtRespuesta = dt.ejecutar(CommandType.StoredProcedure, "REGISTRA_PSE_INFO_TRANSACCION").Tables[0];
                if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                {
                    clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtRespuesta.Rows[0][1].ToString(), Application.ProductName, this.Name, "guardarLogPSECrearTransaccion", Application.ProductVersion, Numero_referencia);
                }
                else
                {
                    mensajeProcesos(this.txtEcollectCon, "numero referencia " + Numero_referencia + " registrado en Log PSE", true);
                }
            }
        }

        /// <summary>
        /// Valida si la cuenta para conciliar el asegurador es valida
        /// </summary>
        /// <returns></returns>
        private bool validarCuentaAsegurador(string cuenta)
        {
            bool respuesta = false;
            if (cuenta != "")
            {
                if (cuenta.Length > 4)
                {
                    if (cuenta.Substring(cuenta.Length - 3, 3) == "974")
                    {
                        respuesta = true;
                    }
                    else
                    {
                        if (cuenta.Substring(cuenta.Length - 4, 4) == "7771")
                        {
                            respuesta = true;
                        }
                    }
                }
            }
            return respuesta;
        }

        /// <summary>
        /// Marca un detalle extracto sin conciliar
        /// </summary>
        /// <param name="dtconsultaSIIF"></param>
        /// <param name="drExt"></param>
        private void marcarSinReferenciaConciliacionPIN(DataRow drExt)
        {
            DataTable dtRespuesta = new DataTable();
            try
            {
               mensajeProcesos(this.txtLogProcesos, "Validando datos SIIF...", true);
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    //concilia directo si concuerda toda la informaciòn
                    dt.nuevoParametro("@id_prc_cab", decimal.Parse(drExt["id_prc_cab"].ToString()), ParameterDirection.Input);
                    dt.nuevoParametro("@id_prc_det", decimal.Parse(drExt["id_prc_det"].ToString()), ParameterDirection.Input);
                    dt.nuevoParametro("@estado_cab", 1, ParameterDirection.Input);
                    dt.nuevoParametro("@estado_det", 3, ParameterDirection.Input);
                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                    dtRespuesta = dt.ejecutar(CommandType.StoredProcedure, "MARCAR_CREDIPOLIZA_SIN_REF").Tables[0];
                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                    {
                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtRespuesta.Rows[0][1].ToString(), Application.ProductName, this.Name, "conciliarBDCredipolizaSinPIN", Application.ProductVersion, drExt["id_prc_cab"].ToString());
                    }
                    else
                    {
                        mensajeProcesos(this.txtLogProcesos, "Extracto " + drExt["id_prc_det"].ToString() + " proceso finalizado...", true);
                    }

                }


            }
            catch (Exception e)
            {
                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, e.Message, Application.ProductName, this.Name, "conciliarBDCredipolizaSinPIN", Application.ProductVersion, drExt["id_prc_cab"].ToString());
            }
        }
        #endregion


        #region DoWork
        private void bwkProcesamientoEntregas_DoWork(object sender, DoWorkEventArgs e)
        {            
            conciliarCredipoliza(sender, e);
        }
        private void bwkGenEcollect_DoWork(object sender, DoWorkEventArgs e)
        {
            generarUrlPSE(sender, e);
        }
        private void bwkMail_DoWork(object sender, DoWorkEventArgs e)
        {
            envioMail(sender, e);
        }
        private void bwkSeteo_DoWork(object sender, DoWorkEventArgs e)
        {
            resolverPinesBizagi(sender, e);
        }
        private void bwkSMS_DoWork(object sender, DoWorkEventArgs e)
        {
            envioSMS(sender, e);
        }
        private void bwkConEcollect_DoWork(object sender, DoWorkEventArgs e)
        {
            conciliarPSECredipoliza(sender, e);
        }
        private void bkwMailRechazos_DoWork(object sender, DoWorkEventArgs e)
        {
            envioMailRechazado(sender, e);
        }
        private void bkwMaillInt_DoWork(object sender, DoWorkEventArgs e)
        {
            envioMailIntermediario(sender, e);
        }
        #endregion

        #region RunWorkerComplete
        private void bwkProcesamientoEntregas_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.btnConciliacion.Enabled = true;
        }
        private void bwkGenEcollect_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (bolProcesos)
            {
                bwkGenEcollect.RunWorkerAsync();
            }
            else
            {
                resetarLog();
                mensajeProcesos(this.txtEcollectUrl, "Proceso Detenido", false);
            }

        }
        private void bwkMail_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (bolProcesos)
            {
                bwkMail.RunWorkerAsync();
            }
            else
            {
                resetarLog();
                mensajeProcesos(this.txtMail, "Proceso Detenido", false);
            }

        }
        private void bwkSeteo_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (bolProcesos)
            {
                bwkSeteo.RunWorkerAsync();
            }
            else
            {
                resetarLog();
                mensajeProcesos(this.txtSeteo, "Proceso Detenido", false);
            }

        }
        private void bwkSMS_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (bolProcesos)
            {
                bwkSMS.RunWorkerAsync();
            }
            else
            {
                resetarLog();
                mensajeProcesos(this.txtSMS, "Proceso Detenido", false);
            }

        }
        private void bwkConEcollect_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (bolProcesos)
            {
                bwkConEcollect.RunWorkerAsync();
            }
            else
            {
                resetarLog();
                mensajeProcesos(this.txtEcollectCon, "Proceso Detenido", false);
            }

        }
        private void bkwMailRechazos_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (bolProcesos)
            {
                bkwMailRechazos.RunWorkerAsync();
            }
            else
            {
                resetarLog();
                mensajeProcesos(this.txtMailRech, "Proceso Detenido", false);
            }

        }
        private void bkwMailIntermediario_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (bolProcesos)
            {
                bkwMaillInt.RunWorkerAsync();
            }
            else
            {
                resetarLog();
                mensajeProcesos(this.txtMailInt, "Proceso Detenido", false);
            }

        }
        #endregion

        #region Generales
        /// <summary>
        /// Activa los procesos masivos de pin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnActivar_Click(object sender, EventArgs e)
        {
            if (busyWorkers())
            {
                MessageBox.Show("Por favor espere, hay procesos pendientes de finalizar...");
            }
            else
            {
                if (iniciaProceso())
                {
                    bolProcesos = true;
                    this.btnActivar.Enabled = false;
                    this.btnDetener.Enabled = true;

                    if (chkgenPse.Checked)
                        bwkGenEcollect.RunWorkerAsync();
                    if (chkSeteo.Checked)
                        bwkSeteo.RunWorkerAsync();
                    if (chkSms.Checked)
                        bwkSMS.RunWorkerAsync();
                    if (chkEmail.Checked)
                        bwkMail.RunWorkerAsync();
                    if (chkConPse.Checked)
                        bwkConEcollect.RunWorkerAsync();
                    if (chkMailRechazos.Checked)
                        bkwMailRechazos.RunWorkerAsync();
                    if (chkinter.Checked)
                        bkwMaillInt.RunWorkerAsync();

                    chkgenPse.Enabled = false;
                    chkConPse.Enabled = false;
                    chkSms.Enabled = false;
                    chkEmail.Enabled = false;
                    chkSeteo.Enabled = false;
                    chkMailRechazos.Enabled = false;
                    chkinter.Enabled = false;
                }
                
            }
        }
        /// <summary>
        /// inactiva los procesos masivos de pin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetener_Click(object sender, EventArgs e)
        {
            this.btnDetener.Enabled = false;
            this.btnActivar.Enabled = true;            
            this.txtLogBoton.Text = "Deteniendo procesos...";
            bolProcesos = false;

        }
        /// <summary>
        /// Activa la conciliación de Credipoliza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConciliacion_Click(object sender, EventArgs e)
        {
            if (iniciaProceso())
            {
                this.txtLogProcesos.Text = "";
                this.btnConciliacion.Enabled = false;
                bwkProcesamientoEntregas.RunWorkerAsync();
            }     
        }

        private bool busyWorkers()
        {
            if (bwkGenEcollect.IsBusy || bwkConEcollect.IsBusy || bwkMail.IsBusy || bwkSMS.IsBusy || bwkSeteo.IsBusy || bkwMailRechazos.IsBusy || bkwMaillInt.IsBusy)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Metodo que genera un mensaje con fecha en las cajas de teto de log
        /// </summary>
        /// <param name="txtObjeto"></param>
        /// <param name="mensaje"></param>
        /// <param name="agregar"></param>
        private void mensajeProcesos(RichTextBox txtObjeto, string mensaje, bool agregar)
        {
            if (agregar)
            {
                txtObjeto.Text += Environment.NewLine + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "= " + mensaje;
            }
            else
            {
                txtObjeto.Text = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "= " + mensaje;
            }

        }
        /// <summary>
        /// funcion de esepra de proceso
        /// </summary>
        /// <param name="milTimeOut"></param>
        /// <param name="txtObjeto"></param>
        private void Esperar(int milTimeOut, RichTextBox txtObjeto)
        {
            int mil = 0;
            while (mil < milTimeOut)
            {
                mil += 1000;
                if (mil % 1000 == 0)
                    mensajeProcesos(txtObjeto, "Esperando nueva iteración en " + ((milTimeOut / (double)1000) - (mil / (double)1000)).ToString() + " segundos...", false);
                System.Threading.Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// Evento que se activa al intentar cerrar la ventana
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmProcesoEntregas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (busyWorkers() || bwkProcesamientoEntregas.IsBusy)
            {
                e.Cancel = true;
                MessageBox.Show("No se puede cerrar la aplicación. Hay procesos en ejecución!!");
            }
        }
        private void resetarLog()
        {
            this.txtLogBoton.Text = "";
            chkgenPse.Enabled = true;
            chkConPse.Enabled = true;
            chkSms.Enabled = true;
            chkEmail.Enabled = true;
            chkSeteo.Enabled = true;
            chkinter.Enabled = true;
        }
        private bool validarHorario()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                Int16 horaIni = Convert.ToInt16(appSettings["param1"]);
                Int16 horaFin = Convert.ToInt16(appSettings["param2"]);
                Int16 horaActual = Convert.ToInt16(DateTime.Now.Hour);
                if (horaActual >= horaIni & horaActual <= horaFin)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los parámetros de hora de ejecución del proceso");
                return false;
            }
        }

        #endregion

        private void txtEcollectUrl_TextChanged(object sender, EventArgs e)
        {
            (sender as RichTextBox).SelectionStart = (sender as RichTextBox).Text.Length;
            (sender as RichTextBox).ScrollToCaret();
        }

        private void txtMail_TextChanged(object sender, EventArgs e)
        {
            (sender as RichTextBox).SelectionStart = (sender as RichTextBox).Text.Length;
            (sender as RichTextBox).ScrollToCaret();
        }

        private void txtEcollectCon_TextChanged(object sender, EventArgs e)
        {
            (sender as RichTextBox).SelectionStart = (sender as RichTextBox).Text.Length;
            (sender as RichTextBox).ScrollToCaret();
        }

        private void txtSMS_TextChanged(object sender, EventArgs e)
        {
            (sender as RichTextBox).SelectionStart = (sender as RichTextBox).Text.Length;
            (sender as RichTextBox).ScrollToCaret();
        }

        private void txtSeteo_TextChanged(object sender, EventArgs e)
        {
            (sender as RichTextBox).SelectionStart = (sender as RichTextBox).Text.Length;
            (sender as RichTextBox).ScrollToCaret();
        }

        private void txtLogProcesos_TextChanged(object sender, EventArgs e)
        {
            (sender as RichTextBox).SelectionStart = (sender as RichTextBox).Text.Length;
            (sender as RichTextBox).ScrollToCaret();
        }
        private void txtMailRech_TextChanged(object sender, EventArgs e)
        {
            (sender as RichTextBox).SelectionStart = (sender as RichTextBox).Text.Length;
            (sender as RichTextBox).ScrollToCaret();
        }
        private void txtMailInt_TextChanged(object sender, EventArgs e)
        {
            (sender as RichTextBox).SelectionStart = (sender as RichTextBox).Text.Length;
            (sender as RichTextBox).ScrollToCaret();
        }
        /// <summary>
        /// Activación automatica boton de conciliación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrConcil_Tick(object sender, EventArgs e)
        {
            
            if (DateTime.Now.Hour == 8 && DateTime.Now.Minute == 30)
            {
                btnConciliacion_Click(sender, e);
            }
            if (DateTime.Now.Hour == 12 && DateTime.Now.Minute == 0)
            {
                btnConciliacion_Click(sender, e);
            }
            if (DateTime.Now.Hour == 15 && DateTime.Now.Minute == 30)
            {
                btnConciliacion_Click(sender, e);
            }
            if (DateTime.Now.Hour == 19 && DateTime.Now.Minute == 0)
            {
                btnConciliacion_Click(sender, e);
            }
        }

        
    }
}
