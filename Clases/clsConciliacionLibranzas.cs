using dllRecaudos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace winEntregas.Clases
{
    class clsConciliacionLibranzas
    {
        public string dbConexion { get; set; }
        public clsConciliacionLibranzas(string Conexion)
        {
            dbConexion = Conexion;
        }
        public void conciliarCliente(string documento, decimal valor, long id_detalle)
        {
            bool encontradoSIIF = false;
            //______________________________________________________________________________________________________________
            DataTable dtconsultaCrediValoresSIIF = new DataTable();
            using (clsDatos dt = new clsDatos(dbConexion))
            {
                dt.nuevoParametro("@NIT", DBNull.Value, ParameterDirection.Input);
                dt.nuevoParametro("@CODIGO_EMPRESA", DBNull.Value, ParameterDirection.Input);
                dt.nuevoParametro("@NRO_CREDITO", DBNull.Value, ParameterDirection.Input);
                dt.nuevoParametro("@NRO_IDENTIFICACION", documento, ParameterDirection.Input);
                DataSet dtsDatos = new DataSet();
                dtsDatos = dt.ejecutar(CommandType.StoredProcedure, "SP_CarteraLibranzaCredivaloresSIIF");
                if (dtsDatos.Tables.Count > 0)
                {
                    dtconsultaCrediValoresSIIF = dtsDatos.Tables[0];
                }

            }
            //______________________________________________________________________________________________________________
            if (dtconsultaCrediValoresSIIF.Rows.Count > 0)
            {
                bool creditoCancelado_CV = true;
                int igualValor_CV = 0;
                int contadorRegistros_CV = 0;
                int posConciliacion_CV = 0;
                foreach (DataRow fila in dtconsultaCrediValoresSIIF.Rows)
                {
                    if (!fila["ESTADO_CREDITO"].ToString().Trim().Equals("CANCELADO"))
                    {
                        creditoCancelado_CV = false;
                        if (Convert.ToDecimal(fila["VALOR_CUOTA"].ToString()) == valor)
                        {
                            igualValor_CV++;
                            posConciliacion_CV = contadorRegistros_CV;
                        }
                    }
                    contadorRegistros_CV++;
                }

                if (igualValor_CV == 1)//Si esta en 1 se puede hacer la conciliación automática por que tiene un solo registro vigente y el valor es igual al reportado en el archivo
                {
                    registrarConciliacion(dtconsultaCrediValoresSIIF.Rows[posConciliacion_CV], id_detalle, "CV");
                    encontradoSIIF = true;
                    return;
                }

                foreach (DataRow fila in dtconsultaCrediValoresSIIF.Rows)
                {
                    importarDataSIIF(fila, id_detalle, "CV", 3, 2);
                    encontradoSIIF = true;
                }
            }
            //if (creditoCancelado_CV == true)//Si esta true es por que todos los registros en CV están cancelados
            //{
            DataTable dtconsultaCrediFinancieraSIIF = new DataTable();
            using (clsDatos dt = new clsDatos(dbConexion))
            {
                dt.nuevoParametro("@NIT", DBNull.Value, ParameterDirection.Input);
                dt.nuevoParametro("@CODIGO_EMPRESA", DBNull.Value, ParameterDirection.Input);
                dt.nuevoParametro("@NRO_CREDITO", DBNull.Value, ParameterDirection.Input);
                dt.nuevoParametro("@NRO_IDENTIFICACION", documento, ParameterDirection.Input);
                DataSet dtsDatos = new DataSet();
                dtsDatos = dt.ejecutar(CommandType.StoredProcedure, "SP_CarteraLibranzaCredifinancieraSIIF");
                if (dtsDatos.Tables.Count > 0)
                {
                    dtconsultaCrediValoresSIIF = dtsDatos.Tables[0];
                }
            }
            if (dtconsultaCrediFinancieraSIIF.Rows.Count > 0)
            {
                foreach (DataRow fila in dtconsultaCrediFinancieraSIIF.Rows)
                {
                    importarDataSIIF(fila, id_detalle, "CF", 3, 2);
                    encontradoSIIF = true;
                }
                //bool creditoCancelado_CF = true;
                //int igualValor_CF = 0;
                //int contadorRegistros_CF = 0;
                //int posConciliacion_CF = 0;
                //foreach (DataRow fila in dtconsultaCrediFinancieraSIIF.Rows)
                //{
                //    if (!fila["ESTADO_CREDITO"].ToString().Trim().Equals("CANCELADO"))
                //    {
                //        creditoCancelado_CF = false;
                //        if (Convert.ToDouble(fila["VALOR_CUOTA"].ToString()) == valor)
                //        {
                //            igualValor_CF++;
                //            posConciliacion_CF = contadorRegistros_CF;
                //        }
                //    }
                //    contadorRegistros_CF++;
                //}
            }
            //}
            if (encontradoSIIF == false)
            {
                using (clsDatos dt = new clsDatos(dbConexion))
                {
                    dt.nuevoParametro("@id_lib_det", id_detalle, ParameterDirection.Input);
                    dt.nuevoParametro("@codi_est_det", 2, ParameterDirection.Input);
                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                    dt.ejecutar(CommandType.StoredProcedure, "REGISTRA_ESTADO_DETALLE_EMPRESA");
                }
            }
        }

        private void registrarConciliacion(DataRow fila, long id_detalle, string origen)
        {
            using (clsDatos dt = new clsDatos(dbConexion))
            {
                dt.nuevoParametro("@COD_PRODUCTO", fila["COD_PRODUCTO"], ParameterDirection.Input);
                dt.nuevoParametro("@PRODUCTO", fila["PRODUCTO"], ParameterDirection.Input);
                dt.nuevoParametro("@EMPRESA", fila["EMPRESA"], ParameterDirection.Input);
                dt.nuevoParametro("@NIT", fila["NIT"], ParameterDirection.Input);
                dt.nuevoParametro("@CODIGO_EMPRESA", fila["CODIGO_EMPRESA"], ParameterDirection.Input);
                dt.nuevoParametro("@NRO_IDENTIFICACION", fila["NRO_IDENTIFICACION"], ParameterDirection.Input);
                dt.nuevoParametro("@NOMBRES", fila["NOMBRES"], ParameterDirection.Input);
                dt.nuevoParametro("@APELLIDOS", fila["APELLIDOS"], ParameterDirection.Input);
                dt.nuevoParametro("@NRO_CREDITO", fila["NRO_CREDITO"], ParameterDirection.Input);
                dt.nuevoParametro("@VALOR_CUOTA", fila["VALOR_CUOTA"], ParameterDirection.Input);
                dt.nuevoParametro("@CIUDAD", fila["CIUDAD"], ParameterDirection.Input);
                dt.nuevoParametro("@MORA_PROYECTADA", fila["MORA_PROYECTADA"], ParameterDirection.Input);
                dt.nuevoParametro("@ESTADO_CREDITO", fila["ESTADO_CREDITO"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_CANCELACION_CREDITO", fila["FECHA_CANCELACION_CREDITO"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_PRIMERA_CUOTA", fila["FECHA_PRIMERA_CUOTA"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_APROBACION", fila["FECHA_APROBACION"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_VENCIMIENTO", fila["FECHA_VENCIMIENTO"], ParameterDirection.Input);
                dt.nuevoParametro("@DIA_FACTURACION", fila["DIA_FACTURACION"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_FACTURA_INICIAL",DBNull.Value, ParameterDirection.Input);// fila["FECHA_FACTURA_INICIAL"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_FACTURA_MAXIMA", DBNull.Value, ParameterDirection.Input); //fila["FECHA_FACTURA_MAXIMA"], ParameterDirection.Input);
                dt.nuevoParametro("@VALOR_VENCIDO_MAS_MORA_PROYECT", fila["VALOR_VENCIDO_MAS_MORA_PROYECT"], ParameterDirection.Input);
                dt.nuevoParametro("@SALDO_CAPITAL", fila["SALDO_CAPITAL"], ParameterDirection.Input);
                dt.nuevoParametro("@id_lib_det", id_detalle, ParameterDirection.Input);
                dt.nuevoParametro("@orig_inf", origen, ParameterDirection.Input);
                dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                dt.ejecutar(CommandType.StoredProcedure, "REGISTRA_CONCILIACION_AUT_LIBRANZAS");

            }
        }
        private void importarDataSIIF(DataRow fila, long id_detalle, string origen, int estadoDataEmpresa, int estadoDataSIFF)
        {
            using (clsDatos dt = new clsDatos(dbConexion))
            {
                dt.nuevoParametro("@COD_PRODUCTO", fila["COD_PRODUCTO"], ParameterDirection.Input);
                dt.nuevoParametro("@PRODUCTO", fila["PRODUCTO"], ParameterDirection.Input);
                dt.nuevoParametro("@EMPRESA", fila["EMPRESA"], ParameterDirection.Input);
                dt.nuevoParametro("@NIT", fila["NIT"], ParameterDirection.Input);
                dt.nuevoParametro("@CODIGO_EMPRESA", fila["CODIGO_EMPRESA"], ParameterDirection.Input);
                dt.nuevoParametro("@NRO_IDENTIFICACION", fila["NRO_IDENTIFICACION"], ParameterDirection.Input);
                dt.nuevoParametro("@NOMBRES", fila["NOMBRES"], ParameterDirection.Input);
                dt.nuevoParametro("@APELLIDOS", fila["APELLIDOS"], ParameterDirection.Input);
                dt.nuevoParametro("@NRO_CREDITO", fila["NRO_CREDITO"], ParameterDirection.Input);
                dt.nuevoParametro("@VALOR_CUOTA", fila["VALOR_CUOTA"], ParameterDirection.Input);
                dt.nuevoParametro("@CIUDAD", fila["CIUDAD"], ParameterDirection.Input);
                dt.nuevoParametro("@MORA_PROYECTADA", fila["MORA_PROYECTADA"], ParameterDirection.Input);
                dt.nuevoParametro("@ESTADO_CREDITO", fila["ESTADO_CREDITO"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_CANCELACION_CREDITO", fila["FECHA_CANCELACION_CREDITO"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_PRIMERA_CUOTA", fila["FECHA_PRIMERA_CUOTA"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_APROBACION", fila["FECHA_APROBACION"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_VENCIMIENTO", fila["FECHA_VENCIMIENTO"], ParameterDirection.Input);
                dt.nuevoParametro("@DIA_FACTURACION", fila["DIA_FACTURACION"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_FACTURA_INICIAL", DBNull.Value, ParameterDirection.Input);// fila["FECHA_FACTURA_INICIAL"], ParameterDirection.Input);
                dt.nuevoParametro("@FECHA_FACTURA_MAXIMA", DBNull.Value, ParameterDirection.Input); //fila["FECHA_FACTURA_MAXIMA"], ParameterDirection.Input);
                dt.nuevoParametro("@VALOR_VENCIDO_MAS_MORA_PROYECT", fila["VALOR_VENCIDO_MAS_MORA_PROYECT"], ParameterDirection.Input);
                dt.nuevoParametro("@SALDO_CAPITAL", fila["SALDO_CAPITAL"], ParameterDirection.Input);
                dt.nuevoParametro("@id_lib_det", id_detalle, ParameterDirection.Input);
                dt.nuevoParametro("@codi_est_dat", estadoDataSIFF, ParameterDirection.Input);
                dt.nuevoParametro("@codi_est_det", estadoDataEmpresa, ParameterDirection.Input);
                dt.nuevoParametro("@orig_inf", origen, ParameterDirection.Input);
                dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                dt.ejecutar(CommandType.StoredProcedure, "REGISTRA_CONCILIACION_DATA_LIBRANZAS");

            }
        }
    }
}
