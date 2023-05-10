using NodeLookup.Methods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NodeLookup
{    
    public partial class NodeLookup : Window
    {
        readonly Comspecs comspecs;
        readonly ComspecFileReader csFileReader;
        SortedDictionary<string, string> activeCMDs;
        Dictionary<string, string> activeBITs;
        public NodeLookup()
        {
            Title = "NodeLookup v" + About.version;
            InitializeComponent();
            comspecs = new Comspecs();
            if(comspecs.direxists)
                csFileReader = new ComspecFileReader();
            else
            {
                MessageBox.Show("Comspec files not found", "No Comspec Files", MessageBoxButton.OK, MessageBoxImage.Error);
                tbCMDinput.IsEnabled = false;
                tbCMDinput.Background = Brushes.Red;
                tbInput.IsEnabled = false;
                tbInput.Background = Brushes.Red;
            }
        }

        private void Search(string searchdetail)
        {
            try
            {
                switch (searchdetail)
                {
                    case "node":
                        if (tbInput.Text != "" && comspecs.Comspec.ContainsKey(tbInput.Text.ToUpper()))
                        {
                            List<NodeDetails> details = comspecs.Comspec[tbInput.Text.ToUpper()];
                            lbNodeList.ItemsSource = details;
                            return;
                        }
                        lbNodeList.ItemsSource = new List<string>() { "Not found!" };
                        break;
                    case "cmd":
                        if (tbCMDinput.Text != "" && tbCMDinput.Text.Count() == 2 && (NodeDetails)lbNodeList.SelectedItem != null)
                        {
                            try
                            {
                                tbCMDResult.Text = activeCMDs[tbCMDinput.Text.ToUpper()];
                                return;
                            }
                            catch
                            {
                                tbCMDResult.Text = "Not found!";
                                return;
                            }
                        }
                        tbCMDResult.Text = "Not found!";
                        break;
                    case "bits":
                        lbDualStatus.Text = "";
                        if (tbBITinput.Text != "" && (NodeDetails)lbNodeList.SelectedItem != null)
                        {
                            try
                            {
                                tbBITResult.Text = activeBITs[tbBITinput.Text.ToUpper()];
                                if(csFileReader.dualStatus == true)
                                {
                                    lbDualStatus.Text = "Dual status bits in comspec file, multiline in results";
                                }
                                return;
                            }
                            catch
                            {
                                tbBITResult.Text = "Not found!";
                                return;
                            }
                        }
                        tbBITResult.Text = "Not found!";
                        break;
                }
            }
            catch
            {
                MessageBox.Show("Lookup error, not able to process serch query.", "Query error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void TbInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            Search("node");
            if ((sender as TextBox).Text != "")
                CalcInverted((sender as TextBox).Text);
            if ((sender as TextBox).Text == "")
            {
                lbsubstract.Content = "";
            }
        }

        private void CalcInverted(string text)
        {
            lbsubstract.Content = "FF - " + text.PadLeft(2, '0') + " = " + (255 - int.Parse(text, System.Globalization.NumberStyles.HexNumber)).ToString("X").ToUpper();
        }

        private void TbInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[A-Fa-f0-9]+$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TbCMDinput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            Search("cmd");
        }

        private void LbNodeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbNodeList.SelectedItem != null && lbNodeList.SelectedItem.ToString() != "Not found!")
            {
                try
                {
                    tbCMDResult.Clear();
                    tbBITResult.Clear();
                    activeCMDs?.Clear();
                    activeBITs?.Clear();
                    NodeDetails test = (NodeDetails)lbNodeList.SelectedItem;
                    csFileReader.ReadFile(test.Path);
                    activeCMDs = csFileReader.cmds;
                    activeBITs = csFileReader.bits;
                    lbAvaiCMDs.ItemsSource = activeCMDs;
                    Search("cmd");
                    Search("bits");
                }
                catch
                {
                    MessageBox.Show("Search indexing failed for selected item.", "Indexing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            }
            if (activeCMDs != null) activeCMDs.Clear();
            lbAvaiCMDs.Items.Refresh();
        }

        private void LbAvaiCMDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as ListBox).SelectedItem != null)
            {
                KeyValuePair<string, string> cmd = (KeyValuePair<string, string>)(sender as ListBox).SelectedItem;
                tbCMDinput.Text = cmd.Key;
                Search("cmd");
            }
        }

        private void TbBITinput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            Search("bits");
        }

        private void TbBITinput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void CSOpen_Click(object sender, RoutedEventArgs e)
        {
            if(lbNodeList.SelectedItem != null && lbNodeList.SelectedItem.ToString() != "Not found!")
            {
                try
                {
                    Process.Start("Notepad.exe", (lbNodeList.SelectedItem as NodeDetails).Path);
                }
                catch
                {

                }
            }
        }
    }
}