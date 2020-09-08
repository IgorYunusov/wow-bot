using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace MiningBot {
    class UI : Form{

        private Bot bot;

        private Label mountNumLab;
        private TextBox mountNumTxt;
        private Button startMiningBtn;

        public UI(Bot bot) {
            this.bot = bot;
            
            //kinézet
            Text = "Mining bot";
            Width  = 220;
            Height = 220;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            
            //elemek
            mountNumLab = new Label();
            mountNumLab.Text = "Flying mount number:";
            mountNumLab.SetBounds(20, 21, 140, 40);
            mountNumLab.Font = new Font(mountNumLab.Font.FontFamily.Name, 10);
            Controls.Add(mountNumLab);

            mountNumTxt = new TextBox();
            mountNumTxt.Text = "1";
            mountNumTxt.SetBounds(160, 20, 20, 40);
            Controls.Add(mountNumTxt);

            startMiningBtn = new Button();
            startMiningBtn.Text = "Start mining";
            startMiningBtn.SetBounds(20, 60, 160, 80);
            startMiningBtn.Click += new EventHandler(OnStartMiningBtnClick);
            Controls.Add(startMiningBtn);

            ShowDialog();
        }

        void OnStartMiningBtnClick(object sender, EventArgs e) {
            if (bot.isMiningTrue()) {
                startMiningBtn.Text = "Start Mining";
                bot.stopMining();
            }
            else {
                startMiningBtn.Text = "Stop mining";
                Thread miningThread = new Thread(bot.startMining);
                miningThread.Start(mountNumTxt.Text);
            }
        }
    }
}
