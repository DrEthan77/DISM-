﻿using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Threading.Tasks;

namespace DISM___
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
            Process proc = new Process();
            proc.StartInfo.FileName = "DISM.exe";
           proc.StartInfo.Arguments = "/Apply-Image /ImageFile:<"+ImageFileFrom.Text+">  /ApplyDir:<"+ApplyImageTo.Text+">";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.Verb = "runas";
           
            proc.WaitForExit();
            proc.Exited += new EventHandler(myProcess_Exited);
            proc.Start();
        }
        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            MessageBox.Show("done?");
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Start_Image_Click(object sender, RoutedEventArgs e)
        {
        }
        private void FindImage(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                ImageFileFrom.Text = openFileDialog.FileName;
        }
        private void FindImageTo(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFileDialog = new VistaFolderBrowserDialog();
            if (openFileDialog.ShowDialog() == true)
                ApplyImageTo.Text = openFileDialog.SelectedPath;
        }
        private void FindImageFrom(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFileDialog = new VistaFolderBrowserDialog();
            if (openFileDialog.ShowDialog() == true)
                ImageFrom.Text = openFileDialog.SelectedPath;
        }

        private void FindSaveTo(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "WIM Files | *.wim";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".wim";
            saveFileDialog.FileName = "Image";
            if (saveFileDialog.ShowDialog() == true)
                ImageTo.Text = saveFileDialog.FileName;
        }
        private void CreateImage(object sender, RoutedEventArgs e)
        {
            CanvasLabel.Text = "Create Image";
            Create_Image_Canvas.Visibility = Visibility.Visible;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Diskpart_Canvas.Visibility = Visibility.Hidden;
        }

        private void ApplyImage(object sender, RoutedEventArgs e)
        {
            CanvasLabel.Text = "Apply Image";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Visible;
            Diskpart_Canvas.Visibility = Visibility.Hidden;
        }

        private void DiskPart(object sender, RoutedEventArgs e)
        {
            CanvasLabel.Text = "Disk Part";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Diskpart_Canvas.Visibility = Visibility.Visible;
        }
    }
}
