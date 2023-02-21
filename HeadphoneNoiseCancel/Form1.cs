﻿using System;
using System.Media;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Windows.Threading;

namespace HeadphoneNoiseCancel
{
    public partial class Form1 : Form
    {
        SoundPlayer myPlayer = new SoundPlayer(Properties.Resources._10);
        
        DispatcherTimer dtPlaySound = new DispatcherTimer();
        DispatcherTimer dtTimer = new DispatcherTimer();
        int beepIntervall = 10;
        TimeSpan ts;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void resetUI(bool startButton, bool stopButton, int counterTime)
        {
            buttonStart.Enabled = startButton;
            buttonStop.Enabled = stopButton;
            startToolStripMenuItem.Enabled = startButton;
            stopToolStripMenuItem.Enabled = stopButton;

            ts = new TimeSpan(0, 0, counterTime);
            labelTime.Text = ts.ToString();

            if (stopButton) notifyIcon.Text = "Runing";
            else notifyIcon.Text = "Stoped";

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            dtPlaySound.Interval = TimeSpan.FromSeconds(beepIntervall);
            dtPlaySound.Tick += playSound;

            dtTimer.Interval = TimeSpan.FromSeconds(1);
            dtTimer.Tick += DtTimer_Tick;

            resetUI(true, false, beepIntervall);
        }

        private void DtTimer_Tick(object sender, EventArgs e)
        {
            ts = ts.Subtract(TimeSpan.FromSeconds(1));
            labelTime.Text = ts.ToString();
        }


        private void playSound(object sender, EventArgs e)
        {           
            myPlayer.PlaySync();

            resetUI(false, true, beepIntervall);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            dtPlaySound.Start();
            dtTimer.Start();

            resetUI(false, true, beepIntervall);
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            dtPlaySound.Stop();
            dtTimer.Stop();

            resetUI(true, false, beepIntervall);
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

            resetUI(false, true, beepIntervall);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtPlaySound.Stop();
            dtTimer.Stop();

            resetUI(true, false, beepIntervall);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}