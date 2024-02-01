
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using iText.Commons.Datastructures;
using iText.Kernel.Pdf;
using iText.Pdfa;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace catalog_of_products
{
    public partial class Article_form : Form
    {
        string article;
        public Article_form(string art)
        {
            InitializeComponent();
            article = art;

        }
        private Bitmap DrawBarcode(string code, int resolution = 20) // resolution - пикселей на миллиметр
        {
            // Параметры для создания изображения
            int numberCount = 13;
           
            float height = 25.93f * resolution;
            float lineHeight = 22.85f * resolution;
            float leftOffset = 3.63f * resolution;
            float rightOffset = 2.31f * resolution;
            float longLineHeight = lineHeight + 1.65f * resolution;
            float fontHeight = 2.75f * resolution;
            float lineToFontOffset = 0.165f * resolution;
            float lineWidthDelta = 0.15f * resolution;
            float lineWidthFull = 1.35f * resolution;
            float lineOffset = 0.2f * resolution;
            float width = leftOffset + rightOffset + 6 * (lineWidthDelta + lineOffset) + numberCount * (lineWidthFull + lineOffset);

            Bitmap bitmap = new Bitmap((int)width, (int)height);
            Graphics g = Graphics.FromImage(bitmap);
            Font font = new Font("Arial", fontHeight, FontStyle.Regular, GraphicsUnit.Pixel);
            StringFormat fontFormat = new StringFormat();
            fontFormat.Alignment = StringAlignment.Center;
            fontFormat.LineAlignment = StringAlignment.Center;
            float x = leftOffset;

            // Пример данных для штрих-кода

            // Создание штрих-кода
            for (int i = 0; i < numberCount; i++)
            {
                int number = Convert.ToInt32(code[i].ToString());
                if (number != 0)
                {
                    g.FillRectangle(Brushes.Black, x, 0, number * lineWidthDelta, lineHeight);
                }
                RectangleF fontRect = new RectangleF(x, lineHeight + lineToFontOffset, lineWidthFull, fontHeight);
                g.DrawString(code[i].ToString(), font, Brushes.Black, fontRect, fontFormat);
                x += lineWidthFull + lineOffset;

                if (i == 0 || i == numberCount / 2 || i == numberCount - 1)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        g.FillRectangle(Brushes.Black, x, 0, lineWidthDelta, longLineHeight);
                        x += lineWidthDelta + lineOffset;
                    }
                }
            }
            return bitmap;
            


        }
        private void button1_Click(object sender, EventArgs e)
        {
            int resolution = 20;

           
            Bitmap bitmap = DrawBarcode(article, resolution);

           
            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.PrinterResolution = new PrinterResolution() { X = resolution, Y = resolution }; // Установка разрешения печати

            
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", (int)(bitmap.Width / resolution * 100), (int)(bitmap.Height / resolution * 100));

            // Обработчик события печати
            pd.PrintPage += (sender, e) =>
            {
                
                e.Graphics.DrawImage(bitmap, 0, 0, bitmap.Width / resolution * 100, bitmap.Height / resolution * 100);
            };

          
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = pd;

            // Показ предпросмотра печати
            if (printPreviewDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print(); 
            }

            bitmap.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int resolution = 20;
            // Создание документа PDF
            Bitmap bitmap = DrawBarcode(article);
            Document pdfDocument = new Document();
            Page page = pdfDocument.Pages.Add();

            // Преобразование Bitmap в MemoryStream
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Добавление изображения в документ PDF
                Aspose.Pdf.Rectangle rectangle = new Aspose.Pdf.Rectangle(0, 0, bitmap.Width / resolution, bitmap.Height / resolution); // Устанавливаем размеры изображения в миллиметрах
                page.SetPageSize(rectangle.Width, rectangle.Height);
                page.AddImage(memoryStream, rectangle);
            }

            // Сохранение документа PDF
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                pdfDocument.Save(saveFileDialog.FileName);
            }
        }
    }
}

