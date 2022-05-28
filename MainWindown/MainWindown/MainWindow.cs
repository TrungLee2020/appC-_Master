using System;
using System.Windows.Forms;
using MvCamCtrl.NET;
using MainWindow.MultiCamera;
using DeviceSource;
using System.Runtime.InteropServices;
using System.IO;
using log4net;
using System.Threading;
using MainWindow.Connection;
using MainWindow.Utils;
using System.Collections.Generic;
using System.Drawing;
using MainWindow.Model;
using MainWindow.Basic;

namespace MainWindow
{
    public partial class MainWindow : Form
    {
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        CameraOperator m_pOperator;
        bool m_bGrabbing;

        UInt32 m_nBufSizeForDriver = 3072 * 2048 * 3;
        byte[] m_pBufForDriver = new byte[3072 * 2048 * 3];            // Buffer for getting image from driver

        UInt32 m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
        byte[] m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];         // Buffer for saving image
        private static ILog Log = LogManager.GetLogger(typeof(MainWindow));
        private ComPort comPort;
        private List<Image> ListOverviewImage = new List<Image>();
        private List<LogInfo> logs = new List<LogInfo>();
        public MainWindow()
        {
            InitializeComponent();
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_pOperator = new CameraOperator();
            m_bGrabbing = false;
            DeviceListAcq();
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
            if (0 != nRet)
            {
                MessageBox.Show("Enumerate devices fail!");
                return;
            }

            //Display device name in the form list
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
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

            //Select the first item
            if (m_pDeviceList.nDeviceNum != 0)
            {
                cbDeviceList.SelectedIndex = 0;
            }
        }
        private void SetCtrlWhenOpen()
        {
            try
            {
                bnOpen.Enabled = false;

                bnClose.Enabled = true;
                //bnStartGrab.Enabled = true;
                //bnStopGrab.Enabled = false;
                //bnContinuesMode.Enabled = true;
                //bnContinuesMode.Checked = true;
                //bnTriggerMode.Enabled = true;
                //cbSoftTrigger.Enabled = false;
                //bnTriggerExec.Enabled = false;

                tbExposure.Enabled = true;
                tbGain.Enabled = true;
                tbFrameRate.Enabled = true;
                bnGetParam.Enabled = true;
                bnSetParam.Enabled = true;
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
                MessageBox.Show(e.Message);
            }
            

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

        private void bnOpen_Click(object sender, EventArgs e)
        {
            //if (m_pDeviceList.nDeviceNum == 0 || cbDeviceList.SelectedIndex == -1)
            //{
            //    MessageBox.Show("No device, please select");
            //    return;
            //}
            //int nRet = -1;

            //Get selected device information
            //MyCamera.MV_CC_DEVICE_INFO device =
            //    (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[cbDeviceList.SelectedIndex],
            //                                                  typeof(MyCamera.MV_CC_DEVICE_INFO));

            //Open device
            //nRet = m_pOperator.Open(ref device);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    MessageBox.Show("Device open fail!");
            //    return;
            //}

            //Set Continues Aquisition Mode
            //m_pOperator.SetEnumValue("AcquisitionMode", 2);
            //m_pOperator.SetEnumValue("TriggerMode", 0);

            //bnGetParam_Click(null, null);//Get parameters

            //Control operation
            //SetCtrlWhenOpen();
            
            //multicam multicam = new multicam();
            //this.Hide();
            //multicam.ShowDialog();
            //this.Close();
            Basic_Form basicform = new Basic_Form();
            this.Hide();
            basicform.ShowDialog();
            this.Close();

        }

    }
}
