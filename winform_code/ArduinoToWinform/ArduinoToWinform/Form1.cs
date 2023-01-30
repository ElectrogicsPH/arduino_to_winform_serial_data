using System;
using System.IO.Ports;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ArduinoToWinform
{
    public partial class Form1 : Form
    {
        public int CounterValue = 0;
        SerialPort arduino;
        private const string LastUsedPortFile = "last_used_port.txt";

        public Form1()
        {
            InitializeComponent();

            // Initialize button name and Click event
            connectButton.Text = "Connect";
            connectButton.Click += new EventHandler(ConnectButtonClick);
            refreshButton.Text = "Refresh";
            refreshButton.Click += new EventHandler(RefreshButtonClick);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);


            // Populate the ComboBox with available serial ports
            PopulateSerialPortsComboBox();

            // try to auto connect
            ConnectSerialPort();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // close the serial port before closing the form
            if (connectButton.Text == "Disconnect")
            {
                try
                {
                    if (arduino.IsOpen)
                    {
                        arduino.Close();
                    }
                }
                catch (Exception ex)
                {
                    // do nothing
                }
            }
        }
        private void ConnectSerialPort()
        {
            if (serialPortsComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a serial port from the Serial port connection.");
                return;
            }

            if (connectButton.Text == "Connect")
            {
                try
                {
                    arduino = new SerialPort(serialPortsComboBox.SelectedItem.ToString(), 9600);
                    arduino.Open();
                    arduino.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    connectButton.Text = "Disconnect";
                    File.WriteAllText(LastUsedPortFile, serialPortsComboBox.SelectedItem.ToString());

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Serial Port is in used or cannot found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    PopulateSerialPortsComboBox();
                }
            }
            else
            {
                try
                {
                    arduino.Close();
                    connectButton.Text = "Connect";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot properly close Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }
        private void ConnectButtonClick(object sender, EventArgs e)
        {
            ConnectSerialPort();
        }

        private void RefreshButtonClick(object sender, EventArgs e)
        {
            PopulateSerialPortsComboBox();
        }

        private void PopulateSerialPortsComboBox()
        {
            serialPortsComboBox.Items.Clear();
            serialPortsComboBox.Text = "";
            string[] availableSerialPorts = SerialPort.GetPortNames();
            serialPortsComboBox.Items.AddRange(availableSerialPorts);

            if (serialPortsComboBox.Items.Count > 0)
            {
                serialPortsComboBox.SelectedIndex = 0;
            }
            if (File.Exists(LastUsedPortFile))
            {
                string lastUsedPort = File.ReadAllText(LastUsedPortFile);
                if (SerialPort.GetPortNames().Contains(lastUsedPort))
                {
                    serialPortsComboBox.SelectedItem = lastUsedPort;
                }
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
                        int value = 0;
                        if (int.TryParse(keyValue[1], out value))
                        {
                            // increment counter by value received
                            CounterValue += value;

                            // Use a delegate to update the textbox from a non-UI thread
                            this.Invoke((MethodInvoker)delegate
                            {
                                textBox1.Text = CounterValue.ToString();
                            });
                        }
                    }
                }
            }
        }
        private void ResetCounterValue()
        {
            CounterValue = 0;
            textBox1.Text = CounterValue.ToString();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {

            // Reset the counter
            ResetCounterValue();

            // do something else...
        }
    }
}