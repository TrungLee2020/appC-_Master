using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MainWindow.Utils;

namespace MainWindow.Connection
{
    public class ComPort
    {
        private SerialPort serialPort;
        private ComData lastData;

        public event EventHandler Received;
        public ComPort(SerialPort serial)
        {
            this.serialPort = serial;
            serialPort.WriteTimeout = 500;
            serialPort.ReadTimeout = 200;
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            ComData comData = new ComData();
            comData.message = sp.ReadExisting();
            comData.dataReceived = DateTime.Now;
            if (lastData == null || comData.dataReceived.Subtract(lastData.dataReceived).Seconds > Constants._OPEN_DURATION)
            {
                lastData = comData;
                if (Received != null)
                {
                    Received(comData, e);
                }
            }

        }
        // control system use command AT
        public static SerialPort ScanPort()
        {
            string[] ports = SerialPort.GetPortNames();

            ASCIIEncoding encoder = new ASCIIEncoding();
            foreach(string port in ports)
            {
                SerialPort serialPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
                if (!(serialPort.IsOpen))
                {
                    serialPort.Open();
                }
                byte[] arrayResponse = SendCommandAndGetResponse("connect\n", serialPort, "connected\n".Length);
                String messageResponse = encoder.GetString(arrayResponse, 0, arrayResponse.Length);
                if ("connected\n".Equals(messageResponse))
                    return serialPort;
                serialPort.Close();
            }
            return null;
        }

        public static byte[] SendCommandAndGetResponse(String command, SerialPort serialPort, int lenResponse)
        {
            try
            {
                serialPort.WriteTimeout = 500;
                serialPort.ReadTimeout = 500;
                serialPort.Write(command);
                int count = lenResponse;
                var buffer = new byte[lenResponse];
                var offset = 0;
                while(count > 0)
                {
                    var readCount = serialPort.Read(buffer, offset, count);
                    offset += readCount;
                    count -= readCount;
                }
                return buffer;
            }
            catch
            {
                return new byte[] { 0x00 };
            }
        }
        public static byte[] SendCommand(byte[] command, SerialPort serialPort, int lenResponse)
        {
            var buffer = new byte[lenResponse];
            var offset = 0;
            int count = lenResponse;
            serialPort.WriteTimeout = 500;
            serialPort.ReadTimeout = 500;
            serialPort.Write(command, 0, command.Length);
            while(count > 0)
            {
                var readCount = serialPort.Read(buffer, offset, count);
                offset += readCount;
                count -= readCount;
            }

            return buffer;
        }
        public void OpenS()
        {
            byte[] cmdOpen = new byte[] { 0x01, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x8C, 0x3A, 0x0A };
            byte[] cmdReponse = new byte[] { 0x01, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x8C, 0x3A, 0x0A };
            serialPort.Write(cmdOpen, 0, cmdOpen.Length);
        }
        
        public void CloseS(SerialPort serialPort)
        {
            byte[] cmdClose = new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA, 0x0A };
            byte[] cmdReponse = new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00, 0xDD, 0xFA, 0x0A };
            serialPort.Write(cmdClose, 0, cmdClose.Length);
        }
    }
    public class ComData
    {
        public String message { set; get; }
        public DateTime dataReceived { set; get; }
    }
}
