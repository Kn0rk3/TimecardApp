using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TimecardApp.Model
{
    static class PDFCreater
    {
        public static async Task createPDFForTimesheet()
        {
            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new folder name DataFolder.
            var dataFolder = await local.CreateFolderAsync("DataFolder", CreationCollisionOption.OpenIfExists);

            // Create a new file named DataFile.txt.
            var file = await dataFolder.CreateFileAsync("Timesheet.pdf", CreationCollisionOption.ReplaceExisting);

            List<long> xrefs = new List<long>();

            //Using stream = Await System.IO.WindowsRuntimeStorageExtensions.OpenStreamForWriteAsync(file),
            //writer As New IO.StreamWriter(stream, Text.Encoding.UTF8)

            using (Stream sw = await System.IO.WindowsRuntimeStorageExtensions.OpenStreamForWriteAsync(file))
            {
                StreamWriter writer = new StreamWriter(sw, Encoding.UTF8);
                // PDF-HEADER
                writer.WriteLine("%PDF-1.2");
                // PDF-BODY. Convention is to start with a 4-byte binary comment
                // so everyone recognizes the pdf as binary. Then the file has
                // a load of numbered objects, #1..#7 in this case
                writer.WriteLine("%");
                writer.Flush();
                Byte[] tmpByte = { 0xC7, 0xEC, 0x8F, 0xA2 };
                sw.Write(tmpByte, 0, 4);
                sw.Flush();
                writer.WriteLine("");

                // #1: catalog - the overall container of the entire PDF
                writer.Flush();
                sw.Flush();
                xrefs.Add(sw.Position);
                writer.WriteLine("1 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("  /Type /Catalog");
                writer.WriteLine("  /Pages 2 0 R");
                writer.WriteLine(">>");
                writer.WriteLine("endobj");

                // #2: page-list - we have only one child page
                writer.Flush();
                sw.Flush();
                xrefs.Add(sw.Position);
                writer.WriteLine("2 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("  /Type /Pages");
                writer.WriteLine("  /Kids [3 0 R]");
                writer.WriteLine("  /Count 1");
                writer.WriteLine(">>");
                writer.WriteLine("endobj");

                // #3: page - this is our page. We specify size, font resources, and the contents
                writer.Flush();
                sw.Flush();
                xrefs.Add(sw.Position);
                writer.WriteLine("3 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("  /Type /Page");
                writer.WriteLine("  /Parent 2 0 R");
                writer.WriteLine("  /MediaBox [0 0 612 792]"); // Default userspace units: 72/inch, origin at bottom left
                writer.WriteLine("  /Resources");
                writer.WriteLine("  <<");
                writer.WriteLine("    /ProcSet [/PDF/Text]"); // This PDF uses only the Text ability
                writer.WriteLine("    /Font");
                writer.WriteLine("    <<");
                writer.WriteLine("      /F0 4 0 R"); // I will define three fonts, #4, #5 and #6
                writer.WriteLine("      /F1 5 0 R");
                writer.WriteLine("      /F2 6 0 R");
                writer.WriteLine("    >>");
                writer.WriteLine("  >>");
                writer.WriteLine("  /Contents 7 0 R");
                writer.WriteLine(">>");
                writer.WriteLine("endobj");

                // #4, #5, #6: three font resources, all using fonts that are built into all PDF-viewers
                // We're going to use WinAnsi character encoding, defined below.
                writer.Flush();
                sw.Flush();
                xrefs.Add(sw.Position);
                writer.WriteLine("4 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("  /Type /Font");
                writer.WriteLine("  /Subtype /Type1");
                writer.WriteLine("  /Encoding /WinAnsiEncoding");
                writer.WriteLine("  /BaseFont /Times-Roman");
                writer.WriteLine(">>");
                writer.Flush();
                sw.Flush();
                xrefs.Add(sw.Position);
                writer.WriteLine("5 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("  /Type /Font");
                writer.WriteLine("  /Subtype /Type1");
                writer.WriteLine("  /Encoding /WinAnsiEncoding");
                writer.WriteLine("  /BaseFont /Times-Bold");
                writer.WriteLine(">>");
                writer.Flush();
                sw.Flush();
                xrefs.Add(sw.Position);
                writer.WriteLine("6 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("  /Type /Font");
                writer.WriteLine("  /Subtype /Type1");
                writer.WriteLine("  /Encoding /WinAnsiEncoding");
                writer.WriteLine("  /BaseFont /Times-Italic");
                writer.WriteLine(">>");

                // #7: contents of page. This is written in postscript, fully described in
                // chapter 8 of the PDF 1.2 reference manual.
                writer.Flush();
                sw.Flush();
                xrefs.Add(sw.Position);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("BT");            // BT = begin text object, with text-units the same as userspace-units
                sb.AppendLine("/F0 40 Tf");     // Tf = start using the named font "F0" with size "40"
                sb.AppendLine("40 TL");         // TL = set line height to "40"
                sb.AppendLine("230.0 400.0 Td");// Td = position text point at coordinates "230.0", "400.0"
                sb.AppendLine("(Hello all) '"); // Apostrophe = print the text, and advance to the next line
                sb.AppendLine("/F2 20 Tf");
                sb.AppendLine("20 TL");
                sb.AppendLine("0.0 0.2 1.0 rg");// rg = set fill color to RGB("0.0", "0.2", "1.0")
                sb.AppendLine("(ole)");
                sb.AppendLine("ET");

                writer.WriteLine("7 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("  /Length " + sb.Length);
                writer.WriteLine(">>");
                writer.WriteLine("stream");
                writer.Write(sb.ToString());
                writer.WriteLine("endstream");
                writer.WriteLine("endobj");

                // PDF-XREFS. This part of the PDF is an index table into every object #1..#7
                // that we defined.
                writer.Flush();
                sw.Flush();
                long xref_pos = sw.Position;
                writer.WriteLine("xref");
                writer.WriteLine("1 " + xrefs.Count);
                foreach (long xref in xrefs)
                {
                    writer.WriteLine("{0:0000000000} {1:00000} n", xref, 0);
                }

                // PDF-TRAILER. Every PDF ends with this trailer.
                writer.WriteLine("trailer");
                writer.WriteLine("<<");
                writer.WriteLine("  /Size " + xrefs.Count);
                writer.WriteLine("  /Root 1 0 R");
                writer.WriteLine(">>");
                writer.WriteLine("startxref");
                writer.WriteLine(xref_pos);
                writer.WriteLine("%%EOF");
            }
            await Windows.System.Launcher.LaunchFileAsync(file);
        }
    }

}

