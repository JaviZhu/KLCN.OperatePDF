using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Library;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace KLCN.OperatePDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = @"C:\Users\cnjazhu\Desktop\324062.pdf";
            var content = PDFOperation.ExtractTextFromPdf(filename,28).Split('\n');

        }
    }
}
