using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Windows.Threading;

namespace HeadphoneNoiseCancel
{
    public partial class Form1 : Form
    {
        // Konfigurációs fájl elérési útja
        static string configFilePath = "config.txt";
        // ConfigManager példány létrehozása
        static ConfigManager configManager = new ConfigManager(configFilePath);
        
        //Config fileból értékek kiolvalsása
        int intervall = Convert.ToInt32(configManager.GetValue("intervall"));
        int frequency = Convert.ToInt32(configManager.GetValue("frequency"));
        int duration = Convert.ToInt32(configManager.GetValue("duration"));

        //Wav generátor létrehozása
        SoundGenerator soundGenerator = new SoundGenerator();
        

        // Relatív elérési útvonal megadása        
        static string relativePath = "audio/sound.wav";        
        // Teljes elérési útvonal konvertálása
        static string fullPath = Path.GetFullPath(relativePath);

        SoundPlayer myPlayer = new SoundPlayer(fullPath);        
        DispatcherTimer dtPlaySound = new DispatcherTimer();
        DispatcherTimer dtTimer = new DispatcherTimer();
        

        MMDevice device;
        int masterSoundLevel = 0;

        TimeSpan ts;

        public Form1()
        {
            InitializeComponent();

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                        
        }

        private void resetUI(bool startButton, bool stopButton, int counterTime)
        {
            buttonStart.Enabled = startButton;
            buttonStop.Enabled = stopButton;
            startToolStripMenuItem.Enabled = startButton;
            stopToolStripMenuItem.Enabled = stopButton;

            ts = new TimeSpan(0, 0, counterTime);
            labelTime.Text = ts.ToString();

            if (stopButton) { 
                notifyIcon.Text = "Runing";
                groupSettings.Enabled = false;
            }
            else {
                notifyIcon.Text = "Stoped";
                groupSettings.Enabled = Enabled;
            } 
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //UI elemek értékadása
            numericUpDownIntervall.Value = intervall;
            numericUpDownFrequency.Value = frequency;
            numericUpDownDuration.Value = duration;

            dtPlaySound.Interval = TimeSpan.FromSeconds(intervall);
            dtPlaySound.Tick += playSound;

            dtTimer.Interval = TimeSpan.FromSeconds(1);
            dtTimer.Tick += DtTimer_Tick;

            resetUI(true, false, intervall);
        }

        private void DtTimer_Tick(object sender, EventArgs e)
        {
            ts = ts.Subtract(TimeSpan.FromSeconds(1));
            labelTime.Text = ts.ToString();
        }


        private void playSound(object sender, EventArgs e)
        {
            
            Debug.WriteLine($"master sound level: {masterSoundLevel}");

            if (masterSoundLevel == 0) {
                
                myPlayer.Play();
                
            }
            resetUI(false, true, intervall);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //Ellenőrzi, hogy létezik-e a lejátszandi file, és ha nem akkor létrehozza
            if(!soundGenerator.FileExist()) soundGenerator.GenerateAndPlaySound(frequency, duration);

            dtPlaySound.Start();
            dtTimer.Start();

            resetUI(false, true, intervall);
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            dtPlaySound.Stop();
            dtTimer.Stop();

            resetUI(true, false, intervall);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                this.ShowInTaskbar = false;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtPlaySound.Start();
            dtTimer.Start();

            resetUI(false, true, intervall);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtPlaySound.Stop();
            dtTimer.Stop();

            resetUI(true, false, intervall);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (device != null) {                
               
                masterSoundLevel = (int)Math.Round(device.AudioMeterInformation.MasterPeakValue * 100);                
                progressBar.Value = masterSoundLevel;

            }
        }

        private void buttonAcceptSetting_Click(object sender, EventArgs e)
        {
            //Értékek kiolvasása numericUpDown-ból            
            intervall = Convert.ToInt32(numericUpDownIntervall.Value);
            frequency = Convert.ToInt32(numericUpDownFrequency.Value);
            duration = Convert.ToInt32(numericUpDownDuration.Value);

            // Érték mentése config file-ba
            configManager.SetValue("intervall", intervall);
            configManager.SetValue("frequency", frequency);
            configManager.SetValue("duration", duration);

            //érték ujraállítása
            dtPlaySound.Interval = TimeSpan.FromSeconds(intervall);

            soundGenerator.GenerateAndPlaySound(frequency, duration);
            
            resetUI(true, false, intervall);
        }

        
    }
}
