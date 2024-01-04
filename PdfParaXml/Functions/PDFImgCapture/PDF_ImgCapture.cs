using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PdfParaXml.Functions.PDFImgCapture
{
    public class PDF_ImgCapture
    {
        public void CaptureRegionFromPdf(string pdfFilePath, int pageNumber, Rectangle region, string fileNAme)
        {
            using (var pdfReader = new PdfReader(pdfFilePath))
            {
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    var page = pdfDocument.GetPage(pageNumber);

                    // Use iTextSharp's parser to extract text and graphics
                    var listener = new ImageRenderListener();
                    PdfCanvasProcessor parser = new PdfCanvasProcessor(listener);
                    parser.ProcessPageContent(page);

                    // Get the captured image
                    Image fullImage = listener.GetImage();

                    // Capture the specified region
                    Image regionImage = CaptureRegion(fullImage, region);

                    // Save the region image to a file (or do whatever you need with it)
                    regionImage.Save($"{fileNAme}captured_region.png", ImageFormat.Png);
                }
            }
        }

        static Image CaptureRegion(Image fullImage, Rectangle region)
        {
            Bitmap bmp = new Bitmap(region.Width, region.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(fullImage, new Rectangle(0, 0, bmp.Width, bmp.Height), region, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        public Image ConverterBytesParaImagem(byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Image imagem = Image.FromStream(ms);
                    return imagem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao converter bytes para imagem: " + ex.Message);
                return null;
            }
        }
    }



    class ImageRenderListener : IEventListener
    {
        private Image image;

        void IEventListener.EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_IMAGE)
            {
                var renderInfo = (ImageRenderInfo)data;
                var imageObject = renderInfo.GetImage();
                if (imageObject != null)
                {
                    image = ConverterBytesParaImagem(imageObject.GetImageBytes());
                }
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            // Retorna a coleção de eventos suportados pelo ouvinte
            return new HashSet<EventType> {             
                EventType.RENDER_TEXT,
                EventType.RENDER_IMAGE,};
            // Adicione outros eventos suportados, se necessário
        }


            public Image GetImage()
        {
            return image;
        }

        private Image ConverterBytesParaImagem(byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Image imagem = Image.FromStream(ms);
                    return imagem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao converter bytes para imagem: " + ex.Message);
                return null;
            }
        }


    }
}


// < one line to give the program's name and a brief idea of what it does.>
//    Copyright (C) < year >  < name of author>
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.