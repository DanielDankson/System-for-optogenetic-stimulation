using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIS.Imaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace OptogeneticStimulaiton
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        public Form1()
        {
            InitializeComponent();

            serialPort = new SerialPort();
        }
        string dir_name_running;
        public string com;
        public int bilde;

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            comboBox1.SelectedIndex = 0;
            button1.Enabled = false;
            button3.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = true;
            if (!icImagingControl1.LoadShowSaveDeviceState("lastSelectedDeviceState.xml"))
            {
                MessageBox.Show("No device was selected.", "Grabbing an Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                icImagingControl1.Sink = new TIS.Imaging.FrameSnapSink();

                icImagingControl1.LiveStart();
            }
        }
        public string appdataGet()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string this_folder = Path.Combine(folder, "ICU_IR_APP");
            Directory.CreateDirectory(this_folder);
            string this_txt = Path.Combine(this_folder, "dir.txt");

            if (!File.Exists(this_txt))
            {
                File.Create(this_txt);
            }

            return this_txt;
        }

        //Open the settings menu for the camera.

        private void cameraSettingsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {

                icImagingControl1.ShowPropertyDialog();

            }
            catch
            {
                MessageBox.Show(e.ToString());
            }
        }

        //Set the directory of where the pictures will be saved.
        private void directoryToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string getDir = appdataGet();
            string setDir;

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dialog.IsFolderPicker = true;
            dialog.Title = "Set Directory";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                setDir = dialog.FileName;
                File.WriteAllText(getDir, setDir);
            }
        }

        //Save as a .jpg
        private void saveAsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {

                if (!icImagingControl1.LiveVideoRunning)
                {
                    MessageBox.Show("Camera is not currently running.");
                }
                else
                {
                    TIS.Imaging.FrameSnapSink snapSink = icImagingControl1.Sink as TIS.Imaging.FrameSnapSink;

                    TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(5));

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    //saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
                    saveFileDialog1.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
                    saveFileDialog1.FilterIndex = 1;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        frm.SaveAsJpeg(saveFileDialog1.FileName, 1310720);
                        //frm.SaveAsBitmap(saveFileDialog1.FileName);
                    }
                }
            }
            catch { }
        }

        //Exit the application
        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Button 1 is the start button. It creates a folder for the pictures to be saved in and starts timer 1.
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {

                string new_directory = appdataGet();

                if (!Directory.Exists(File.ReadAllText(new_directory)))
                {
                    MessageBox.Show("Directory Error", "Selected directory does not exist.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    DateTime time = DateTime.Now;
                    string format_folder = "d_MMM_yyyy_HH_mm";
                    string name_folder = time.ToString(format_folder);
                    string new_folder = File.ReadAllText(new_directory) + @"\" + name_folder;
                    Directory.CreateDirectory(new_folder);
                    dir_name_running = new_folder;


                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    timer1.Interval = 1000;
                    timer1.Start();
                }
            }
            catch
            {
                MessageBox.Show("Wrong COM");
            }
        }

        //Button 2 checks if the opened port is the right port.
        private void button2_Click_1(object sender, EventArgs e)
        {
            string Data = string.Empty;
            try
            {


                Data = serialPort.ReadExisting();



                if (Data != string.Empty)
                {
                    button1.Enabled = true;
                    button2.Enabled = false;
                    button3.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Wrong port", "Wrong port", MessageBoxButtons.OK);
                    button2.Enabled = false;
                    button3.Enabled = true;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error selecting port. Try another.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = true;
            }
        }

        //Button 3 closes the open port.
        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            button4.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
        }

        //Button 4 opens the port selected in comboBox 1.
        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                string comName = comboBox1.Text;
                serialPort.PortName = comName;
                serialPort.BaudRate = 9600;
                serialPort.Open();

                button4.Enabled = false;
                button2.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Timer 1 runs untill it gets a start bit (first position) then it stops and starts timer 2.
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            bool haveData = false;
            string Data = string.Empty;


            while (!haveData)
            {
                Data = serialPort.ReadExisting();
                if (!string.IsNullOrEmpty(Data))
                {
                    haveData = true;
                }
            }
            string[] dataParts = Data.Split(',');
            try
            {

                string a = dataParts[0];

                if (Convert.ToInt32(a) == 1)
                {
                    timer2.Interval = 1000;
                    timer2.Start();
                    timer1.Stop();
                }
            }
            catch { }
        }

        //Timer 2 reads all the information sendt from the open port. The information is displayed in textBox 2, 3 and 4.
        //When the second bit changes to a 1 the timer takes a picture. When the first bit changes to a 0 the loop closes.

        private void timer2_Tick_1(object sender, EventArgs e)
        {
            bool haveData = false;
            string Data = string.Empty;

            while (!haveData)
            {
                Data = serialPort.ReadExisting();
                if (!string.IsNullOrEmpty(Data))
                {
                    haveData = true;
                }
            }

            string[] dataParts = Data.Split(',');
            try
            {
                textBox2.Text = dataParts[2] + " C";
                textBox3.Text = dataParts[3] + "%";
                textBox4.Text = dataParts[4] + " Lux";




                if (Convert.ToInt32(dataParts[1]) == 1)
                {
                    if (bilde == 0)
                    {
                        //ta bilde
                        if (!icImagingControl1.LiveVideoRunning)
                        {
                            MessageBox.Show("Camera is not currently running.");
                        }
                        else
                        {
                            if (!Directory.Exists(File.ReadAllText(appdataGet())))
                            {
                                MessageBox.Show("Directory does not match any existing directory.");
                            }
                            else
                            {
                                DateTime time = DateTime.Now;
                                string format_file = "HH.mm";
                                string name_file = time.ToString(format_file);

                                System.IO.Directory.CreateDirectory(dir_name_running);

                                TIS.Imaging.FrameSnapSink snapSink = icImagingControl1.Sink as TIS.Imaging.FrameSnapSink;

                                TIS.Imaging.IFrameQueueBuffer frm = snapSink.SnapSingle(TimeSpan.FromSeconds(5));

                                frm.SaveAsJpeg(dir_name_running + @"\" + name_file + ".jpg", 1310720);

                                bilde = 1;
                            }
                        }
                    }
                }
                else
                {
                    bilde = 0;
                }
                if (Convert.ToInt32(dataParts[3]) >= 70)
                {
                    MessageBox.Show("Error", "Humidity is to high.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    timer2.Stop();
                    button3.Enabled = true;
                    button1.Enabled = true;
                }
            }
            catch { }
            if (dataParts[0] == "0")
            {
                timer2.Stop();
                MessageBox.Show("Done", "Done", MessageBoxButtons.OK);
                button3.Enabled = true;
                button1.Enabled = true;
            }

            dataParts = Array.Empty<string>();
        }
    }
}
