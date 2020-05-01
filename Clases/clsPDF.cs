using System;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Windows.Forms;
using System.Data;
using GD.Soap.Bussines;

namespace winRef.Clases
{
    public class clsPDF
    {
        public void generarCuponNormal(DataRow dr, string gs1, out string error)
        {
            error = "";
            try
            {
                string formatofuente = Application.StartupPath;
                formatofuente = formatofuente.Replace("\\bin\\Debug", "");
                string codebar = "";
                string codebarText = "";
                int i = 0;

                PdfReader reader = new PdfReader(formatofuente + "\\pdf\\ModeloCupon.pdf");
                PdfStamper stamper;
                stamper = new PdfStamper(reader, new FileStream(formatofuente + "\\pdf\\tmp\\cupon_" + dr["Numero_Referencia"].ToString() + ".pdf", FileMode.Create));
                AcroFields fields = stamper.AcroFields;
                fields.SetField("nombres", dr["Nombres"].ToString() + " " + dr["Apellidos"].ToString());
                fields.SetField("documento", dr["Numero_Documento"].ToString());
                fields.SetField("referencia", dr["Numero_Referencia"].ToString());
                fields.SetField("fecha_vencimiento", "PROXIMAS 48 HORAS");
                fields.SetField("valor_pagar", dr["Valor_Cuota"].ToString());
                fields.SetField("texto1", Convert.ToString(""));
                fields.SetField("texto2", Convert.ToString(""));
                fields.SetField("tipo", dr["Tipo_documento"].ToString());
                fields.SetField("x1", Convert.ToString(""));
                fields.SetField("x2", Convert.ToString(""));

                codebar = "(415)" + gs1 + "(8020)" + dr["Numero_Referencia"].ToString() + "(3900)" + rellenaCeros(dr["valor_noformat"].ToString(), 13) + "(96)" + dr["Fecha_actual"].ToString();
                codebarText = "415" + gs1 + "8020" + dr["Numero_Referencia"].ToString() + "3900" + rellenaCeros(dr["valor_noformat"].ToString(), 13) + "96" + dr["Fecha_actual"].ToString();

                string ls_codigoConSilencios1 = "";
                if (codebarText.Length > 0)
                {
                    ls_codigoConSilencios1 += Barcode128.FNC1;
                    for (i = 0; i < codebarText.Length; i++)
                    {
                        ls_codigoConSilencios1 += codebarText.Substring(i, 1);//Strings.Mid(codebarText, i, 1);
                        if (i == 35 | i == 53)
                        {
                            ls_codigoConSilencios1 += Barcode128.FNC1;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ls_codigoConSilencios1))
                {
                    iTextSharp.text.pdf.Barcode128 barc128_1 = new iTextSharp.text.pdf.Barcode128();
                    barc128_1.Code = ls_codigoConSilencios1;

                    barc128_1.CodeType = Barcode128.CODE128_UCC;
                    barc128_1.StartStopText = false;
                    barc128_1.ChecksumText = true;
                    barc128_1.GenerateChecksum = true;
                    barc128_1.Extended = true;
                    barc128_1.BarHeight = 35f;
                    barc128_1.AltText = codebar;

                    iTextSharp.text.Image imgCode1 = barc128_1.CreateImageWithBarcode(stamper.GetUnderContent(1), BaseColor.BLACK, BaseColor.BLACK);
                    imgCode1.ScalePercent(85, 100);
                    imgCode1.SetAbsolutePosition(240, 215);
                    stamper.GetOverContent(1).AddImage(imgCode1);

                }


                stamper.FormFlattening = true;
                stamper.FreeTextFlattening = true;
                stamper.Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
                stamper.Close();
            }
            catch (Exception e)
            {
                error = e.ToString();
            }
        }

        public void generarCuponAlternativo(DataRow dr, string gs1, out string error)
        {
            error = "";
            try
            {
                CodigoBarras cb = new CodigoBarras();

                string formatofuente = Application.StartupPath;
                formatofuente = formatofuente.Replace("\\bin\\Debug", "");
                string codebar = "";
                string codebarText = "";
                int i = 0;

                PdfReader reader = new PdfReader(formatofuente + "\\pdf\\ModeloCupon.pdf");
                PdfStamper stamper;
                stamper = new PdfStamper(reader, new FileStream(formatofuente + "\\pdf\\tmp\\cupon_" + dr["Numero_Referencia"].ToString() + ".pdf", FileMode.Create));
                AcroFields fields = stamper.AcroFields;
                fields.SetField("nombres", dr["Nombres"].ToString() + " " + dr["Apellidos"].ToString());
                fields.SetField("documento", dr["Numero_Documento"].ToString());
                fields.SetField("referencia", dr["Numero_Referencia"].ToString());
                fields.SetField("fecha_vencimiento", "PROXIMAS 48 HORAS");
                fields.SetField("valor_pagar", dr["Valor_Cuota"].ToString());
                fields.SetField("texto1", Convert.ToString(""));
                fields.SetField("texto2", Convert.ToString(""));
                fields.SetField("tipo", dr["Tipo_documento"].ToString());
                fields.SetField("x1", Convert.ToString(""));
                fields.SetField("x2", Convert.ToString(""));

                //codebar = "(415)" + gs1 + "(8020)" + dr["Numero_Referencia"].ToString() + "(3900)" + rellenaCeros(dr["valor_noformat"].ToString(), 13) + "(96)" + dr["Fecha_actual"].ToString();

                string codigo = string.Format("{0}415{1}8020{2}{3}3900{4}{5}96{6}"
                , Vintasoft.Barcode.BarcodeInfo.NonDataFlags.Fnc1
                , gs1 //GTIN
                , dr["Numero_Referencia"].ToString()
                , Vintasoft.Barcode.BarcodeInfo.NonDataFlags.Fnc1
                , decimal.Parse(dr["valor_noformat"].ToString())
                , Vintasoft.Barcode.BarcodeInfo.NonDataFlags.Fnc1
                , dr["Fecha_actual"].ToString()
                );

                string print = "(415)"+gs1;
                print += $"(8020)"+ dr["Numero_Referencia"].ToString();
                print += $"(3900)"+ decimal.Parse(dr["valor_noformat"].ToString());
                print += $"(96)"+ dr["Fecha_actual"].ToString();

                System.Drawing.Image image = cb.getBarcode(codigo, 800, 100, print);

                string path = formatofuente + "\\pdf\\tmp\\" + "BarcodePNG.png";
                image.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                Image imgItext = Image.GetInstance(path);
                imgItext.ScalePercent(60, 56);
                //formato horizontal
                imgItext.SetAbsolutePosition(169, 213);

                //formato vertical
                //imgItext.SetAbsolutePosition(58, 520);
                stamper.GetOverContent(1).AddImage(imgItext);


                stamper.FormFlattening = true;
                stamper.FreeTextFlattening = true;
                stamper.Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
                stamper.Close();
            }
            catch (Exception e)
            {
                error = e.ToString();
            }
        }

        public void borrarCuponTemporal(string numero_referencia)
        {
            string formatofuente = Application.StartupPath;
            formatofuente = formatofuente.Replace("\\bin\\Debug", "");
            if (File.Exists(formatofuente + "\\pdf\\tmp\\cupon_" + numero_referencia + ".pdf"))
            {
                File.Delete(formatofuente + "\\pdf\\tmp\\cupon_" + numero_referencia + ".pdf");
            }
            //borra codigo de barras temporal
            if (File.Exists(formatofuente + "\\pdf\\tmp\\BarcodePNG.png"))
            {
                File.Delete(formatofuente + "\\pdf\\tmp\\BarcodePNG.png");
            }

        }
        public string generarBase64(string numero_referencia)
        {
            string formatofuente = Application.StartupPath;
            formatofuente = formatofuente.Replace("\\bin\\Debug", "");
            if (File.Exists(formatofuente + "\\pdf\\tmp\\cupon_" + numero_referencia + ".pdf"))
            {
                byte[] imageArray = File.ReadAllBytes(formatofuente + "\\pdf\\tmp\\cupon_" + numero_referencia + ".pdf");
                return Convert.ToBase64String(imageArray);
            }
            else
            {
                return "";
            }
                
        }

        public string rellenaCeros(string referencia, int limite)
        {
            int j = 0;

            limite = limite - referencia.Length;
            string cadenaValor = "";
            for (j = 0; j <= limite; j++)
            {
                cadenaValor += "0";
            }
            cadenaValor += referencia;

            return cadenaValor;
        }

    }
}
