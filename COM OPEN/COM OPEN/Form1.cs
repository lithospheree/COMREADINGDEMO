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
using System.Windows.Forms.DataVisualization.Charting;
using System.Management;
using System.Windows.Forms.VisualStyles;
using ScottPlot.WinForms;
using ScottPlot;
using ScottPlot.Plottables;


namespace COM_OPEN
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();

            populatePorts();

            loggerTest();
        }

        private void populatePorts()
        {

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                string[] portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                foreach(string s in portList)
                {
                    comboBox1.Items.Add(s);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string portName = comboBox1.SelectedItem.ToString();

            string[] portnames = SerialPort.GetPortNames();

            string port = "";

            foreach (string s in portnames) 
            {
                if (portName.Contains(s))
                {
                    port = s; break;
                }
            }

            try
            {
                SerialPort serialPort = new SerialPort(port, 921600, Parity.None, 8, StopBits.One);
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                serialPort.Open();
                
                serialPort.Write("Hello World!");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
                
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();
            string[] dataSplit = data.Select(s => s.ToString()).ToArray();

        }

        private void updateLogger(int id, int value)
        {

        }

        private void formsPlot2_Load(object sender, EventArgs e)
        {
        }

        private void loggerTest()
        {
            var logger = formsPlot2.Plot.Add.DataLogger();
            for (int i = 0; i < 100000; i++)
            {
                double y = Generate.RandomWalker.Next();

                logger.Add(i, y);
            }
            formsPlot2.Refresh();
        }
    }
}
