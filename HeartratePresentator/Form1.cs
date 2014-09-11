using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;
using System.Collections;

namespace HeartratePresentator
{
    public partial class Form1 : Form
    {

        int time = 0;
        string filepath = "";
        int currentHeartrate = 0;

        ArrayList currentBlockingObjects;

        public Form1()
        {
            InitializeComponent();
        }

        private void closeBT_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void generateBT_Click(object sender, EventArgs e)
        {
            int min = 55;
            int max = 180;
            int numberOfDataPoints = 20 * 60 / 5; //every 5 sec, 20 min presentation

            String folder = "heartrate";

            if (!Directory.Exists(folder))
            {
                Console.WriteLine("The directory does not exist.");
                DirectoryInfo di = Directory.CreateDirectory(folder);
            }

            String filename = System.DateTime.Now.ToString("yyyy-MM-dd_hhmmss");
            Random rand = new Random();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"heartrate\" + filename + ".txt", true))
            {
                for (int i = 0; i < numberOfDataPoints; i++)
                {
                    int ran = rand.Next(min, max + 1);
                    file.WriteLine(ran);
                }
                MessageBox.Show("File has been generated."); 
            }
            updateFileSelectionBox();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String folder = "heartrate";
            this.currentBlockingObjects = new ArrayList();

            if (!Directory.Exists(folder))
            {
                Console.WriteLine("The directory does not exist.");
                return;
            }

            updateFileSelectionBox();
        }

        private void updateFileSelectionBox()
        {
            string[] filePaths = Directory.GetFiles(@"heartrate\");
            this.filesCB.Items.Clear();
            this.filesCB.Items.AddRange(filePaths);
            if (filePaths.Count() > 0)
            {
                this.filesCB.SelectedIndex = 0;
            }
            this.filesCB.Refresh();
        }

        private void startBT_Click(object sender, EventArgs e)
        {
            //start timer
            this.timer1.Start();

            //get filepath from combobox
            filepath = this.filesCB.GetItemText(this.filesCB.SelectedItem);
            
            //start background worker
            this.backgroundWorker1.RunWorkerAsync();
            this.backgroundWorker2.RunWorkerAsync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeLB.Text = this.time.ToString() + " sec";
            this.time++;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.timer1.Stop();
            this.backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //start to read file
            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    //every other second, read a new line and display the heartrate on the window
                    String line = sr.ReadLine();
                    while (line != null)
                    {
                        this.currentHeartrate = Convert.ToInt32(line);
                        this.heartrateLB.Invoke((MethodInvoker)delegate
                        {
                            this.heartrateLB.Text = line;
                        });
                        Console.WriteLine(line);
                        Thread.Sleep(1500);
                        line = sr.ReadLine();
                        if (backgroundWorker1.CancellationPending)
                        {
                            line = null;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(exc.Message);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            int minRadius = 200;
            int maxRadius = 500;
            
            Screen[] screens = Screen.AllScreens;
            Rectangle bnds = screens[1].Bounds;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            int minX = bnds.X;
            int maxX = bnds.Width;
            int minY = bnds.Y;
            int maxY = bnds.Height;
            Random rand = new Random();
            //pop up black circles to block the screen
            while (!backgroundWorker2.CancellationPending)
            {
                Thread.Sleep(1500);
                //Add or change circles
                if (this.currentHeartrate < 90)
                {
                    int x = rand.Next(minX, maxX + 1);
                    int y = rand.Next(minY, maxY + 1);
                    int r = rand.Next(minRadius, maxRadius + 1);
                    Circle circle = new Circle(x, y, r);
                    this.currentBlockingObjects.Add(circle);
                }

                

                //Refresh display of objects on the screen
                for (int i = 0; i < this.currentBlockingObjects.Count; i++)
                {
                    Circle c = (Circle) this.currentBlockingObjects[i];
                    if (!c.onScreen)
                    {
                        Form f = new Form();
                        f.BackColor = Color.Black;
                        f.FormBorderStyle = FormBorderStyle.None;
                        f.StartPosition = FormStartPosition.Manual;
                        f.ShowInTaskbar = false;
                        f.Width = c.r;
                        f.Height = c.r;
                        f.Location = new Point(c.x, c.y);
                        f.TopMost = true;

                        Console.WriteLine("Form: {0}, {1}, {2}", c.x, c.y, c.r);

                        c.onScreen = true;

                        f.Show();
                    }
                }
            }
        }

        #region test button
        private void button2_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            f.BackColor = Color.Black;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Width = 100;
            f.Height = 100;
            f.Location = new Point(50, 50);
            f.StartPosition = FormStartPosition.Manual;
            f.Show();

            Form f2 = new Form();
            f2.BackColor = Color.White;
            f2.FormBorderStyle = FormBorderStyle.None;
            f2.Width = 100;
            f2.Height = 100;
            f2.Location = new Point(500, 500);
            f2.StartPosition = FormStartPosition.Manual;
            f2.Show();

            Form f3 = new Form();
            f3.BackColor = Color.Red;
            f3.FormBorderStyle = FormBorderStyle.None;
            f3.Width = 100;
            f3.Height = 100;
            f3.Location = new Point(800, 800);
            f3.StartPosition = FormStartPosition.Manual;
            f3.Show();
        }
        #endregion
    }
}
