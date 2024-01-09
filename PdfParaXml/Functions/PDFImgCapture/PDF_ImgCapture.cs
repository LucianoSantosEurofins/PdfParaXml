using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;

namespace PdfParaXml.Functions.PDFImgCapture
{//d
    public class PDF_ImgCapture
    {
        public void CaptureRegionFromPdf(string pdfFilePath, int pageNumber, string fileName, int imgPosition)
        {
            using (var pdfReader = new PdfReader(pdfFilePath))
            {
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    var page = pdfDocument.GetPage(pageNumber);

                    // Use iTextSharp's parser to extract text and graphics
                    var listener = new ImageExtractionListener();
                    PdfCanvasProcessor parserImgCordinates = new PdfCanvasProcessor(listener);
                    parserImgCordinates.ProcessPageContent(page);

                    // Get the captured image
                    var resultadoDeImgExam = listener.GetImagens();
                    var imagemBytes = resultadoDeImgExam[imgPosition].imgBytes;
                    // Capture the specified region
                    Bitmap resultImage = ConverterBytesParaImagem(imagemBytes);

                    // Save the region image to a file (or do whatever you need with it)
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream($"{fileName}captured_region.png",FileMode.Create, FileAccess.ReadWrite))
                        {
                            var saveImage =  new Bitmap(resultImage);
                            saveImage.Save(ms, ImageFormat.Png);
                            byte[] bytes = ms.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
            }
        }

        static Image CaptureRegion(Image fullImage, System.Drawing.Rectangle region)
        {
            Bitmap bmp = new Bitmap(region.Width, region.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(fullImage, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), region, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        private Bitmap ConverterBytesParaImagem(byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Bitmap imagem = (Bitmap)Bitmap.FromStream(ms);
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

    public class ImageExtractionListener : IEventListener
    {

        public readonly List<ImageInfo> imagens = new List<ImageInfo>();

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_IMAGE)
            {
                var renderInfo = (ImageRenderInfo)data;
                var imageObject = renderInfo.GetImage();
                var xObject = imageObject;

                var imagem = new ImageInfo
                {
                    X = renderInfo.GetImageCtm().Get(Matrix.I31),
                    Y = renderInfo.GetImageCtm().Get(Matrix.I32),
                    Largura = xObject.GetWidth(),
                    Altura = xObject.GetHeight(),
                    imgBytes = xObject.GetImageBytes()
                };
                imagens.Add(imagem);
            }
        }

        public List<ImageInfo> GetImagens()
        {
            return imagens;
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            // Retorna a coleção de eventos suportados pelo ouvinte
            return new HashSet<EventType> {
                EventType.RENDER_TEXT,
                EventType.RENDER_IMAGE,};
            // Adicione outros eventos suportados, se necessário
        }

        public class ImageInfo
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Largura { get; set; }
            public float Altura { get; set; }
            public byte[] imgBytes { get; set; }
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