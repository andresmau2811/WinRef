using dllRecaudos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winEntregas.Clases
{
    public class clsReferenciador
    {
        clsGeneral clsgeneral = new clsGeneral();
        public string dbConexion { get; set; }

        public clsReferenciador(string Conexion)
        {
            dbConexion = Conexion;
        }
        public string transferenciaMovimiento(string Cabecero, string detalle)
        {
            try
            {
                using (clsDatos dt = new clsDatos(dbConexion))
                {
                    dt.nuevoParametro("@dato_enc", Cabecero, ParameterDirection.Input);
                    dt.nuevoParametro("@dato_det", detalle, ParameterDirection.Input);
                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                    dt.ejecutar(CommandType.StoredProcedure, "SPCV_SWR_Movimientos_Insert");
                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                    {
                        return dt.retornaParametro("@db_desc_err").ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public Int64 validacionMovimientocargadoWs(string fechaRecaudo, string numeroCuneta, decimal valorRecaudo, string numeroReferencia)
        {
            try
            {
                using (clsDatos dt = new clsDatos(dbConexion))
                {
                    dt.nuevoParametro("@FechaRecaudo", fechaRecaudo, ParameterDirection.Input);
                    dt.nuevoParametro("@NumeroCuenta", numeroCuneta, ParameterDirection.Input);
                    dt.nuevoParametro("@numeroReferencia", numeroReferencia, ParameterDirection.Input);
                    dt.nuevoParametro("@ValorRecaudado", valorRecaudo, ParameterDirection.Input);
                    dt.nuevoParametro("@IdDetalle", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                    dt.ejecutar(CommandType.StoredProcedure, "SPCV_SWR_validar_Movimientos_Ws");
                    if (!dt.retornaParametro("@db_codi_err").ToString().Equals("0"))
                    {
                        return 0;
                    }
                    if (!dt.retornaParametro("@IdDetalle").ToString().Equals("0"))
                    {
                        return Convert.ToInt64(dt.retornaParametro("@IdDetalle").ToString());
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        public void actualizaMovimientoWs(Int64 IdDetalle, Int64 idRecaudo)
        {
            try
            {
                using (clsDatos dt = new clsDatos(dbConexion))
                {
                    dt.nuevoParametro("@IdDetalle", IdDetalle, ParameterDirection.Input);
                    dt.nuevoParametro("@idRecaudo", idRecaudo, ParameterDirection.Input);
                    dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                    dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output, 300);
                    dt.ejecutar(CommandType.StoredProcedure, "SPCV_SWR_actualizar_Movimientos_Ws");
                    if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                    {
                        throw new Exception(dt.retornaParametro("@db_desc_err").ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //public string transferenciaMovimiento(DataRow registro)
        //{
        //    try
        //    {
        //        using (clsDatos dt = new clsDatos(dbConexion))
        //        {
        //            dt.nuevoParametro("@FechaRecaudo", registro["fech_mov"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@NombreServicio", registro["NombreServicio"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@NombreArchivo", registro["nomb_ext"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@CodigoEntidadFinanciera", registro["ent_fin"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@CodigoBanco", registro["codi_ban"].ToString().Trim().PadLeft(3, '0'), ParameterDirection.Input);
        //            dt.nuevoParametro("@NumeroCuenta", registro["nume_cue"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@NumeroReferencia", registro["nume_ref_pri"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@NumeroReferenciaOriginal", registro["nume_ref_sec"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@ValorRecaudado", registro["val_tot"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@ValorEfectivo", registro["val_efe"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@ValorCheque", registro["val_che"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@ProcedenciaPago", registro["proc_pag"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@NumeroOperacion", registro["NumeroOperacion"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@NumeroAutorizacion", registro["NumeroAutorizacion"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@CodigoEntidadFinancieraDebitada", registro["CodigoEntidadFinancieraDebitada"].ToString().Trim().PadLeft(3, '0'), ParameterDirection.Input);
        //            dt.nuevoParametro("@CodigoSucursal", registro["codi_suc"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@Secuencia", registro["nume_lin"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@CausalDevolucion", registro["caus_rec"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@Reservado", registro["Reservado"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@IdEstado", registro["IdEstado"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@EstadoProceso", registro["EstadoProceso"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@CodigoEan", registro["CodigoEan"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@NumeroLote", registro["NumeroLote"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@CreateBy", registro["CreateBy"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@CreateOn", registro["CreateOn"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@ModifiedBy", registro["CreateBy"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@ModifiedOn", registro["CreateOn"].ToString(), ParameterDirection.Input);
        //            dt.nuevoParametro("@Activo", registro["Activo"].ToString(), ParameterDirection.Input);
        //            DataTable dtResultado = dt.ejecutar(CommandType.StoredProcedure, "SPCV_SWR_Transaccion_Insert").Tables[0];
        //            return dtResultado.Rows[0]["Mensaje"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //}
    }
}
