using System;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;
using System.Windows.Forms;
struct _MV_MATCH_INFO_NET_DETECT_
{
    public UInt64 nReviceDataSize; // Received Data Size  [Calculate the data size between StartGrabbing and StopGrabbing]
    public UInt32 nLostPacketCount; // Lost Packets Number
    public uint nLostFrameCount;    // Lost Frames Number
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public uint[] nReserved; // Reserved
}
namespace DeviceSource
{
    using ImageCallBack = MyCamera.cbOutputdelegate;
    using ExceptionCallBack = MyCamera.cbExceptiondelegate;

    public class CameraOperator
    {
        public const int CO_FAIL = -1;
        public const int CO_OK = 0;
        private MyCamera m_pCSI;
        public UInt32 m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
        public byte[] m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];

        public CameraOperator()
        {
            //m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_pCSI = new MyCamera();
        }
        /*
         * nLayerType: IN Transport layer protocol 1-GigE; 4 usb; can be stacked
         * stDeviceList OUT Device list
         * return Success: 0; Fail: Error code
         */
        public static int EnumDevices(uint nLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDeviceList)
        {
            return MyCamera.MV_CC_EnumDevices_NET(nLayerType, ref stDeviceList);
        }

        /*
         * Open deivce
         * stDeviceInfo IN Device Information Structure
         * return Success:0; Fail: -1
         */
        public int Open(ref MyCamera.MV_CC_DEVICE_INFO stDeviceInfo)
        {
            if (null == m_pCSI)
            {
                m_pCSI = new MyCamera();
                if (null == m_pCSI)
                {
                    return CO_FAIL;
                }
            }
            int nRet;
            nRet = m_pCSI.MV_CC_CreateDevice_NET(ref stDeviceInfo);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Create Failed!");
                return CO_FAIL;
            }

            nRet = m_pCSI.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;

        }

        /* Close Deice
         * success 0; fail: -1
         */
        public int Close()
        {
            int nRet;
            nRet = m_pCSI.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            nRet = m_pCSI.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        // Start Grabbing
        public int StartGrabbing()
        {
            int nRet;
            nRet = m_pCSI.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        // Stop Grabbing
        public int StopGrabbing()
        {
            int nRet;
            nRet = m_pCSI.MV_CC_StopGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        // register image
        // param: callback function
        public int RegisterImageCallBack(ImageCallBack CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterImageCallBack_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        // register exception callback
        public int RegisterExceptionCallBack(ExceptionCallBack CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterExceptionCallBack_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        // Get one frame
        // param: pData IN-OUT  Data Array Pointer
        // param: pnDataLen IN  Data Size
        // param: nDataSize IN  Array buffer size
        // param: pFrameInfo OUT Data Information
        public int GetOneFrame(IntPtr pData, ref UInt32 pnDataLen, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo)
        {
            pnDataLen = 0;
            int nRet = m_pCSI.MV_CC_GetOneFrame_NET(pData, nDataSize, ref pFrameInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            pnDataLen = (uint)(pFrameInfo.nWidth * pFrameInfo.nHeight * (((((UInt32)pFrameInfo.enPixelType) >> 16) & 0x00ff) >> 3));
            return CO_OK;
        }
        public int GetOneFrameTimeout(IntPtr pData, ref UInt32 pnDataLen, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, Int32 nMsec)
        {
            pnDataLen = 0;
            int nRet = m_pCSI.MV_CC_GetOneFrameTimeout_NET(pData, nDataSize, ref pFrameInfo, nMsec);
            pnDataLen = pFrameInfo.nFrameLen;
            if (MyCamera.MV_OK != nRet)
            {
                return nRet;
            }

            return nRet;
        }

        /****************************************************************************
         * @fn           Display
         * @brief        Display Image
         * @param        hWnd                  IN        Windows Handle
         * */
        public int Display(IntPtr hWnd)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_Display_NET(hWnd);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /*
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @param        pnValue               OUT       Return Value
         */
        public int GetIntValue(string strKey, ref UInt32 pnValue)
        {
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = m_pCSI.MV_CC_GetIntValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /****************************************************************************
        * @fn           SetIntValue
        * @brief        Set Int Type Paremeters Value
        * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
        * @param        nValue                IN        Set parameters value, for specific value range please refer to HikCameraNode.xls
        */
        public int SetIntValue(string strKey, UInt32 nValue)
        {
            int nRet = m_pCSI.MV_CC_SetIntValue_NET(strKey, nValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        public int GetFloatValue(string strKey, ref float pfValue)
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pCSI.MV_CC_GetFloatValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pfValue = stParam.fCurValue;

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetFloatValue
         * @brief        Set Floot Type Paremeters Value
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @param        fValue                IN        Set parameters value, for specific value range please refer to HikCameraNode.xls
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int SetFloatValue(string strKey, float fValue)
        {
            int nRet = m_pCSI.MV_CC_SetFloatValue_NET(strKey, fValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetEnumValue
         * @brief        Get Enum Type Paremeters Value
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @param        pnValue               OUT       Return Value
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int GetEnumValue(string strKey, ref UInt32 pnValue)
        {
            MyCamera.MVCC_ENUMVALUE stParam = new MyCamera.MVCC_ENUMVALUE();
            int nRet = m_pCSI.MV_CC_GetEnumValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pnValue = stParam.nCurValue;

            return CO_OK;
        }



        /****************************************************************************
         * @fn           SetEnumValue
         * @brief        Set Enum Type Paremeters Value
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @param        nValue                IN        Set parameters value, for specific value range please refer to HikCameraNode.xls
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int SetEnumValue(string strKey, UInt32 nValue)
        {
            int nRet = m_pCSI.MV_CC_SetEnumValue_NET(strKey, nValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }



        /****************************************************************************
         * @fn           GetBoolValue
         * @brief        Get Bool Type Paremeters Value
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @param        pbValue               OUT       Return Value
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int GetBoolValue(string strKey, ref bool pbValue)
        {
            int nRet = m_pCSI.MV_CC_GetBoolValue_NET(strKey, ref pbValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetBoolValue
         * @brief        Set Bool Type Paremeters Value
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @param        bValue                IN        Set parameters value, for specific value range please refer to HikCameraNode.xls
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int SetBoolValue(string strKey, bool bValue)
        {
            int nRet = m_pCSI.MV_CC_SetBoolValue_NET(strKey, bValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetStringValue
         * @brief        Get String Type Paremeters Value
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @param        strValue              OUT       Return Value
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int GetStringValue(string strKey, ref string strValue)
        {
            MyCamera.MVCC_STRINGVALUE stParam = new MyCamera.MVCC_STRINGVALUE();
            int nRet = m_pCSI.MV_CC_GetStringValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            strValue = stParam.chCurValue;

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetStringValue
         * @brief        Set String Type Paremeters Value
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @param        strValue              IN        Set parameters value, for specific value range please refer to HikCameraNode.xls
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int SetStringValue(string strKey, string strValue)
        {
            int nRet = m_pCSI.MV_CC_SetStringValue_NET(strKey, strValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           CommandExecute
         * @brief        Command
         * @param        strKey                IN        Parameters key value, for detail value name please refer to HikCameraNode.xls
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int CommandExecute(string strKey)
        {
            int nRet = m_pCSI.MV_CC_SetCommandValue_NET(strKey);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           SaveImage
         * @brief        Save Image
         * @param        pSaveParam            IN        Save image configure parameters structure 
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int SaveImage(ref MyCamera.MV_SAVE_IMAGE_PARAM_EX pSaveParam)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_SaveImageEx_NET(ref pSaveParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /*
         * FreeImageBuffer
         * Parameters: 
         *          pFrame:  
         *                  Image data and image information
         * Returns:
         *          Success, return MV_OK. Failure, return error code
         */
        public int SetFreeImageBuffer(ref MyCamera.MV_FRAME_OUT stOutFrame)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_FreeImageBuffer_NET(ref stOutFrame);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /*Summary:
         * Get a frame of an image using an internal cache
         * Parameters:
         * pFrame:  Image data and image information
         * nMsec:   Waiting timeout
         * Returns:   Success, return MV_OK. Failure, return error code
         * Gọi FreeImageBuffer trước khi gọi GetImageBuffer
         * Gọi StartGrabbing trước khi gọi GetImageBuffer
        */
        public int GetImageBuffer(ref MyCamera.MV_FRAME_OUT stOutFrame)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_GetImageBuffer_NET(ref stOutFrame, 5);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /****************************************************************************
         * @fn           GetAllMatchInfo
         * @brief        Get various types of information
         * @param        pstInfo             IN  OUT      An information structure of the whole match
         * @return       Success:0; Fail:-1
         ****************************************************************************/
        public int GetAllMatchInfo(ref MyCamera.MV_ALL_MATCH_INFO pstInfo)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_GetAllMatchInfo_NET(ref pstInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        public int LogInfo( ref MyCamera.MV_CC_DEVICE_INFO stDeviceInfo)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_CreateDeviceWithoutLog_NET(ref stDeviceInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
    }
}
