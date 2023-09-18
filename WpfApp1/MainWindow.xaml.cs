using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.Win32;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using Microsoft.VisualBasic;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel.Sketches;
using LiveCharts;
using System.Windows.Ink;
using LiveCharts.Helpers;

namespace WpfApp1
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
        //Dictionary<string, Dictionary<int, List<string>>> messages = new Dictionary<string, Dictionary<int, List<string>>>();
        public Dictionary<string, List<List<string>>> _messages;
        List<string> _uniqId;
        public void Btn_ACE(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.log)|*.log|All files (*.*)|*.*";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                TB_List.Clear();
                string filename = ofd.FileName;
                var files = System.IO.File.ReadAllText(filename);
                List<string> words = new List<string>();
                words = files.Split('\n').ToList();
                var oneWord = words[0];
                words.RemoveRange(0, 2);
                Label_Speed_CAN.Content = string.Concat(oneWord.Where(Char.IsDigit));
                List<string> dataSheets = new List<string>();
                int range;
                //убираем тайминг, он не нужен для обработки и построения графика
                for (int i = 0; i < words.Count; i++)
                {
                    dataSheets.Add(words[i]);
                    dataSheets[i] = Regex.Replace(dataSheets[i], @"\s+", " ");
                    range = dataSheets[i].IndexOf(' ', 0) + 1;
                    while (range == 1)
                    {
                        dataSheets[i] = dataSheets[i].Remove(0, 1);
                        range = dataSheets[i].IndexOf(' ', 0) + 1;
                    }
                    dataSheets[i] = dataSheets[i].Remove(0, range);
                }


                //Оставляем нужные данные для гарфика, убирая между ID и сообщением значение
                int foundSpace1;
                int foundSpace2;
                for (int i = 0; i < dataSheets.Count; i++)
                {
                    foundSpace1 = dataSheets[i].IndexOf(' ', 0) + 1;
                    foundSpace2 = dataSheets[i].IndexOf(' ', foundSpace1);
                    if (foundSpace2 == -1 || foundSpace1 == 0)
                    {
                        dataSheets.Remove(dataSheets[i]);
                        continue;
                    }
                    dataSheets[i] = dataSheets[i].Remove(foundSpace1, foundSpace2 - foundSpace1);
                }
                Label_Msg_In.Content = dataSheets.Count;
                List<string> distinctData = new List<string>();
                distinctData = dataSheets;
                //создаем словарь
                Dictionary<string, List<List<string>>> messages = new Dictionary<string, List<List<string>>>();
                //список для уникальных ID
                List<string> uniqId = new List<string>();
                for (int i = 0; i < distinctData.Count; i++)
                {
                    uniqId.Add(Regex.Replace(distinctData[i], @" \S*", ""));
                }
                uniqId = uniqId.Distinct().ToList();
                List<string> containData = new List<string>();


                //удаление неизменяемых элементов, если этого хочет пользователь.
                if (CheckBox_Filter.IsChecked == true)
                {
                    for (int i = 0; i < uniqId.Count; i++)
                    {
                        containData = distinctData.FindAll(d => d.Contains(uniqId[i]));
                        containData = containData.Distinct().ToList();
                        if (containData.Count <= 1)
                        {
                            distinctData.RemoveAll(d => d.Contains(uniqId[i]));
                            uniqId.RemoveAt(i);
                        }
                    }
                    Label_Msq_Unique.Content = distinctData.Count;

                    //distinctData = dataSheets.Distinct().ToList();
                }
                //заполняем обработанные данные в TextBox
                TB_List.Text = string.Join("\n", distinctData.ToArray());

                LB_Uniq.ItemsSource = uniqId;

                //лист сообщений
                //List<List<string>> listBytes = new List<List<string>>();
                //лист для одного сообщения
                List<string> listMsg = new List<string>();

                for (int i = 0; i < uniqId.Count; i++)
                {
                    List<List<string>> listBytes = new List<List<string>>();
                    //Predicate<string> predicate = uniqId[i];
                    //containData = distinctData.FindAll($@"uniqId[i][0-9A-Z ]*");
                    containData = distinctData.FindAll(d => d.Contains(uniqId[i]));
                    for (int j = 0; j < containData.Count; j++)
                    {
                        listMsg = containData[j].Split(' ').ToList();
                        listMsg.RemoveRange(0, 2);
                        listMsg.RemoveAt(listMsg.Count - 1);
                        listBytes.Add(listMsg);
                    }

                    messages.Add(uniqId[i], listBytes);
                    //listBytes.Clear();
                }
                _messages = messages;
                _uniqId = uniqId;
                //для точки остановки
                List<string> uniqId1 = new List<string>();
                List<string> uniqId2 = new List<string>();
            }
            else MessageBox.Show("Файл не был выбран.");

        }

        //public ISeries[] Series0 { get; set; } = new ISeries[]
        //{
        //        new LineSeries<byte>
        //        {

        //            Stroke = new SolidColorPaint(SKColors.DarkBlue) { StrokeThickness = 1 },
        //            GeometryStroke = new SolidColorPaint(SKColors.Black) { StrokeThickness = 1 },
        //            GeometrySize = 1,
        //            IsHoverable = false,
        //            LineSmoothness = 0,
                    
        //            Fill = null
        //        },

        //};
        public void Lb_Uniq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LineByte0.Values = null;
            LineByte1.Values = null;
            LineByte2.Values = null;
            LineByte3.Values = null;
            LineByte4.Values = null;
            LineByte5.Values = null;
            LineByte6.Values = null;
            LineByte7.Values = null;
            string selectedID = (string)LB_Uniq.SelectedItem;
            List<double> massByte0 = new List<double>();
            List<double> massByte1 = new List<double>();
            List<double> massByte2 = new List<double>();
            List<double> massByte3 = new List<double>();
            List<double> massByte4 = new List<double>();
            List<double> massByte5 = new List<double>();
            List<double> massByte6 = new List<double>();
            List<double> massByte7 = new List<double>();

            for (int i = 0; i < _messages[selectedID].Count; i++)
            {
                massByte0.Add(Convert.FromHexString(_messages[selectedID][i][0]).ToList()[0]);
                massByte1.Add(Convert.FromHexString(_messages[selectedID][i][1]).ToList()[0]);
                massByte2.Add(Convert.FromHexString(_messages[selectedID][i][2]).ToList()[0]);
                massByte3.Add(Convert.FromHexString(_messages[selectedID][i][3]).ToList()[0]);
                massByte4.Add(Convert.FromHexString(_messages[selectedID][i][4]).ToList()[0]);
                massByte5.Add(Convert.FromHexString(_messages[selectedID][i][5]).ToList()[0]);
                massByte6.Add(Convert.FromHexString(_messages[selectedID][i][6]).ToList()[0]);
                massByte7.Add(Convert.FromHexString(_messages[selectedID][i][7]).ToList()[0]);
            }
            LineByte0.Values = massByte0.AsChartValues();
            //LineByte0.PointGeometry = null;
            //LineByte0.ScalesYAt = 0;
            //LineByte0.ToolTip = null;

            LineByte1.Values = massByte1.AsChartValues();
            LineByte2.Values = massByte2.AsChartValues();
            LineByte3.Values = massByte3.AsChartValues();
            LineByte4.Values = massByte4.AsChartValues();
            LineByte5.Values = massByte5.AsChartValues();
            LineByte6.Values = massByte6.AsChartValues();
            LineByte7.Values = massByte7.AsChartValues();

            List<string> strings = new List<string>();
            List<string> strings1 = new List<string>();
            
        }

    }
}
