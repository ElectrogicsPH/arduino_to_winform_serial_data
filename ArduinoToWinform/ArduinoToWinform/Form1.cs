using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ArduinoToWinform
{
    public partial class Form1 : Form
    {
        SerialPort arduino;

        public Form1()
        {
            InitializeComponent();

            // Initialize button name and Click event
            connectButton.Text = "Connect";
            connectButton.Click += new EventHandler(ConnectButtonClick);
            refreshButton.Text = "Refresh";
            refreshButton.Click += new EventHandler(RefreshButtonClick);

            // Populate the ComboBox with available serial ports
            PopulateSerialPortsComboBox();
        }

        private void ConnectButtonClick(object sender, EventArgs e)
        {
            if (connectButton.Text == "Connect")
            {
                arduino = new SerialPort(serialPortsComboBox.SelectedItem.ToString(), 9600);
                arduino.Open();
                arduino.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                connectButton.Text = "Disconnect";
            }
            else
            {
                arduino.Close();
                connectButton.Text = "Connect";
            }
        }

        private void RefreshButtonClick(object sender, EventArgs e)
        {
            PopulateSerialPortsComboBox();
        }

        private void PopulateSerialPortsComboBox()
        {
            serialPortsComboBox.Items.Clear();

            string[] availableSerialPorts = SerialPort.GetPortNames();
            serialPortsComboBox.Items.AddRange(availableSerialPorts);
            if (serialPortsComboBox.Items.Count > 0)
            {
                serialPortsComboBox.SelectedIndex = 0;
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            // Split the received data by newline characters
            string[] lines = indata.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // Iterate through each line
            foreach (string line in lines)
            {
                // Try to parse the line as a key-value pair
                string[] keyValue = line.Split('=');
                if (keyValue.Length == 2)
                {
                    // If the line is a key-value pair, update the corresponding textbox
                    if (keyValue[0] == "count")
                    {
                        // Use a delegate to update the textbox from a non-UI thread
                        this.Invoke((MethodInvoker)delegate
                        {
                            textBox1.Text = keyValue[1];
                        });
                    }
                }
            }
        }
    }
}