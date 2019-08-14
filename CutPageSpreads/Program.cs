using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO; 

// modified from
// https://stackoverflow.com/questions/27011829/divide-one-page-pdf-file-in-two-pages-pdf-file

namespace CutPageSpreads
{
    class Program
    {
        private static string sourceFile = string.Empty;
        private static string destFile = string.Empty;
        private static bool execute = true;

        static void Main(string[] args)
        {
            // proc args
            ProcessArguments(args);
            
            if(execute == false)
            {
                PrintHelp();
                return;
            }

            // Creating a reader
            PdfReader reader = new PdfReader(sourceFile);
            int pageCount = reader.NumberOfPages;

            // step 1
            Rectangle mediabox = new Rectangle(GetHalfPageSize(reader.GetPageSizeWithRotation(1)));
            Document document = new Document(mediabox);

            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(destFile, FileMode.Create));

            // step 3
            document.Open();

            // step 4
            PdfContentByte content = writer.DirectContent;
            PdfImportedPage page;

            for (int i = 1; i <= pageCount; i++)
            {
                page = writer.GetImportedPage(reader, i);
                content.AddTemplate(page, 0, 0);
                document.NewPage();
                content.AddTemplate(page, -mediabox.Width, 0);
                mediabox = new Rectangle(GetHalfPageSize(reader.GetPageSizeWithRotation(i)));
                document.SetPageSize(mediabox);
                document.NewPage();
            }

            // step 5
            document.Close();
            reader.Close();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Instructions:");
            Console.WriteLine("CutPageSpread.exe [source.pdf] [output.pdf]"); 
            Console.WriteLine("");
            Console.WriteLine("");
        }

        private static void ProcessArguments(string[] args)
        {
            execute = true;

            sourceFile = args[0];

            if (File.Exists(sourceFile) == false)
            {
                Console.WriteLine("");
                Console.WriteLine("ERROR ");
                Console.WriteLine("The source file should exist!");
                Console.WriteLine("");
                sourceFile = string.Empty;
                execute = false;
            }

            if (args.Length > 1)
            {
                destFile = args[1];
            }

            if (string.IsNullOrEmpty(destFile))
            {
                destFile = "cut_" + Path.GetFileName(sourceFile);
            }
        }

        private static Rectangle GetHalfPageSize(Rectangle pagesize)
        {
            float width = pagesize.Width;
            float height = pagesize.Height;
            return new Rectangle(width / 2, height);
        }


    }
}
