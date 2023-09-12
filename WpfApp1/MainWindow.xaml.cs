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

        public void Btn_ACE(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.log)|*.log|All files (*.*)|*.*";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                ////заполнение массива сообщений без выборки
                //if (CheckBox_Filter.IsChecked == false)
                //{

                //    TB_List.Clear();
                //    string filename = ofd.FileName;
                //    var files = System.IO.File.ReadAllText(filename);
                //    List<string> words = new List<string>();
                //    words = files.Split('\n').ToList();
                //    var oneWord = words[0];
                //    words.RemoveRange(0, 2);
                //    Label_Speed_CAN.Content = string.Concat(oneWord.Where(Char.IsDigit));
                //    Label_Msg_In.Content = words.Count;
                //    TB_List.Text = string.Join("\n", words.ToArray());

                //}
                //заполнение массива сообщений с выборкой только различных данных
                //else if (CheckBox_Filter.IsChecked == true)
                //{
                    TB_List.Clear();
                    string filename = ofd.FileName;
                    var files = System.IO.File.ReadAllText(filename);
                    List<string> words = new List<string>();
                    words = files.Split('\n').ToList();
                    var oneWord = words[0];
                    words.RemoveRange(0, 2);
                    Label_Speed_CAN.Content = string.Concat(oneWord.Where(Char.IsDigit));
                    Label_Msg_In.Content = words.Count;



                    List<string> dataSheets = new List<string>();

                    //int foundSpace2 = words.IndexOf(i, foundSpace + 1);
                    int range;
                    for (int i = 0; i < words.Count; i++)
                    {
                        dataSheets.Add(words[i]/*.Remove(0, range)*/);
                        dataSheets[i] = Regex.Replace(dataSheets[i], "\\s+", " ");

                        range = dataSheets[i].IndexOf(' ', 0) + 1;

                        while (range == 1)
                        {
                            dataSheets[i] = dataSheets[i].Remove(0, 1);
                            range = dataSheets[i].IndexOf(' ', 0) + 1;
                        }
                        
                        dataSheets[i] = dataSheets[i].Remove(0, range);
                        //dataSheets.Add(words[i].Remove(foundSpace, foundSpace2-foundSpace));
                    }
                    List<string> distinctData = new List<string>();
                    //distinctData = dataSheets.Distinct().ToList();
                    Label_Msq_Unique.Content = distinctData.Count;
                    //TB_List.Text = string.Join("\n", distinctData.ToArray());

                    //Оставляем нужные данные для гарфика 
                    int foundSpace1;
                    int foundSpace2;
                    for (int i = 0; i < dataSheets.Count; i++)
                    {
                        foundSpace1 = dataSheets[i].IndexOf(' ', 0) + 1;
                        foundSpace2 = dataSheets[i].IndexOf(' ', foundSpace1);
                        dataSheets[i] = dataSheets[i].Remove(foundSpace1, foundSpace2 - foundSpace1);
                    }
                    //Поменять, нужна другая логика!!
                if (CheckBox_Filter.IsChecked == true)
                {
                    distinctData = dataSheets.Distinct().ToList();
                }
                else if (CheckBox_Filter.IsChecked == false)
                {
                    distinctData = dataSheets;
                }
                    TB_List.Text = string.Join("\n", distinctData.ToArray());
                //создаем словарь
                Dictionary<string, Dictionary<int, List<string>>> messages = new Dictionary<string, Dictionary<int, List<string>>>();

                List<string> uniqId = new List<string>();


                    //range = distinctData[0].IndexOf(" ", 0) + 1;
                    //uniqId.Add(distinctData[0].Remove(range));
                    for (int i = 0; i < distinctData.Count; i++)
                    {
                        distinctData[i] = Regex.Replace(distinctData[i], @" \s", "");
                    uniqId.Add(Regex.Replace(distinctData[i], @" \S*", ""));
                    }
                    uniqId = uniqId.Distinct().ToList();
                    //_listUniqId = uniqId;
                    LB_Uniq.ItemsSource = uniqId;

                    //лист сообщений
                    List<List<string>> listBytes = new List<List<string>>();
                    //лист для одного сообщения
                    List<string> listMsg = new List<string>();

                    //цикл, в котором в списке оставляем только сообщения
                    for (int i = 0; i < dataSheets.Count; i++)
                    {
                        
                        dataSheets[i] = Regex.Replace(dataSheets[i], @"\b\S{3,15} ", "");
                    }

                    for (int i = 0; i < uniqId.Count; i++) 
                    {
                        int count_msg = 0;
                        Dictionary<int, List<string>> dict_msg_id = new Dictionary<int, List<string>>();
                        for (int j = 0; j < distinctData.Count; j++)
                        {
                            //обрабатываем старый массив, находя сообщения по идентификатору


                            if (distinctData[j].Contains(uniqId[i]) == true)
                            {
                                //разделяем сообщение по байтам и добавляем в лист конкретного сообщения
                                listMsg = dataSheets[j].Split('\r').ToList();
                                //listMsg.RemoveRange(0,1);
                                //listMsg.RemoveAt(listMsg.Count - 1 - 1);
                                listBytes.Add(listMsg);
                                //добавляем в словарь идентификатора сообщение
                                dict_msg_id.Add(count_msg, listBytes[count_msg]);
                                count_msg++;
                                //удаляем обработанную строку для оптимизации
                                distinctData.RemoveAt(j);
                            }
                        }
                        //заполняем конечный словарь
                        messages.Add(uniqId[i], dict_msg_id);
                        
                    }
                    //для точки остановки
                    List<string> uniqId1 = new List<string>();
                //}


    }
            else MessageBox.Show("Файл не был выбран.");

        }
        public ISeries[] Series { get; set; } = new ISeries[]
             {
                new LineSeries<double>
                {
                    
                    Stroke = new SolidColorPaint(SKColors.DarkBlue) { StrokeThickness = 1 },
                    GeometryStroke = new SolidColorPaint(SKColors.Black) { StrokeThickness = 1 },
                    GeometrySize = 1,
                    LineSmoothness = 0,
                    Values = new double[] { },
                    Fill = null

                }
              };

        

        public void Lb_Uniq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string oneUniq = LB_Uniq.SelectedItem.ToString();
            //List<int> listByte0 = new List<int>();
            //for (int i = 0; i <; i++)
            //{

            //}
        }
    }
}
