using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace AutoBrowserChooser
{
    /// <summary>
    /// Interaction logic for FileBox.xaml
    /// </summary>
    public partial class FileInputBox : UserControl
    {
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            "FileName",
            typeof(String),
            typeof(FileInputBox),
            new UIPropertyMetadata(
                String.Empty,
                TextChangedCallback));

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }

        }

        private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileInputBox input = (FileInputBox)d;
            input.uxFileName.Text = e.NewValue as String;
        }

        private OpenFileDialog ofd;
        public FileInputBox()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "Executable files (.exe)|*.exe";
            InitializeComponent();
        }

        private void uxBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                uxFileName.Text = ofd.FileName;
            }
        }

        private void uxFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValue(FileNameProperty, uxFileName.Text);
        }
    }
}
