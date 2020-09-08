using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WowBot.BotStuff;

namespace WowBot.Visuals
{
	public partial class MainForm : Form
	{
		Timer timer;
		Main main = new Main();

		public MainForm()
		{
			InitializeComponent();
			SetupTimer();

			main.BackgroundThreadUpdated += OnBackgroundThreadUpdated;
		}

		private void SetupTimer()
		{
			timer = new Timer();
			timer.Interval = 100;
			timer.Tick += Timer_Tick;
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			main.UpdateStuff();
		}

		void OnBackgroundThreadUpdated(object sender, EventArgs e) 
		{
			Vector3 position = PlayerInfo.Instance.Position;
			Position.Text = $"Current position: x: {position.x}, y: {position.y}, z: {position.z}";
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Application.Exit();
		}

		private void AddRoadBtn_Click(object sender, EventArgs e)
		{

		}

		private void MainForm_Load(object sender, EventArgs e)
		{
		}
	}
}
