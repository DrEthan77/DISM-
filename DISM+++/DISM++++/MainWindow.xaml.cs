using System;
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
using System.IO;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Management;

namespace DISM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CanvasLabel.Text = "Apply Image";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Visible;
            Completion_Canvas.Visibility = Visibility.Hidden;
        }

        private void Start_Apply_Image_Click(object sender, RoutedEventArgs e)
        {
            if (ImageFileFrom.Text.Contains(".wim"))
            {
                Format_Bootable_part1();

            }
            else
            {
                MessageBox.Show("No image file selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void image()
        {
            Process p = new Process();
            p.StartInfo.FileName = "DISM.exe";
            p.StartInfo.Arguments = "/Apply-Image /ImageFile:" + ImageFileFrom.Text + " /index:1 /ApplyDir:" + ApplyImageTo.Text + " /CheckIntegrity /NoRpFix /Compact /EA";
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            //proc.StartInfo.Verb = "runas";
            p.Start();

            p.WaitForExit();
            Format_Bootable_part2();
        }
        private void Format_Bootable_part1()
        {
            Process p = new Process();
            p.StartInfo.FileName = "diskpart.exe";
            p.StartInfo.RedirectStandardInput = true;
           // p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            bool remove_s = false;
            if (Directory.Exists(@"S:\"))
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                for (int i = 0; i < drives.Length; i++)
                {
                    if (drives[i].RootDirectory.ToString().Contains("S") && drives[i].VolumeLabel.ToLower() != "system")
                    {
                        MessageBox.Show("A drive with mount point S already exists, removing mount point, you will have to remount it!!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        remove_s = true;
                        break;
                    }
                }
             }
                p.Start();

            if (remove_s) { 
                p.StandardInput.WriteLine(@"select volume S:\");
                p.StandardInput.WriteLine("remove letter S");
            }
            p.StandardInput.WriteLine("select volume "+ ApplyImageTo.Text);
            p.StandardInput.WriteLine("clean");
            p.StandardInput.WriteLine("convert gpt");
            p.StandardInput.WriteLine("create partition efi size=250");
            p.StandardInput.WriteLine("format quick fs=fat32 label=system");
            p.StandardInput.WriteLine("assign letter S");
            p.StandardInput.WriteLine("create partition primary");
            p.StandardInput.WriteLine("assign letter " + ApplyImageTo.Text[0]);
            p.StandardInput.WriteLine("format quick fs=ntfs");
            p.StandardInput.WriteLine("exit");
            //string output = p.StandardOutput.ReadToEnd();
            //MessageBox.Show(output);
            p.WaitForExit();
            if (Directory.Exists(@"S:\"))
            {
                image();

            }
            else
            {
                Format_Bootable_part1();
            }

        }
        private void Format_Bootable_part2()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
            p.StandardInput.WriteLine("bcdboot " + ApplyImageTo.Text + "windows /s S: /f uefi");
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();           
            myProcess_Exited();
        }
        private void myProcess_Exited()
        {
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "DISM.exe";
            proc.StartInfo.Arguments = "/Capture-Image /ImageFile:" + ImageTo.Text + " /CaptureDir:" + ImageFrom.Text + " /Name:Drive-C /Compress:max /Bootable";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.CreateNoWindow = false;
            MessageBox.Show("running");
            try
            {
                proc.Start();
                proc.WaitForExit();
                myProcess_Exited();

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                throw;
            }
            
        }
        private void Start_Image_Click(object sender, RoutedEventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "DISM.exe";
            proc.StartInfo.Arguments = "/Capture-Image /ImageFile:"+ImageFileFrom.Text+" /CaptureDir:"+ApplyImageTo.Text+" /Name:Drive-C /Compress:max /Bootable";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.Verb = "runas";
//            MessageBox.Show(proc.ExitCode.ToString());
            proc.Start();
            proc.WaitForExit();
            myProcess_Exited();

        }
        private void FindImage(object sender, RoutedEventArgs e)
        {
            try { 
            VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                ImageFileFrom.Text = openFileDialog.FileName;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                throw;
            }
            
}
        private void FindImageTo(object sender, RoutedEventArgs e)
        {
            try { 
            VistaFolderBrowserDialog openFileDialog = new VistaFolderBrowserDialog();
            if (openFileDialog.ShowDialog() == true)
                ApplyImageTo.Text = openFileDialog.SelectedPath;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                throw;
            }
        }
        private void FindImageFrom(object sender, RoutedEventArgs e)
        {
            try
            {
                VistaFolderBrowserDialog openFileDialog = new VistaFolderBrowserDialog();
                if (openFileDialog.ShowDialog() == true)
                    ImageFrom.Text = openFileDialog.SelectedPath;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                throw;
            }
        }

        private void FindSaveTo(object sender, RoutedEventArgs e)
        {
            try { 
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "WIM Files | *.wim";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".wim";
            saveFileDialog.FileName = "Image";
            if (saveFileDialog.ShowDialog() == true)
                ImageTo.Text = saveFileDialog.FileName;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                throw;
            }
        }

        
        private void CreateImage(object sender, RoutedEventArgs e)
        {
            CanvasLabel.Text = "Create Image";
            Create_Image_Canvas.Visibility = Visibility.Visible;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Hidden;
        }

        private void ApplyImage(object sender, RoutedEventArgs e)
        {
            CanvasLabel.Text = "Apply Image";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Visible;
            Completion_Canvas.Visibility = Visibility.Hidden;
        }

        private void DiskPart(object sender, RoutedEventArgs e)
        {
            CanvasLabel.Text = "Disk Part";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
        }

    }
}
