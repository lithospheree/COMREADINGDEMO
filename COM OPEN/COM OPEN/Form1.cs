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
        private Timer timer;
        private DataLogger dataLogger;
        
        public Form1()
        {
            InitializeComponent();

            dataLogger = new DataLogger();

            formsPlot2.Plot.Add.Plottable(dataLogger);
            formsPlot2.Plot.Title("Serial Graph");

            formsPlot2.Refresh();

            populatePorts();
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
                SerialPort _serialPort = new SerialPort();
                _serialPort.PortName = port;
                _serialPort.BaudRate = 115200;
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;

                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                _serialPort.Open();
                _serialPort.WriteLine("Hello World!");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
                
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();
            Console.WriteLine(data);
            dataLogger.Add(1, 4);
            dataLogger.Add(2, 6);
            dataLogger.Add(11, 11);
            if (formsPlot2.InvokeRequired)
            {
                formsPlot2.Invoke(new Action(() => formsPlot2.Plot.Axes.AutoScale()));
                formsPlot2.Invoke(new Action(() => formsPlot2.Refresh()));
            }
            else
            {
                formsPlot2.Refresh();
            }
        }

        private void formsPlot2_Load(object sender, EventArgs e)
        {
        }
    }
}
