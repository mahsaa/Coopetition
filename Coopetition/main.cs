using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Coopetition
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        private void main_Load(object sender, EventArgs e)
        {

        }

        private void btnStrategy_Click(object sender, EventArgs e)
        {
            Environment.outputLog = outputLog;
            switch (cboStrategies.SelectedItem.ToString())
            {
                case "All Competitive":
                    new Environment().Run(Constants.SimulationType.AllCompetitive);
                    break;
                case "All random":
                    new Environment().Run(Constants.SimulationType.AllRandom);
                    break;
                case "Coopetitive":
                    new Environment().Run(Constants.SimulationType.Coopetitive);
                    break;
            }
        }

    }
}
