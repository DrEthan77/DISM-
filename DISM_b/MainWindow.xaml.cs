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
        public static string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		public static int TimesPt1HasRun;
		public MainWindow()
        {
            InitializeComponent();
            CanvasLabel.Text = "Apply Image";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Visible;
            Completion_Canvas.Visibility = Visibility.Hidden;
        }
        //Runs when apply image is clicked
        private void Start_Apply_Image_Click(object sender, RoutedEventArgs e)
        {

            if (ImageFileFrom.Text.Contains(".wim"))
            {
				TimesPt1HasRun = 0;
				Format_Bootable_part1();            
            }
            else
                MessageBox.Show("No image file selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        //Formats the drive to allow for the imaging process
        private void Format_Bootable_part1()
        {			
			Process p = new Process();
            p.StartInfo.FileName = "diskpart.exe";
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
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
            p.WaitForExit();
            if (Directory.Exists(@"S:\"))
            {
                image();
                return;
            }
            else
            {
                if (TimesPt1HasRun < 4)
                {
					TimesPt1HasRun++;
					Format_Bootable_part1();
                    return;
                }
                else
                {
                    MessageBox.Show("Diskpart failed too many times, showing output. " +
                        "(I recommend manually wiping the disk using partition wizard and creating a new partition on it with the label C, " +
                        "then rerunning DISM.b)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    string output = p.StandardOutput.ReadToEnd();
                    MessageBox.Show(output);
                    return;
                }
            }			
		}
        //Runs DISM command to image the computer with selected image
        private void image()
        {
            Process p = new Process();
            p.StartInfo.FileName = "DISM.exe";
            p.StartInfo.Arguments = "/Apply-Image /ImageFile:" + ImageFileFrom.Text + " /index:1 /ApplyDir:" + ApplyImageTo.Text + " /CheckIntegrity /NoRpFix /Compact /EA";
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
            p.WaitForExit();
            Format_Bootable_part2();
        }
        //Creates the Windows Boot Manager on the automatically created system partition from part 1
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
            myProcess_Exited();
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
        }
        //You will see this stuff run 4 times because for some reason if it is run only once it doesnt actually show
        private void myProcess_Exited()
        {
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
            CanvasLabel.Text = "Proccess Complete!";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Visible;
        }
        //Starts the wim capture process.
        private void Capture_Start_Click(object sender, RoutedEventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "DISM.exe";
            proc.StartInfo.Arguments = "/Capture-Image /ImageFile:" + ImageTo.Text + " /CaptureDir:" + ImageFrom.Text + " /Name:Drive-C /Compress:max /Bootable";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.CreateNoWindow = false;
            try
            {
                proc.Start();
                proc.WaitForExit();
                myProcess_Exited();
                CanvasLabel.Text = "Proccess Complete!";
                Create_Image_Canvas.Visibility = Visibility.Hidden;
                Apply_Image_Canvas.Visibility = Visibility.Hidden;
                Completion_Canvas.Visibility = Visibility.Visible;
                CanvasLabel.Text = "Proccess Complete!";
                Create_Image_Canvas.Visibility = Visibility.Hidden;
                Apply_Image_Canvas.Visibility = Visibility.Hidden;
                Completion_Canvas.Visibility = Visibility.Visible;
                CanvasLabel.Text = "Proccess Complete!";
                Create_Image_Canvas.Visibility = Visibility.Hidden;
                Apply_Image_Canvas.Visibility = Visibility.Hidden;
                Completion_Canvas.Visibility = Visibility.Visible;
                CanvasLabel.Text = "Proccess Complete!";
                Create_Image_Canvas.Visibility = Visibility.Hidden;
                Apply_Image_Canvas.Visibility = Visibility.Hidden;
                Completion_Canvas.Visibility = Visibility.Visible;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }            
        }
        //Opens file explorer to find the .wim image to use to image the computer
        private void FindImage(object sender, RoutedEventArgs e)
        {
            try { 
            VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();
                openFileDialog.InitialDirectory = @"P:\";
                openFileDialog.Filter = "WIM Image Files (*.wim)|*.wim|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                ImageFileFrom.Text = openFileDialog.FileName;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            
        }
        //Opens folder explorer to find the folder to image to
        private void FindImageTo(object sender, RoutedEventArgs e)
        {
            try { 
            VistaFolderBrowserDialog openFileDialog = new VistaFolderBrowserDialog();
            if (openFileDialog.ShowDialog() == true)
                ApplyImageTo.Text = openFileDialog.SelectedPath;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        //Opens folder explorer to find the the folder to image from
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
                MessageBox.Show(ee.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        //Opens the file explorer so you can choose where to save the wim
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
                MessageBox.Show(ee.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        //Switches Main Window view to the create image view  
        private void CreateImage(object sender, RoutedEventArgs e)
        {
            CanvasLabel.Text = "Create Image";
            Create_Image_Canvas.Visibility = Visibility.Visible;
            Apply_Image_Canvas.Visibility = Visibility.Hidden;
            Completion_Canvas.Visibility = Visibility.Hidden;
        }
        //Switches Main Window view to the apply image view 
        private void ApplyImage(object sender, RoutedEventArgs e)
        {
            CanvasLabel.Text = "Apply Image";
            Create_Image_Canvas.Visibility = Visibility.Hidden;
            Apply_Image_Canvas.Visibility = Visibility.Visible;
            Completion_Canvas.Visibility = Visibility.Hidden;
        }

		private void CheckVersion(object sender, RoutedEventArgs e)
		{
            MessageBox.Show("Current version #" + Version);
		}
	}
}
