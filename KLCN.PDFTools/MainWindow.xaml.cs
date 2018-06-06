using Library;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KLCN.PDFTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pdf Files|*.pdf";
            try
            {
                if (ofd.ShowDialog() != DialogResult.HasValue)
                {
                    txt_filepath.Text = ofd.FileName;
                }
            }
            catch
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            /*********************check valid ***************************/
            bool flag = true;
            lbl_startpage.Content = @"* Can't Empty and Figure greater than 0 ";
            lbl_startpage.Visibility = Visibility.Hidden;
            progress.Visibility = Visibility.Hidden;
            progress.Value = 0;
            btn_open.Visibility = Visibility.Hidden;
            int pdfPageCount = 0;
            items_result.ItemsSource = null;
            if (string.IsNullOrEmpty(txt_filepath.Text.Trim()))
            {
                flag = false;
                MessageBox.Show("Please Choose PDF file first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                pdfPageCount = PDFOperation.GetPDFPageCount(txt_filepath.Text);
            if (string.IsNullOrEmpty(txt_startpage.Text.Trim())||!System.Text.RegularExpressions.Regex.IsMatch(txt_startpage.Text.Trim(), @"^[1-9]\d*$"))
            {
                flag = false;
                lbl_startpage.Visibility = Visibility.Visible;
            }
            else if (Convert.ToInt32(txt_startpage.Text) > pdfPageCount)
            {
                flag = false;
                lbl_startpage.Content = @"* Figure can't greater than total pages! ";
                lbl_startpage.Visibility = Visibility.Visible;
            }
            if (flag)
            {
                Invoke(pdfPageCount);//practical event
            }
        }
        private void Invoke(int pdfPageCount)
        {
            progress.Visibility = Visibility.Visible;
            /*************************************************************/
            #region explore fields position
            var specificPage=PDFOperation.ExtractTextFromPdf(txt_filepath.Text, Convert.ToInt32(txt_startpage.Text.Trim())).Split('\n').Where(p => p != null && p.Trim() != "").ToList();
            int position1=0, position2=0;
            List<int> pArr1 = new List<int>();
            List<int> pArr2 = new List<int>();
            foreach (string m in specificPage)
            {
                if (!string.IsNullOrEmpty(txt_position1.Text.Trim()))
                {
                    if (m.IndexOf(txt_position1.Text) >= 0)
                    {
                        position1 = specificPage.IndexOf(m);
                        var c = specificPage[position1].Split(' ').ToList<string>();
                        foreach(var cc in c)
                        {
                            if (txt_position1.Text.IndexOf(cc) >= 0)
                                pArr1.Add(c.IndexOf(cc));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(txt_position2.Text.Trim()))
                {
                    if (m.IndexOf(txt_position2.Text) >= 0)
                    {
                        position2 = specificPage.IndexOf(m);
                        var c = specificPage[position2].Split(' ').ToList<string>();
                        foreach (var cc in c)
                        {
                            if (txt_position2.Text.IndexOf(cc) >= 0)
                                pArr2.Add(c.IndexOf(cc));
                        }
                    }
                }
            }
            #endregion
            /******************************************************************/
            #region calculate splitter
            int split = Convert.ToInt32(cbx_paging.Text.Split(' ')[0]);
            var pagingCount = Math.Ceiling((double)(pdfPageCount - Convert.ToInt32(txt_startpage.Text.Trim())+1) / (double)split);

            var list = new List<int[]>();
            var intArr = new List<int>();
            for (int j = 1; j <= pagingCount; j++)
            {
                int last = 0;
                if (intArr.Count() > 0)
                    last = intArr.Last();
                intArr = new List<int>();
                for (int i = 1; i <= Convert.ToInt32(cbx_paging.Text.Split(' ')[0]); i++)
                {
                    if (Convert.ToInt32(txt_startpage.Text.Trim()) + (i-1) + last <= pdfPageCount)
                        intArr.Add(Convert.ToInt32(txt_startpage.Text.Trim()) + (i - 1) + last);
                }
                list.Add(intArr.ToArray());
            }
            #endregion
            /*******************************************************************/
            #region output
            List<Model> models = new List<Model>();
            foreach (var m in list)
            {
                var text = PDFOperation.ExtractTextFromPdf(txt_filepath.Text, m[0]).Split('\n').Where(p => p != null && p.Trim() != "").ToList();
                string filename = string.Empty;
                if (position1 != 0)
                {
                    var f = text[position1].Split(' ');
                    foreach (int f1 in pArr1)
                    {
                        try
                        {
                            filename += f[f1] + " ";
                        }
                        catch { }
                    }
                    filename = filename.Substring(0, filename.Length - 1);
                }
                if (position1 != 0 && position2 != 0)
                    filename=filename+txt_join1.Text;
                if (position2 != 0)
                {
                    var f = text[position2].Split(' ');
                    foreach (int f1 in pArr2)
                    {
                        try
                        {
                            filename += f[f1] + " ";
                        }
                        catch { }
                    }
                    filename = filename.Substring(0, filename.Length - 1);
                }
                if (string.IsNullOrEmpty(filename))
                    filename = (list.IndexOf(m) + 1).ToString();
                filename=filename+".pdf";

                string currPath = System.IO.Directory.GetCurrentDirectory()+"\\"+DateTime.Now.ToString("yyyyMMdd");
                if (!System.IO.Directory.Exists(currPath))
                    System.IO.Directory.CreateDirectory(currPath);
                PDFOperation.ExtractToSingleFile(m, txt_filepath.Text, currPath + "\\" + filename.Replace("/"," ").Replace("\\"," "));

                models.Add(new Model
                {
                    filename = filename
                });
                int num = (int)Math.Ceiling((double)100 / (double)list.Count());
                progress.Dispatcher.Invoke(() => progress.Value = progress.Value+num, DispatcherPriority.Background);
            }
            items_result.ItemsSource = models;
            btn_open.Visibility = Visibility.Visible;
            #endregion
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string v_OpenFolderPath = System.IO.Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("yyyyMMdd");
            System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
        }
    }
    public class Model
    {
        public string filename { get; set; }
    }
}
