using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DeviceSource;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections.ObjectModel;
using MainWindow.Connection;
using log4net;
using MainWindow.Utils;

namespace MainWindow.Basic
{
    public partial class Basic_Form : Form
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        CameraOperator m_pOperator;
        bool m_bGrabbing;
        private ComPort comPort;
        private static ILog Log = LogManager.GetLogger(typeof(MainWindow));


        UInt32 m_nBufSizeForDriver = 3072 * 2048 * 3;
        byte[] m_pBufForDriver = new byte[3072 * 2048 * 3];            // Buffer for getting image from driver

        UInt32 m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
        byte[] m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];         // Buffer for saving image
        public Basic_Form()
        {
            InitializeComponent();
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_pOperator = new CameraOperator();
            m_bGrabbing = false;
            DeviceListAcq();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void bnEnum_Click(object sender, EventArgs e)
        {
            DeviceListAcq();
        }

        private void DeviceListAcq()
        {
            int nRet;
            // Create Device List
            System.GC.Collect();
            cbDeviceList.Items.Clear();
            nRet = CameraOperator.EnumDevices(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            if ( 0 != nRet)
            {
                MessageBox.Show("Enumurate devices fail!");
                return;
            }

            // Display device name in the form list
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if  (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo.chUserDefinedName != "")
                    {
                        cbDeviceList.Items.Add("GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chUserDefinedName != "")
                    {
                        cbDeviceList.Items.Add("USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                    }
                    else
                    {
                        cbDeviceList.Items.Add("USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                    }
                }
            }

            // Select the first item
            if (m_pDeviceList.nDeviceNum != 0)
            {
                cbDeviceList.SelectedIndex = 0;
            }
        }

        private void SetCtrlWhenOpen()
        {
            bnOpen.Enabled = false;

            bnClose.Enabled = true;
            bnStartGrab.Enabled = true;
            bnStopGrab.Enabled = false;
            bnContinuesMode.Enabled = true;
            bnContinuesMode.Checked = true;
            bnTriggerMode.Enabled = true;
            cbSoftTrigger.Enabled = false;
            bnTriggerExec.Enabled = false;

            tbExposure.Enabled = true;
            tbGain.Enabled = true;
            tbFrameRate.Enabled = true;
            bnGetParam.Enabled = true;
            bnSetParam.Enabled = true;

        }

        private void bnOpen_Click(object sender, EventArgs e)
        {
            if (m_pDeviceList.nDeviceNum == 0 || cbDeviceList.SelectedIndex == -1)
            {
                //MessageBox.Show("No device, please select");
                ShowErrorMsg("No device, please select", 0);
                return;
            }
            int nRet = -1;

            // Get select device information
            MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[cbDeviceList.SelectedIndex],
                                                              typeof(MyCamera.MV_CC_DEVICE_INFO));

            // Open device
            nRet = m_pOperator.Open(ref device);
            if (nRet != MyCamera.MV_OK)
            {
                //MessageBox.Show("Device open fail!");
                ShowErrorMsg("Device open fail!", nRet);
                return;
            }

            // Set Continues Aquisition Mode
            m_pOperator.SetEnumValue("AcquisitionMode", 2);
            m_pOperator.SetEnumValue("TriggerMode", 0);

            bnGetParam_Click(null, null); //Get parameters

            //Control operation
            SetCtrlWhenOpen();
        }

        private void SetCtrlWhenClose()
        {
            bnOpen.Enabled = true;

            bnClose.Enabled = false;
            bnStartGrab.Enabled = false;
            bnStopGrab.Enabled = false;
            bnContinuesMode.Enabled = false;
            bnTriggerMode.Enabled = false;
            cbSoftTrigger.Enabled = false;
            bnTriggerExec.Enabled = false;

            bnSaveBmp.Enabled = false;
            bnSaveJpg.Enabled = false;
            tbExposure.Enabled = false;
            tbGain.Enabled = false;
            tbFrameRate.Enabled = false;
            bnGetParam.Enabled = false;
            bnSetParam.Enabled = false;

        }
        private void bnClose_Click(object sender, EventArgs e)
        {
            //Close Device
            m_pOperator.Close();

            //Control Operation
            SetCtrlWhenClose();

            //Reset flow flag bit
            m_bGrabbing = false;
        }

        private void bnContinuesMode_CheckedChanged(object sender, EventArgs e)
        {
            if (bnContinuesMode.Checked)
            {
                m_pOperator.SetEnumValue("TriggerMode", 0);
                cbSoftTrigger.Enabled = false;
                bnTriggerExec.Enabled = false;
            }

        }

        private void bnTriggerMode_CheckedChanged(object sender, EventArgs e)
        {
            //Open Trigger Mode
            if (bnTriggerMode.Checked)
            {
                m_pOperator.SetEnumValue("TriggerMode", 1);

                //Trigger source select:0 - Line0;
                //                      1 - Line1;
                //                      2 - Line2;
                //                      3 - Line3;
                //                      4 - Counter;
                //                      7 - Software;
                if (cbSoftTrigger.Checked)
                {
                    m_pOperator.SetEnumValue("TriggerSource", 7);
                    if (m_bGrabbing)
                    {
                        bnTriggerExec.Enabled = true;
                    }
                }
                else
                {
                    m_pOperator.SetEnumValue("TriggerSource", 0);
                }
                cbSoftTrigger.Enabled = true;
            }

        }

        private void SetCtrlWhenStartGrab()
        {

            bnStartGrab.Enabled = false;
            bnStopGrab.Enabled = true;

            if (bnTriggerMode.Checked && cbSoftTrigger.Checked)
            {
                bnTriggerExec.Enabled = true;
            }

            bnSaveBmp.Enabled = true;
            bnSaveJpg.Enabled = true;
        }

        // Get image data and information

        private void bnStartGrab_Click(object sender, EventArgs e)
        {
            int nRet;

            //Start Grabbing
            nRet = m_pOperator.StartGrabbing();
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Start Grabbing Fail!");
                return;
            }
            // Get iamge data and information
            //nRet = m_pOperator.GetImageBuffer( ref Frame, 1000);

            //Control Operation
            SetCtrlWhenStartGrab();

            //Set position bit true
            m_bGrabbing = true;


            //Display
            nRet = m_pOperator.Display(pictureBox2.Handle);

            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Display Fail!");
            }
        }

        private void cbSoftTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSoftTrigger.Checked)
            {

                //Set trigger source as Software
                m_pOperator.SetEnumValue("TriggerSource", 7);
                if (m_bGrabbing)
                {
                    bnTriggerExec.Enabled = true;
                }
            }
            else
            {
                m_pOperator.SetEnumValue("TriggerSource", 0);
                bnTriggerExec.Enabled = false;
            }
        }

        private void bnTriggerExec_Click(object sender, EventArgs e)        // Trigger once
        {
            //int nRet;

            //Trigger command
            //nRet = m_pOperator.CommandExecute("TriggerSoftware");
            if (CameraOperator.CO_OK != m_pOperator.CommandExecute("TriggerSoftware"))
            {
                MessageBox.Show("Trigger Software Fail!");
            }
            else if (CameraOperator.CO_OK != m_pOperator.CommandExecute("TriggerDelay"))
            {
                MessageBox.Show("Trigger Delay Fail!");
            }

        }

        // User trigger delay before hardware trigger
        private void bnHardTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHardTrigger.Checked)
            {
                float fvalue = 5;
                // Set trigger delay
                m_pOperator.SetFloatValue("TriggerDelay", fvalue); // trigger delay
                if (m_bGrabbing)
                {
                    bnTriggerExec.Enabled = true;
                }
            }
            else
            {
                m_pOperator.SetEnumValue("TriggerSource", 0);
                bnTriggerExec.Enabled = false;
            }
        }

        private void SetCtrlWhenStopGrab()
        {
            bnStartGrab.Enabled = true;
            bnStopGrab.Enabled = false;

            bnTriggerExec.Enabled = false;


            bnSaveBmp.Enabled = false;
            bnSaveJpg.Enabled = false;
        }
        private void bnStopGrab_Click(object sender, EventArgs e)
        {
            int nRet = -1;
            //Stop Grabbing
            nRet = m_pOperator.StopGrabbing();
            if (nRet != CameraOperator.CO_OK)
            {
                MessageBox.Show("Stop Grabbing Fail!");
            }

            //Set flag bit false
            m_bGrabbing = false;

            //Control Operation
            SetCtrlWhenStopGrab();

        }

        private void bnSaveBmp_Click(object sender, EventArgs e)
        {
            int nRet;
            UInt32 nPayloadSize = 0;
            nRet = m_pOperator.GetIntValue("PayloadSize", ref nPayloadSize);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Get PayloadSize failed");
                return;
            }
            if (nPayloadSize + 2048 > m_nBufSizeForDriver)
            {
                m_nBufSizeForDriver = nPayloadSize + 2048;
                m_pBufForDriver = new byte[m_nBufSizeForDriver];

                // Determine the buffer size to save image
                // BMP image size: width * height * 3 + 2048 (Reserved for BMP header)
                m_nBufSizeForSaveImage = m_nBufSizeForDriver * 3 + 2048;
                m_pBufForSaveImage = new byte[m_nBufSizeForSaveImage];
            }

            IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForDriver, 0);
            UInt32 nDataLen = 0;
            MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();

            //Get one frame timeout, timeout is 1 sec
            nRet = m_pOperator.GetOneFrameTimeout(pData, ref nDataLen, m_nBufSizeForDriver, ref stFrameInfo, 1000);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("No Data!");
                return;
            }

            /************************Mono8 to Bitmap*******************************
            Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 1, PixelFormat.Format8bppIndexed, pData);

            ColorPalette cp = bmp.Palette;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            // set palette back
            bmp.Palette = cp;

            bmp.Save("D:\\test.bmp", ImageFormat.Bmp);

            *********************RGB8 to Bitmap**************************
            for (int i = 0; i < stFrameInfo.nHeight; i++ )
            {
                for (int j = 0; j < stFrameInfo.nWidth; j++)
                {
                    byte chRed = m_buffer[i * stFrameInfo.nWidth * 3 + j * 3];
                    m_buffer[i * stFrameInfo.nWidth * 3 + j * 3] = m_buffer[i * stFrameInfo.nWidth * 3 + j * 3 + 2];
                    m_buffer[i * stFrameInfo.nWidth * 3 + j * 3 + 2] = chRed;
                }
            }
            Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pData);
            bmp.Save("D:\\test.bmp", ImageFormat.Bmp);

            ************************************************************************/

            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
            stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
            stSaveParam.enPixelType = stFrameInfo.enPixelType;
            stSaveParam.pData = pData;
            stSaveParam.nDataLen = stFrameInfo.nFrameLen;
            stSaveParam.nHeight = stFrameInfo.nHeight;
            stSaveParam.nWidth = stFrameInfo.nWidth;
            stSaveParam.pImageBuffer = pImage;
            stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
            stSaveParam.nJpgQuality = 80;
            nRet = m_pOperator.SaveImage(ref stSaveParam);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Save Fail!");
                return;
            }
            string save_image_path = @"C:\Users\ttxid\source\repos\MainWindown\MainWindown\Data_image\";
            string filename = save_image_path + $"Image_{DateTime.Now.ToString("MMdd_hhmm")}.bmp";

            FileStream file = new FileStream(filename, FileMode.Create , FileAccess.Write);
            file.Write(m_pBufForSaveImage, 0, (int)stSaveParam.nImageLen);
            file.Close();

            MessageBox.Show("Save Succeed!");
        }

        private void bnSaveJpg_Click(object sender, EventArgs e)
        {
            int nRet;
            UInt32 nPayloadSize = 0;
            nRet = m_pOperator.GetIntValue("PayloadSize", ref nPayloadSize);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Get PayloadSize failed");
                return;
            }
            if (nPayloadSize + 2048 > m_nBufSizeForDriver)
            {
                m_nBufSizeForDriver = nPayloadSize + 2048;
                m_pBufForDriver = new byte[m_nBufSizeForDriver];

                // Determine the buffer size to save image
                // BMP image size: width * height * 3 + 2048 (Reserved for BMP header)
                m_nBufSizeForSaveImage = m_nBufSizeForDriver * 3 + 2048;
                m_pBufForSaveImage = new byte[m_nBufSizeForSaveImage];
            }

            IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForDriver, 0);
            UInt32 nDataLen = 0;
            MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();

            //Get one frame timeout, timeout is 1 sec
            nRet = m_pOperator.GetOneFrameTimeout(pData, ref nDataLen, m_nBufSizeForDriver, ref stFrameInfo, 1000);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("No Data!");
                return;
            }

            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
            stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Jpeg;
            stSaveParam.enPixelType = stFrameInfo.enPixelType;
            stSaveParam.pData = pData;
            stSaveParam.nDataLen = nDataLen;
            stSaveParam.nHeight = stFrameInfo.nHeight;
            stSaveParam.nWidth = stFrameInfo.nWidth;
            stSaveParam.pImageBuffer = pImage;
            stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
            stSaveParam.nJpgQuality = 80;
            nRet = m_pOperator.SaveImage(ref stSaveParam);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Save Fail!");
                return;
            }
            string save_image_path = @"C:\Users\ttxid\source\repos\MainWindown\MainWindown\Data_image\";
            string filename = save_image_path + $"Image_{DateTime.Now.ToString("MMdd_hhmm")}.jpg";

            FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write);
            file.Write(m_pBufForSaveImage, 0, (int)stSaveParam.nImageLen);
            file.Close();

            MessageBox.Show("Save Succeed!");
        }

        private void bnGetParam_Click(object sender, EventArgs e)
        {
            float fExposure = 0;
            m_pOperator.GetFloatValue("ExposureTime", ref fExposure);
            tbExposure.Text = fExposure.ToString("F1");

            float fGain = 0;
            m_pOperator.GetFloatValue("Gain", ref fGain);
            tbGain.Text = fGain.ToString("F1");

            float fFrameRate = 0;
            m_pOperator.GetFloatValue("ResultingFrameRate", ref fFrameRate);
            tbFrameRate.Text = fFrameRate.ToString("F1");
        }

        private void bnSetParam_Click(object sender, EventArgs e)
        {
            int nRet;
            // ExposureAuto after this time
            uint nValue = 0;
            m_pOperator.SetEnumValue("ExposureAuto", nValue);

            try
            {
                float.Parse(tbExposure.Text);
                float.Parse(tbGain.Text);
                float.Parse(tbFrameRate.Text);
            }
            catch
            {
                MessageBox.Show("Please enter correct type!");
                return;
            }

            nRet = m_pOperator.SetFloatValue("ExposureTime", float.Parse(tbExposure.Text));
            if (nRet != CameraOperator.CO_OK)
            {
                MessageBox.Show("Set Exposure Time Fail!");
            }

            m_pOperator.SetEnumValue("GainAuto", 0);
            nRet = m_pOperator.SetFloatValue("Gain", float.Parse(tbGain.Text));
            if (nRet != CameraOperator.CO_OK)
            {
                MessageBox.Show("Set Gain Fail!");
            }

            nRet = m_pOperator.SetFloatValue("AcquisitionFrameRate", float.Parse(tbFrameRate.Text));
            if (nRet != CameraOperator.CO_OK)
            {
                MessageBox.Show("Set Frame Rate Fail!");
            }
        }
        // Error Message
        private void ShowErrorMsg(string csMessage, int nErrorNum)
        {
            string errorMsg;
            if (nErrorNum == 0)
            {
                errorMsg = csMessage;
            }
            else
            {
                errorMsg = csMessage + ": Error =" + String.Format("{0:X}", nErrorNum);
                richTextBox.Text += String.Format(csMessage + ": Error =" + "{0:X}", nErrorNum);
            }

            switch (nErrorNum)
            {
                case MyCamera.MV_E_HANDLE: errorMsg += " Error or invalid handle "; break;
                case MyCamera.MV_E_SUPPORT: errorMsg += " Not supported function "; break;
                case MyCamera.MV_E_BUFOVER: errorMsg += " Cache is full "; break;
                case MyCamera.MV_E_CALLORDER: errorMsg += " Function calling order error "; break;
                case MyCamera.MV_E_PARAMETER: errorMsg += " Incorrect parameter "; break;
                case MyCamera.MV_E_RESOURCE: errorMsg += " Applying resource failed "; break;
                case MyCamera.MV_E_NODATA: errorMsg += " No data "; break;
                case MyCamera.MV_E_PRECONDITION: errorMsg += " Precondition error, or running environment changed "; break;
                case MyCamera.MV_E_VERSION: errorMsg += " Version mismatches "; break;
                case MyCamera.MV_E_NOENOUGH_BUF: errorMsg += " Insufficient memory "; break;
                case MyCamera.MV_E_UNKNOW: errorMsg += " Unknown error "; break;
                case MyCamera.MV_E_GC_GENERIC: errorMsg += " General error "; break;
                case MyCamera.MV_E_GC_ACCESS: errorMsg += " Node accessing condition error "; break;
                case MyCamera.MV_E_ACCESS_DENIED: errorMsg += " No permission "; break;
                case MyCamera.MV_E_BUSY: errorMsg += " Device is busy, or network disconnected "; break;
                case MyCamera.MV_E_NETER: errorMsg += " Network error "; break;
            }

            MessageBox.Show(errorMsg, "PROMPT");
        }
        private Boolean IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }
        

        // Communication vision with device control valve
        private void InitializeComport()
        {
            comPort = null;
            var serialPort = ComPort.ScanPort();
            if (serialPort != null)
            {
                comPort = new ComPort(serialPort);
                comPort.Received += new EventHandler(Comport_Received);
                Log.Info("InitializeComPort done!");
            }
            if (comPort == null)
            {
                Log.Error("Cannot connect to COM port.");
                if (Constants.SHOW_MESSAGE_NOT_CONNECT_COMPORT)
                {
                    MessageBox.Show("Không kết nối được cổng COM.");
                }
            }
        }
        private void Comport_Received(object sender, EventArgs e)
        {
            this.BeginInvoke((Action)(() => {
                try
                {

                    ComData comData = (ComData)sender;
                    if (comData.message.Equals(Constants.AUTO_OPEN_MESSAGE))
                    {
                        Log.Info("Com port");
                        MessageBox.Show("Order ComPort!");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }));

        }
        private void ExcuteOpenValve(Constants.OpenType openType, bool isMainThread)
        {
            bool openSuccess = false;
            try
            {
                if (comPort != null)
                {
                    comPort.OpenS();
                    openSuccess = true;
                }
                else
                {
                    InitializeComport();
                    if (comPort != null)
                    {
                        comPort.OpenS();
                        openSuccess = true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                InitializeComport();
                if (comPort != null)
                {
                    comPort.OpenS();
                    openSuccess = true;
                }
                else
                {
                    if (Constants.SHOW_MESSAGE_NOT_CONNECT_COMPORT)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
            if (Constants.CREATE_LOG_WHEN_NOT_CONNECT_COMPORT)
            {
                MessageBox.Show("CREATE_LOG_WHEN_NOT_CONNECT_COMPORT");
            }
            else if (openSuccess)
            {
                MessageBox.Show("CREATE_LOG_WHEN_NOT_CONNECT_COMPORT");
            }
        }
        private void bnAuto_Click(object sender, EventArgs e)
        {
            bnManual.Enabled = false;
            Log.Info("Tự Động");
            return;
        }

        private void bnManual_Click(object sender, EventArgs e)
        {
            try
            {
                ExcuteOpenValve(Constants.OpenType.MANUAL, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private void richTextBox_TextChanged_1(object sender, EventArgs e)
        {
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }
        private void richTextBox_DoubleClick_1(object sender, EventArgs e)
        {
            richTextBox.Clear();
        }

    }
}
