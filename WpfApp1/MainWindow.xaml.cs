
using Microsoft.Win32;
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
using System.Windows.Ink;
using System.Windows.Interop;

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
            this.Show();

        }
        //Dictionary<string, Dictionary<int, List<string>>> messages = new Dictionary<string, Dictionary<int, List<string>>>();
        public Dictionary<string, List<List<string>>> _messages;
        List<string> _uniqId;
        List<string> _distinctData;

        string _selectedID;
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
                _distinctData = distinctData;
                //создаем словарь
                Dictionary<string, List<List<string>>> messages = new Dictionary<string, List<List<string>>>();
                //список для уникальных ID
                List<string> uniqId = new List<string>();
                for (int i = 0; i < distinctData.Count; i++)
                {
                    uniqId.Add(Regex.Replace(distinctData[i], @" \S*", ""));
                }
                uniqId = uniqId.Distinct().ToList();

                //удаление неизменяемых элементов, если этого хочет пользователь.
                List<string> containData = new List<string>();
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

                    containData = distinctData.FindAll(d => d.Contains(uniqId[i]));
                    for (int j = 0; j < containData.Count; j++)
                    {
                        listMsg = containData[j].Split(' ').ToList();
                        listMsg.RemoveAll(l => l.Contains(" "));
                        listMsg.RemoveAll(l => l.Equals(string.Empty));
                        listMsg.RemoveRange(0, 1);

                        if (listMsg.Count < 8)
                        {
                            for (int x = listMsg.Count; x < 8; x++)
                            {
                                listMsg.Add("00");
                            }
                        }
                        listBytes.Add(listMsg);
                    }

                    messages.Add(uniqId[i], listBytes);
                    //listBytes.Clear();
                }
                _messages = messages;
                _uniqId = uniqId;
                uniqId = null;
                //для точки остановки
                List<string> uniqId1 = new List<string>();
                List<string> uniqId2 = new List<string>();
            }
            else MessageBox.Show("Файл не был выбран.");

        }


        private void Lb_Uniq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            _selectedID = (string)LB_Uniq.SelectedItem;
            List<float> massByte0 = new List<float>();
            List<float> massByte1 = new List<float>();
            List<float> massByte2 = new List<float>();
            List<float> massByte3 = new List<float>();
            List<float> massByte4 = new List<float>();
            List<float> massByte5 = new List<float>();
            List<float> massByte6 = new List<float>();
            List<float> massByte7 = new List<float>();

            //заполнение массивов для добавления их значений в график(закомментирована попытка обхода пустого значения)
            for (int i = 0; i < _messages[_selectedID].Count; i++)
            {

                massByte0.Add(Convert.FromHexString(_messages[_selectedID][i][0]).ToList()[0]);
                massByte1.Add(Convert.FromHexString(_messages[_selectedID][i][1]).ToList()[0]);
                massByte2.Add(Convert.FromHexString(_messages[_selectedID][i][2]).ToList()[0]);
                massByte3.Add(Convert.FromHexString(_messages[_selectedID][i][3]).ToList()[0]);
                massByte4.Add(Convert.FromHexString(_messages[_selectedID][i][4]).ToList()[0]);
                massByte5.Add(Convert.FromHexString(_messages[_selectedID][i][5]).ToList()[0]);
                massByte6.Add(Convert.FromHexString(_messages[_selectedID][i][6]).ToList()[0]);
                massByte7.Add(Convert.FromHexString(_messages[_selectedID][i][7]).ToList()[0]);

            }
            //графики
            #region charts
            this.Show();
            LineByte0.PeData.Points = massByte0.Count;
            LineByte0.PeString.MainTitle = "0";
            LineByte0.PeUserInterface.Allow.Zooming = Gigasoft.ProEssentials.Enums.AllowZooming.HorzAndVert;
            LineByte0.PeUserInterface.HotSpot.Data = true;
            LineByte0.PeString.SubTitle = "";
            LineByte0.PePlot.Method = Gigasoft.ProEssentials.Enums.GraphPlottingMethod.Line;
            LineByte0.PeColor.SubsetColors[0] = Color.FromArgb(180, 0, 0, 130);
            LineByte0.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Small;
            for (int i = 0; i < massByte0.Count; i++)
            {
                LineByte0.PeData.Y[0, i] = massByte0[i]; ;
            }
            LineByte0.PePlot.SubsetLineTypes[0] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
            LineByte0.PeFunction.ReinitializeResetImage();
            LineByte0.Invalidate();
            LineByte0.UpdateLayout();


            this.Show();
            LineByte1.PeData.Points = massByte1.Count;
            LineByte1.PeString.MainTitle = "1";
            LineByte1.PeUserInterface.Allow.Zooming = Gigasoft.ProEssentials.Enums.AllowZooming.HorzAndVert;
            LineByte1.PeUserInterface.HotSpot.Data = true;
            LineByte1.PeString.SubTitle = "";
            LineByte1.PePlot.Method = Gigasoft.ProEssentials.Enums.GraphPlottingMethod.Line;
            LineByte1.PeColor.SubsetColors[0] = Color.FromArgb(180, 0, 0, 130);
            LineByte1.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Small;
            for (int i = 0; i < massByte1.Count; i++)
            {
                LineByte1.PeData.Y[0, i] = massByte1[i]; ;
            }
            LineByte1.PePlot.SubsetLineTypes[0] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
            LineByte1.PeFunction.ReinitializeResetImage();
            LineByte1.Invalidate();
            LineByte1.UpdateLayout();

            this.Show();
            LineByte2.PeData.Points = massByte2.Count;
            LineByte2.PeString.MainTitle = "2";
            LineByte2.PeString.SubTitle = "";
            LineByte2.PeUserInterface.Allow.Zooming = Gigasoft.ProEssentials.Enums.AllowZooming.HorzAndVert;
            LineByte2.PeUserInterface.HotSpot.Data = true;
            LineByte2.PePlot.Method = Gigasoft.ProEssentials.Enums.GraphPlottingMethod.Line;
            LineByte2.PeColor.SubsetColors[0] = Color.FromArgb(180, 0, 0, 130);
            LineByte2.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Small;
            for (int i = 0; i < massByte2.Count; i++)
            {
                LineByte2.PeData.Y[0, i] = massByte2[i]; ;
            }
            LineByte2.PePlot.SubsetLineTypes[0] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
            LineByte2.PeFunction.ReinitializeResetImage();
            LineByte2.Invalidate();
            LineByte2.UpdateLayout();

            this.Show();
            LineByte3.PeData.Points = massByte3.Count;
            LineByte3.PeString.MainTitle = "3";
            LineByte3.PeString.SubTitle = "";
            LineByte3.PeUserInterface.Allow.Zooming = Gigasoft.ProEssentials.Enums.AllowZooming.HorzAndVert;
            LineByte3.PeUserInterface.HotSpot.Data = true;
            LineByte3.PePlot.Method = Gigasoft.ProEssentials.Enums.GraphPlottingMethod.Line;
            LineByte3.PeColor.SubsetColors[0] = Color.FromArgb(180, 0, 0, 130);
            LineByte3.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Small;
            for (int i = 0; i < massByte3.Count; i++)
            {
                LineByte3.PeData.Y[0, i] = massByte3[i]; ;
            }
            LineByte3.PePlot.SubsetLineTypes[0] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
            LineByte3.PeFunction.ReinitializeResetImage();
            LineByte3.Invalidate();
            LineByte3.UpdateLayout();

            this.Show();
            LineByte4.PeData.Points = massByte4.Count;
            LineByte4.PeString.MainTitle = "4";
            LineByte4.PeString.SubTitle = "";
            LineByte4.PeUserInterface.Allow.Zooming = Gigasoft.ProEssentials.Enums.AllowZooming.HorzAndVert;
            LineByte4.PeUserInterface.HotSpot.Data = true;
            LineByte4.PePlot.Method = Gigasoft.ProEssentials.Enums.GraphPlottingMethod.Line;
            LineByte4.PeColor.SubsetColors[0] = Color.FromArgb(180, 0, 0, 130);
            LineByte4.PeColor.PointBorderColor = Color.FromRgb(255, 255, 255);
            LineByte4.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Small;
            for (int i = 0; i < massByte4.Count; i++)
            {
                LineByte4.PeData.Y[0, i] = massByte4[i]; ;
            }
            LineByte4.PePlot.SubsetLineTypes[0] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
            LineByte4.PeFunction.ReinitializeResetImage();
            LineByte4.Invalidate();
            LineByte4.UpdateLayout();

            this.Show();
            LineByte5.PeData.Points = massByte5.Count;
            LineByte5.PeString.MainTitle = "5";
            LineByte5.PeString.SubTitle = "";
            LineByte5.PeUserInterface.Allow.Zooming = Gigasoft.ProEssentials.Enums.AllowZooming.HorzAndVert;
            LineByte5.PeUserInterface.HotSpot.Data = true;
            LineByte5.PePlot.Method = Gigasoft.ProEssentials.Enums.GraphPlottingMethod.Line;
            LineByte5.PeColor.SubsetColors[0] = Color.FromArgb(180, 0, 0, 130);
            LineByte5.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Small;
            for (int i = 0; i < massByte5.Count; i++)
            {
                LineByte5.PeData.Y[0, i] = massByte5[i]; ;
            }
            LineByte5.PePlot.SubsetLineTypes[0] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
            LineByte5.PeFunction.ReinitializeResetImage();
            LineByte5.Invalidate();
            LineByte5.UpdateLayout();

            this.Show();
            LineByte6.PeData.Points = massByte6.Count;
            LineByte6.PeString.MainTitle = "6";
            LineByte6.PeString.SubTitle = "";
            LineByte6.PeUserInterface.Allow.Zooming = Gigasoft.ProEssentials.Enums.AllowZooming.HorzAndVert;
            LineByte6.PeUserInterface.HotSpot.Data = true;
            LineByte6.PePlot.Method = Gigasoft.ProEssentials.Enums.GraphPlottingMethod.Line;
            LineByte6.PeColor.SubsetColors[0] = Color.FromArgb(180, 0, 0, 130);
            LineByte6.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Small;
            for (int i = 0; i < massByte6.Count; i++)
            {
                LineByte6.PeData.Y[0, i] = massByte6[i]; ;
            }
            LineByte6.PePlot.SubsetLineTypes[0] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
            LineByte6.PeFunction.ReinitializeResetImage();
            LineByte6.Invalidate();
            LineByte6.UpdateLayout();

            this.Show();
            LineByte7.PeData.Points = massByte7.Count;
            LineByte7.PeString.MainTitle = "7";
            LineByte7.PeString.SubTitle = "";
            LineByte7.PeUserInterface.Allow.Zooming = Gigasoft.ProEssentials.Enums.AllowZooming.HorzAndVert;
            LineByte7.PeUserInterface.HotSpot.Data = true;
            LineByte7.PePlot.Method = Gigasoft.ProEssentials.Enums.GraphPlottingMethod.Line;
            LineByte7.PeColor.SubsetColors[0] = Color.FromArgb(180, 0, 0, 130);
            LineByte7.PeFont.FontSize = Gigasoft.ProEssentials.Enums.FontSize.Small;
            for (int i = 0; i < massByte7.Count; i++)
            {
                LineByte7.PeData.Y[0, i] = massByte7[i]; ;
            }
            LineByte7.PePlot.SubsetLineTypes[0] = Gigasoft.ProEssentials.Enums.LineType.ThinSolid;
            LineByte7.PeFunction.ReinitializeResetImage();
            LineByte7.Invalidate();
            LineByte7.UpdateLayout();
            #endregion


            LB_Messages.ItemsSource = string.Empty;
            LB_Messages.ItemsSource = _distinctData.FindAll(d => d.Contains(_selectedID));
            List<string> strings1 = new List<string>();

        }





        #region методы графиков при выборе точки
        private void LineByte0_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
        {
            try
            {
                Tab_Charts.IsSelected = false;
                Tab_Msg.IsSelected = true;
                LB_Messages.SelectedIndex = e.PointIndex;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LineByte1_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
        {
            try
            {
                Tab_Charts.IsSelected = false;
                Tab_Msg.IsSelected = true;
                LB_Messages.SelectedIndex = e.PointIndex;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LineByte2_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
        {
            try
            {
                Tab_Charts.IsSelected = false;
                Tab_Msg.IsSelected = true;
                LB_Messages.SelectedIndex = e.PointIndex;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LineByte3_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
        {
            try
            {
                Tab_Charts.IsSelected = false;
                Tab_Msg.IsSelected = true;
                LB_Messages.SelectedIndex = e.PointIndex;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LineByte4_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
        {
            try
            {
                Tab_Charts.IsSelected = false;
                Tab_Msg.IsSelected = true;
                LB_Messages.SelectedIndex = e.PointIndex;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LineByte5_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
        {
            try
            {
                Tab_Charts.IsSelected = false;
                Tab_Msg.IsSelected = true;
                LB_Messages.SelectedIndex = e.PointIndex;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LineByte6_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
        {
            try
            {
                Tab_Charts.IsSelected = false;
                Tab_Msg.IsSelected = true;
                LB_Messages.SelectedIndex = e.PointIndex;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LineByte7_PeDataHotSpot(object sender, Gigasoft.ProEssentials.EventArg.DataHotSpotEventArgs e)
        {
            try
            {
                Tab_Charts.IsSelected = false;
                Tab_Msg.IsSelected = true;
                LB_Messages.SelectedIndex = e.PointIndex;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}

