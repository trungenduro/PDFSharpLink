
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using BitMiracle.Docotic.Pdf;

namespace PDFlink
{
    public partial class PDFリンク追加 : Form
    {
        public PDFリンク追加()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    //textBox1.Text = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                 
                   
                }
            }

        }

        private void load(object sender, EventArgs e)
        {
          //  textBox1.Text = @"C:\temp\model.pdf";
          //  textBox2.Text = "file";
            textBox3.Text = @"Z:\0_TRUNG\PDF_Test";
        }

        private void Form1_QueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fo = new FolderBrowserDialog();
            fo.Description = "フォルダ選択";
            fo.RootFolder = Environment.SpecialFolder.MyComputer;    //Myドキュメントをルートフォルダにする
         
            //フォルダ選択ダイアログを表示する
            DialogResult result = fo.ShowDialog();

            if (result == DialogResult.OK)
            {
                //「OK」ボタンが選択された時の処理
                string folderPath = fo.SelectedPath;  //こんな感じで選択されたフォルダのパスが取得できる
                textBox3.Text = folderPath;


            }
            else if (result == DialogResult.Cancel)
            {
                //「キャンセル」ボタンまたは「×」ボタンが選択された時の処理
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            textBox3.Text = @"C:\TEMP\PDF";
            string[] Folders = Directory.GetDirectories(textBox3.Text);

            string[] Files = Directory.GetFiles(textBox3.Text, "*.pdf",SearchOption.AllDirectories);




            for (int i = 0; i < Files.Length; i++)
            {
               
              
                string filename = Files[i];
                //    var tt = PdfTextExtractor.GetText(filename);

              //  var tt =  PdfSharpTextExtractor.Extractor.PdfToText(filename);
                if (filename.EndsWith("_link.pdf"))
                    continue;

                string Folder = Path.GetFileName(Path.GetDirectoryName(filename));
                bool found = false;
                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                PdfDocument pdf = PdfSharp.Pdf.IO.PdfReader.Open(filename, PdfDocumentOpenMode.Import);

                List<string> CommentList = new List<string>();
                List<PdfRectangle> Rectangles = new List<PdfRectangle>();

                BitMiracle.Docotic.LicenseManager.AddLicenseData("62Z6K-0SCDY-HIN04-J1W2L-JLWZO");
                using (BitMiracle.Docotic.Pdf.PdfDocument Docoticpdf = new BitMiracle.Docotic.Pdf.PdfDocument(filename)) 
                {

                    PdfRectangle pdfRectangle = new PdfRectangle();



                    for (int PageNo = 0; PageNo < pdf.Pages.Count; PageNo++)
                    {
                        PdfPage page1 = pdf.Pages[PageNo];

                        var DocoticPage = Docoticpdf.Pages[PageNo];

                        foreach (var obj in DocoticPage.GetObjects())
                        {

                            if (obj.Type == BitMiracle.Docotic.Pdf.PdfPageObjectType.Text)
                            {
                                var Text = (BitMiracle.Docotic.Pdf.PdfTextData)obj;

                                if (Text.Text.Length > 1)
                                {
                                    XRect rect = new XRect(Text.Bounds.X, page1.Height - Text.Bounds.Y - Text.Bounds.Height, Text.Bounds.Width, Text.Bounds.Height);
                                    CommentList.Add(Text.Text);
                                    Rectangles.Add(new PdfRectangle(rect));
                                }
                            }
                        }




                        PdfPage page = document.AddPage(page1);





                        for (int p = 0; p < page.Annotations.Elements.Count; p++)
                        {
                            PdfAnnotation textAnnot = page.Annotations[p];

                            string content = textAnnot.Contents;

                            if (content != null && content.Length > 1 )
                            {
                                CommentList.Add(content);
                                Rectangles.Add(textAnnot.Rectangle);

                            }

                        }



                        for (int k = 0; k < Files.Length; k++)
                        {
                            string otherfile = Files[k];

                            if (otherfile.EndsWith("_link.pdf"))
                                continue;


                            string LinkFolder = Path.GetFileName(Path.GetDirectoryName(otherfile));

                            if (i != k)
                            {
                                for (int i1 = 0; i1 < CommentList.Count; i1++)
                                {


                                    if (CommentList[i1].IndexOf(Path.GetFileNameWithoutExtension(otherfile)) > -1)
                                    {
                                        Uri path1 = new Uri(filename);
                                        Uri path2 = new Uri(otherfile);

                                        var RelativeUri = path1.MakeRelativeUri(path2);

                                        page.AddFileLink(Rectangles[i1], RelativeUri.OriginalString);
                                        /*
                                        if (LinkFolder == Folder)
                                            page.AddFileLink(Rectangles[i1], Path.GetFileName(otherfile));

                                        else
                                            page.AddFileLink(Rectangles[i1], "../" + LinkFolder + "/" + Path.GetFileName(otherfile));

                                        //  page.AddHyperlink(textAnnot.Rectangle, new Uri("../" + LinkFolder + "/" + Path.GetFileName(otherfile), UriKind.Relative));
                                        //  page.AddHyperlink(textAnnot.Rectangle, new Uri(Path.GetFileName(Path.GetDirectoryName(Filepath[i])) + @"\" + Text.Text + ".pdf", UriKind.Relative));

                                        //  ../ サポート図 / 2100 - PS - 00154.pdf   "../アイソメ図/4B-DP-2052-DPT4HHP-H-H70.pdf"
                                        */


                                        found = true;


                                    }
                                }

                            }

                        }



                    }


                    if (found)
                    {
                        document.Save(filename.Replace(".pdf", "_link.pdf"));


                    }


                }


                if (found)
                {

                    try
                    {

                        File.Delete(filename);

                        File.Move(filename.Replace(".pdf", "_link.pdf"), filename);
                        MessageBox.Show(filename + "にリンク追加して、保存しました", "保存しました");

                    }
                    catch (Exception)
                    {
                        MessageBox.Show(  filename + "が開いてる為、上書きできません","Error");

                    }


                }

            }




            MessageBox.Show( "完成", "Finish");
        }

        private IEnumerable<string> ExtractText(CObject cObject)
        {
            var textList = new List<string>();
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                    {
                        textList.AddRange(ExtractText(cOperand));
                    }
                }
            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                {
                    textList.AddRange(ExtractText(element));
                }
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                textList.Add(cString.Value);
            }
            return textList;
        }
    }
}
