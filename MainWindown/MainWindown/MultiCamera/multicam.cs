using System;
using System.Windows.Forms;
using DeviceSource;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using log4net;
using MainWindow.Utils;
using MainWindow.Connection;
using System.Collections.Generic;
using MainWindow.Model;
using System.Drawing;

namespace MainWindow.MultiCamera
{
    //using TextCallback = SetTextCallback ;
    public partial class multicam : Form
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        private ComPort comPort;
        MyCamera.cbOutputdelegate cbImage;
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        CameraOperator[] m_pOperator;

        Thread m_hReceiveThread = null;
        MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
        private static Object BufForDriverLock = new Object();
        private MyCamera m_MyCamera = new MyCamera();

        bool m_bGrabbing;
        int m_nCanOpenDeviceNum;        //Used Device Number
        int m_nDevNum;        //Online Device Number
        int[] m_nFrames;      //Frame Number
        bool m_bTimerFlag;     //Timer Start Timing Flag Bit
        bool[] m_bSaveImg;    //Save Image Flag Bit
        IntPtr[] m_hDisplayHandle;
        private static ILog Log = LogManager.GetLogger(typeof(MainWindow));
        private List<byte[]> DefectImages = new List<byte[]>();
        private List<byte[]> overViewImages = new List<byte[]>();

        //Buffer for getting image from driver
        UInt32 m_nBufSizeForDriver = 0;
        IntPtr m_BufForDriver = IntPtr.Zero;

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
        public multicam()
        {
            InitializeComponent();
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_bGrabbing = false;
            m_nCanOpenDeviceNum = 0;
            m_nDevNum = 0;
            DeviceListAcq();
            m_pOperator = new CameraOperator[4];
            m_nFrames = new int[4];
            cbImage = new MyCamera.cbOutputdelegate(ImageCallBack1);
            m_bTimerFlag = false;
            m_bSaveImg = new bool[4];
            m_hDisplayHandle = new IntPtr[4];
        }

        public void ResetMember()
        {
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_bGrabbing = false;
            m_nCanOpenDeviceNum = 0;
            m_nDevNum = 0;
            DeviceListAcq();
            m_pOperator = new CameraOperator[4];
            m_nFrames = new int[4];
            cbImage = new MyCamera.cbOutputdelegate(ImageCallBack1);
            m_bTimerFlag = false;
            m_bSaveImg = new bool[4];
            m_hDisplayHandle = new IntPtr[4];
        }

        /*Create Device List*/
        private void DeviceListAcq()
        {
            int nRet;

            System.GC.Collect();
            nRet = CameraOperator.EnumDevices(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            if (0 != nRet)
            {
                MessageBox.Show("Enumerate devices fail!");
                Log.Error(nRet);
                ShowErrorMsg("Enumerate devices fail!", 0);
                return;
            }

            m_nDevNum = (int)m_pDeviceList.nDeviceNum;
            tbDevNum.Text = m_nDevNum.ToString("d");
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
            cbHardTrigger.Enabled = false;
            bnTriggerExec.Enabled = false;

            tbExposure.Enabled = true;
            tbGain.Enabled = true;
            bnSetParam.Enabled = true;
        }

        //Initialization and open devices
        private void bnOpen_Click(object sender, EventArgs e)
        {
            bool bOpened = false;
            //Determine whether the input format is correct
            try
            {
                int.Parse(tbUseNum.Text);
            }
            catch
            {
                MessageBox.Show("Please enter correct format!");
                ShowErrorMsg("No device, please select", 0);
                
                return;
            }
            //Get Used Device Number
            int nCameraUsingNum = int.Parse(tbUseNum.Text);
            // Parameters inspection
            if (nCameraUsingNum <= 0)
            {
                nCameraUsingNum = 1;
            }
            if (nCameraUsingNum > 4)
            {
                nCameraUsingNum = 4;
            }

            int nRet = -1;

            for (int i = 0, j = 0; j < m_nDevNum; ++i, ++j)
            {
                //Get Selected Device Information
                m_pOperator[i] = new CameraOperator();
                MyCamera.MV_CC_DEVICE_INFO device =
                    (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[j],
                                                              typeof(MyCamera.MV_CC_DEVICE_INFO));

                //Open Device
                nRet = m_pOperator[i].Open(ref device);
                if (MyCamera.MV_OK != nRet)
                {
                    i--;
                    string temp = "No. " + (i+1).ToString() + " Device Open Failed!";
                    Log.Error(temp);
                    MessageBox.Show(temp);
                    ShowErrorMsg("Device open fail!", nRet);
                }

                else
                {
                    m_nCanOpenDeviceNum++;
                    m_pOperator[i].SetEnumValue("TriggerMode", 0);  // trigger mode OFF
                    m_pOperator[i].SetEnumValue("TriggerMode", 1);  // trigger mode ON
                    m_pOperator[i].RegisterImageCallBack(cbImage, (IntPtr)i);  //Register Callback Function
                    bOpened = true;
                    if (m_nCanOpenDeviceNum == nCameraUsingNum)
                    {
                        break;
                    }
                }
                
                // Detection network optimal package size (GigE Camera)
                if(device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    MyCamera m_MyCamera = new MyCamera();

                    int nPacketSize = m_MyCamera.MV_CC_GetOptimalPacketSize_NET();
                    if(nPacketSize > 0)
                    {
                        nRet = m_MyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                        if(nRet != MyCamera.MV_OK)
                        {
                            ShowErrorMsg("Set Packet Size failed!", nRet);
                        }
                    }
                    else
                    {
                        ShowErrorMsg("Get Packet Size failed!", nPacketSize);
                    }
                }
            }

            //As long as there is a device successfully opened, control operation
            if (bOpened)
            {
                tbUseNum.Text = m_nCanOpenDeviceNum.ToString();
                Log.Info("Open Success!!!");
                SetCtrlWhenOpen();
            }
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
            cbHardTrigger.Enabled = false;
            bnTriggerExec.Enabled = false;

            bnSaveBmp.Enabled = false;
            tbExposure.Enabled = false;
            tbGain.Enabled = false;
            bnSetParam.Enabled = false;
        }

        //Close Device
        private void bnClose_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                m_pOperator[i].Close();
            }

            //Control Operation
            SetCtrlWhenClose();
            //Zero setting grabbing flag bit
            m_bGrabbing = false;
            // Reset member variable
            ResetMember();
        }

        //Continuous Acquisition
        private void bnContinuesMode_CheckedChanged(object sender, EventArgs e)
        {
            if (bnContinuesMode.Checked)
            {
                for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
                {
                    m_pOperator[i].SetEnumValue("TriggerMode", 0);
                    cbSoftTrigger.Enabled = false;
                    cbHardTrigger.Enabled = false;
                    bnTriggerExec.Enabled = false;
                }
            }
        }

        //Open Trigger Mode
        private void bnTriggerMode_CheckedChanged(object sender, EventArgs e)
        {
            if (bnTriggerMode.Checked)
            {
                for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
                {
                    m_pOperator[i].SetEnumValue("TriggerMode", 1);
                    if(cbHardTrigger.Checked)
                    {
                        bnTriggerExec.Enabled = true;
                    }
                    cbHardTrigger.Enabled = true;
                    //Trigger source select:0 - Line0;
                    //                      1 - Line1;
                    //                      2 - Line2;
                    //                      3 - Line3;
                    //                      4 - Counter;
                    //                      7 - Software;
                    if (cbSoftTrigger.Checked)
                    {
                        m_pOperator[i].SetEnumValue("TriggerSource", 7);
                        if (m_bGrabbing)
                        {
                            bnTriggerExec.Enabled = true;
                        }
                    }
                    else
                    {
                        m_pOperator[i].SetEnumValue("TriggerSource", 0);
                    }
                    cbSoftTrigger.Enabled = true;
                    bnSaveBmp.Enabled = false;

                }
            }
            
        }

        private void SetCtrlWhenStartGrab()
        {
            bnStartGrab.Enabled = false;
            bnStopGrab.Enabled = true;
            bnClose.Enabled = false;

            //bnTriggerExec.Enabled = true;
            if (bnTriggerMode.Checked && cbSoftTrigger.Checked && cbHardTrigger.Checked)
            {
                bnTriggerExec.Enabled = true;
            }

            bnSaveBmp.Enabled = true;
        }

        public void ReceiveThreadProcess()
        {
            MyCamera.MV_FRAME_OUT stFrameInfo = new MyCamera.MV_FRAME_OUT();
            MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();

            int nRet = MyCamera.MV_OK;
            MyCamera m_MyCamera = new MyCamera();
            while (m_bGrabbing)
            {
                nRet = m_MyCamera.MV_CC_GetImageBuffer_NET(ref stFrameInfo, 1000);
                if (nRet == MyCamera.MV_OK)
                {
                    lock(BufForDriverLock)
                    {
                        if(m_BufForDriver == IntPtr.Zero || stFrameInfo.stFrameInfo.nFrameLen > m_nBufSizeForDriver)
                        {
                            if(m_BufForDriver != IntPtr.Zero)
                            {
                                Marshal.Release(m_BufForDriver);
                                m_BufForDriver = IntPtr.Zero;
                            }
                            m_BufForDriver = Marshal.AllocHGlobal((Int32)stFrameInfo.stFrameInfo.nFrameLen);
                            if(m_BufForDriver == IntPtr.Zero)
                            {
                                return;
                            }
                            m_nBufSizeForDriver = stFrameInfo.stFrameInfo.nFrameLen;
                        }
                        m_stFrameInfo = stFrameInfo.stFrameInfo;
                        CopyMemory(m_BufForDriver, stFrameInfo.pBufAddr, stFrameInfo.stFrameInfo.nFrameLen);
                    }

                    if(RemoveCustomPixelFormats(stFrameInfo.stFrameInfo.enPixelType))
                    {
                        m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameInfo);
                        continue;
                    }
                    stDisplayInfo.hWnd = pictureBox1.Handle;
                    stDisplayInfo.hWnd = pictureBox2.Handle;
                    stDisplayInfo.nDataLen = stFrameInfo.stFrameInfo.nFrameLen;
                    stDisplayInfo.nWidth = stFrameInfo.stFrameInfo.nWidth;
                    stDisplayInfo.nHeight = stFrameInfo.stFrameInfo.nHeight;
                    stDisplayInfo.enPixelType = stFrameInfo.stFrameInfo.enPixelType;
                    m_MyCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);

                    m_MyCamera.MV_CC_FreeImageBuffer_NET(ref stFrameInfo);
                }
                else
                {
                    if(bnTriggerMode.Checked)
                    {
                        Thread.Sleep(5);
                    }
                }
            }
        }

        // Remove custom pixel formats
        private bool RemoveCustomPixelFormats(MyCamera.MvGvspPixelType enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & (unchecked((Int32)0x80000000));
            if (0x80000000 == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Save image BMP
        private void SaveImage(IntPtr pData, MyCamera.MV_FRAME_OUT_INFO stFrameInfo, int nIndex)
        {
           
            string[] path = { "image1.bmp", "image2.bmp", "image3.bmp", "image4.bmp" };
            int nRet;

            if ((3 * stFrameInfo.nFrameLen + 2048) > m_pOperator[nIndex].m_nBufSizeForSaveImage)
            {
                m_pOperator[nIndex].m_nBufSizeForSaveImage = 3 * stFrameInfo.nFrameLen + 2048;
                m_pOperator[nIndex].m_pBufForSaveImage = new byte[m_pOperator[nIndex].m_nBufSizeForSaveImage];
            }

            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pOperator[nIndex].m_pBufForSaveImage, 0);
            MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
            stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
            stSaveParam.enPixelType = stFrameInfo.enPixelType;
            stSaveParam.pData = pData;
            stSaveParam.nDataLen = stFrameInfo.nFrameLen;
            stSaveParam.nHeight = stFrameInfo.nHeight;
            stSaveParam.nWidth = stFrameInfo.nWidth;
            stSaveParam.pImageBuffer = pImage;
            stSaveParam.nBufferSize = m_pOperator[nIndex].m_nBufSizeForSaveImage;
            //stSaveParam.nJpgQuality = 80; // jpeg
            nRet = m_pOperator[nIndex].SaveImage(ref stSaveParam);
            if (CameraOperator.CO_OK != nRet)
            {
                string temp = "No. " + (nIndex + 1).ToString() + " Device Save Failed!";
                MessageBox.Show(temp);
            }
            else
            {
                FileStream file = new FileStream(path[nIndex], FileMode.Create, FileAccess.Write);
                file.Write(m_pOperator[nIndex].m_pBufForSaveImage, 0, (int)stSaveParam.nImageLen);
                file.Close();
                string temp = "No. " + (nIndex + 1).ToString() + " Device Save Succeed!";
                MessageBox.Show(temp);
            }
        }

        //Aquisition Callback Function
        private void ImageCallBack1(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser)
        {
            int nIndex = (int)pUser;

            //Aquired Frame Number
            ++m_nFrames[nIndex];

            //Determine whether to save image
            if (m_bSaveImg[nIndex])
            {
                SaveImage(pData, pFrameInfo, nIndex);
                m_bSaveImg[nIndex] = false;
            }
        }

        private void bnStartGrab_Click(object sender, EventArgs e)
        {
            m_bGrabbing = true;

            m_hReceiveThread = new Thread(ReceiveThreadProcess);
            m_hReceiveThread.Start();

            int nRet;
            m_hDisplayHandle[0] = pictureBox1.Handle;
            m_hDisplayHandle[1] = pictureBox2.Handle;
            m_hDisplayHandle[2] = pictureBox3.Handle;
            m_hDisplayHandle[3] = pictureBox4.Handle;

            //Start Grabbing
            for (int i = 0; i < m_nCanOpenDeviceNum; i++)
            {
                m_nFrames[i] = 0;
                nRet = m_pOperator[i].StartGrabbing();
                if (MyCamera.MV_OK != nRet)
                {
                    m_hReceiveThread.Join();
                    string temp = "No. " + (i + 1).ToString() + " Device Aqusition Failed!";
                    ShowErrorMsg("Start Grabbing Fail!", nRet);
                    MessageBox.Show(temp);
                }
                nRet = m_pOperator[i].Display(m_hDisplayHandle[i]);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("No.1 device display fail!");
                }
            }
            m_bTimerFlag = true;     //Start Timing

            //Control Operation
            SetCtrlWhenStartGrab();
            //Set Position Bit true
            m_bGrabbing = true;
        }

        private void cbSoftTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSoftTrigger.Checked)
            {
                //Set Trigger Source As Software
                for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
                {
                    m_pOperator[i].SetEnumValue("TriggerSource", 7);
                }
                if (m_bGrabbing)
                {
                    bnTriggerExec.Enabled = true;
                    
                }
            }
            else
            {
                bnTriggerExec.Enabled = false;
            }
        }
        /* test hardware trigger*/
        private void cbHardTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHardTrigger.Checked)
            {
                // Set Trigger Source As Hardware
                for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
                {
                    m_pOperator[i].SetEnumValue("TriggerSource", 1);
                    richTextBox.Text += String.Format("No.{0} of camera trigger!\n", (i + 1).ToString());
                }
                if (m_bGrabbing)
                {
                    bnTriggerExec.Enabled = true;

                    float nRet;
                    nRet = m_pOperator[2].SetFloatValue("TriggerDelay", 1000000);
                    if (MyCamera.MV_OK != nRet)
                    {
                        Log.Error("Set TriggerDelay failed!");
                        ShowErrorMsg("Set TriggerDelay failed!", -1);
                        return;
                    }

                }
            }
            else
            {
                bnTriggerExec.Enabled = false;
            }
        }

        //Trigger Command
        private void bnTriggerExec_Click(object sender, EventArgs e)
        {
            int nRet;

            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                nRet = m_pOperator[i].CommandExecute("TriggerSoftware");
                if (CameraOperator.CO_OK != nRet)
                {
                    string temp = "No. " + (i + 1).ToString() + " Device Trigger Fail!";
                    Log.Error(temp);
                    richTextBox.Text += String.Format("No.{0} Set software trigger failed! nRet=0x{1}\r\n", (i + 1).ToString(), nRet.ToString("X"));
                }
            }
            // Trigger Hardware
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                nRet = m_pOperator[i].CommandExecute("TriggerDelay");
                if(CameraOperator.CO_OK != nRet)
                {
                    string temp = "No. " + (i + 1).ToString() + " Device Trigger Failed!";
                    richTextBox.Text += String.Format("No.{0} Set hardware trigger failed! nRet=0x{1}\r\n", (i + 1).ToString(), nRet.ToString("X"));
                    Log.Error(temp);
                }
            }

        }

        private void SetCtrlWhenStopGrab()
        {
            bnStartGrab.Enabled = true;
            bnStopGrab.Enabled = false;
            bnClose.Enabled = true;

            bnTriggerExec.Enabled = false;
            bnSaveBmp.Enabled = false;
        }

        //Stop Grabbing
        private void bnStopGrab_Click(object sender, EventArgs e)
        {
            m_bGrabbing = false;
            m_hReceiveThread.Join();
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                m_pOperator[i].StopGrabbing();
                int nRet = m_MyCamera.MV_CC_StopGrabbing_NET();
                if (nRet != MyCamera.MV_OK)
                {
                    ShowErrorMsg("Stop Grabbing Fail!", nRet);
                }
            }
            //Set Flag Bit false
            m_bGrabbing = false;

            m_bTimerFlag = false;     //Stop Timing

            //Control Operation
            SetCtrlWhenStopGrab();
        }

        //Click on Save Image Button
        private void bnSaveBmp_Click(object sender, EventArgs e)
        {
            MyCamera.MV_SAVE_IMG_TO_FILE_PARAM stSaveParam = new MyCamera.MV_SAVE_IMG_TO_FILE_PARAM();
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                m_bSaveImg[i] = true;  //Save Image Flag Bit, execute save image in the callback function
                int nRet;
                nRet = m_MyCamera.MV_CC_SaveImageToFile_NET(ref stSaveParam);
                if (MyCamera.MV_OK != nRet)
                {
                    richTextBox.Text += String.Format("No.{0} save image failed! nRet=0x{1}\r\n", (i + 1).ToString(), nRet.ToString("X"));
                }
                else
                {
                    richTextBox.Text += String.Format("No.{0} save image Succeed!\r\n", (i + 1).ToString());
                }
            }
        }

        //Set Exposure Time and Gain
        private void bnSetParam_Click(object sender, EventArgs e)
        {
            try
            {
                float.Parse(tbExposure.Text);
                float.Parse(tbGain.Text);
            }
            catch
            {
                MessageBox.Show("Please Enter Correct Type!");
                return;
            }

            int nRet;
            for (int i = 0; i < m_nCanOpenDeviceNum; ++i)
            {
                bool bSuccess = true;
                m_pOperator[i].SetEnumValue("ExposureAuto", 0);

                nRet = m_pOperator[i].SetFloatValue("ExposureTime", float.Parse(tbExposure.Text));
                if (nRet != CameraOperator.CO_OK)
                {
                    string temp = "No. " + (i + 1).ToString() + " Device Set Exposure Time Failed!";
                    richTextBox.Text += String.Format("No.{0} Set Exposure Time Failed! nRet=0x{1}\r\n", (i + 1).ToString(), nRet.ToString("X"));
                    MessageBox.Show(temp);
                    bSuccess = false;
                }

                m_pOperator[i].SetEnumValue("GainAuto", 0);
                nRet = m_pOperator[i].SetFloatValue("Gain", float.Parse(tbGain.Text));
                if (nRet != CameraOperator.CO_OK)
                {
                    string temp = "No. " + (i + 1).ToString() + " Device Set Gain Failed!";
                    richTextBox.Text += String.Format("No.{0} Set Gain Failed! nRet=0x{1}\r\n", (i + 1).ToString(), nRet.ToString("X"));
                    bSuccess = false;
                }

                if (bSuccess)
                {
                    string temp = "No. " + (i + 1).ToString() + " Device Set Parameters Failed!";
                    richTextBox.Text += String.Format("No.{0} Set Parameters Succeed!\r\n", (i + 1).ToString());
                }
            }
        }

        //Get Throw Frame Number
        private string GetLostFrame(int nIndex)
        {
            MyCamera.MV_ALL_MATCH_INFO pstInfo = new MyCamera.MV_ALL_MATCH_INFO();
            _MV_MATCH_INFO_NET_DETECT_ MV_NetInfo = new _MV_MATCH_INFO_NET_DETECT_();
            pstInfo.nInfoSize = (uint)Marshal.SizeOf(typeof(_MV_MATCH_INFO_NET_DETECT_));
            pstInfo.nType = 0x00000001;
            int size = Marshal.SizeOf(MV_NetInfo);
            pstInfo.pInfo = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(MV_NetInfo, pstInfo.pInfo, false);

            m_pOperator[nIndex].GetAllMatchInfo(ref pstInfo);
            MV_NetInfo = (_MV_MATCH_INFO_NET_DETECT_)Marshal.PtrToStructure(pstInfo.pInfo, typeof(_MV_MATCH_INFO_NET_DETECT_));

            string sTemp = MV_NetInfo.nLostFrameCount.ToString();
            Marshal.FreeHGlobal(pstInfo.pInfo);
            return sTemp;
        }

        //Timer, run once a second
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_bTimerFlag)
            {
                if (m_nCanOpenDeviceNum > 0)
                {
                    tbGrabFrame1.Text = m_nFrames[0].ToString();
                    tbLostFrame1.Text = GetLostFrame(0);
                }
                if (m_nCanOpenDeviceNum > 1)
                {
                    tbGrabFrame2.Text = m_nFrames[1].ToString();
                    tbLostFrame2.Text = GetLostFrame(1);
                }
                if (m_nCanOpenDeviceNum > 2)
                {
                    tbGrabFrame3.Text = m_nFrames[2].ToString();
                    tbLostFrame3.Text = GetLostFrame(2);
                }
                if (m_nCanOpenDeviceNum > 3)
                {
                    tbGrabFrame4.Text = m_nFrames[3].ToString();
                    tbLostFrame4.Text = GetLostFrame(3);
                }
            }
        }
        
        // communication vision with device control valve
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
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
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
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
        // show image trigger in pictureBox3 and pictureBox4
        private void AddImageTriggerToPictureBox()
        {
            bnTriggerExec.Enabled = true;
            pictureBox3.Image = new Bitmap(Application.StartupPath + cbHardTrigger.Checked);
            
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }
        private void richTextBox_DoubleClick(object sender, EventArgs e)
        {
            richTextBox.Clear();
        }
    }
}
