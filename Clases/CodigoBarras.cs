using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Vintasoft.Barcode;
using Vintasoft.Barcode.SymbologySubsets;

namespace GD.Soap.Bussines
{
    public class CodigoBarras
    {
        public CodigoBarras()
        {

            BarcodeGlobalSettings.RegisterBarcodeWriter(" Juan Suarez", "ing_jgso@hotmail.com", "VYrKslgdPolftKcC1YDR5HTi7w1uwi+3/2XVlmAlSq1nkBDRx3GIVMspphxWMO+J4HS546O4PXDar7UDkg492zzlboSQ4rKzuhaPjQnk4R2C8kpge/xH758Z1xpDxNV4AovHB53FyfTOwDmua6fhF2WJGVooIwlgiaUBwbYf1ptA");

            //BarcodeGlobalSettings.RegisterBarcodeWriter(" Juan Suarez (Servers)", "ing_jgso@hotmail.com", "MAVHYfedejcItq6obxf5TnGkOPhOmkrwlCrsM7AtVBe9MXvFWBLpNYKZZe52Fe7b1tlReosxKg4m/3WZt6LeQsTKHpHApo3FxKFJvr3r+r8R/nZzYE+fV53BbSes+dNmda9+4dEz6m9A1C01iFjPajb6sdujHg6Xr3cclo+dnGGU");

        }

        #region Generar Barcode Vintasoft

        public string getBarcode(string PDato, string PrintPDato, string PTipo = "Code 128")
        {

            BarcodeWriter generadorAux = new BarcodeWriter();
            System.Drawing.Image imagenCodigoDeBarras = null;
            string urlImagenCodigoDeBarras = string.Empty;

            //AJUSTA GENERADOR DE CÓDIGO DE BARRAS ajustaGeneradorCodigoBarras
            generadorAux = ajustaGeneradorCodigoBarras(PDato, PTipo, PrintPDato);

            //GENERA IMAGEN
            imagenCodigoDeBarras = generadorAux.GetBarcodeAsBitmap();

            //Base 64 DE LA IMAGEN
            return GetImageAsBase64String(imagenCodigoDeBarras);
        }

        public Image getBarcode(string PDato, int PAncho, int PAlto, string PrintPDato, string PTipo = "Code 128")
        {

            BarcodeWriter generadorAux = new BarcodeWriter();
            System.Drawing.Image imagenCodigoDeBarras = null;
            string urlImagenCodigoDeBarras = string.Empty;

            //AJUSTA GENERADOR DE CÓDIGO DE BARRAS ajustaGeneradorCodigoBarras
            generadorAux = ajustaGeneradorCodigoBarras(PDato, PTipo, PrintPDato);

            //GENERA IMAGEN
            imagenCodigoDeBarras = generadorAux.GetBarcodeAsBitmap();

            //URL DE LA IMAGEN
            urlImagenCodigoDeBarras = GetImageAsBase64String(imagenCodigoDeBarras);

            return ResizeImage(imagenCodigoDeBarras, PAncho, PAlto);
        }

        private BarcodeWriter ajustaGeneradorCodigoBarras(string PDato, string PTipo, string print)
        {
            // create the barcode writer object
            BarcodeWriter barcodeWriter = new BarcodeWriter();

            BarcodeSymbologySubset barcodeSymbologySubset = null;

            //DEFINE TIPO DE CODIGO DE BARRAS
            switch (PTipo)
            {
                case "Code 11":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Code11;
                    break;
                case "Code 39":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Code39;
                    break;
                case "Code 39 Extended":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.Code39Extended;
                    break;
                case "Code 32":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.Code32;
                    break;
                case "Code 93":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Code93;
                    break;
                case "Code 128":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Code128;                    
                    break;
                case "Codabar":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Codabar;
                    break;
                case "EAN-8":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.EAN8;
                    break;
                case "EAN-8 + 2":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.EAN8Plus2;
                    break;
                case "EAN-8 + 5":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.EAN8Plus5;
                    break;
                case "EAN-13":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.EAN13;
                    break;
                case "EAN-13 + 2":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.EAN13Plus2;
                    break;
                case "EAN-13 + 5":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.EAN13Plus5;
                    break;
                case "Interleaved 2 of 5":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Interleaved2of5;
                    break;
                case "MSI":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.MSI;
                    break;
                case "Patch Code":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.PatchCode;
                    break;
                case "Pharmacode":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Pharmacode;
                    break;
                case "RSS-14":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.RSS14;
                    break;
                case "RSS-14 Stacked":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.RSS14Stacked;
                    break;
                case "RSS Limited":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.RSSLimited;
                    break;
                case "RSS Expanded":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.RSSExpanded;
                    break;
                case "RSS Expanded Stacked":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.RSSExpandedStacked;
                    break;
                case "Standard 2 of 5":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Standard2of5;
                    break;
                case "Telepen":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Telepen;
                    break;
                case "UPCA":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.UPCA;
                    break;
                case "UPCA + 2":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.UPCAPlus2;
                    break;
                case "UPCA + 5":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.UPCAPlus5;
                    break;
                case "UPCE":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.UPCE;
                    break;
                case "UPCE + 2":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.UPCEPlus2;
                    break;
                case "UPCE + 5":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.UPCEPlus5;
                    break;
                case "JAN-8":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.JAN8;
                    break;
                case "JAN-8 + 2":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.JAN8Plus2;
                    break;
                case "JAN-8 + 5":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.JAN8Plus5;
                    break;
                case "JAN-13":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.JAN8;
                    break;
                case "JAN-13 + 2":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.JAN8Plus2;
                    break;
                case "JAN-13 + 5":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.JAN8Plus5;
                    break;
                case "ISBN":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISBN;
                    break;
                case "ISBN + 2":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISBNPlus2;
                    break;
                case "ISBN + 5":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISBNPlus5;
                    break;
                case "ISSN":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISSN;
                    break;
                case "ISSN + 2":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISSNPlus2;
                    break;
                case "ISSN + 5":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISSNPlus5;
                    break;
                case "ISMN":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISMN;
                    break;
                case "ISMN + 2":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISMNPlus2;
                    break;
                case "ISMN + 5":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ISMNPlus5;
                    break;
                case "EAN-Velocity":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.EANVelocity;
                    break;
                case "Code 16K":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Code16K;
                    break;
                case "Matrix 2 of 5":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.Matrix2of5;
                    break;
                case "IATA 2 of 5":
                    barcodeWriter.Settings.Barcode = Vintasoft.Barcode.BarcodeType.IATA2of5;
                    break;
                case "GS1-128":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.GS1_128;
                    break;
                case "GS1 DataBar":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.GS1DataBar;
                    break;
                case "GS1 DataBar Expanded":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.GS1DataBarExpanded;
                    break;
                case "GS1 DataBar Expanded Stacked":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.GS1DataBarExpandedStacked;
                    break;
                case "GS1 DataBar Limited":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.GS1DataBarLimited;
                    break;
                case "GS1 DataBar Stacked":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.GS1DataBarStacked;
                    break;
                case "ITF-14":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.ITF14;
                    break;
                case "Numly Number":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.NumlyNumber;
                    break;
                case "OPC":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.OPC;
                    break;
                case "PZN":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.PZN;
                    break;
                case "SSCC-18":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.SSCC18;
                    break;
                case "VIN":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.VIN;
                    break;
                case "VICS BOL":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.VicsBol;
                    break;
                case "VICS SCAC PRO":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.VicsScacPro;
                    break;
                case "Deutsche Post Identcode":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.DeutschePostIdentcode;
                    break;
                case "Deutsche Post Leitcode":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.DeutschePostLeitcode;
                    break;
                case "DHL AWB":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.DhlAwb;
                    break;
                case "FedEx Ground 96":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.FedExGround96;
                    break;
                case "Swiss PostParcel":
                    barcodeSymbologySubset = BarcodeSymbologySubsets.SwissPostParcel;
                    break;

            }
            if (barcodeSymbologySubset == null &&
                barcodeWriter.Settings.Barcode == Vintasoft.Barcode.BarcodeType.None)
            {
                return null;
            }

            // set the value of barcode
            if (barcodeSymbologySubset != null)
            {
                barcodeSymbologySubset.Encode(PDato, barcodeWriter.Settings);
            }
            else
            {
                //string barCode = PDato.Replace("(", string.Empty).Replace(")", string.Empty);
                barcodeWriter.Settings.Value = PDato;                 
                barcodeWriter.Settings.PrintableValue = print;
            }

            //COLOR
            barcodeWriter.Settings.BackColor = System.Drawing.Color.White;
            barcodeWriter.Settings.ForeColor = System.Drawing.Color.Black;

            barcodeWriter.Settings.PixelFormat = BarcodeImagePixelFormat.Bgr24;

            //ALTURA
            barcodeWriter.Settings.Height = 100;

            //PADDING
            barcodeWriter.Settings.Padding = 1;

            //ANCHO MINIMO DE BARRAS
            barcodeWriter.Settings.MinWidth = 3;

            //TEXTO
            //barcodeWriter.Settings.ValueVisible = false;
            barcodeWriter.Settings.ValueVisible = true;

            return barcodeWriter;
        }

        public static string GetImageAsBase64String(System.Drawing.Image bitmap)
        {
            //
            using (MemoryStream mem = new MemoryStream())
            {
                bitmap.Save(mem, System.Drawing.Imaging.ImageFormat.Png);

                // get image bytes
                byte[] imageBytes = new byte[mem.Length];
                mem.Position = 0;
                mem.Read(imageBytes, 0, imageBytes.Length);
                // convert image bytes to a Base64 string
                string base64String = Convert.ToBase64String(imageBytes);

                // return an image as Base64 string
                return string.Format("data:image/png;base64,{0}", base64String);
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            return result;
        }
        #endregion
    }
}
