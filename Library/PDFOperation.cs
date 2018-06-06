using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class PDFOperation
    {
        /// <summary>
        /// Get specifical page text
        /// </summary>
        /// <param name="path"></param>
        /// <param name="splitPageQty"></param>
        /// <returns></returns>
        public static string ExtractTextFromPdf(string path, int splitPageQty = 1)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                //for (int i = 1; i <= reader.NumberOfPages; i++)
                //{
                //    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                //}
                text.Append(PdfTextExtractor.GetTextFromPage(reader, splitPageQty));
                return text.ToString();
            }
        }
        public static java.util.List SplitPDFFile(string SourcePath,int splitPageQty = 1)
        {
            var doc = PDDocument.load(SourcePath);
            var splitter = new Splitter();
            splitter.setSplitAtPage(splitPageQty);

            return (java.util.List)splitter.split(doc);
        }
        public static object[] SplitPDFFileByArray(string SourcePath,int splitPageQty = 1)
        {
            var doc = PDDocument.load(SourcePath);
            var splitter = new Splitter();
            splitter.setSplitAtPage(splitPageQty);

            return (object[])splitter.split(doc).toArray();
        }
        public static String[] PdfToText(string path)
        {
            PDDocument doc = PDDocument.load(path);

            PDFTextStripper pdfStripper = new PDFTextStripper();
            var text = pdfStripper.getText(doc).Split('\r');
            return text;
        }
        public static int GetPDFPageCount(string path)
        {
            PDDocument doc = PDDocument.load(path);
            return doc.getPageCount();
        }
        /// <summary>
        /// Save specifical page to file
        /// </summary>
        /// <param name="PageNumbers"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="outputFilePath"></param>
        public static void ExtractToSingleFile(int[] PageNumbers,string sourceFilePath, string outputFilePath)
        {
            var originalDocument = PDDocument.load(sourceFilePath);
            var originalCatalog = originalDocument.getDocumentCatalog();
            java.util.List sourceDocumentPages = originalCatalog.getAllPages();
            var newDocument = new PDDocument();

            foreach (var pageNumber in PageNumbers)
            {
                // Page numbers are 1-based, but PDPages are contained in a zero-based array:
                int pageIndex = pageNumber - 1;
                newDocument.addPage((PDPage)sourceDocumentPages.get(pageIndex));
            }
            newDocument.save(outputFilePath);
        }
    }
}
