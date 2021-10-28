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
            CanvasLabel.Text = "Create Image";
            Create_Image_Canvas.Visibility = Visibility.Visible;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "DISM.exe";
            proc.StartInfo.Arguments = "/Apply-Image /ImageFile:<" + ImageFileFrom.Text + ">  /ApplyDir:<" + ApplyImageTo.Text + ">";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.Verb = "runas";

            proc.WaitForExit();
            proc.Exited += new EventHandler(myProcess_Exited);
            proc.Start();
        }
        private void myProcess_Exited(object sender, System.EventArgs e)
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
            proc.StartInfo.Verb = "runas";
            MessageBox.Show("running");
            proc.Exited += new EventHandler(myProcess_Exited);
            try
            {
                proc.Start();
                proc.WaitForExit();
                MessageBox.Show(proc.ExitCode.ToString());
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
            proc.Exited += new EventHandler(myProcess_Exited);
            proc.Start();
            proc.WaitForExit();

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

        private void Start_Apply_Image_Click(object sender, RoutedEventArgs e)
        {
            if (!FormatDrive_CommandLine(ApplyImageTo.Text.ElementAt(0)))
                MessageBox.Show("Error failed to format drive");
            else
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "DISM.exe";
                proc.StartInfo.Arguments = "/Apply-Image /ImageFile:" + ImageFileFrom.Text + " /index:1 /ApplyDir:" + ApplyImageTo.Text + " /CheckIntegrity /NoRpFix /Compact /EA";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.Verb = "runas";
                proc.Exited += new EventHandler(myProcess_Exited);
                proc.Start();

                proc.WaitForExit();
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

        public static bool FormatDrive_CommandLine(char driveLetter, string label = "", string fileSystem = "NTFS", bool quickFormat = true, bool enableCompression = false, int? clusterSize = null)
        {
            #region args check

            if (!Char.IsLetter(driveLetter))
            {
                return false;
            }

            #endregion
            bool success = false;
            string drive = driveLetter + ":";
            try
            {
                var di = new DriveInfo(drive);
                var psi = new ProcessStartInfo();
                psi.FileName = "format.com";
                psi.CreateNoWindow = true; //if you want to hide the window
                psi.WorkingDirectory = Environment.SystemDirectory;
                psi.Arguments = "/FS:" + fileSystem +
                                             " /Y" +
                                             " /V:" + label +
                                             (quickFormat ? " /Q" : "") +
                                             ((fileSystem == "NTFS" && enableCompression) ? " /C" : "") +
                                             (clusterSize.HasValue ? " /A:" + clusterSize.Value : "") +
                                             " " + drive;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                var formatProcess = Process.Start(psi);
                var swStandardInput = formatProcess.StandardInput;
                swStandardInput.WriteLine();
                formatProcess.WaitForExit();
                success = true;
            }
            catch (Exception) { }
            return success;
        }
    }
}
