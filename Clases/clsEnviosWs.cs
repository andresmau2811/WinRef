using dllRecaudos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace winRef.Clases
{
    public class clsEnviosWs
    {

        public string consumirSMS(string cliente, string celular, string apellido, string pin, string valor,
           string vigencia, string UrlPse, int intentos_pse, string cadenaCon, DataTable dtConf, out string error)
        {
            string result = "false";
            string soapAction = "";
            string metodoSMS = "";
            string urlSMS = "";
            string mensaje = "";
            string token = "";
            error = "";
            try
            {
                if (dtConf.Rows.Count > 0)
                {
                    urlSMS = dtConf.Rows[0]["url"].ToString();
                    token = dtConf.Rows[0]["token"].ToString();
                    mensaje = "Sr(A) " + apellido + ", puede realizar el pago de la primera cuota de la financiación de su póliza";

                    mensaje = mensaje + "." + "Para pago en Bancolombia o Banco de Bogotá, descargue o imprima el Cupón enviado por correo electrónico. PIN No. " + pin + " por valor " + valor;

                    if (UrlPse != "")
                    {
                        mensaje = mensaje + ". A través de PSE dando click en el siguiente enlace ";
                    }

                    if (UrlPse == "" || intentos_pse == 4)
                    {
                        soapAction = "http://tempuri.org/IWSCentralMensajes/SendSms";
                        metodoSMS = "SendSms";
                    }
                    else
                    {
                        soapAction = "http://tempuri.org/IWSCentralMensajes/SendSmsLongUrl";
                        metodoSMS = "SendSmsLongUrl";
                    }

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlSMS);
                    request.Method = "POST";
                    request.ContentType = "text/xml";
                    request.Headers.Add("SOAPAction", soapAction);
                    request.Headers.Add("token", token);

                    XmlDocument SOAPReqBody = new XmlDocument();
                    //SOAP Body Request  
                    if (metodoSMS == "SendSms")
                    {
                        SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>            
                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:cm=""http://schemas.datacontract.org/2004/07/CM.Entity"">
                         <soapenv:Header/>
                       <soapenv:Body>
                          <tem:SendSms>
                             <!--Optional:-->
                             <tem:smsRequest>
                                <!--Optional:-->
                                <cm:ClientDocument>" + cliente + @"</cm:ClientDocument>
                                <!--Optional:-->
                                <cm:Message>" + mensaje + @"</cm:Message>
                                <!--Optional:-->
                                <cm:Mobile>" + celular + @"</cm:Mobile>
                                <!--Optional:-->
                                <cm:Operator>57</cm:Operator>
                             </tem:smsRequest>
                          </tem:SendSms>
                       </soapenv:Body>
                    </soapenv:Envelope>");
                    }
                    else
                    {
                        SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>            
                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:cm=""http://schemas.datacontract.org/2004/07/CM.Entity"">
                         <soapenv:Header/>
                       <soapenv:Body>
                          <tem:SendSmsLongUrl>
                             <!--Optional:-->
                             <tem:smsRequestLongUrl>
                                <!--Optional:-->
                                <cm:ClientDocument>" + cliente + @"</cm:ClientDocument>
                                <!--Optional:-->
                                <cm:LongUrl>" + UrlPse + @"</cm:LongUrl>
                                <!--Optional:-->
                                <cm:Message>" + mensaje + @"</cm:Message>
                                <!--Optional:-->
                                <cm:Mobile>" + celular + @"</cm:Mobile>
                                <!--Optional:-->
                                <cm:Operator>57</cm:Operator>
                             </tem:smsRequestLongUrl>
                          </tem:SendSmsLongUrl>
                       </soapenv:Body>
                    </soapenv:Envelope>");
                    }

                    using (Stream stream = request.GetRequestStream())
                    {
                        SOAPReqBody.Save(stream);
                    }

                    using (WebResponse Serviceres = request.GetResponse())
                    {
                        using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                        {
                            string ServiceResult = rd.ReadToEnd();

                            XmlDocument xmltest = new XmlDocument();
                            xmltest.LoadXml(ServiceResult);

                            XmlNodeList elemlist = xmltest.GetElementsByTagName("a:MessageSendOK");//true

                            if (elemlist.Count > 0)
                            {
                                result = elemlist[0].InnerXml;
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos de configuración para enviar el mensaje SMS");
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                result = "error";
            }
            try
            {
                registraLogSMSMail(cadenaCon, 2, "", "", "", "", "", "", error, cliente, celular, mensaje, urlSMS, soapAction);
            }
            catch (Exception e) { }


            return result;
        }

        private string EnviarEmailDirecto(string envia, string destinatario, string asunto, string cuerpo,
                                         string nombreArchivo, string archivo, string host_mail, string pwds_mail, string port_mail,
                                         out string error)
        {
            error = "";
            string resultado = "";
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = host_mail;
                smtp.Port = int.Parse(port_mail);
                smtp.EnableSsl = false; //poner en false al publicar
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(envia, pwds_mail);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage email = new MailMessage();
                email.To.Add(new MailAddress(destinatario));
                email.From = new MailAddress(envia);
                email.Subject = asunto;
                email.IsBodyHtml = true;
                email.Body = cuerpo;
                email.Priority = MailPriority.Normal;

                var bytes = Convert.FromBase64String(archivo);
                MemoryStream strm = new MemoryStream(bytes);
                Attachment data = new Attachment(strm, nombreArchivo);
                email.Attachments.Add(data);

                smtp.Send(email);
                email.Dispose();
                resultado = "true";
            }
            catch (Exception E)
            {
                resultado = "false";
                error = E.ToString();
            }
            return resultado;
        }

        private string EnviarEmailDirectoNoAdj(string envia, string destinatario, string asunto, string cuerpo,
                                         string host_mail, string pwds_mail, string port_mail,
                                         out string error)
        {
            error = "";
            string resultado = "";
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = host_mail;
                smtp.Port = int.Parse(port_mail);
                smtp.EnableSsl = false; 
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(envia, pwds_mail);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage email = new MailMessage();
                email.To.Add(new MailAddress(destinatario));
                email.From = new MailAddress(envia);
                email.Subject = asunto;
                email.IsBodyHtml = true;
                email.Body = cuerpo;
                email.Priority = MailPriority.Normal;

                smtp.Send(email);
                email.Dispose();
                resultado = "true";
            }
            catch (Exception E)
            {
                resultado = "false";
                error = E.ToString();
            }
            return resultado;
        }


        public string consumirMail(string cliente, string destinatario, string pin_ref, string archivo,
                                    string archivoBase64, string apellido, string valor, string vigencia,
                                    string urlPse, int intentos_pse, string cadenaCon, DataTable dtConf, out string error)
        {
            string result = "false";
            string html = "true";
            string body = "";
            string envia = "";
            string asunto = "";
            string token = "";
            string url = "";
            string host_mail = "";
            string pwds_mail = "";
            string port_mail = "";

            error = "";
            try
            {
                if (dtConf.Rows.Count > 0)
                {
                    envia = dtConf.Rows[0]["sender"].ToString();
                    asunto = dtConf.Rows[0]["asunto"].ToString();
                    token = dtConf.Rows[0]["token"].ToString();
                    url = dtConf.Rows[0]["url"].ToString();
                    host_mail = dtConf.Rows[0]["host_mail"].ToString();
                    pwds_mail = dtConf.Rows[0]["pwds_mail"].ToString();
                    port_mail = dtConf.Rows[0]["port_mail"].ToString();
                    body = "Buen día Sr(A) <b>" + apellido + "</b><br/><br/>";
                    body += "Le Informamos que a partir de este momento puede realizar el pago de la primera cuota correspondiente ";
                    body += "a la financiación de la Póliza de seguro bajo el Pin de Recaudo No. <b>" + pin_ref + "</b> por valor <b>" + valor;
                    body += "</b>.<br/><br/> Descargue e imprima el Cupón adjunto y preséntelo en las oficinas y puntos de recaudo de nuestros aliados: ";
                    body += " Bancolombia, Banco de Bogotá, o corresponsales Bancarios del grupo Casino: Éxito, Pomona, Carulla.<br/><br/>";
                    if (intentos_pse == 4 || urlPse == "")
                    {
                        body = body + "";
                    }
                    else
                    {
                        body = body + "Para pagos por PSE – Electrónico debe ingresar al siguiente link <u>" + urlPse + "</u> <br/>";
                    }

                    body += "Recuerde que este proceso es indispensable para efectuar el desembolso ante la Aseguradora, por lo cual le";
                    body += " recomendamos realizar el pago lo antes posible <br/><br/>";
                    body += " Gracias por preferirnos… <br/> Gerencia de Producto <br/> Credivalores – Crediservicios SAS";

                    result = EnviarEmailDirecto(envia, destinatario, asunto, body, archivo, archivoBase64, host_mail, pwds_mail, port_mail, out error);
                    try
                    {
                        registraLogSMSMail(cadenaCon, 1, envia, destinatario, asunto, body, archivo, host_mail, error, "", "", "", "", "");
                    }
                    catch (Exception e) { }

                    if (error != "")
                    {
                        throw new Exception(error);
                    }
                }
            }
            catch (Exception e)
            {
				registraLogSMSMail(cadenaCon, 1, envia, destinatario, asunto, body, archivo, host_mail, e.Message, "", "", "", "", "");
				error = e.ToString();
                result = "error";
            }

            return result;
        }





        public string seteoBizagi(string idCase, string SEstadoRecibido, string tipoPagoCi, decimal valorAplicado, out string error)
        {
            string result = "false";
            try
            {
                error = "";
                string error2 = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["urlBizagi"].ToString());
                request.Method = "POST";
                request.ContentType = "text/xml";
                request.Headers.Add("SOAPAction", "http://tempuri.org/performActivity");
                SEstadoRecibido = "0" + SEstadoRecibido;

                XmlDocument SOAPReqBody = new XmlDocument();
                //SOAP Body Request  
                SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>            
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
				   <soapenv:Header/>
				   <soapenv:Body>
					  <tem:performActivity>
						 <tem:activityInfo>
						   <BizAgiWSParam>
							   <domain>domain</domain>
							   <userName>admon</userName>
							   <ActivityData>
								  <idCase>" + idCase + @"</idCase>
								  <taskName>ConfirmacionPagoPIN</taskName>
							   </ActivityData>
							   <Entities>
								  <CAT_OriginarCrediPoliza>
									 <idM_InformacionPIN>
										<SEstadoRecibido>" + SEstadoRecibido + @"</SEstadoRecibido>   
                                        <CValorAplicado>" + valorAplicado + @"</CValorAplicado>     
									 </idM_InformacionPIN >
								  </CAT_OriginarCrediPoliza>
							   </Entities>
							</BizAgiWSParam>
						 </tem:activityInfo>
					  </tem:performActivity>
				   </soapenv:Body>
				</soapenv:Envelope>");
                using (Stream stream = request.GetRequestStream())
                {
                    SOAPReqBody.Save(stream);
                }

                using (WebResponse Serviceres = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                    {
                        string ServiceResult = rd.ReadToEnd();

                        XmlDocument xmltest = new XmlDocument();
                        xmltest.LoadXml(ServiceResult);

                        result = ServiceResult;

                        XmlNodeList elemlist = xmltest.GetElementsByTagName("errorCode");//errorCode

                        if (elemlist.Count > 0)
                        {
                            error = elemlist[0].InnerXml;
                            if (error == "1" || error == "")
                            {
                                result = "true";
                                error = "";

                            }
                            else
                            {
                                result = "false";
                                error = "errorCode: " + error;
                                XmlNodeList elemlist2 = xmltest.GetElementsByTagName("errorMessage");
                                if (elemlist2.Count > 0)
                                {
                                    error2 = elemlist2[0].InnerXml;
                                    error = error + " errorMessage: " + error2;
                                }
                            }

                        }

                    }
                }
            }
            catch (Exception e)
            {
                result = "error";
                error = e.Message;
            }
            return result;


        }
        /// <summary>
        /// Crea un ticket de pago
        /// </summary>
        /// <param name="drPin"></param>
        /// <param name="drConf"></param>
        /// <param name="wsErr"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public WsEcollect.createTransactionResponseType consumirWsEcollect(DataRow drPin, DataRow drConf, out string wsErr, out string sign)
        {
            try
            {
                string md5 = "";
                wsErr = "";

                WsEcollect.eCollectWebservicesv3SoapClient wsCliente = new WsEcollect.eCollectWebservicesv3SoapClient();
                wsCliente.Endpoint.Address = new EndpointAddress(drConf["UrlECollectWs"].ToString());
                WsEcollect.createTransactionType request = new WsEcollect.createTransactionType();
                WsEcollect.createTransactionResponseType response = new WsEcollect.createTransactionResponseType();
                request.EntityCode = drConf["EntityCodePSE"].ToString();
                request.SrvCode = drConf["SrvCodePSE"].ToString();
                request.TransValue = decimal.Parse(drPin["Valor_Cuota"].ToString());
                request.TransVatValue = 0;
                request.URLRedirect = drConf["UrlRedirectPSE"].ToString();
                request.PaymentSystem = drConf["PaymentSystem"].ToString();

                using (MD5 md5Hash = MD5.Create())
                {
                    md5 = GetMd5Hash(md5Hash, drPin["Numero_Referencia"].ToString());
                }
                request.Sign = md5;
                request.SignFields = "Referencia";

                sign = request.Sign;

                request.ReferenceArray = new string[7];
                request.ReferenceArray[0] = drPin["Numero_Documento"].ToString();
                request.ReferenceArray[1] = drPin["Numero_Referencia"].ToString();
                request.ReferenceArray[2] = drPin["Nombres"].ToString() + " " + drPin["Apellidos"].ToString();
                request.ReferenceArray[3] = drPin["Tipo_documento"].ToString();
                request.ReferenceArray[4] = drPin["Direccion_Ubicacion"].ToString();
                request.ReferenceArray[5] = drPin["Telefono1"].ToString();
                request.ReferenceArray[6] = drPin["Email1"].ToString();

                response = wsCliente.createTransactionPayment(request);

                return response;

            }
            catch (Exception e)
            {
                wsErr = e.ToString();
                sign = "";
                return null;
            }

        }
        /// <summary>
        /// Valida la información de un Ticket Creado
        /// </summary>
        /// <param name="drPin"></param>
        /// <param name="drConf"></param>
        /// <param name="wsErr"></param>
        /// <returns></returns>
        public WsEcollect.getTransactionInformationResponseType infoWsEcollect(DataRow drPin, DataRow drConf, out string wsErr)
        {
            try
            {
                wsErr = "";

                WsEcollect.eCollectWebservicesv3SoapClient wsCliente = new WsEcollect.eCollectWebservicesv3SoapClient();
                wsCliente.Endpoint.Address = new EndpointAddress(drConf["UrlECollectWs"].ToString());
                WsEcollect.getTransactionInformationType wsRequestInfo = new WsEcollect.getTransactionInformationType();
                WsEcollect.getTransactionInformationResponseType wsResponseInfo = new WsEcollect.getTransactionInformationResponseType();
                wsRequestInfo.TicketId = drPin["Ticket_id"].ToString();
                wsRequestInfo.EntityCode = drConf["EntityCodePSE"].ToString();
                wsResponseInfo = wsCliente.getTransactionInformation(wsRequestInfo);

                return wsResponseInfo;

            }
            catch (Exception e)
            {
                wsErr = e.ToString();
                return null;
            }

        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Metodo que envio un correo de rechazo por pago NOT_AUTHORIZED
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="destinatario"></param>
        /// <param name="pin_ref"></param>
        /// <param name="archivo"></param>
        /// <param name="archivoBase64"></param>
        /// <param name="apellido"></param>
        /// <param name="valor"></param>
        /// <param name="vigencia"></param>
        /// <param name="urlPse"></param>
        /// <param name="intentos_pse"></param>
        /// <param name="cadenaCon"></param>
        /// <param name="dtConf"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string consumirMailRechazoEcollect(string cliente, string destinatario, string pin_ref, 
                                    string apellido, string valor, string vigencia,
                                    string cadenaCon, DataTable dtConf, out string error)
        {
            string result = "false";
            string html = "true";
            string body = "";
            string envia = "";
            string asunto = "";
            string token = "";
            string url = "";
            string host_mail = "";
            string pwds_mail = "";
            string port_mail = "";

            error = "";
            try
            {
                if (dtConf.Rows.Count > 0)
                {
                    envia = dtConf.Rows[0]["sender"].ToString();
                    asunto = dtConf.Rows[0]["asunto"].ToString();
                    token = dtConf.Rows[0]["token"].ToString();
                    url = dtConf.Rows[0]["url"].ToString();
                    host_mail = dtConf.Rows[0]["host_mail"].ToString();
                    pwds_mail = dtConf.Rows[0]["pwds_mail"].ToString();
                    port_mail = dtConf.Rows[0]["port_mail"].ToString();
                    body = "Sr(A) <b>" + apellido + ", </b><br/><br/>";
                    body += "su transacción para pago por PSE fue rechazada, en el transcurso de unos minutos ";
                    body += "recibirá un correo y mensaje de texto, con un nuevo enlace para que pueda realizar nuevamente el ";
                    body += "proceso de pago de la primera cuota de su póliza por PSE, con el PIN No. <b>" + pin_ref + "</b>";
                    body += "  por valor de <b> "+ valor + "</b><br/><br/>";

                    body += " Gracias por preferirnos… <br/> Gerencia de Producto <br/> Credivalores – Crediservicios SAS";

                    result = EnviarEmailDirectoNoAdj(envia, destinatario, asunto + " RECHAZO DE PAGO", body, host_mail, pwds_mail, port_mail, out error);
                    try
                    {
                        registraLogSMSMail(cadenaCon, 1, envia, destinatario, asunto + " RECHAZO DE PAGO", body, "", host_mail, error, "", "", "", "", "");
                    }
                    catch (Exception e) { }

                    if (error != "")
                    {
                        throw new Exception(error);
                    }
                }
            }
            catch (Exception e)
            {
                error = e.ToString();
                result = "error";
            }

            return result;
        }
        /// <summary>
        /// Proceso de envio de mail a intermediarios
        /// </summary>
        /// <param name="destinatario"></param>
        /// <param name="archivo"></param>
        /// <param name="archivoBase64"></param>
        /// <param name="nombres"></param>
        /// <param name="body"></param>
        /// <param name="cadenaCon"></param>
        /// <param name="dtConf"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string consumirMailIntermediario(string destinatario, string archivo,
                                   string archivoBase64, string nombres, string body,
                                   string cadenaCon, DataTable dtConf, out string error)
        {
            string result = "false";
            string envia = "";
            string asunto = "";
            string token = "";
            string url = "";
            string host_mail = "";
            string pwds_mail = "";
            string port_mail = "";

            error = "";
            try
            {
                if (dtConf.Rows.Count > 0)
                {
                    envia = dtConf.Rows[0]["sender"].ToString();
                    asunto = dtConf.Rows[0]["asunto"].ToString();
                    token = dtConf.Rows[0]["token"].ToString();
                    url = dtConf.Rows[0]["url"].ToString();
                    host_mail = dtConf.Rows[0]["host_mail"].ToString();
                    pwds_mail = dtConf.Rows[0]["pwds_mail"].ToString();
                    port_mail = dtConf.Rows[0]["port_mail"].ToString();

                    body = body.Replace("[NOMBRE_DESTINATARIO]", nombres);

                    result = EnviarEmailDirecto(envia, destinatario, asunto, body, archivo, archivoBase64, host_mail, pwds_mail, port_mail, out error);
                    try
                    {
                        registraLogSMSMail(cadenaCon, 1, envia, destinatario, asunto + " ENVIO A INTERMEDIARIO", body, archivo, host_mail, error, "", "", "", "", "");
                    }
                    catch (Exception e) { }

                    if (error != "")
                    {
                        throw new Exception(error);
                    }
                }
            }
            catch (Exception e)
            {
                registraLogSMSMail(cadenaCon, 1, envia, destinatario, asunto, body, archivo, host_mail, e.Message, "", "", "", "", "");
                error = e.ToString();
                result = "error";
            }

            return result;
        }

        /// <summary>
        /// Registra el log de Mail o SMS
        /// </summary>
        /// <param name="cConexionRecaudos"></param>
        /// <param name="tipo_log"></param>
        /// <param name="envia_mail"></param>
        /// <param name="dest_mail"></param>
        /// <param name="asunto_mail"></param>
        /// <param name="cuerpo_mail"></param>
        /// <param name="narchivo_mail"></param>
        /// <param name="barchivo_mail"></param>
        /// <param name="error_mail"></param>
        /// <param name="cliente_sms"></param>
        /// <param name="celular_sms"></param>
        /// <param name="mensaje_sms"></param>
        /// <param name="url_sms"></param>
        /// <param name="soap_sms"></param>
        public void registraLogSMSMail(string cConexionRecaudos, int tipo_log, string envia_mail, string dest_mail, string asunto_mail
                                        , string cuerpo_mail, string narchivo_mail, string host_mail, string error_mail, string cliente_sms
                                        , string celular_sms, string mensaje_sms, string url_sms, string soap_sms)
        {
            DataTable dtRespuesta = new DataTable();
            clsGeneral clsgeneral = new clsGeneral();

            using (clsDatos dt = new clsDatos(cConexionRecaudos))
            {

                dt.nuevoParametro("@tipo_log", tipo_log, ParameterDirection.Input);
                dt.nuevoParametro("@envia_mail", envia_mail, ParameterDirection.Input);     
                dt.nuevoParametro("@dest_mail", dest_mail, ParameterDirection.Input);
                dt.nuevoParametro("@asunto_mail", asunto_mail, ParameterDirection.Input); 
                dt.nuevoParametro("@cuerpo_mail", cuerpo_mail, ParameterDirection.Input);
                dt.nuevoParametro("@narchivo_mail", narchivo_mail, ParameterDirection.Input);
                dt.nuevoParametro("@host_mail", host_mail, ParameterDirection.Input);
                dt.nuevoParametro("@error_mail", error_mail, ParameterDirection.Input);
                dt.nuevoParametro("@cliente_sms", cliente_sms, ParameterDirection.Input);
                dt.nuevoParametro("@celular_sms", celular_sms, ParameterDirection.Input);
                dt.nuevoParametro("@mensaje_sms", mensaje_sms, ParameterDirection.Input);
                dt.nuevoParametro("@url_sms", url_sms, ParameterDirection.Input);
                dt.nuevoParametro("@soap_sms", soap_sms, ParameterDirection.Input);  
                dt.nuevoParametro("@db_codi_err", 0, ParameterDirection.Output);
                dt.nuevoParametro("@db_desc_err", "", ParameterDirection.Output);
                dtRespuesta = dt.ejecutar(CommandType.StoredProcedure, "REGISTRA_LOG_SMS_MAIL").Tables[0];
                if (dt.retornaParametro("@db_codi_err").ToString() != "0")
                {
                    clsgeneral.registraErroresAplicaciones(cConexionRecaudos, dtRespuesta.Rows[0][1].ToString(), "WinRef", "clsEnviosWs.cs", "registraLogSMSMail", "", "0");
                }
            }
        }
    }
}
