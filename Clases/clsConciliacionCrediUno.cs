using dllRecaudos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace winEntregas.Clases
{
    public class clsConciliacionCrediUno
    {
        clsGeneral clsgeneral = new clsGeneral();
        public void validarAscard(string NumeroTarjeta, string idDetalle, string cConexionRecaudos)
        {
            string prefijo = NumeroTarjeta.Trim().Substring(0, 6);
            string numero = NumeroTarjeta.Trim().Substring(6);

            DataTable dtConsultaAscard = new DataTable();
            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {
                dt.nuevoParametro("@numero_tarjeta", numero, ParameterDirection.Input);
                dt.nuevoParametro("@prefijo_tarjeta", prefijo, ParameterDirection.Input);
                dtConsultaAscard = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_REGISTROS_ASCARD_PARA_CREDIUNO").Tables[0];
            }
            if (dtConsultaAscard.Rows.Count > 0)
            {
                cambiarEstadoDetalle(idDetalle, 4, cConexionRecaudos);
            }
            //--------------------------------------------------------
            else
            {
                if (NumeroTarjeta.Length < 23)
                {
                    NumeroTarjeta = NumeroTarjeta.PadLeft(23, '0');
                }
                else if (NumeroTarjeta.Length > 23)
                {
                    int posInicio = NumeroTarjeta.Length - 23;
                    NumeroTarjeta = NumeroTarjeta.Substring(posInicio, 23);
                }
                //Consulta Opencard
                //--------------------------------------------------------
                DataTable dtConsultaOraOpenCard = new DataTable();
                using (clsDatos dt = new clsDatos(cConexionRecaudos))
                {
                    dt.nuevoParametro("@numero_tarjeta", NumeroTarjeta, ParameterDirection.Input);
                    dtConsultaOraOpenCard = dt.ejecutar(CommandType.StoredProcedure, "CONSULTA_REGISTROS_ORAOPENCARD_PARA_CREDIUNO").Tables[0];
                }
                //--------------------------------------------------------
                if (dtConsultaOraOpenCard.Rows.Count > 0)
                {
                    cambiarEstadoDetalle(idDetalle, 4, cConexionRecaudos);
                }
                else
                {
                    cambiarEstadoDetalle(idDetalle, 3, cConexionRecaudos);
                }
            }
        }
        private void cambiarEstadoDetalle(string id_prc_det, Int32 codi_est_det, string cConexionRecaudos)
        {
            try
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
                        clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dt.retornaParametro("@db_desc_err").ToString(),
                        "winEntregas", "clsConciliacionCrediUno", "cambiarEstadoDetalle", Environment.Version.ToString(), id_prc_det);
                    }
                }
            }
            catch (Exception ex)
            {
                clsgeneral.registraErroresAplicaciones(cConexionRecaudos, ex.ToString(),
                "winEntregas", "clsConciliacionCrediUno", "cambiarEstadoDetalle", 
                Environment.Version.ToString(), id_prc_det);
            }
        }
    }
}
