// Copyright (c) 2023, Michael Kunz and Artic Imaging SARL. All rights reserved.
// http://kunzmi.github.io/managedCuda
//
// This file is part of ManagedCuda.
//
// Commercial License Usage
//  Licensees holding valid commercial ManagedCuda licenses may use this
//  file in accordance with the commercial license agreement provided with
//  the Software or, alternatively, in accordance with the terms contained
//  in a written agreement between you and Artic Imaging SARL. For further
//  information contact us at managedcuda@articimaging.eu.
//  
// GNU General Public License Usage
//  Alternatively, this file may be used under the terms of the GNU General
//  Public License as published by the Free Software Foundation, either 
//  version 3 of the License, or (at your option) any later version.
//  
//  ManagedCuda is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see <http://www.gnu.org/licenses/>.


using ManagedCuda.BasicTypes;
using System;
using System.Diagnostics;

namespace ManagedCuda.NPP
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NPPImage_8uC3 : NPPImageBase
    {
        #region Constructors
        /// <summary>
        /// Allocates new memory on device using NPP-Api.
        /// </summary>
        /// <param name="nWidthPixels">Image width in pixels</param>
        /// <param name="nHeightPixels">Image height in pixels</param>
        public NPPImage_8uC3(int nWidthPixels, int nHeightPixels)
        {
            _sizeOriginal.width = nWidthPixels;
            _sizeOriginal.height = nHeightPixels;
            _sizeRoi.width = nWidthPixels;
            _sizeRoi.height = nHeightPixels;
            _channels = 3;
            _isOwner = true;
            _typeSize = sizeof(byte);
            _dataType = NppDataType.NPP_8U;
            _nppChannels = NppiChannels.NPP_CH_3;

            _devPtr = NPPNativeMethods.NPPi.MemAlloc.nppiMalloc_8u_C3(nWidthPixels, nHeightPixels, ref _pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}, Pitch is: {3}, Number of color channels: {4}", DateTime.Now, "nppiMalloc_8u_C3", res, _pitch, _channels));

            if (_devPtr.Pointer == 0)
            {
                throw new NPPException("Device allocation error", null);
            }
            _devPtrRoi = _devPtr;
        }

        /// <summary>
        /// Creates a new NPPImage from allocated device ptr.
        /// </summary>
        /// <param name="devPtr">Already allocated device ptr.</param>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="pitch">Pitch / Line step</param>
        /// <param name="isOwner">If TRUE, devPtr is freed when disposing</param>
        public NPPImage_8uC3(CUdeviceptr devPtr, int width, int height, int pitch, bool isOwner)
        {
            _devPtr = devPtr;
            _devPtrRoi = _devPtr;
            _sizeOriginal.width = width;
            _sizeOriginal.height = height;
            _sizeRoi.width = width;
            _sizeRoi.height = height;
            _pitch = pitch;
            _channels = 3;
            _isOwner = isOwner;
            _typeSize = sizeof(byte);
            _dataType = NppDataType.NPP_8U;
            _nppChannels = NppiChannels.NPP_CH_3;
        }

        /// <summary>
        /// Creates a new NPPImage from allocated device ptr. Does not take ownership of decPtr.
        /// </summary>
        /// <param name="devPtr">Already allocated device ptr.</param>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="pitch">Pitch / Line step</param>
        public NPPImage_8uC3(CUdeviceptr devPtr, int width, int height, int pitch)
            : this(devPtr, width, height, pitch, false)
        {

        }

        /// <summary>
        /// Creates a new NPPImage from allocated device ptr. Does not take ownership of inner image device pointer.
        /// </summary>
        /// <param name="image">NPP image</param>
        public NPPImage_8uC3(NPPImageBase image)
            : this(image.DevicePointer, image.Width, image.Height, image.Pitch, false)
        {

        }

        /// <summary>
        /// Allocates new memory on device using NPP-Api.
        /// </summary>
        /// <param name="size">Image size</param>
        public NPPImage_8uC3(NppiSize size)
            : this(size.width, size.height)
        {

        }

        /// <summary>
        /// Creates a new NPPImage from allocated device ptr.
        /// </summary>
        /// <param name="devPtr">Already allocated device ptr.</param>
        /// <param name="size">Image size</param>
        /// <param name="pitch">Pitch / Line step</param>
        /// <param name="isOwner">If TRUE, devPtr is freed when disposing</param>
        public NPPImage_8uC3(CUdeviceptr devPtr, NppiSize size, int pitch, bool isOwner)
            : this(devPtr, size.width, size.height, pitch, isOwner)
        {

        }

        /// <summary>
        /// Creates a new NPPImage from allocated device ptr.
        /// </summary>
        /// <param name="devPtr">Already allocated device ptr.</param>
        /// <param name="size">Image size</param>
        /// <param name="pitch">Pitch / Line step</param>
        public NPPImage_8uC3(CUdeviceptr devPtr, NppiSize size, int pitch)
            : this(devPtr, size.width, size.height, pitch)
        {

        }

        /// <summary>
        /// For dispose
        /// </summary>
        ~NPPImage_8uC3()
        {
            Dispose(false);
        }
        #endregion

        #region Converter operators

        /// <summary>
        /// Converts a NPPImage to a CudaPitchedDeviceVariable
        /// </summary>
        public CudaPitchedDeviceVariable<VectorTypes.uchar3> ToCudaPitchedDeviceVariable()
        {
            return new CudaPitchedDeviceVariable<VectorTypes.uchar3>(_devPtr, _sizeOriginal.width, _sizeOriginal.height, _pitch);
        }

        /// <summary>
        /// Converts a NPPImage to a CudaPitchedDeviceVariable
        /// </summary>
        /// <param name="img">NPPImage</param>
        /// <returns>CudaPitchedDeviceVariable with the same device pointer and size of NPPImage without ROI information</returns>
        public static implicit operator CudaPitchedDeviceVariable<VectorTypes.uchar3>(NPPImage_8uC3 img)
        {
            return img.ToCudaPitchedDeviceVariable();
        }

        /// <summary>
        /// Converts a CudaPitchedDeviceVariable to a NPPImage 
        /// </summary>
        /// <param name="img">CudaPitchedDeviceVariable</param>
        /// <returns>NPPImage with the same device pointer and size of CudaPitchedDeviceVariable with ROI set to full image</returns>
        public static implicit operator NPPImage_8uC3(CudaPitchedDeviceVariable<VectorTypes.uchar3> img)
        {
            return img.ToNPPImage();
        }
        #endregion

        #region Color conversion
        /// <summary>
        /// 3 channel 8-bit unsigned RGB to 2 channel chroma packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void RGBToYCbCr(NPPImage_8uC2 dest)
        {
            status = NPPNativeMethods.NPPi.RGBToYCbCr.nppiRGBToYCbCr422_8u_C3C2R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr422_8u_C3C2R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to packed YCbCr color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void RGBToYCbCr(NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.RGBToYCbCr.nppiRGBToYCbCr_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to planar YCbCr420 color conversion.
        /// </summary>
        /// <param name="dst0">Destination image channel 0</param>
        /// <param name="dst1">Destination image channel 1</param>
        /// <param name="dst2">Destination image channel 2</param>
        public void RGBToYCbCr420(NPPImage_8uC1 dst0, NPPImage_8uC1 dst1, NPPImage_8uC1 dst2)
        {
            CUdeviceptr[] array = new CUdeviceptr[] { dst0.DevicePointerRoi, dst1.DevicePointerRoi, dst2.DevicePointerRoi };
            int[] arrayStep = new int[] { dst0.Pitch, dst1.Pitch, dst2.Pitch };
            status = NPPNativeMethods.NPPi.RGBToYCbCr.nppiRGBToYCbCr420_8u_C3P3R(_devPtrRoi, _pitch, array, arrayStep, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr420_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YCbCr to RGB color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void YCbCrToRGB(NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCrToRGB_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToRGB_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel planar 8-bit unsigned RGB to YCbCr color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public static void RGBToYCbCr(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr.nppiRGBToYCbCr_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public static void YCbCrToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCrToRGB_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToRGB_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to packed RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr420ToYCbCr411(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arraySrcStep = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCr420ToRGB_8u_P3C3R(arraySrc, arraySrcStep, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToRGB_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr:420 to YCbCr:422 resampling.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public static void YCbCr420ToYCbCr422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arraySrcStep = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            int[] arrayDestStep = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.SamplePatternConversion.nppiYCbCr420ToYCbCr422_8u_P3R(arraySrc, arraySrcStep, arrayDest, arrayDestStep, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToYCbCr422_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr:422 to YCbCr:411 resampling.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public static void YCbCr422ToYCbCr411(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arraySrcStep = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            int[] arrayDestStep = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.SamplePatternConversion.nppiYCbCr422ToYCbCr411_8u_P3R(arraySrc, arraySrcStep, arrayDest, arrayDestStep, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToYCbCr411_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr:422 to YCbCr:420 resampling.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public static void YCbCr422ToYCbCr420(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arraySrcStep = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            int[] arrayDestStep = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.SamplePatternConversion.nppiYCbCr422ToYCbCr420_8u_P3R(arraySrc, arraySrcStep, arrayDest, arrayDestStep, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToYCbCr420_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr:420 to YCbCr:411 resampling.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="destY">Destination image channel 0</param>
        /// <param name="destCbCr">Destination image channel 1</param>
        public static void YCbCr420ToYCbCr411(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 destY, NPPImage_8uC1 destCbCr)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arraySrcStep = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.SamplePatternConversion.nppiYCbCr420ToYCbCr411_8u_P3P2R(arraySrc, arraySrcStep, destY.DevicePointerRoi, destY.Pitch, destCbCr.DevicePointerRoi, destCbCr.Pitch, destY.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToYCbCr411_8u_P3P2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        #endregion

        #region Copy
        /// <summary>
        /// Image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="channel">Channel number. This number is added to the dst pointer</param>
        public void Copy(NPPImage_8uC1 dst, int channel)
        {
            if (channel < 0 | channel >= _channels) throw new ArgumentOutOfRangeException("channel", "channel must be in range [0..2].");
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_8u_C3C1R(_devPtrRoi + channel * _typeSize, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_8u_C3C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Three-channel 8-bit unsigned packed to planar image copy.
        /// </summary>
        /// <param name="dst0">Destination image channel 0</param>
        /// <param name="dst1">Destination image channel 1</param>
        /// <param name="dst2">Destination image channel 2</param>
        public void Copy(NPPImage_8uC1 dst0, NPPImage_8uC1 dst1, NPPImage_8uC1 dst2)
        {
            CUdeviceptr[] array = new CUdeviceptr[] { dst0.DevicePointerRoi, dst1.DevicePointerRoi, dst2.DevicePointerRoi };
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_8u_C3P3R(_devPtrRoi, _pitch, array, dst0.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Three-channel 8-bit unsigned planar to packed image copy.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void Copy(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] array = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_8u_P3C3R(array, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// Image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="channelSrc">Channel number. This number is added to the src pointer</param>
        /// <param name="channelDst">Channel number. This number is added to the dst pointer</param>
        public void Copy(NPPImage_8uC3 dst, int channelSrc, int channelDst)
        {
            if (channelSrc < 0 | channelSrc >= _channels) throw new ArgumentOutOfRangeException("channelSrc", "channelSrc must be in range [0..2].");
            if (channelDst < 0 | channelDst >= dst.Channels) throw new ArgumentOutOfRangeException("channelDst", "channelDst must be in range [0..2].");
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_8u_C3CR(_devPtrRoi + channelSrc * _typeSize, _pitch, dst.DevicePointerRoi + channelDst * _typeSize, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_8u_C3CR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Masked Operation 8-bit unsigned image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="mask">Mask image</param>
        public void Copy(NPPImage_8uC3 dst, NPPImage_8uC1 mask)
        {
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_8u_C3MR(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, mask.DevicePointerRoi, mask.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_8u_C3MR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Logical
        /// <summary>
        /// image bit shift by constant (left).
        /// </summary>
        /// <param name="nConstant">Constant (Array length = 3)</param>
        /// <param name="dest">Destination image</param>
        public void LShiftC(uint[] nConstant, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.LeftShiftConst.nppiLShiftC_8u_C3R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLShiftC_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image bit shift by constant (left), inplace.
        /// </summary>
        /// <param name="nConstant">Constant (Array length = 3)</param>
        public void LShiftC(uint[] nConstant)
        {
            status = NPPNativeMethods.NPPi.LeftShiftConst.nppiLShiftC_8u_C3IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLShiftC_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image bit shift by constant (right).
        /// </summary>
        /// <param name="nConstant">Constant (Array length = 3)</param>
        /// <param name="dest">Destination image</param>
        public void RShiftC(uint[] nConstant, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.RightShiftConst.nppiRShiftC_8u_C3R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRShiftC_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image bit shift by constant (right), inplace.
        /// </summary>
        /// <param name="nConstant">Constant (Array length = 3)</param>
        public void RShiftC(uint[] nConstant)
        {
            status = NPPNativeMethods.NPPi.RightShiftConst.nppiRShiftC_8u_C3IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRShiftC_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image logical and.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void And(NPPImage_8uC3 src2, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.And.nppiAnd_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAnd_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image logical and.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        public void And(NPPImage_8uC3 src2)
        {
            status = NPPNativeMethods.NPPi.And.nppiAnd_8u_C3IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAnd_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image logical and with constant.
        /// </summary>
        /// <param name="nConstant">Value (Array length = 3)</param>
        /// <param name="dest">Destination image</param>
        public void And(byte[] nConstant, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.AndConst.nppiAndC_8u_C3R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAndC_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image logical and with constant.
        /// </summary>
        /// <param name="nConstant">Value (Array length = 3)</param>
        public void And(byte[] nConstant)
        {
            status = NPPNativeMethods.NPPi.AndConst.nppiAndC_8u_C3IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAndC_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image logical Or.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void Or(NPPImage_8uC3 src2, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.Or.nppiOr_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiOr_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image logical Or.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        public void Or(NPPImage_8uC3 src2)
        {
            status = NPPNativeMethods.NPPi.Or.nppiOr_8u_C3IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiOr_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image logical Or with constant.
        /// </summary>
        /// <param name="nConstant">Value (Array length = 3)</param>
        /// <param name="dest">Destination image</param>
        public void Or(byte[] nConstant, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.OrConst.nppiOrC_8u_C3R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiOrC_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image logical Or with constant.
        /// </summary>
        /// <param name="nConstant">Value (Array length = 3)</param>
        public void Or(byte[] nConstant)
        {
            status = NPPNativeMethods.NPPi.OrConst.nppiOrC_8u_C3IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiOrC_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image logical Xor.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void Xor(NPPImage_8uC3 src2, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.Xor.nppiXor_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiXor_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image logical Xor.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        public void Xor(NPPImage_8uC3 src2)
        {
            status = NPPNativeMethods.NPPi.Xor.nppiXor_8u_C3IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiXor_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image logical Xor with constant.
        /// </summary>
        /// <param name="nConstant">Value (Array length = 3)</param>
        /// <param name="dest">Destination image</param>
        public void Xor(byte[] nConstant, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.XorConst.nppiXorC_8u_C3R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiXorC_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image logical Xor with constant.
        /// </summary>
        /// <param name="nConstant">Value (Array length = 3)</param>
        public void Xor(byte[] nConstant)
        {
            status = NPPNativeMethods.NPPi.XorConst.nppiXorC_8u_C3IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiXorC_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Image logical Not.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Not(NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.Not.nppiNot_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNot_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image logical Not.
        /// </summary>
        public void Not()
        {
            status = NPPNativeMethods.NPPi.Not.nppiNot_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNot_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Add
        /// <summary>
        /// Image addition, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Add(NPPImage_8uC3 src2, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Add.nppiAdd_8u_C3RSfs(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAdd_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image addition, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Add(NPPImage_8uC3 src2, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Add.nppiAdd_8u_C3IRSfs(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAdd_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Add constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nConstant">Values to add</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Add(byte[] nConstant, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.AddConst.nppiAddC_8u_C3RSfs(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddC_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Add constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value. Inplace.
        /// </summary>
        /// <param name="nConstant">Values to add</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Add(byte[] nConstant, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.AddConst.nppiAddC_8u_C3IRSfs(nConstant, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddC_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sub
        /// <summary>
        /// Image subtraction, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sub(NPPImage_8uC3 src2, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Sub.nppiSub_8u_C3RSfs(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSub_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image subtraction, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sub(NPPImage_8uC3 src2, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Sub.nppiSub_8u_C3IRSfs(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSub_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Subtract constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sub(byte[] nConstant, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.SubConst.nppiSubC_8u_C3RSfs(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSubC_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Subtract constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value. Inplace.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sub(byte[] nConstant, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.SubConst.nppiSubC_8u_C3IRSfs(nConstant, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSubC_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Mul
        /// <summary>
        /// Image multiplication, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Mul(NPPImage_8uC3 src2, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Mul.nppiMul_8u_C3RSfs(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMul_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image multiplication, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Mul(NPPImage_8uC3 src2, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Mul.nppiMul_8u_C3IRSfs(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMul_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Multiply constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Mul(byte[] nConstant, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.MulConst.nppiMulC_8u_C3RSfs(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulC_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Multiply constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value. Inplace.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Mul(byte[] nConstant, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.MulConst.nppiMulC_8u_C3IRSfs(nConstant, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulC_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image multiplication and scale by max bit width value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void Mul(NPPImage_8uC3 src2, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.MulScale.nppiMulScale_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulScale_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image multiplication and scale by max bit width value
        /// </summary>
        /// <param name="src2">2nd source image</param>
        public void Mul(NPPImage_8uC3 src2)
        {
            status = NPPNativeMethods.NPPi.MulScale.nppiMulScale_8u_C3IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulScale_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Multiply constant to image and scale by max bit width value
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        public void Mul(byte[] nConstant, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.MulConstScale.nppiMulCScale_8u_C3R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulCScale_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Multiply constant to image and scale by max bit width value
        /// </summary>
        /// <param name="nConstant">Value</param>
        public void Mul(byte[] nConstant)
        {
            status = NPPNativeMethods.NPPi.MulConstScale.nppiMulCScale_8u_C3IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulCScale_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Div
        /// <summary>
        /// Image division, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Div(NPPImage_8uC3 src2, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Div.nppiDiv_8u_C3RSfs(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDiv_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image division, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Div(NPPImage_8uC3 src2, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Div.nppiDiv_8u_C3IRSfs(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDiv_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Divide constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Div(byte[] nConstant, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.DivConst.nppiDivC_8u_C3RSfs(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDivC_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Divide constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value. Inplace.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Div(byte[] nConstant, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.DivConst.nppiDivC_8u_C3IRSfs(nConstant, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDivC_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image division, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        /// <param name="rndMode">Result Rounding mode to be used</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Div(NPPImage_8uC3 src2, NPPImage_8uC3 dest, NppRoundMode rndMode, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.DivRound.nppiDiv_Round_8u_C3RSfs(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, rndMode, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDiv_Round_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image division, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="rndMode">Result Rounding mode to be used</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Div(NPPImage_8uC3 src2, NppRoundMode rndMode, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.DivRound.nppiDiv_Round_8u_C3IRSfs(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi, rndMode, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDiv_Round_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Exp
        /// <summary>
        /// Exponential, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Exp(NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Exp.nppiExp_8u_C3RSfs(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiExp_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace exponential, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Exp(int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Exp.nppiExp_8u_C3IRSfs(_devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiExp_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Ln
        /// <summary>
        /// Natural logarithm, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Ln(NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Ln.nppiLn_8u_C3RSfs(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLn_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Natural logarithm, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Ln(int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Ln.nppiLn_8u_C3IRSfs(_devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLn_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sqr
        /// <summary>
        /// Image squared, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sqr(NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Sqr.nppiSqr_8u_C3RSfs(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqr_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace image squared, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sqr(int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Sqr.nppiSqr_8u_C3IRSfs(_devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqr_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sqrt
        /// <summary>
        /// Image square root, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sqrt(NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Sqrt.nppiSqrt_8u_C3RSfs(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrt_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace image square root, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sqrt(int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.Sqrt.nppiSqrt_8u_C3IRSfs(_devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrt_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region LUT
        /// <summary>
        /// look-up-table color conversion.<para/>
        /// The LUT is derived from a set of user defined mapping points through linear interpolation.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="values0">array of user defined OUTPUT values, channel 0</param>
        /// <param name="levels0">array of user defined INPUT values, channel 0</param>
        /// <param name="values1">array of user defined OUTPUT values, channel 1</param>
        /// <param name="levels1">array of user defined INPUT values, channel 1</param>
        /// <param name="values2">array of user defined OUTPUT values, channel 2</param>
        /// <param name="levels2">array of user defined INPUT values, channel 2</param>
        public void Lut(NPPImage_8uC3 dest, CudaDeviceVariable<int> values0, CudaDeviceVariable<int> levels0, CudaDeviceVariable<int> values1, CudaDeviceVariable<int> levels1, CudaDeviceVariable<int> values2, CudaDeviceVariable<int> levels2)
        {
            if (values0.Size != levels0.Size) throw new ArgumentException("values0 and levels0 must have same size.");
            if (values1.Size != levels1.Size) throw new ArgumentException("values1 and levels1 must have same size.");
            if (values2.Size != levels2.Size) throw new ArgumentException("values2 and levels2 must have same size.");

            CUdeviceptr[] values = new CUdeviceptr[3];
            CUdeviceptr[] levels = new CUdeviceptr[3];
            int[] levelLengths = new int[3];

            values[0] = values0.DevicePointer;
            values[1] = values1.DevicePointer;
            values[2] = values2.DevicePointer;
            levels[0] = levels0.DevicePointer;
            levels[1] = levels1.DevicePointer;
            levels[2] = levels2.DevicePointer;

            levelLengths[0] = (int)levels0.Size;
            levelLengths[1] = (int)levels1.Size;
            levelLengths[2] = (int)levels2.Size;

            status = NPPNativeMethods.NPPi.ColorProcessing.nppiLUT_Linear_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, values, levels, levelLengths);

            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_Linear_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Geometric Transforms

        /// <summary>
        /// Compute shape of rotated image.
        /// </summary>
        /// <param name="nAngle">The angle of rotation in degrees.</param>
        /// <param name="nShiftX">Shift along horizontal axis</param>
        /// <param name="nShiftY">Shift along vertical axis</param>
        public double[,] GetRotateQuad(double nAngle, double nShiftX, double nShiftY)
        {
            double[,] quad = new double[4, 2];
            status = NPPNativeMethods.NPPi.GeometricTransforms.nppiGetRotateQuad(new NppiRect(_pointRoi, _sizeRoi), quad, nAngle, nShiftX, nShiftY);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetRotateQuad", status));
            NPPException.CheckNppStatus(status, this);
            return quad;
        }

        /// <summary>
        /// Compute bounding-box of rotated image.
        /// </summary>
        /// <param name="nAngle">The angle of rotation in degrees.</param>
        /// <param name="nShiftX">Shift along horizontal axis</param>
        /// <param name="nShiftY">Shift along vertical axis</param>
        public double[,] GetRotateBound(double nAngle, double nShiftX, double nShiftY)
        {
            double[,] bbox = new double[2, 2];
            status = NPPNativeMethods.NPPi.GeometricTransforms.nppiGetRotateBound(new NppiRect(_pointRoi, _sizeRoi), bbox, nAngle, nShiftX, nShiftY);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetRotateBound", status));
            NPPException.CheckNppStatus(status, this);
            return bbox;
        }

        #endregion

        #region Affine Transformations
        /// <summary>
        /// Calculates affine transform coefficients given source rectangular ROI and its destination quadrangle projection
        /// </summary>
        /// <param name="quad">Destination quadrangle [4,2]</param>
        /// <returns>Affine transform coefficients [2,3]</returns>
        public double[,] GetAffineTransform(double[,] quad)
        {
            double[,] coeffs = new double[2, 3];
            NppiRect rect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.AffinTransforms.nppiGetAffineTransform(rect, quad, coeffs);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetAffineTransform", status));
            NPPException.CheckNppStatus(status, this);
            return coeffs;
        }

        /// <summary>
        /// Calculates affine transform projection of given source rectangular ROI
        /// </summary>
        /// <param name="coeffs">Affine transform coefficients [2,3]</param>
        /// <returns>Destination quadrangle [4,2]</returns>
        public double[,] GetAffineQuad(double[,] coeffs)
        {
            double[,] quad = new double[4, 2];
            NppiRect rect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.AffinTransforms.nppiGetAffineQuad(rect, quad, coeffs);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetAffineQuad", status));
            NPPException.CheckNppStatus(status, this);
            return quad;
        }

        /// <summary>
        /// Calculates bounding box of the affine transform projection of the given source rectangular ROI
        /// </summary>
        /// <param name="coeffs">Affine transform coefficients [2,3]</param>
        /// <returns>Destination quadrangle [2,2]</returns>
        public double[,] GetAffineBound(double[,] coeffs)
        {
            double[,] bound = new double[2, 2];
            NppiRect rect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.AffinTransforms.nppiGetAffineBound(rect, bound, coeffs);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetAffineBound", status));
            NPPException.CheckNppStatus(status, this);
            return bound;
        }
        #endregion

        #region Perspective Transformations
        /// <summary>
        /// Calculates affine transform coefficients given source rectangular ROI and its destination quadrangle projection
        /// </summary>
        /// <param name="quad">Destination quadrangle [4,2]</param>
        /// <returns>Perspective transform coefficients [3,3]</returns>
        public double[,] GetPerspectiveTransform(double[,] quad)
        {
            double[,] coeffs = new double[3, 3];
            NppiRect rect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.PerspectiveTransforms.nppiGetPerspectiveTransform(rect, quad, coeffs);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetPerspectiveTransform", status));
            NPPException.CheckNppStatus(status, this);
            return coeffs;
        }

        /// <summary>
        ///Calculates perspective transform projection of given source rectangular ROI
        /// </summary>
        /// <param name="coeffs">Perspective transform coefficients [3,3]</param>
        /// <returns>Destination quadrangle [4,2]</returns>
        public double[,] GetPerspectiveQuad(double[,] coeffs)
        {
            double[,] quad = new double[4, 2];
            NppiRect rect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.PerspectiveTransforms.nppiGetPerspectiveQuad(rect, quad, coeffs);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetPerspectiveQuad", status));
            NPPException.CheckNppStatus(status, this);
            return quad;
        }

        /// <summary>
        /// Calculates bounding box of the affine transform projection of the given source rectangular ROI
        /// </summary>
        /// <param name="coeffs">Perspective transform coefficients [3,3]</param>
        /// <returns>Destination quadrangle [2,2]</returns>
        public double[,] GetPerspectiveBound(double[,] coeffs)
        {
            double[,] bound = new double[2, 2];
            NppiRect rect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.PerspectiveTransforms.nppiGetPerspectiveBound(rect, bound, coeffs);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetPerspectiveBound", status));
            NPPException.CheckNppStatus(status, this);
            return bound;
        }
        #endregion

        #region Alpha Composition

        /// <summary>
        /// Image composition using constant alpha.
        /// </summary>
        /// <param name="alpha1">constant alpha for this image</param>
        /// <param name="src2">2nd source image</param>
        /// <param name="alpha2">constant alpha for src2</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nppAlphaOp">alpha compositing operation</param>
        public void AlphaComp(byte alpha1, NPPImage_8uC3 src2, byte alpha2, NPPImage_8uC3 dest, NppiAlphaOp nppAlphaOp)
        {
            status = NPPNativeMethods.NPPi.AlphaCompConst.nppiAlphaCompC_8u_C3R(_devPtrRoi, _pitch, alpha1, src2.DevicePointerRoi, src2.Pitch, alpha2, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nppAlphaOp);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAlphaCompC_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image premultiplication using constant alpha.
        /// </summary>
        /// <param name="alpha">alpha</param>
        /// <param name="dest">Destination image</param>
        public void AlphaPremul(byte alpha, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.AlphaPremulConst.nppiAlphaPremulC_8u_C3R(_devPtrRoi, _pitch, alpha, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAlphaPremulC_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// In place alpha premultiplication using constant alpha.
        /// </summary>
        /// <param name="alpha">alpha</param>
        public void AlphaPremul(byte alpha)
        {
            status = NPPNativeMethods.NPPi.AlphaPremulConst.nppiAlphaPremulC_8u_C3IR(alpha, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAlphaPremulC_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region ColorTwist
        /// <summary>
        /// An input color twist matrix with floating-point pixel values is applied
        /// within ROI.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="twistMatrix">The color twist matrix with floating-point pixel values [3,4].</param>
        public void ColorTwist(NPPImage_8uC3 dest, float[,] twistMatrix)
        {
            status = NPPNativeMethods.NPPi.ColorProcessing.nppiColorTwist32f_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, twistMatrix);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwist32f_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel planar 8-bit unsigned color twist.
        /// An input color twist matrix with floating-point pixel values is applied
        /// within ROI.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        /// <param name="twistMatrix">The color twist matrix with floating-point pixel values [3,4].</param>
        public static void ColorTwist(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2, float[,] twistMatrix)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };

            NppStatus status = NPPNativeMethods.NPPi.ColorProcessing.nppiColorTwist32f_8u_P3R(src, src0.Pitch, dst, dest0.Pitch, src0.SizeRoi, twistMatrix);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwist32f_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel planar 8-bit unsigned inplace color twist.
        /// An input color twist matrix with floating-point pixel values is applied
        /// within ROI.
        /// </summary>
        /// <param name="srcDest0">Source / Destination image (Channel 0)</param>
        /// <param name="srcDest1">Source / Destinationimage (Channel 1)</param>
        /// <param name="srcDest2">Source / Destinationimage (Channel 2)</param>
        /// <param name="twistMatrix">The color twist matrix with floating-point pixel values [3,4].</param>
        public static void ColorTwist(NPPImage_8uC1 srcDest0, NPPImage_8uC1 srcDest1, NPPImage_8uC1 srcDest2, float[,] twistMatrix)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { srcDest0.DevicePointerRoi, srcDest1.DevicePointerRoi, srcDest2.DevicePointerRoi };

            NppStatus status = NPPNativeMethods.NPPi.ColorTwist.nppiColorTwist32f_8u_IP3R(src, srcDest0.Pitch, srcDest0.SizeRoi, twistMatrix);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwist32f_8u_IP3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        #endregion

        #region AbsDiff
        /// <summary>
        /// Absolute difference of this minus src2.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void AbsDiff(NPPImage_8uC3 src2, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.AbsDiff.nppiAbsDiff_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAbsDiff_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region CompColorKey
        /// <summary>
        /// packed color complement color key replacement of source image 1 by source image 2
        /// </summary>
        /// <param name="src2">source2 packed pixel format image.</param>
        /// <param name="dest">Destination image</param>
        /// <param name="colorKeyConst">color key constants</param>
        public void CompColorKey(NPPImage_8uC3 src2, NPPImage_8uC3 dest, byte[] colorKeyConst)
        {
            status = NPPNativeMethods.NPPi.CompColorKey.nppiCompColorKey_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, colorKeyConst);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCompColorKey_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Convert
        /// <summary>
        /// 8-bit unsigned to 16-bit unsigned conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        public void Convert(NPPImage_16uC3 dst)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_8u16u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_8u16u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 8-bit unsigned to 16-bit signed conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        public void Convert(NPPImage_16sC3 dst)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_8u16s_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_8u16s_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 8-bit unsigned to 32-bit floating point conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        public void Convert(NPPImage_32fC3 dst)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_8u32f_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 8-bit unsigned to 32-bit signed conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        public void Convert(NPPImage_32sC3 dst)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_8u32s_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_8u32s_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        #endregion

        #region Compare
        /// <summary>
        /// Compare pSrc1's pixels with corresponding pixels in pSrc2.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        /// <param name="eComparisonOperation">Specifies the comparison operation to be used in the pixel comparison.</param>
        public void Compare(NPPImage_8uC3 src2, NPPImage_8uC1 dest, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Compare.nppiCompare_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCompare_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Compare pSrc's pixels with constant value.
        /// </summary>
        /// <param name="nConstant">constant values</param>
        /// <param name="dest">Destination image</param>
        /// <param name="eComparisonOperation">Specifies the comparison operation to be used in the pixel comparison.</param>
        public void Compare(byte[] nConstant, NPPImage_8uC1 dest, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Compare.nppiCompareC_8u_C3R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCompareC_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region NormInf
        /// <summary>
        /// Scratch-buffer size for Norm inf.
        /// </summary>
        /// <returns></returns>
        public SizeT NormInfGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormInf.nppiNormInfGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormInfGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Scratch-buffer size for Norm inf (masked).
        /// </summary>
        /// <returns></returns>
        public SizeT NormInfMaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormInf.nppiNormInfGetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormInfGetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image infinity norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        public void NormInf(CudaDeviceVariable<double> norm)
        {
            SizeT bufferSize = NormInfGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormInf.nppiNorm_Inf_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_Inf_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image infinity norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormInfGetBufferHostSize()"/></param>
        public void NormInf(CudaDeviceVariable<double> norm, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormInfGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormInf.nppiNorm_Inf_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_Inf_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image infinity norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        public void NormInf(int coi, CudaDeviceVariable<double> norm, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = NormInfMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormInf.nppiNorm_Inf_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_Inf_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image infinity norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormInfMaskedGetBufferHostSize()"/></param>
        public void NormInf(int coi, CudaDeviceVariable<double> norm, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormInfMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormInf.nppiNorm_Inf_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_Inf_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region NormL1
        /// <summary>
        /// Scratch-buffer size for Norm L1.
        /// </summary>
        /// <returns></returns>
        public SizeT NormL1GetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormL1.nppiNormL1GetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormL1GetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Scratch-buffer size for Norm L1 (masked).
        /// </summary>
        /// <returns></returns>
        public SizeT NormL1MaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormL1.nppiNormL1GetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormL1GetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image L1 norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        public void NormL1(CudaDeviceVariable<double> norm)
        {
            SizeT bufferSize = NormL1GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormL1.nppiNorm_L1_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L1_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L1 norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormL1GetBufferHostSize()"/></param>
        public void NormL1(CudaDeviceVariable<double> norm, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormL1GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormL1.nppiNorm_L1_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L1_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L1 norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        public void NormL1(int coi, CudaDeviceVariable<double> norm, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = NormL1MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormL1.nppiNorm_L1_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L1_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L1 norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormL1MaskedGetBufferHostSize()"/></param>
        public void NormL1(int coi, CudaDeviceVariable<double> norm, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormL1MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormL1.nppiNorm_L1_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L1_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region NormL2
        /// <summary>
        /// Scratch-buffer size for Norm L2.
        /// </summary>
        /// <returns></returns>
        public SizeT NormL2GetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormL2.nppiNormL2GetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormL2GetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Scratch-buffer size for Norm L2 (masked).
        /// </summary>
        /// <returns></returns>
        public SizeT NormL2MaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormL2.nppiNormL2GetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormL2GetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image L2 norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        public void NormL2(CudaDeviceVariable<double> norm)
        {
            SizeT bufferSize = NormL2GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormL2.nppiNorm_L2_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L2_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L2 norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormL2GetBufferHostSize()"/></param>
        public void NormL2(CudaDeviceVariable<double> norm, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormL2GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormL2.nppiNorm_L2_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L2_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L2 norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        public void NormL2(int coi, CudaDeviceVariable<double> norm, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = NormL2MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormL2.nppiNorm_L2_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L2_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L2 norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="norm">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormL2MaskedGetBufferHostSize()"/></param>
        public void NormL2(int coi, CudaDeviceVariable<double> norm, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormL2MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormL2.nppiNorm_L2_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L2_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Threshold
        /// <summary>
        /// Image threshold.<para/>
        /// If for a comparison operations OP the predicate (sourcePixel OP nThreshold) is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="eComparisonOperation">eComparisonOperation. Only allowed values are <see cref="NppCmpOp.Less"/> and <see cref="NppCmpOp.Greater"/></param>
        public void Threshold(NPPImage_8uC3 dest, byte[] nThreshold, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations OP the predicate (sourcePixel OP nThreshold) is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="eComparisonOperation">eComparisonOperation. Only allowed values are <see cref="NppCmpOp.Less"/> and <see cref="NppCmpOp.Greater"/></param>
        public void Threshold(byte[] nThreshold, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region ThresholdGT
        /// <summary>
        /// Image threshold.<para/>
        /// If for a comparison operations sourcePixel is greater than nThreshold is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nThreshold">The threshold value.</param>
        public void ThresholdGT(NPPImage_8uC3 dest, byte[] nThreshold)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_GT_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_GT_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is greater than nThreshold is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        public void ThresholdGT(byte[] nThreshold)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_GT_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_GT_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region ThresholdLT
        /// <summary>
        /// Image threshold.<para/>
        /// If for a comparison operations sourcePixel is less than nThreshold is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nThreshold">The threshold value.</param>
        public void ThresholdLT(NPPImage_8uC3 dest, byte[] nThreshold)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LT_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LT_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is less than nThreshold is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        public void ThresholdLT(byte[] nThreshold)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LT_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LT_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region ThresholdVal
        /// <summary>
        /// Image threshold.<para/>
        /// If for a comparison operations OP the predicate (sourcePixel OP nThreshold) is true, the pixel is set
        /// to nValue, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="nValue">The threshold replacement value.</param>
        /// <param name="eComparisonOperation">eComparisonOperation. Only allowed values are <see cref="NppCmpOp.Less"/> and <see cref="NppCmpOp.Greater"/></param>
        public void Threshold(NPPImage_8uC3 dest, byte[] nThreshold, byte[] nValue, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_Val_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold, nValue, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_Val_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations OP the predicate (sourcePixel OP nThreshold) is true, the pixel is set
        /// to nValue, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="nValue">The threshold replacement value.</param>
        /// <param name="eComparisonOperation">eComparisonOperation. Only allowed values are <see cref="NppCmpOp.Less"/> and <see cref="NppCmpOp.Greater"/></param>
        public void Threshold(byte[] nThreshold, byte[] nValue, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_Val_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold, nValue, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_Val_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region ThresholdGTVal
        /// <summary>
        /// Image threshold.<para/>
        /// If for a comparison operations sourcePixel is greater than nThreshold is true, the pixel is set
        /// to nValue, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="nValue">The threshold replacement value.</param>
        public void ThresholdGT(NPPImage_8uC3 dest, byte[] nThreshold, byte[] nValue)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_GTVal_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_GTVal_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is greater than nThreshold is true, the pixel is set
        /// to nValue, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="nValue">The threshold replacement value.</param>
        public void ThresholdGT(byte[] nThreshold, byte[] nValue)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_GTVal_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_GTVal_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region ThresholdLTVal
        /// <summary>
        /// Image threshold.<para/>
        /// If for a comparison operations sourcePixel is less than nThreshold is true, the pixel is set
        /// to nValue, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="nValue">The threshold replacement value.</param>
        public void ThresholdLT(NPPImage_8uC3 dest, byte[] nThreshold, byte[] nValue)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LTVal_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LTVal_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is less than nThreshold is true, the pixel is set
        /// to nValue, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="nValue">The threshold replacement value.</param>
        public void ThresholdLT(byte[] nThreshold, byte[] nValue)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LTVal_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LTVal_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region ThresholdLTValGTVal
        /// <summary>
        /// Image threshold.<para/>
        /// If for a comparison operations sourcePixel is less than nThresholdLT is true, the pixel is set
        /// to nValueLT, else if sourcePixel is greater than nThresholdGT the pixel is set to nValueGT, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nThresholdLT">The thresholdLT value.</param>
        /// <param name="nValueLT">The thresholdLT replacement value.</param>
        /// <param name="nThresholdGT">The thresholdGT value.</param>
        /// <param name="nValueGT">The thresholdGT replacement value.</param>
        public void ThresholdLTGT(NPPImage_8uC3 dest, byte[] nThresholdLT, byte[] nValueLT, byte[] nThresholdGT, byte[] nValueGT)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LTValGTVal_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThresholdLT, nValueLT, nThresholdGT, nValueGT);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LTValGTVal_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is less than nThresholdLT is true, the pixel is set
        /// to nValueLT, else if sourcePixel is greater than nThresholdGT the pixel is set to nValueGT, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThresholdLT">The thresholdLT value.</param>
        /// <param name="nValueLT">The thresholdLT replacement value.</param>
        /// <param name="nThresholdGT">The thresholdGT value.</param>
        /// <param name="nValueGT">The thresholdGT replacement value.</param>
        public void ThresholdLTGT(byte[] nThresholdLT, byte[] nValueLT, byte[] nThresholdGT, byte[] nValueGT)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LTValGTVal_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, nThresholdLT, nValueLT, nThresholdGT, nValueGT);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LTValGTVal_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Mean
        /// <summary>
        /// Scratch-buffer size for Mean.
        /// </summary>
        /// <returns></returns>
        public SizeT MeanGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MeanNew.nppiMeanGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMeanGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Scratch-buffer size for Mean with mask.
        /// </summary>
        /// <returns></returns>
        public SizeT MeanMaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MeanNew.nppiMeanGetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMeanGetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image mean with 64-bit double precision result. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 3 * sizeof(double)</param>
        public void Mean(CudaDeviceVariable<double> mean)
        {
            SizeT bufferSize = MeanGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MeanNew.nppiMean_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image mean with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MeanGetBufferHostSize()"/></param>
        public void Mean(CudaDeviceVariable<double> mean, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MeanGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MeanNew.nppiMean_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image mean with 64-bit double precision result. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="mean">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">If the mask is filled with zeros, then all the returned values are zeros, i.e., pMinIndex = {0, 0}, pMaxIndex = {0, 0}, pMinValue = 0, pMaxValue = 0.</param>
        public void Mean(int coi, CudaDeviceVariable<double> mean, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = MeanMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MeanNew.nppiMean_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, buffer.DevicePointer, mean.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image mean with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="mean">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MeanMaskedGetBufferHostSize()"/></param>
        /// <param name="mask">If the mask is filled with zeros, then all the returned values are zeros, i.e., pMinIndex = {0, 0}, pMaxIndex = {0, 0}, pMinValue = 0, pMaxValue = 0.</param>
        public void Mean(int coi, CudaDeviceVariable<double> mean, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MeanMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MeanNew.nppiMean_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, buffer.DevicePointer, mean.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region MeanStdDev
        /// <summary>
        /// Scratch-buffer size for MeanStdDev.
        /// </summary>
        /// <returns></returns>
        public SizeT MeanStdDevGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMeanStdDevGetBufferHostSize_8u_C3CR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMeanStdDevGetBufferHostSize_8u_C3CR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Scratch-buffer size for MeanStdDev (masked).
        /// </summary>
        /// <returns></returns>
        public SizeT MeanStdDevMaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMeanStdDevGetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMeanStdDevGetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image mean and standard deviation. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="mean">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="stdDev">Allocated device memory with size of at least 3 * sizeof(double)</param>
        public void MeanStdDev(int coi, CudaDeviceVariable<double> mean, CudaDeviceVariable<double> stdDev)
        {
            SizeT bufferSize = MeanStdDevGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMean_StdDev_8u_C3CR(_devPtrRoi, _pitch, _sizeRoi, coi, buffer.DevicePointer, mean.DevicePointer, stdDev.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_StdDev_8u_C3CR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image sum with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="mean">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="stdDev">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MeanStdDevGetBufferHostSize()"/></param>
        public void MeanStdDev(int coi, CudaDeviceVariable<double> mean, CudaDeviceVariable<double> stdDev, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MeanStdDevGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMean_StdDev_8u_C3CR(_devPtrRoi, _pitch, _sizeRoi, coi, buffer.DevicePointer, mean.DevicePointer, stdDev.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_StdDev_8u_C3CR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image mean and standard deviation. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="mean">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="stdDev">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        public void MeanStdDev(int coi, CudaDeviceVariable<double> mean, CudaDeviceVariable<double> stdDev, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = MeanStdDevMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMean_StdDev_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, buffer.DevicePointer, mean.DevicePointer, stdDev.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_StdDev_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image sum with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="mean">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="stdDev">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MeanStdDevMaskedGetBufferHostSize()"/></param>
        public void MeanStdDev(int coi, CudaDeviceVariable<double> mean, CudaDeviceVariable<double> stdDev, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MeanStdDevMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMean_StdDev_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, buffer.DevicePointer, mean.DevicePointer, stdDev.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_StdDev_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sum
        /// <summary>
        /// Scratch-buffer size for nppiSum_8u_C3R.
        /// </summary>
        /// <returns></returns>
        public SizeT SumDoubleGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.Sum.nppiSumGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSumGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image sum with 64-bit double precision result. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="result">Allocated device memory with size of at least 3 * sizeof(double)</param>
        public void Sum(CudaDeviceVariable<double> result)
        {
            SizeT bufferSize = SumDoubleGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Sum.nppiSum_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, result.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSum_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image sum with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="result">Allocated device memory with size of at least 3 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="SumDoubleGetBufferHostSize()"/></param>
        public void Sum(CudaDeviceVariable<double> result, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = SumDoubleGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Sum.nppiSum_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, result.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSum_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Min
        /// <summary>
        /// Scratch-buffer size for Min.
        /// </summary>
        /// <returns></returns>
        public SizeT MinGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.Min.nppiMinGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel minimum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        public void Min(CudaDeviceVariable<byte> min)
        {
            SizeT bufferSize = MinGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Min.nppiMin_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, min.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMin_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum. No additional buffer is allocated.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinGetBufferHostSize()"/></param>
        public void Min(CudaDeviceVariable<byte> min, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Min.nppiMin_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, min.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMin_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region MinIndex
        /// <summary>
        /// Scratch-buffer size for MinIndex.
        /// </summary>
        /// <returns></returns>
        public SizeT MinIndexGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MinIdx.nppiMinIndxGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinIndxGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel minimum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="indexX">Allocated device memory with size of at least 3 * sizeof(int)</param>
        /// <param name="indexY">Allocated device memory with size of at least 3 * sizeof(int)</param>
        public void MinIndex(CudaDeviceVariable<byte> min, CudaDeviceVariable<int> indexX, CudaDeviceVariable<int> indexY)
        {
            SizeT bufferSize = MinIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MinIdx.nppiMinIndx_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, min.DevicePointer, indexX.DevicePointer, indexY.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinIndx_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum. No additional buffer is allocated.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="indexX">Allocated device memory with size of at least 3 * sizeof(int)</param>
        /// <param name="indexY">Allocated device memory with size of at least 3 * sizeof(int)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinIndexGetBufferHostSize()"/></param>
        public void MinIndex(CudaDeviceVariable<byte> min, CudaDeviceVariable<int> indexX, CudaDeviceVariable<int> indexY, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MinIdx.nppiMinIndx_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, min.DevicePointer, indexX.DevicePointer, indexY.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinIndx_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Max
        /// <summary>
        /// Scratch-buffer size for Max.
        /// </summary>
        /// <returns></returns>
        public SizeT MaxGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.Max.nppiMaxGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel maximum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="max">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        public void Max(CudaDeviceVariable<byte> max)
        {
            SizeT bufferSize = MaxGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Max.nppiMax_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, max.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMax_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel maximum. No additional buffer is allocated.
        /// </summary>
        /// <param name="max">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MaxGetBufferHostSize()"/></param>
        public void Max(CudaDeviceVariable<byte> max, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MaxGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Max.nppiMax_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, max.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMax_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region MaxIndex
        /// <summary>
        /// Scratch-buffer size for MaxIndex.
        /// </summary>
        /// <returns></returns>
        public SizeT MaxIndexGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MaxIdx.nppiMaxIndxGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxIndxGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel maximum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="max">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="indexX">Allocated device memory with size of at least 3 * sizeof(int)</param>
        /// <param name="indexY">Allocated device memory with size of at least 3 * sizeof(int)</param>
        public void MaxIndex(CudaDeviceVariable<byte> max, CudaDeviceVariable<int> indexX, CudaDeviceVariable<int> indexY)
        {
            SizeT bufferSize = MaxIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MaxIdx.nppiMaxIndx_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, max.DevicePointer, indexX.DevicePointer, indexY.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxIndx_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum. No additional buffer is allocated.
        /// </summary>
        /// <param name="max">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="indexX">Allocated device memory with size of at least 3 * sizeof(int)</param>
        /// <param name="indexY">Allocated device memory with size of at least 3 * sizeof(int)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MaxIndexGetBufferHostSize()"/></param>
        public void MaxIndex(CudaDeviceVariable<byte> max, CudaDeviceVariable<int> indexX, CudaDeviceVariable<int> indexY, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MaxIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MaxIdx.nppiMaxIndx_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, max.DevicePointer, indexX.DevicePointer, indexY.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxIndx_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region MinMax
        /// <summary>
        /// Scratch-buffer size for MinMax.
        /// </summary>
        /// <returns></returns>
        public SizeT MinMaxGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MinMaxNew.nppiMinMaxGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel minimum and maximum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="max">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        public void MinMax(CudaDeviceVariable<byte> min, CudaDeviceVariable<byte> max)
        {
            SizeT bufferSize = MinMaxGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MinMaxNew.nppiMinMax_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, min.DevicePointer, max.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMax_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum and maximum. No additional buffer is allocated.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="max">Allocated device memory with size of at least 3 * sizeof(byte)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinMaxGetBufferHostSize()"/></param>
        public void MinMax(CudaDeviceVariable<byte> min, CudaDeviceVariable<byte> max, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinMaxGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MinMaxNew.nppiMinMax_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, min.DevicePointer, max.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMax_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region MinMaxIndex
        /// <summary>
        /// Scratch-buffer size for MinMaxIndex.
        /// </summary>
        /// <returns></returns>
        public SizeT MinMaxIndexGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndxGetBufferHostSize_8u_C3CR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndxGetBufferHostSize_8u_C3CR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Scratch-buffer size for MinMaxIndex with mask.
        /// </summary>
        /// <returns></returns>
        public SizeT MinMaxIndexMaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndxGetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndxGetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel minimum and maximum values with their indices. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(byte)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(byte)</param>
        /// <param name="minIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="maxIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        public void MinMaxIndex(int coi, CudaDeviceVariable<byte> min, CudaDeviceVariable<byte> max, CudaDeviceVariable<NppiPoint> minIndex, CudaDeviceVariable<NppiPoint> maxIndex)
        {
            SizeT bufferSize = MinMaxIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndx_8u_C3CR(_devPtrRoi, _pitch, _sizeRoi, coi, min.DevicePointer, max.DevicePointer, minIndex.DevicePointer, maxIndex.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndx_8u_C3CR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum and maximum values with their indices. No additional buffer is allocated.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(byte)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(byte)</param>
        /// <param name="minIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="maxIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinMaxIndexGetBufferHostSize()"/></param>
        public void MinMaxIndex(int coi, CudaDeviceVariable<byte> min, CudaDeviceVariable<byte> max, CudaDeviceVariable<NppiPoint> minIndex, CudaDeviceVariable<NppiPoint> maxIndex, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinMaxIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndx_8u_C3CR(_devPtrRoi, _pitch, _sizeRoi, coi, min.DevicePointer, max.DevicePointer, minIndex.DevicePointer, maxIndex.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndx_8u_C3CR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum and maximum values with their indices. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(byte)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(byte)</param>
        /// <param name="minIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="maxIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="mask">If the mask is filled with zeros, then all the returned values are zeros, i.e., pMinIndex = {0, 0}, pMaxIndex = {0, 0}, pMinValue = 0, pMaxValue = 0.</param>
        public void MinMaxIndex(int coi, CudaDeviceVariable<byte> min, CudaDeviceVariable<byte> max, CudaDeviceVariable<NppiPoint> minIndex, CudaDeviceVariable<NppiPoint> maxIndex, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = MinMaxIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndx_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, min.DevicePointer, max.DevicePointer, minIndex.DevicePointer, maxIndex.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndx_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum and maximum values with their indices. No additional buffer is allocated.
        /// </summary>
        /// <param name="coi">Channel of interest (0, 1 or 2)</param>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(byte)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(byte)</param>
        /// <param name="minIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="maxIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinMaxIndexMaskedGetBufferHostSize()"/></param>
        /// <param name="mask">If the mask is filled with zeros, then all the returned values are zeros, i.e., pMinIndex = {0, 0}, pMaxIndex = {0, 0}, pMinValue = 0, pMaxValue = 0.</param>
        public void MinMaxIndex(int coi, CudaDeviceVariable<byte> min, CudaDeviceVariable<byte> max, CudaDeviceVariable<NppiPoint> minIndex, CudaDeviceVariable<NppiPoint> maxIndex, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinMaxIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndx_8u_C3CMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, coi, min.DevicePointer, max.DevicePointer, minIndex.DevicePointer, maxIndex.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndx_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Gamma
        /// <summary>
        /// image forward gamma correction.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Gamma(NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.Gamma.nppiGammaFwd_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGammaFwd_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace image forward gamma correction.
        /// </summary>
        public void Gamma()
        {
            status = NPPNativeMethods.NPPi.Gamma.nppiGammaFwd_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGammaFwd_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar color not in place forward gamma correction.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>   
        public static void Gamma(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.Gamma.nppiGammaFwd_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGammaFwd_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar color in place forward gamma correction.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        public static void Gamma(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.Gamma.nppiGammaFwd_8u_IP3R(arraySrc, src0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGammaFwd_8u_IP3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        #endregion

        #region GammaInv
        /// <summary>
        /// image inverse gamma correction.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void GammaInv(NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.Gamma.nppiGammaInv_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGammaInv_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace image inverse gamma correction.
        /// </summary>
        public void GammaInv()
        {
            status = NPPNativeMethods.NPPi.Gamma.nppiGammaInv_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGammaInv_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar color not in place inverse gamma correction.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>   
        public static void GammaInv(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.Gamma.nppiGammaInv_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGammaInv_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar color in place inverse gamma correction.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        public static void GammaInv(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.Gamma.nppiGammaInv_8u_IP3R(arraySrc, src0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGammaInv_8u_IP3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        #endregion

        #region Color Space conversion new in CUDA 5
        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void YUVToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YUVToRGB.nppiYUVToRGB_8u_P3C3R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToRGB_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCrToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCrToRGB_8u_P3C3R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToRGB_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 3 channel 8-bit unsigned packed BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCrToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCrToBGR_8u_P3C3R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToBGR_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 3 channel 8-bit unsigned packed BGR_709CSC color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCrToBGR_709CSC(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCrToBGR_709CSC_8u_P3C3R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToBGR_709CSC_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned packed HLS color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void BGRToHLS(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.BGRToHLS.nppiBGRToHLS_8u_P3C3R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToHLS_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar HLS to 3 channel 8-bit unsigned packed BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void HLSToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.HLSToBGR.nppiHLSToBGR_8u_P3C3R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHLSToBGR_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 2 channel 8-bit unsigned packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void RGBToYCbCr422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC2 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr.nppiRGBToYCbCr422_8u_P3C2R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr422_8u_P3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 2 channel 8-bit unsigned packed YCrCb422 color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void RGBToYCrCb422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC2 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCrCb.nppiRGBToYCrCb422_8u_P3C2R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCrCb422_8u_P3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 4 channel 8-bit unsigned packed RGB color conversion with constant alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nAval">8-bit unsigned alpha constant.</param>         
        public static void YCbCrToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest, byte nAval)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCrToRGB_8u_P3C4R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi, nAval);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToRGB_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 4 channel 8-bit unsigned packed BGR color conversion with constant alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nAval">8-bit unsigned alpha constant.</param>       
        public static void YCbCrToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest, byte nAval)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCrToBGR_8u_P3C4R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi, nAval);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToBGR_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 4 channel 8-bit unsigned packed BGR_709CSC color conversion with constant alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nAval">8-bit unsigned alpha constant.</param>         
        public static void YCbCrToBGR_709CSC(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest, byte nAval)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCrToBGR_709CSC_8u_P3C4R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi, nAval);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToBGR_709CSC_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YUV color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>     
        public static void RGBToYUV(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYUV.nppiRGBToYUV_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYUV_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV to 3 channel 8-bit unsigned planar RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>     
        public static void YUVToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YUVToRGB.nppiYUVToRGB_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToRGB_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned planar HLS color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>     
        public static void BGRToHLS(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.BGRToHLS.nppiBGRToHLS_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToHLS_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar HLS to 3 channel 8-bit unsigned planar BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>     
        public static void HLSToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.HLSToBGR.nppiHLSToBGR_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHLSToBGR_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YUV422 color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>     
        public static void RGBToYUV422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayDestPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYUV422.nppiRGBToYUV422_8u_P3R(arraySrc, src0.Pitch, arrayDest, arrayDestPitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYUV422_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YUV420 color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>     
        public static void RGBToYUV420(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayDestPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYUV420.nppiRGBToYUV420_8u_P3R(arraySrc, src0.Pitch, arrayDest, arrayDestPitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYUV420_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 2 channel 8-bit unsigned packed YCbCr422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC2 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr422_8u_P3C2R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422_8u_P3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 2 channel 8-bit unsigned packed YCrCb422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr422ToYCrCb422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC2 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr422ToYCrCb422_8u_P3C2R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToYCrCb422_8u_P3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCrCb420 to 2 channel 8-bit unsigned packed YCbCr422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCrCb420ToYCbCr422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC2 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCrCb420ToYCbCr422_8u_P3C2R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCrCb420ToYCbCr422_8u_P3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCrCb420 to 2 channel 8-bit unsigned packed CbYCr422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCrCb420ToCbYCr422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC2 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCrCb420ToCbYCr422_8u_P3C2R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCrCb420ToCbYCr422_8u_P3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 2 channel 8-bit unsigned packed YCbCr422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr411ToYCbCr422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC2 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr411ToYCbCr422_8u_P3C2R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToYCbCr422_8u_P3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 2 channel 8-bit unsigned packed YCrCb422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr411ToYCrCb422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC2 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr411ToYCrCb422_8u_P3C2R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToYCrCb422_8u_P3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV422 to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YUV422ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YUV422ToRGB.nppiYUV422ToRGB_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV422ToRGB_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YUV420ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToRGB.nppiYUV420ToRGB_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToRGB_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 4 channel 8-bit unsigned packed RGB color conversion with constant alpha (0xFF).
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YUV420ToRGBA(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToRGB.nppiYUV420ToRGB_8u_P3C4R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToRGB_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 3 channel 8-bit unsigned packed BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YUV420ToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToBGR.nppiYUV420ToBGR_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToBGR_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 4 channel 8-bit unsigned packed BGR color conversion with constant alpha (0xFF).
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YUV420ToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToBGR.nppiYUV420ToBGR_8u_P3C4R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToBGR_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr422ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCr422ToRGB_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToRGB_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 3 channel 8-bit unsigned packed BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr422ToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr422ToBGR_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToBGR_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 3 channel 8-bit unsigned packed BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr420ToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr420ToBGR_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToBGR_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 3 channel 8-bit unsigned packed BGR_709CSC color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr420ToBGR_709CSC(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr420ToBGR_709CSC_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToBGR_709CSC_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 3 channel 8-bit unsigned packed BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YCbCr411ToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr411ToBGR_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToBGR_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV422 to 4 channel 8-bit unsigned packed RGB color conversion with alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YUV422ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YUV422ToRGB.nppiYUV422ToRGB_8u_P3AC4R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV422ToRGB_8u_P3AC4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 4 channel 8-bit unsigned packed RGB color conversion with alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        public static void YUV420ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToRGB.nppiYUV420ToRGB_8u_P3AC4R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToRGB_8u_P3AC4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCrCb420 to 4 channel 8-bit unsigned packed RGB color conversion with constant alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        /// <param name="nAval">8-bit unsigned alpha constant.</param> 
        public static void YCrCb420ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest, byte nAval)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCrCbToRGB.nppiYCrCb420ToRGB_8u_P3C4R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi, nAval);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCrCb420ToRGB_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 4 channel 8-bit unsigned packed BGR color conversion with constant alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        /// <param name="nAval">8-bit unsigned alpha constant.</param> 
        public static void YCbCr420ToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest, byte nAval)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr420ToBGR_8u_P3C4R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi, nAval);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToBGR_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 4 channel 8-bit unsigned packed BGR_709HDTV color conversion with constant alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        /// <param name="nAval">8-bit unsigned alpha constant.</param> 
        public static void YCbCr420ToBGR_709HDTV(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest, byte nAval)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr420ToBGR_709HDTV_8u_P3C4R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi, nAval);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToBGR_709HDTV_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 4 channel 8-bit unsigned packed BGR color conversion with constant alpha.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>     
        /// <param name="nAval">8-bit unsigned alpha constant.</param> 
        public static void YCbCr411ToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC4 dest, byte nAval)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr411ToBGR_8u_P3C4R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, src0.SizeRoi, nAval);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToBGR_8u_P3C4R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV422 to 3 channel 8-bit unsigned planar RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>   
        public static void YUV422ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YUV422ToRGB.nppiYUV422ToRGB_8u_P3R(arraySrc, arrayPitchSrc, arrayDest, dest0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV422ToRGB_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 3 channel 8-bit unsigned planar RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>   
        public static void YUV420ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToRGB.nppiYUV420ToRGB_8u_P3R(arraySrc, arrayPitchSrc, arrayDest, dest0.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToRGB_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCrCb420 to 3 channel 8-bit unsigned planar YCbCr422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>   
        public static void YCrCb420ToYCbCr422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitchDest = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCrCb420ToYCbCr422_8u_P3R(arraySrc, arrayPitchSrc, arrayDest, arrayPitchDest, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCrCb420ToYCbCr422_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 3 channel 8-bit unsigned planar YCbCr422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>   
        public static void YCbCr411ToYCbCr422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitchDest = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr411ToYCbCr422_8u_P3R(arraySrc, arrayPitchSrc, arrayDest, arrayPitchDest, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToYCbCr422_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 3 channel 8-bit unsigned planar YCrCb422 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>   
        public static void YCbCr411ToYCrCb422(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitchDest = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr411ToYCrCb422_8u_P3R(arraySrc, arrayPitchSrc, arrayDest, arrayPitchDest, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToYCrCb422_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 3 channel 8-bit unsigned planar YCbCr420 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param>  
        /// <param name="dest2">Destination image channel 2</param>   
        public static void YCbCr411ToYCbCr420(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitchDest = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr411ToYCbCr420_8u_P3R(arraySrc, arrayPitchSrc, arrayDest, arrayPitchDest, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToYCbCr420_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 2 channel 8-bit unsigned planar YCbCr420 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="destY">Destination image channel Y</param>  
        /// <param name="destCbCr">Destination image channel CbCr</param> 
        public static void YCbCr422ToYCbCr420(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 destY, NPPImage_8uC1 destCbCr)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr422ToYCbCr420_8u_P3P2R(arraySrc, arrayPitchSrc, destY.DevicePointerRoi, destY.Pitch, destCbCr.DevicePointerRoi, destCbCr.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToYCbCr420_8u_P3P2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 2 channel 8-bit unsigned planar YCbCr411 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="destY">Destination image channel Y</param>  
        /// <param name="destCbCr">Destination image channel CbCr</param> 
        public static void YCbCr422ToYCbCr411(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 destY, NPPImage_8uC1 destCbCr)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr422ToYCbCr411_8u_P3P2R(arraySrc, arrayPitchSrc, destY.DevicePointerRoi, destY.Pitch, destCbCr.DevicePointerRoi, destCbCr.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToYCbCr411_8u_P3P2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 2 channel 8-bit unsigned planar YCbCr420 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="destY">Destination image channel Y</param>  
        /// <param name="destCbCr">Destination image channel CbCr</param> 
        public static void YCbCr420(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 destY, NPPImage_8uC1 destCbCr)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr420_8u_P3P2R(arraySrc, arrayPitchSrc, destY.DevicePointerRoi, destY.Pitch, destCbCr.DevicePointerRoi, destCbCr.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420_8u_P3P2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCrCb420 to 2 channel 8-bit unsigned planar YCbCr420 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="destY">Destination image channel Y</param>  
        /// <param name="destCbCr">Destination image channel CbCr</param> 
        public static void YCrCb420ToYCbCr420(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 destY, NPPImage_8uC1 destCbCr)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCrCb420ToYCbCr420_8u_P3P2R(arraySrc, arrayPitchSrc, destY.DevicePointerRoi, destY.Pitch, destCbCr.DevicePointerRoi, destCbCr.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCrCb420ToYCbCr420_8u_P3P2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCrCb420 to 2 channel 8-bit unsigned planar YCbCr411 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="destY">Destination image channel Y</param>  
        /// <param name="destCbCr">Destination image channel CbCr</param> 
        public static void YCrCb420ToYCbCr411(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 destY, NPPImage_8uC1 destCbCr)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCrCb420ToYCbCr411_8u_P3P2R(arraySrc, arrayPitchSrc, destY.DevicePointerRoi, destY.Pitch, destCbCr.DevicePointerRoi, destCbCr.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCrCb420ToYCbCr411_8u_P3P2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 2 channel 8-bit unsigned planar YCbCr411 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="destY">Destination image channel Y</param>  
        /// <param name="destCbCr">Destination image channel CbCr</param> 
        public static void YCbCr411(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 destY, NPPImage_8uC1 destCbCr)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr411_8u_P3P2R(arraySrc, arrayPitchSrc, destY.DevicePointerRoi, destY.Pitch, destCbCr.DevicePointerRoi, destCbCr.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411_8u_P3P2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 2 channel 8-bit unsigned planar YCbCr420 sampling format conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="destY">Destination image channel Y</param>  
        /// <param name="destCbCr">Destination image channel CbCr</param> 
        public static void YCbCr411ToYCbCr420(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 destY, NPPImage_8uC1 destCbCr)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitchSrc = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrAndACrCbAndOther.nppiYCbCr411ToYCbCr420_8u_P3P2R(arraySrc, arrayPitchSrc, destY.DevicePointerRoi, destY.Pitch, destCbCr.DevicePointerRoi, destCbCr.Pitch, src0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToYCbCr420_8u_P3P2R", status));
            NPPException.CheckNppStatus(status, null);
        }





        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YUV color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void RGBToYUV(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYUV.nppiRGBToYUV_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, dest0.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYUV_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel unsigned 8-bit packed YCbCr color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void RGBToYCbCr(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr.nppiRGBToYCbCr_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, dest0.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar HLS color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void BGRToHLS(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.BGRToHLS.nppiBGRToHLS_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, dest0.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToHLS_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed HLS to 3 channel 8-bit unsigned planar BGR color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void HLSToBGR(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.HLSToBGR.nppiHLSToBGR_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, dest0.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHLSToBGR_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YUV422 color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void RGBToYUV422(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYUV422.nppiRGBToYUV422_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYUV422_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YUV420 color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void RGBToYUV420(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYUV420.nppiRGBToYUV420_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYUV420_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void RGBToYCbCr422(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr.nppiRGBToYCbCr422_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr422_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void BGRToYCbCr422(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYCbCr.nppiBGRToYCbCr422_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr422_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr420 color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void BGRToYCbCr420(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYCbCr.nppiBGRToYCbCr420_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr420_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr420_709CSC color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void BGRToYCbCr420_709CSC(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYCbCr.nppiBGRToYCbCr420_709CSC_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr420_709CSC_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCrCb420_709CSC color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void BGRToYCrCb420_709CSC(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYCrCb.nppiBGRToYCrCb420_709CSC_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCrCb420_709CSC_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCrCb420 color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void BGRToYCrCb420(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYCrCb.nppiBGRToYCrCb420_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCrCb420_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr411 color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>  
        /// <param name="dest1">Destination image channel 1</param> 
        /// <param name="dest2">Destination image channel 2</param>  
        public void BGRToYCbCr411(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYCbCr.nppiBGRToYCbCr411_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, arrayPitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr411_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 2 channel 8-bit unsigned packed YUV422 color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToYUV422(NPPImage_8uC2 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToYUV422.nppiRGBToYUV422_8u_C3C2R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYUV422_8u_C3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 2 channel 8-bit unsigned packed YCrCb422 color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToYCrCb422(NPPImage_8uC2 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCrCb.nppiRGBToYCrCb422_8u_C3C2R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCrCb422_8u_C3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 2 channel 8-bit unsigned packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void BGRToYCbCr422(NPPImage_8uC2 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.BGRToYCbCr.nppiBGRToYCbCr422_8u_C3C2R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr422_8u_C3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 2 channel 8-bit unsigned packed CbYCr422 color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToCbYCr422(NPPImage_8uC2 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToCbYCr.nppiRGBToCbYCr422_8u_C3C2R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToCbYCr422_8u_C3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB first gets forward gamma corrected then converted to 2 channel 8-bit unsigned packed CbYCr422 color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToCbYCr422Gamma(NPPImage_8uC2 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToCbYCr.nppiRGBToCbYCr422Gamma_8u_C3C2R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToCbYCr422Gamma_8u_C3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 2 channel 8-bit unsigned packed CbYCr422_709HDTV color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void BGRToCbYCr422_709HDTV(NPPImage_8uC2 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.BGRToCbYCr.nppiBGRToCbYCr422_709HDTV_8u_C3C2R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToCbYCr422_709HDTV_8u_C3C2R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned packed YUV color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToYUV(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToYUV.nppiRGBToYUV_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYUV_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YUV to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void YUVToRGB(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.YUVToRGB.nppiYUVToRGB_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToRGB_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned packed XYZ color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToXYZ(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToXYZ.nppiRGBToXYZ_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToXYZ_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed XYZ to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void XYZToRGB(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.XYZToRGB.nppiXYZToRGB_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiXYZToRGB_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned packed LUV color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToLUV(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToLUV.nppiRGBToLUV_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToLUV_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed LUV to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void LUVToRGB(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.LUVToRGB.nppiLUVToRGB_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUVToRGB_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned packed Lab color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void BGRToLab(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.BGRToLab.nppiBGRToLab_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToLab_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed Lab to 3 channel 8-bit unsigned packed BGR color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void LabToBGR(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.LabToBGR.nppiLabToBGR_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLabToBGR_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned packed YCC color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToYCC(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCC.nppiRGBToYCC_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCC_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YCC to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void YCCToRGB(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.YCCToRGB.nppiYCCToRGB_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCCToRGB_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned packed HLS color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToHLS(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToHLS.nppiRGBToHLS_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToHLS_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed HLS to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void HLSToRGB(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.HLSToRGB.nppiHLSToRGB_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHLSToRGB_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned packed HSV color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void RGBToHSV(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.RGBToHSV.nppiRGBToHSV_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToHSV_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed HSV to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>  
        public void HSVToRGB(NPPImage_8uC3 dest)
        {
            NppStatus status = NPPNativeMethods.NPPi.HSVToRGB.nppiHSVToRGB_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHSVToRGB_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        #endregion

        #region Histogram
        /// <summary>
        /// Scratch-buffer size for HistogramEven.
        /// </summary>
        /// <param name="nLevels"></param>
        /// <returns></returns>
        public SizeT HistogramEvenGetBufferSize(int[] nLevels)
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramEvenGetBufferSize_8u_C3R(_sizeRoi, nLevels, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramEvenGetBufferSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Compute levels with even distribution.
        /// </summary>
        /// <param name="nLevels">The number of levels being computed. nLevels must be at least 2, otherwise an NPP_-
        /// HISTO_NUMBER_OF_LEVELS_ERROR error is returned.</param>
        /// <param name="nLowerBound">Lower boundary value of the lowest level.</param>
        /// <param name="nUpperBound">Upper boundary value of the greatest level.</param>
        /// <returns>An array of size nLevels which receives the levels being computed.</returns>
        public int[] EvenLevels(int nLevels, int nLowerBound, int nUpperBound)
        {
            int[] Levels = new int[nLevels];
            status = NPPNativeMethods.NPPi.Histogram.nppiEvenLevelsHost_32s(Levels, nLevels, nLowerBound, nUpperBound);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiEvenLevelsHost_32s", status));
            NPPException.CheckNppStatus(status, this);
            return Levels;
        }

        /// <summary>
        /// Histogram with evenly distributed bins. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="histogram">Allocated device memory of size nLevels (3 Variables)</param>
        /// <param name="nLowerLevel">Lower boundary of lowest level bin. E.g. 0 for [0..255]. Size = 3</param>
        /// <param name="nUpperLevel">Upper boundary of highest level bin. E.g. 256 for [0..255]. Size = 3</param>
        public void HistogramEven(CudaDeviceVariable<int>[] histogram, int[] nLowerLevel, int[] nUpperLevel)
        {
            int[] size = new int[] { (int)histogram[0].Size + 1, (int)histogram[1].Size + 1, (int)histogram[2].Size + 1 };
            CUdeviceptr[] devPtrs = new CUdeviceptr[] { histogram[0].DevicePointer, histogram[1].DevicePointer, histogram[2].DevicePointer };


            SizeT bufferSize = HistogramEvenGetBufferSize(size);
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramEven_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, devPtrs, size, nLowerLevel, nUpperLevel, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramEven_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Histogram with evenly distributed bins. No additional buffer is allocated.
        /// </summary>
        /// <param name="histogram">Allocated device memory of size nLevels (3 Variables)</param>
        /// <param name="nLowerLevel">Lower boundary of lowest level bin. E.g. 0 for [0..255]. Size = 3</param>
        /// <param name="nUpperLevel">Upper boundary of highest level bin. E.g. 256 for [0..255]. Size = 3</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="HistogramEvenGetBufferSize(int[])"/></param>
        public void HistogramEven(CudaDeviceVariable<int>[] histogram, int[] nLowerLevel, int[] nUpperLevel, CudaDeviceVariable<byte> buffer)
        {
            int[] size = new int[] { (int)histogram[0].Size + 1, (int)histogram[1].Size + 1, (int)histogram[2].Size + 1 };
            CUdeviceptr[] devPtrs = new CUdeviceptr[] { histogram[0].DevicePointer, histogram[1].DevicePointer, histogram[2].DevicePointer };

            SizeT bufferSize = HistogramEvenGetBufferSize(size);
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramEven_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, devPtrs, size, nLowerLevel, nUpperLevel, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramEven_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Scratch-buffer size for HistogramRange.
        /// </summary>
        /// <param name="nLevels"></param>
        /// <returns></returns>
        public SizeT HistogramRangeGetBufferSize(int[] nLevels)
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramRangeGetBufferSize_8u_C3R(_sizeRoi, nLevels, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramRangeGetBufferSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Histogram with bins determined by pLevels array. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="histogram">array that receives the computed histogram. The CudaDeviceVariable must be of size nLevels-1. Array size = 3</param>
        /// <param name="pLevels">Array in device memory containing the level sizes of the bins. The CudaDeviceVariable must be of size nLevels. Array size = 3</param>
        public void HistogramRange(CudaDeviceVariable<int>[] histogram, CudaDeviceVariable<int>[] pLevels)
        {
            int[] size = new int[] { (int)histogram[0].Size, (int)histogram[1].Size, (int)histogram[2].Size };
            CUdeviceptr[] devPtrs = new CUdeviceptr[] { histogram[0].DevicePointer, histogram[1].DevicePointer, histogram[2].DevicePointer };
            CUdeviceptr[] devLevels = new CUdeviceptr[] { pLevels[0].DevicePointer, pLevels[1].DevicePointer, pLevels[2].DevicePointer };

            SizeT bufferSize = HistogramRangeGetBufferSize(size);
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramRange_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, devPtrs, devLevels, size, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramRange_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Histogram with bins determined by pLevels array. No additional buffer is allocated.
        /// </summary>
        /// <param name="histogram">array that receives the computed histogram. The CudaDeviceVariable must be of size nLevels-1. Array size = 3</param>
        /// <param name="pLevels">Array in device memory containing the level sizes of the bins. The CudaDeviceVariable must be of size nLevels. Array size = 3</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="HistogramRangeGetBufferSize(int[])"/></param>
        public void HistogramRange(CudaDeviceVariable<int>[] histogram, CudaDeviceVariable<int>[] pLevels, CudaDeviceVariable<byte> buffer)
        {
            int[] size = new int[] { (int)histogram[0].Size, (int)histogram[1].Size, (int)histogram[2].Size };
            CUdeviceptr[] devPtrs = new CUdeviceptr[] { histogram[0].DevicePointer, histogram[1].DevicePointer, histogram[2].DevicePointer };
            CUdeviceptr[] devLevels = new CUdeviceptr[] { pLevels[0].DevicePointer, pLevels[1].DevicePointer, pLevels[2].DevicePointer };

            SizeT bufferSize = HistogramRangeGetBufferSize(size);
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramRange_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, devPtrs, devLevels, size, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramRange_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        //new in Cuda 5.5
        #region DotProduct
        /// <summary>
        /// Device scratch buffer size (in bytes) for nppiDotProd_8u64f_C3R.
        /// </summary>
        /// <returns></returns>
        public SizeT DotProdGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.DotProd.nppiDotProdGetBufferHostSize_8u64f_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDotProdGetBufferHostSize_8u64f_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Three-channel 8-bit unsigned image DotProd.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pDp">Pointer to the computed dot product of the two images. (3 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="DotProdGetBufferHostSize()"/></param>
        public void DotProduct(NPPImage_8uC3 src2, CudaDeviceVariable<double> pDp, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = DotProdGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.DotProd.nppiDotProd_8u64f_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pDp.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDotProd_8u64f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Three-channel 8-bit unsigned image DotProd. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pDp">Pointer to the computed dot product of the two images. (3 * sizeof(double))</param>
        public void DotProduct(NPPImage_8uC3 src2, CudaDeviceVariable<double> pDp)
        {
            SizeT bufferSize = DotProdGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.DotProd.nppiDotProd_8u64f_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pDp.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDotProd_8u64f_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        #endregion

        #region LUT
        /// <summary>
        /// Three channel 8-bit unsigned source bit range restricted palette look-up-table color conversion to four channel 8-bit unsigned destination output with alpha.
        /// The LUT is derived from a set of user defined mapping points in a palette and 
        /// source pixels are then processed using a restricted bit range when looking up palette values.
        /// This function also reverses the source pixel channel order in the destination so the Alpha channel is the first channel.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="nAlphaValue">Signed alpha value that will be used to initialize the pixel alpha channel position in all modified destination pixels.</param>
        /// <param name="pTables0">Host pointer to an array of 3 device memory pointers, channel 0, pointing to user defined OUTPUT palette values.
        /// <para/>Alpha values &lt; 0 or &gt; 255 will cause destination pixel alpha channel values to be unmodified.</param>
        /// <param name="pTables1">Host pointer to an array of 3 device memory pointers, channel 1, pointing to user defined OUTPUT palette values.
        /// <para/>Alpha values &lt; 0 or &gt; 255 will cause destination pixel alpha channel values to be unmodified.</param>
        /// <param name="pTables2">Host pointer to an array of 3 device memory pointers, channel 2, pointing to user defined OUTPUT palette values.
        /// <para/>Alpha values &lt; 0 or &gt; 255 will cause destination pixel alpha channel values to be unmodified.</param>
        /// <param name="nBitSize">Number of least significant bits (must be &gt; 0 and &lt;= 8) of each source pixel value to use as index into palette table during conversion.</param>
        public void LUTPaletteSwap(NPPImage_8uC4 dst, int nAlphaValue, CudaDeviceVariable<byte> pTables0, CudaDeviceVariable<byte> pTables1, CudaDeviceVariable<byte> pTables2, int nBitSize)
        {
            CUdeviceptr[] ptrs = new CUdeviceptr[] { pTables0.DevicePointer, pTables1.DevicePointer, pTables2.DevicePointer };
            status = NPPNativeMethods.NPPi.ColorLUTPalette.nppiLUTPaletteSwap_8u_C3A0C4R(_devPtrRoi, _pitch, nAlphaValue, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, ptrs, nBitSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUTPaletteSwap_8u_C3A0C4R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points with no interpolation.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="pValues">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined OUTPUT values.</param>
        /// <param name="pLevels">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUT(NPPImage_8uC3 dst, CudaDeviceVariable<int>[] pValues, CudaDeviceVariable<int>[] pLevels)
        {
            CUdeviceptr[] ptrsV = new CUdeviceptr[] { pValues[0].DevicePointer, pValues[1].DevicePointer, pValues[2].DevicePointer };
            CUdeviceptr[] ptrsL = new CUdeviceptr[] { pLevels[0].DevicePointer, pLevels[1].DevicePointer, pLevels[2].DevicePointer };
            int[] size = new int[] { (int)pLevels[0].Size, (int)pLevels[1].Size, (int)pLevels[2].Size };
            status = NPPNativeMethods.NPPi.ColorLUT.nppiLUT_8u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, ptrsV, ptrsL, size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// cubic interpolated look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points through cubic interpolation. 
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="pValues">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined OUTPUT values.</param>
        /// <param name="pLevels">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUTCubic(NPPImage_8uC3 dst, CudaDeviceVariable<int>[] pValues, CudaDeviceVariable<int>[] pLevels)
        {
            CUdeviceptr[] ptrsV = new CUdeviceptr[] { pValues[0].DevicePointer, pValues[1].DevicePointer, pValues[2].DevicePointer };
            CUdeviceptr[] ptrsL = new CUdeviceptr[] { pLevels[0].DevicePointer, pLevels[1].DevicePointer, pLevels[2].DevicePointer };
            int[] size = new int[] { (int)pLevels[0].Size, (int)pLevels[1].Size, (int)pLevels[2].Size };
            status = NPPNativeMethods.NPPi.ColorLUTCubic.nppiLUT_Cubic_8u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, ptrsV, ptrsL, size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_Cubic_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// range restricted palette look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points in a palette and 
        /// source pixels are then processed using a restricted bit range when looking up palette values.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="pTable">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined OUTPUT palette values.</param>
        /// <param name="nBitSize">Number of least significant bits (must be &gt; 0 and &lt;= 8) of each source pixel value to use as index into palette table during conversion.</param>
        public void LUTPalette(NPPImage_8uC3 dst, CudaDeviceVariable<byte>[] pTable, int nBitSize)
        {
            CUdeviceptr[] ptrsT = new CUdeviceptr[] { pTable[0].DevicePointer, pTable[1].DevicePointer, pTable[2].DevicePointer };
            status = NPPNativeMethods.NPPi.ColorLUTPalette.nppiLUTPalette_8u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, ptrsT, nBitSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUTPalette_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points with no interpolation.
        /// </summary>
        /// <param name="pValues">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined OUTPUT values.</param>
        /// <param name="pLevels">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUT(CudaDeviceVariable<int>[] pValues, CudaDeviceVariable<int>[] pLevels)
        {
            CUdeviceptr[] ptrsV = new CUdeviceptr[] { pValues[0].DevicePointer, pValues[1].DevicePointer, pValues[2].DevicePointer };
            CUdeviceptr[] ptrsL = new CUdeviceptr[] { pLevels[0].DevicePointer, pLevels[1].DevicePointer, pLevels[2].DevicePointer };
            int[] size = new int[] { (int)pLevels[0].Size, (int)pLevels[1].Size, (int)pLevels[2].Size };
            status = NPPNativeMethods.NPPi.ColorLUT.nppiLUT_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, ptrsV, ptrsL, size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Inplace cubic interpolated look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points through cubic interpolation. 
        /// </summary>
        /// <param name="pValues">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined OUTPUT values.</param>
        /// <param name="pLevels">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUTCubic(CudaDeviceVariable<int>[] pValues, CudaDeviceVariable<int>[] pLevels)
        {
            CUdeviceptr[] ptrsV = new CUdeviceptr[] { pValues[0].DevicePointer, pValues[1].DevicePointer, pValues[2].DevicePointer };
            CUdeviceptr[] ptrsL = new CUdeviceptr[] { pLevels[0].DevicePointer, pLevels[1].DevicePointer, pLevels[2].DevicePointer };
            int[] size = new int[] { (int)pLevels[0].Size, (int)pLevels[1].Size, (int)pLevels[2].Size };
            status = NPPNativeMethods.NPPi.ColorLUTCubic.nppiLUT_Cubic_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, ptrsV, ptrsL, size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_Cubic_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Inplace linear interpolated look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points through cubic interpolation. 
        /// </summary>
        /// <param name="pValues">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined OUTPUT values.</param>
        /// <param name="pLevels">Host pointer to an array of 3 device memory pointers, one per color CHANNEL, pointing to user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUTLinear(CudaDeviceVariable<int>[] pValues, CudaDeviceVariable<int>[] pLevels)
        {
            CUdeviceptr[] ptrsV = new CUdeviceptr[] { pValues[0].DevicePointer, pValues[1].DevicePointer, pValues[2].DevicePointer };
            CUdeviceptr[] ptrsL = new CUdeviceptr[] { pLevels[0].DevicePointer, pLevels[1].DevicePointer, pLevels[2].DevicePointer };
            int[] size = new int[] { (int)pLevels[0].Size, (int)pLevels[1].Size, (int)pLevels[2].Size };
            status = NPPNativeMethods.NPPi.ColorLUTLinear.nppiLUT_Linear_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, ptrsV, ptrsL, size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_Linear_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        #endregion

        #region Transpose
        /// <summary>
        /// image transpose
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Transpose(NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.Transpose.nppiTranspose_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiTranspose_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Color...New

        /// <summary>
        /// Swap color channels
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="aDstOrder">Integer array describing how channel values are permutated. <para/>The n-th entry of the array
        /// contains the number of the channel that is stored in the n-th channel of the output image. <para/>E.g.
        /// Given an RGB image, aDstOrder = [2,1,0] converts this to BGR channel order.</param>
        public void SwapChannels(NPPImage_8uC3 dest, int[] aDstOrder)
        {
            status = NPPNativeMethods.NPPi.SwapChannel.nppiSwapChannels_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, aDstOrder);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSwapChannels_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Swap color channels
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="aDstOrder">Integer array describing how channel values are permutated. <para/>The n-th entry of the array
        /// contains the number of the channel that is stored in the n-th channel of the output image. <para/>E.g.
        /// Given an RGB image, aDstOrder = [3,2,1,0] converts this to VBGR channel order.</param>
        /// <param name="nValue">(V) Single channel constant value that can be replicated in one or more of the 4 destination channels.<para/>
        /// nValue is either written or not written to a particular channel depending on the aDstOrder entry for that destination
        /// channel. <para/>An aDstOrder value of 3 will output nValue to that channel, an aDstOrder value greater than 3 will leave that
        /// particular destination channel value unmodified.</param>
        public void SwapChannels(NPPImage_8uC4 dest, int[] aDstOrder, byte nValue)
        {
            status = NPPNativeMethods.NPPi.SwapChannel.nppiSwapChannels_8u_C3C4R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, aDstOrder, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSwapChannels_8u_C3C4R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Swap color channels inplace
        /// </summary>
        /// <param name="aDstOrder">Integer array describing how channel values are permutated. <para/>The n-th entry of the array
        /// contains the number of the channel that is stored in the n-th channel of the output image. <para/>E.g.
        /// Given an RGB image, aDstOrder = [2,1,0] converts this to BGR channel order.</param>
        public void SwapChannels(int[] aDstOrder)
        {
            status = NPPNativeMethods.NPPi.SwapChannel.nppiSwapChannels_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, aDstOrder);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSwapChannels_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// RGB to Gray conversion
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void RGBToGray(NPPImage_8uC1 dest)
        {
            status = NPPNativeMethods.NPPi.RGBToGray.nppiRGBToGray_8u_C3C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToGray_8u_C3C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Color to Gray conversion
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="aCoeffs">fixed size array of constant floating point conversion coefficient values, one per color channel.</param>
        public void ColorToGray(NPPImage_8uC1 dest, float[] aCoeffs)
        {
            status = NPPNativeMethods.NPPi.ColorToGray.nppiColorToGray_8u_C3C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, aCoeffs);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorToGray_8u_C3C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// in place color twist.
        /// 
        /// An input color twist matrix with floating-point coefficient values is applied
        /// within ROI.
        /// </summary>
        /// <param name="aTwist">The color twist matrix with floating-point coefficient values. [3,4]</param>
        public void ColorTwist(float[,] aTwist)
        {
            status = NPPNativeMethods.NPPi.ColorTwist.nppiColorTwist32f_8u_C3IR(_devPtrRoi, _pitch, _sizeRoi, aTwist);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwist32f_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Set
        /// <summary>
        /// Set pixel values to nValue.
        /// </summary>
        /// <param name="nValue">Value to be set (Array size = 3)</param>
        public void Set(byte[] nValue)
        {
            status = NPPNativeMethods.NPPi.MemSet.nppiSet_8u_C3R(nValue, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSet_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Set pixel values to nValue. <para/>
        /// The 8-bit mask image affects setting of the respective pixels in the destination image. <para/>
        /// If the mask value is zero (0) the pixel is not set, if the mask is non-zero, the corresponding
        /// destination pixel is set to specified value.
        /// </summary>
        /// <param name="nValue">Value to be set (Array size = 3)</param>
        /// <param name="mask">Mask image</param>
        public void Set(byte[] nValue, NPPImage_8uC1 mask)
        {
            status = NPPNativeMethods.NPPi.MemSet.nppiSet_8u_C3MR(nValue, _devPtrRoi, _pitch, _sizeRoi, mask.DevicePointerRoi, mask.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSet_8u_C3MR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Set pixel values to nValue. <para/>
        /// The 8-bit mask image affects setting of the respective pixels in the destination image. <para/>
        /// If the mask value is zero (0) the pixel is not set, if the mask is non-zero, the corresponding
        /// destination pixel is set to specified value.
        /// </summary>
        /// <param name="nValue">Value to be set</param>
        /// <param name="channel">Channel number. This number is added to the dst pointer</param>
        public void Set(byte nValue, int channel)
        {
            if (channel < 0 | channel >= _channels) throw new ArgumentOutOfRangeException("channel", "channel must be in range [0..2].");
            status = NPPNativeMethods.NPPi.MemSet.nppiSet_8u_C3CR(nValue, _devPtrRoi + channel * _typeSize, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSet_8u_C3CR", status));
            NPPException.CheckNppStatus(status, this);
        }

        #endregion

        #region Copy

        /// <summary>
        /// image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        public void Copy(NPPImage_8uC3 dst)
        {
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_8u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Copy image and pad borders with a constant, user-specifiable color.
        /// </summary>
        /// <param name="dst">Destination image. The image ROI defines the destination region, i.e. the region that gets filled with data from
        /// the source image (inner part) and constant border color (outer part).</param>
        /// <param name="nTopBorderHeight">Height (in pixels) of the top border. The height of the border at the bottom of
        /// the destination ROI is implicitly defined by the size of the source ROI: nBottomBorderHeight =
        /// oDstSizeROI.height - nTopBorderHeight - oSrcSizeROI.height.</param>
        /// <param name="nLeftBorderWidth">Width (in pixels) of the left border. The width of the border at the right side of
        /// the destination ROI is implicitly defined by the size of the source ROI: nRightBorderWidth =
        /// oDstSizeROI.width - nLeftBorderWidth - oSrcSizeROI.width.</param>
        /// <param name="nValue">The pixel value to be set for border pixels.</param>
        public void Copy(NPPImage_8uC3 dst, int nTopBorderHeight, int nLeftBorderWidth, byte[] nValue)
        {
            status = NPPNativeMethods.NPPi.CopyConstBorder.nppiCopyConstBorder_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, dst.DevicePointerRoi, dst.Pitch, dst.SizeRoi, nTopBorderHeight, nLeftBorderWidth, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopyConstBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }



        /// <summary>
        /// image copy with nearest source image pixel color.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nTopBorderHeight">Height (in pixels) of the top border. The height of the border at the bottom of
        /// the destination ROI is implicitly defined by the size of the source ROI: nBottomBorderHeight =
        /// oDstSizeROI.height - nTopBorderHeight - oSrcSizeROI.height.</param>
        /// <param name="nLeftBorderWidth">Width (in pixels) of the left border. The width of the border at the right side of
        /// the destination ROI is implicitly defined by the size of the source ROI: nRightBorderWidth =
        /// oDstSizeROI.width - nLeftBorderWidth - oSrcSizeROI.width.</param>
        public void CopyReplicateBorder(NPPImage_8uC3 dst, int nTopBorderHeight, int nLeftBorderWidth)
        {
            status = NPPNativeMethods.NPPi.CopyReplicateBorder.nppiCopyReplicateBorder_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, dst.DevicePointerRoi, dst.Pitch, dst.SizeRoi, nTopBorderHeight, nLeftBorderWidth);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopyReplicateBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image copy with the borders wrapped by replication of source image pixel colors.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nTopBorderHeight">Height (in pixels) of the top border. The height of the border at the bottom of
        /// the destination ROI is implicitly defined by the size of the source ROI: nBottomBorderHeight =
        /// oDstSizeROI.height - nTopBorderHeight - oSrcSizeROI.height.</param>
        /// <param name="nLeftBorderWidth">Width (in pixels) of the left border. The width of the border at the right side of
        /// the destination ROI is implicitly defined by the size of the source ROI: nRightBorderWidth =
        /// oDstSizeROI.width - nLeftBorderWidth - oSrcSizeROI.width.</param>
        public void CopyWrapBorder(NPPImage_8uC3 dst, int nTopBorderHeight, int nLeftBorderWidth)
        {
            status = NPPNativeMethods.NPPi.CopyWrapBorder.nppiCopyWrapBorder_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, dst.DevicePointerRoi, dst.Pitch, dst.SizeRoi, nTopBorderHeight, nLeftBorderWidth);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopyWrapBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// linearly interpolated source image subpixel coordinate color copy.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nDx">Fractional part of source image X coordinate.</param>
        /// <param name="nDy">Fractional part of source image Y coordinate.</param>
        public void CopySubpix(NPPImage_8uC3 dst, float nDx, float nDy)
        {
            status = NPPNativeMethods.NPPi.CopySubpix.nppiCopySubpix_8u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, nDx, nDy);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopySubpix_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region MorphologicalNew
        /// <summary>
        /// Dilation computes the output pixel as the maximum pixel value of the pixels under the mask. Pixels who’s
        /// corresponding mask values are zero to not participate in the maximum search.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="Mask">Pointer to the start address of the mask array.</param>
        /// <param name="aMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        public void Dilate(NPPImage_8uC3 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor)
        {
            status = NPPNativeMethods.NPPi.MorphologyFilter2D.nppiDilate_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, Mask.DevicePointer, aMaskSize, oAnchor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDilate_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Erosion computes the output pixel as the minimum pixel value of the pixels under the mask. Pixels who’s
        /// corresponding mask values are zero to not participate in the maximum search.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="Mask">Pointer to the start address of the mask array.</param>
        /// <param name="aMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        public void Erode(NPPImage_8uC3 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor)
        {
            status = NPPNativeMethods.NPPi.MorphologyFilter2D.nppiErode_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, Mask.DevicePointer, aMaskSize, oAnchor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiErode_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 3x3 dilation.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Dilate3x3(NPPImage_8uC3 dst)
        {
            status = NPPNativeMethods.NPPi.MorphologyFilter2D.nppiDilate3x3_8u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDilate3x3_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3x3 erosion.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Erode3x3(NPPImage_8uC3 dst)
        {
            status = NPPNativeMethods.NPPi.MorphologyFilter2D.nppiErode3x3_8u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiErode3x3_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Dilation computes the output pixel as the maximum pixel value of the pixels under the mask. Pixels who’s
        /// corresponding mask values are zero to not participate in the maximum search. With border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="Mask">Pointer to the start address of the mask array.</param>
        /// <param name="aMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void DilateBorder(NPPImage_8uC3 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.DilationWithBorderControl.nppiDilateBorder_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, Mask.DevicePointer, aMaskSize, oAnchor, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDilateBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Erosion computes the output pixel as the minimum pixel value of the pixels under the mask. Pixels who’s
        /// corresponding mask values are zero to not participate in the maximum search. With border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="Mask">Pointer to the start address of the mask array.</param>
        /// <param name="aMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void ErodeBorder(NPPImage_8uC3 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ErosionWithBorderControl.nppiErodeBorder_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, Mask.DevicePointer, aMaskSize, oAnchor, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiErodeBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3x3 dilation with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void Dilate3x3Border(NPPImage_8uC3 dest, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.Dilate3x3Border.nppiDilate3x3Border_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDilate3x3Border_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3x3 erosion with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void Erode3x3Border(NPPImage_8uC3 dest, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.Erode3x3Border.nppiErode3x3Border_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiErode3x3Border_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region NormNew
        /// <summary>
        /// Device scratch buffer size (in bytes) for NormDiff_Inf.
        /// </summary>
        /// <returns></returns>
        public SizeT NormDiffInfGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffInfGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffInfGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_Inf.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed Inf-norm of differences. (3 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffInfGetBufferHostSize()"/></param>
        public void NormDiff_Inf(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffInfGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_Inf_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_Inf_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_Inf. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed Inf-norm of differences. (3 * sizeof(double))</param>
        public void NormDiff_Inf(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff)
        {
            SizeT bufferSize = NormDiffInfGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_Inf_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_Inf_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Device scratch buffer size (in bytes) for NormDiff_L1.
        /// </summary>
        /// <returns></returns>
        public SizeT NormDiffL1GetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffL1GetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffL1GetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_L1.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L1-norm of differences. (3 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffL1GetBufferHostSize()"/></param>
        public void NormDiff_L1(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffL1GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L1_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L1_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_L1. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L1-norm of differences. (3 * sizeof(double))</param>
        public void NormDiff_L1(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff)
        {
            SizeT bufferSize = NormDiffL1GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L1_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L1_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Device scratch buffer size (in bytes) for NormDiff_L2.
        /// </summary>
        /// <returns></returns>
        public SizeT NormDiffL2GetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffL2GetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffL2GetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_L2.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L2-norm of differences. (3 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffL2GetBufferHostSize()"/></param>
        public void NormDiff_L2(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffL2GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L2_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L2_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_L2. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L2-norm of differences. (3 * sizeof(double))</param>
        public void NormDiff_L2(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff)
        {
            SizeT bufferSize = NormDiffL2GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L2_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L2_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }



        /// <summary>
        /// Device scratch buffer size (in bytes) for NormRel_Inf.
        /// </summary>
        /// <returns></returns>
        public SizeT NormRelInfGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelInfGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelInfGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_Inf.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (3 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelInfGetBufferHostSize()"/></param>
        public void NormRel_Inf(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelInfGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_Inf_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_Inf_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_Inf. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (3 * sizeof(double))</param>
        public void NormRel_Inf(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel)
        {
            SizeT bufferSize = NormRelInfGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_Inf_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_Inf_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Device scratch buffer size (in bytes) for NormRel_L1.
        /// </summary>
        /// <returns></returns>
        public SizeT NormRelL1GetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelL1GetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelL1GetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_L1.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (3 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelL1GetBufferHostSize()"/></param>
        public void NormRel_L1(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelL1GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L1_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L1_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_L1. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (3 * sizeof(double))</param>
        public void NormRel_L1(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel)
        {
            SizeT bufferSize = NormRelL1GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L1_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L1_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Device scratch buffer size (in bytes) for NormRel_L2.
        /// </summary>
        /// <returns></returns>
        public SizeT NormRelL2GetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelL2GetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelL2GetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_L2.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (3 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelL2GetBufferHostSize()"/></param>
        public void NormRel_L2(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelL2GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L2_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L2_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_L2. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (3 * sizeof(double))</param>
        public void NormRel_L2(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel)
        {
            SizeT bufferSize = NormRelL2GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L2_8u_C3R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L2_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }





        /// <summary>
        /// Device scratch buffer size (in bytes) for CrossCorrFull_NormLevel.
        /// </summary>
        /// <returns></returns>
        public SizeT FullNormLevelGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiFullNormLevelGetBufferHostSize_8u32f_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFullNormLevelGetBufferHostSize_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrFull_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="FullNormLevelGetBufferHostSize()"/></param>
        public void CrossCorrFull_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = FullNormLevelGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevel_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevel_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrFull_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        public void CrossCorrFull_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            SizeT bufferSize = FullNormLevelGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevel_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevel_8u32f_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }



        /// <summary>
        /// Device scratch buffer size (in bytes) for CrossCorrSame_NormLevel.
        /// </summary>
        /// <returns></returns>
        public SizeT SameNormLevelGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSameNormLevelGetBufferHostSize_8u32f_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSameNormLevelGetBufferHostSize_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrSame_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="SameNormLevelGetBufferHostSize()"/></param>
        public void CrossCorrSame_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = SameNormLevelGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevel_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevel_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrSame_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        public void CrossCorrSame_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            SizeT bufferSize = SameNormLevelGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevel_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevel_8u32f_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// Device scratch buffer size (in bytes) for CrossCorrValid_NormLevel.
        /// </summary>
        /// <returns></returns>
        public SizeT ValidNormLevelGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiValidNormLevelGetBufferHostSize_8u32f_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiValidNormLevelGetBufferHostSize_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrValid_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="ValidNormLevelGetBufferHostSize()"/></param>
        public void CrossCorrValid_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = ValidNormLevelGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevel_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevel_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrValid_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        public void CrossCorrValid_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            SizeT bufferSize = ValidNormLevelGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevel_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevel_8u32f_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }






        /// <summary>
        /// Device scratch buffer size (in bytes) for CrossCorrFull_NormLevel.
        /// </summary>
        /// <returns></returns>
        public SizeT FullNormLevelScaledGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiFullNormLevelGetBufferHostSize_8u_C3RSfs(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFullNormLevelGetBufferHostSize_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrFull_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="FullNormLevelScaledGetBufferHostSize()"/></param>
        public void CrossCorrFull_NormLevel(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = FullNormLevelScaledGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevel_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, nScaleFactor, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevel_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrFull_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void CrossCorrFull_NormLevel(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            SizeT bufferSize = FullNormLevelScaledGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevel_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, nScaleFactor, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevel_8u_C3RSfs", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }



        /// <summary>
        /// Device scratch buffer size (in bytes) for CrossCorrSame_NormLevel.
        /// </summary>
        /// <returns></returns>
        public SizeT SameNormLevelScaledGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSameNormLevelGetBufferHostSize_8u_C3RSfs(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSameNormLevelGetBufferHostSize_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrSame_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="SameNormLevelScaledGetBufferHostSize()"/></param>
        public void CrossCorrSame_NormLevel(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = SameNormLevelScaledGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevel_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, nScaleFactor, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevel_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrSame_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void CrossCorrSame_NormLevel(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            SizeT bufferSize = SameNormLevelScaledGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevel_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, nScaleFactor, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevel_8u_C3RSfs", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// Device scratch buffer size (in bytes) for CrossCorrValid_NormLevel.
        /// </summary>
        /// <returns></returns>
        public SizeT ValidNormLevelScaledGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiValidNormLevelGetBufferHostSize_8u_C3RSfs(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiValidNormLevelGetBufferHostSize_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrValid_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="ValidNormLevelScaledGetBufferHostSize()"/></param>
        public void CrossCorrValid_NormLevel(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = ValidNormLevelScaledGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevel_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, nScaleFactor, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevel_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrValid_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void CrossCorrValid_NormLevel(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            SizeT bufferSize = ValidNormLevelScaledGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevel_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, nScaleFactor, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevel_8u_C3RSfs", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }






        /// <summary>
        /// image SqrDistanceFull_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void SqrDistanceFull_Norm(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceFull_Norm_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceFull_Norm_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image SqrDistanceSame_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void SqrDistanceSame_Norm(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceSame_Norm_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceSame_Norm_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image SqrDistanceValid_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void SqrDistanceValid_Norm(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceValid_Norm_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceValid_Norm_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }



        /// <summary>
        /// image SqrDistanceFull_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void SqrDistanceFull_Norm(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceFull_Norm_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceFull_Norm_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image SqrDistanceSame_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void SqrDistanceSame_Norm(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceSame_Norm_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceSame_Norm_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image SqrDistanceValid_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void SqrDistanceValid_Norm(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceValid_Norm_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceValid_Norm_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// image CrossCorrFull_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void CrossCorrFull_Norm(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_Norm_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_Norm_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image CrossCorrSame_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void CrossCorrSame_Norm(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_Norm_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_Norm_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image CrossCorrValid_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void CrossCorrValid_Norm(NPPImage_8uC3 tpl, NPPImage_32fC3 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_Norm_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_Norm_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// image CrossCorrFull_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void CrossCorrFull_Norm(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_Norm_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_Norm_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image CrossCorrSame_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void CrossCorrSame_Norm(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_Norm_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_Norm_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image CrossCorrValid_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nScaleFactor">Integer Result Scaling.</param>
        public void CrossCorrValid_Norm(NPPImage_8uC3 tpl, NPPImage_8uC3 dst, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_Norm_8u_C3RSfs(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_Norm_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        #endregion

        #region NormMaskedNew
        /// <summary>
        /// Device scratch buffer size (in bytes) for NormDiff_Inf.
        /// </summary>
        /// <returns></returns>
        public SizeT NormDiffInfMaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffInfGetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffInfGetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_Inf.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed Inf-norm of differences. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffInfMaskedGetBufferHostSize()"/></param>
        public void NormDiff_Inf(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, int nCOI, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffInfMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_Inf_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_Inf_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_Inf. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed Inf-norm of differences. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        public void NormDiff_Inf(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, int nCOI, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormDiffInfMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_Inf_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_Inf_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Device scratch buffer size (in bytes) for NormDiff_L1.
        /// </summary>
        /// <returns></returns>
        public SizeT NormDiffL1MaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffL1GetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffL1GetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_L1.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L1-norm of differences. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffL1MaskedGetBufferHostSize()"/></param>
        public void NormDiff_L1(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, int nCOI, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffL1MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L1_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L1_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_L1. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L1-norm of differences. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        public void NormDiff_L1(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, int nCOI, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormDiffL1MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L1_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L1_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Device scratch buffer size (in bytes) for NormDiff_L2.
        /// </summary>
        /// <returns></returns>
        public SizeT NormDiffL2MaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffL2GetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffL2GetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_L2.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L2-norm of differences. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffL2MaskedGetBufferHostSize()"/></param>
        public void NormDiff_L2(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, int nCOI, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffL2MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L2_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L2_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_L2. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L2-norm of differences. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        public void NormDiff_L2(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormDiff, int nCOI, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormDiffL2MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L2_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L2_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }



        /// <summary>
        /// Device scratch buffer size (in bytes) for NormRel_Inf.
        /// </summary>
        /// <returns></returns>
        public SizeT NormRelInfMaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelInfGetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelInfGetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_Inf.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelInfMaskedGetBufferHostSize()"/></param>
        public void NormRel_Inf(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, int nCOI, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelInfMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_Inf_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_Inf_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_Inf. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        public void NormRel_Inf(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, int nCOI, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormRelInfMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_Inf_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_Inf_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Device scratch buffer size (in bytes) for NormRel_L1.
        /// </summary>
        /// <returns></returns>
        public SizeT NormRelL1MaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelL1GetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelL1GetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_L1.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelL1MaskedGetBufferHostSize()"/></param>
        public void NormRel_L1(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, int nCOI, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelL1MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L1_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L1_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_L1. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        public void NormRel_L1(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, int nCOI, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormRelL1MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L1_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L1_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// Device scratch buffer size (in bytes) for NormRel_L2.
        /// </summary>
        /// <returns></returns>
        public SizeT NormRelL2MaskedGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelL2GetBufferHostSize_8u_C3CMR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelL2GetBufferHostSize_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_L2.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelL2MaskedGetBufferHostSize()"/></param>
        public void NormRel_L2(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, int nCOI, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelL2MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L2_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L2_8u_C3CMR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_L2. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="nCOI">channel of interest.</param>
        /// <param name="pMask">Mask image.</param>
        public void NormRel_L2(NPPImage_8uC3 tpl, CudaDeviceVariable<double> pNormRel, int nCOI, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormRelL2MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L2_8u_C3CMR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, nCOI, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L2_8u_C3CMR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }






        #endregion

        #region MinMaxEveryNew
        /// <summary>
        /// image MinEvery
        /// </summary>
        /// <param name="src2">Source-Image</param>
        public void MinEvery(NPPImage_8uC3 src2)
        {
            status = NPPNativeMethods.NPPi.MinMaxEvery.nppiMinEvery_8u_C3IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinEvery_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image MaxEvery
        /// </summary>
        /// <param name="src2">Source-Image</param>
        public void MaxEvery(NPPImage_8uC3 src2)
        {
            status = NPPNativeMethods.NPPi.MinMaxEvery.nppiMaxEvery_8u_C3IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxEvery_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region CountInRange
        /// <summary>
        /// Device scratch buffer size (in bytes) for CountInRange.
        /// </summary>
        /// <returns></returns>
        public SizeT CountInRangeGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.CountInRange.nppiCountInRangeGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCountInRangeGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image CountInRange.
        /// </summary>
        /// <param name="pCounts">Pointer to the number of pixels that fall into the specified range. (3 * sizeof(int))</param>
        /// <param name="nLowerBound">Fixed size array of the lower bound of the specified range, one per channel.</param>
        /// <param name="nUpperBound">Fixed size array of the upper bound of the specified range, one per channel.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="CountInRangeGetBufferHostSize()"/></param>
        public void CountInRange(CudaDeviceVariable<int> pCounts, byte[] nLowerBound, byte[] nUpperBound, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = CountInRangeGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.CountInRange.nppiCountInRange_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, pCounts.DevicePointer, nLowerBound, nUpperBound, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCountInRange_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image CountInRange.
        /// </summary>
        /// <param name="pCounts">Pointer to the number of pixels that fall into the specified range. (3 * sizeof(int))</param>
        /// <param name="nLowerBound">Fixed size array of the lower bound of the specified range, one per channel.</param>
        /// <param name="nUpperBound">Fixed size array of the upper bound of the specified range, one per channel.</param>
        public void CountInRange(CudaDeviceVariable<int> pCounts, byte[] nLowerBound, byte[] nUpperBound)
        {
            SizeT bufferSize = CountInRangeGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.CountInRange.nppiCountInRange_8u_C3R(_devPtrRoi, _pitch, _sizeRoi, pCounts.DevicePointer, nLowerBound, nUpperBound, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCountInRange_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region QualityIndex
        /// <summary>
        /// Device scratch buffer size (in bytes) for QualityIndex.
        /// </summary>
        /// <returns></returns>
        public SizeT QualityIndexGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.QualityIndex.nppiQualityIndexGetBufferHostSize_8u32f_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiQualityIndexGetBufferHostSize_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image QualityIndex.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dst">Pointer to the quality index. (3 * sizeof(float))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="QualityIndexGetBufferHostSize()"/></param>
        public void QualityIndex(NPPImage_8uC3 src2, CudaDeviceVariable<float> dst, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = QualityIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.QualityIndex.nppiQualityIndex_8u32f_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, dst.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiQualityIndex_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image QualityIndex.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dst">Pointer to the quality index. (3 * sizeof(float))</param>
        public void QualityIndex(NPPImage_8uC3 src2, CudaDeviceVariable<float> dst)
        {
            SizeT bufferSize = QualityIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.QualityIndex.nppiQualityIndex_8u32f_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, dst.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiQualityIndex_8u32f_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region GeometryNew

        /// <summary>
        /// image conversion.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Scale(NPPImage_16sC3 dst)
        {
            NppiRect srcRect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.Scale.nppiScale_8u16s_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiScale_8u16s_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// image conversion.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Scale(NPPImage_16uC3 dst)
        {
            NppiRect srcRect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.Scale.nppiScale_8u16u_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiScale_8u16u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image conversion.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nMin">specifies the minimum saturation value to which every output value will be clamped.</param>
        /// <param name="nMax">specifies the maximum saturation value to which every output value will be clamped.</param>
        public void Scale(NPPImage_32fC3 dst, float nMin, float nMax)
        {
            NppiRect srcRect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.Scale.nppiScale_8u32f_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, nMin, nMax);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiScale_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image conversion.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Scale(NPPImage_32sC3 dst)
        {
            NppiRect srcRect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.Scale.nppiScale_8u32s_C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiScale_8u32s_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        #endregion

        #region Color Space conversion new in CUDA 6
        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned packed YUV color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void BGRToYUV(NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.BGRToYUV.nppiBGRToYUV_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYUV_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned planar YUV color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public static void BGRToYUV(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYUV.nppiBGRToYUV_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYUV_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YUV color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public void BGRToYUV(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYUV.nppiBGRToYUV_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, dest0.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYUV_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YUV to 3 channel 8-bit unsigned packed BGR color conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void YUVToBGR(NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.YUVToBGR.nppiYUVToBGR_8u_C3R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToBGR_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV to 3 channel 8-bit unsigned planar BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public static void YUVToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YUVToBGR.nppiYUVToBGR_8u_P3R(arraySrc, src0.Pitch, arrayDest, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToBGR_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV to 3 channel 8-bit unsigned planar BGR color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void YUVToBGR(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.YUVToBGR.nppiYUVToBGR_8u_P3C3R(arraySrc, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToBGR_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }


        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public void BGRToYCbCr(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.BGRToYCbCr.nppiBGRToYCbCr_8u_C3P3R(_devPtrRoi, _pitch, arrayDest, dest0.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        #endregion

        #region MaxError
        /// <summary>
        /// image maximum error. User buffer is internally allocated and freed.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        public void MaxError(NPPImage_8uC3 src2, CudaDeviceVariable<double> pError)
        {
            SizeT bufferSize = MaxErrorGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);
            status = NPPNativeMethods.NPPi.MaximumError.nppiMaximumError_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumError_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image maximum error.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        /// <param name="buffer">Pointer to the user-allocated scratch buffer required for the MaxError operation.</param>
        public void MaxError(NPPImage_8uC3 src2, CudaDeviceVariable<double> pError, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MaxErrorGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MaximumError.nppiMaximumError_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumError_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Device scratch buffer size (in bytes) for MaxError.
        /// </summary>
        /// <returns></returns>
        public SizeT MaxErrorGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MaximumError.nppiMaximumErrorGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumErrorGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        #endregion

        #region AverageError
        /// <summary>
        /// image average error. User buffer is internally allocated and freed.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        public void AverageError(NPPImage_8uC3 src2, CudaDeviceVariable<double> pError)
        {
            SizeT bufferSize = AverageErrorGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);
            status = NPPNativeMethods.NPPi.AverageError.nppiAverageError_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageError_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image average error.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        /// <param name="buffer">Pointer to the user-allocated scratch buffer required for the AverageError operation.</param>
        public void AverageError(NPPImage_8uC3 src2, CudaDeviceVariable<double> pError, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = AverageErrorGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.AverageError.nppiAverageError_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageError_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Device scratch buffer size (in bytes) for AverageError.
        /// </summary>
        /// <returns></returns>
        public SizeT AverageErrorGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.AverageError.nppiAverageErrorGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageErrorGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        #endregion

        #region MaximumRelative_Error
        /// <summary>
        /// image maximum relative error. User buffer is internally allocated and freed.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        public void MaximumRelativeError(NPPImage_8uC3 src2, CudaDeviceVariable<double> pError)
        {
            SizeT bufferSize = MaximumRelativeErrorGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);
            status = NPPNativeMethods.NPPi.MaximumRelativeError.nppiMaximumRelativeError_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumRelativeError_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image maximum relative error.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        /// <param name="buffer">Pointer to the user-allocated scratch buffer required for the MaximumRelativeError operation.</param>
        public void MaximumRelativeError(NPPImage_8uC3 src2, CudaDeviceVariable<double> pError, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MaximumRelativeErrorGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MaximumRelativeError.nppiMaximumRelativeError_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumRelativeError_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Device scratch buffer size (in bytes) for MaximumRelativeError.
        /// </summary>
        /// <returns></returns>
        public SizeT MaximumRelativeErrorGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MaximumRelativeError.nppiMaximumRelativeErrorGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumRelativeErrorGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        #endregion

        #region AverageRelative_Error
        /// <summary>
        /// image average relative error. User buffer is internally allocated and freed.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        public void AverageRelativeError(NPPImage_8uC3 src2, CudaDeviceVariable<double> pError)
        {
            SizeT bufferSize = AverageRelativeErrorGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);
            status = NPPNativeMethods.NPPi.AverageRelativeError.nppiAverageRelativeError_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageRelativeError_8u_C3R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image average relative error.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        /// <param name="buffer">Pointer to the user-allocated scratch buffer required for the AverageRelativeError operation.</param>
        public void AverageRelativeError(NPPImage_8uC3 src2, CudaDeviceVariable<double> pError, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = AverageRelativeErrorGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.AverageRelativeError.nppiAverageRelativeError_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageRelativeError_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Device scratch buffer size (in bytes) for AverageRelativeError.
        /// </summary>
        /// <returns></returns>
        public SizeT AverageRelativeErrorGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.AverageRelativeError.nppiAverageRelativeErrorGetBufferHostSize_8u_C3R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageRelativeErrorGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        #endregion



        //New in Cuda 7.0

        #region Filter Unsharp

        /// <summary>
        /// Scratch-buffer size for unsharp filter.
        /// </summary>
        /// <param name="nRadius">The radius of the Gaussian filter, in pixles, not counting the center pixel.</param>
        /// <param name="nSigma">The standard deviation of the Gaussian filter, in pixel.</param>
        /// <returns></returns>
        public int FilterUnsharpGetBufferSize(float nRadius, float nSigma)
        {
            int bufferSize = 0;
            status = NPPNativeMethods.NPPi.FixedFilters.nppiFilterUnsharpGetBufferSize_8u_C3R(nRadius, nSigma, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterUnsharpGetBufferSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        #endregion

        //New in Cuda 8.0
        #region ColorConversion new in Cuda 8.0


        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YCbCr411 color conversion.
        /// </summary>
        /// <param name="dest0">Destination image channel 0</param>
        /// <param name="dest1">Destination image channel 1</param>
        /// <param name="dest2">Destination image channel 2</param>
        public void RGBToYCbCr411(NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] arrayDest = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] arrayPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr.nppiRGBToYCbCr411_8u_C3P3R(DevicePointerRoi, Pitch, arrayDest, arrayPitch, SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr411_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr411 to 3 channel 8-bit unsigned packed RGB color conversion.
        /// </summary>
        /// <param name="src0">Source image channel 0</param>
        /// <param name="src1">Source image channel 1</param>
        /// <param name="src2">Source image channel 2</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr411ToRGB(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] arraySrc = new CUdeviceptr[] { src0.DevicePointerRoi, src1.DevicePointerRoi, src2.DevicePointerRoi };
            int[] arrayPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCr411ToRGB_8u_P3C3R(arraySrc, arrayPitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToRGB_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        #endregion

        #region GradientColorToGray


        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 1 channel 8-bit unsigned packed Gray Gradient conversion.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="eNorm">Gradient distance method to use.</param>
        public void GradientColorToGray(NPPImage_8uC1 dest, NppiNorm eNorm)
        {
            NppStatus status = NPPNativeMethods.NPPi.GradientColorToGray.nppiGradientColorToGray_8u_C3C1R(DevicePointerRoi, Pitch, dest.DevicePointerRoi, dest.Pitch, SizeRoi, eNorm);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGradientColorToGray_8u_C3C1R", status));
            NPPException.CheckNppStatus(status, null);
        }

        #endregion

        #region FilterGaussAdvancedBorder


        /// <summary>
        /// Calculate destination image SizeROI width and height from source image ROI width and height and downsampling rate.
        /// It is highly recommended that this function be use to determine the destination image ROI for consistent results.
        /// </summary>
        /// <param name="nRate">The downsampling rate to be used.  For integer equivalent rates unnecessary source pixels are just skipped. For non-integer rates the source image is bilinear interpolated. nRate must be > 1.0F and &lt;=  10.0F. </param>
        /// <returns>
        /// the destination image roi_specification.
        /// </returns>
        public NppiSize GetFilterGaussPyramidLayerDownBorderDstROI(float nRate)
        {
            NppiSize retSize = new NppiSize();
            status = NPPNativeMethods.NPPi.FilterGaussPyramid.nppiGetFilterGaussPyramidLayerDownBorderDstROI(_sizeRoi.width, _sizeRoi.height, ref retSize, nRate);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetFilterGaussPyramidLayerDownBorderDstROI", status));
            NPPException.CheckNppStatus(status, this);
            return retSize;
        }

        /// <summary>
        /// Calculate destination image SizeROI width and height from source image ROI width and height and downsampling rate.
        /// It is highly recommended that this function be use to determine the destination image ROI for consistent results.
        /// </summary>
        /// <param name="nRate">The downsampling rate to be used.  For integer equivalent rates unnecessary source pixels are just skipped. For non-integer rates the source image is bilinear interpolated. nRate must be > 1.0F and &lt;=  10.0F. </param>
        /// <param name="pDstSizeROIMin">Minimum recommended destination image roi_specification.</param>
        /// <param name="pDstSizeROIMax">Maximum recommended destination image roi_specification.</param>
        public void GetFilterGaussPyramidLayerUpBorderDstROI(float nRate, out NppiSize pDstSizeROIMin, out NppiSize pDstSizeROIMax)
        {
            pDstSizeROIMin = new NppiSize();
            pDstSizeROIMax = new NppiSize();
            status = NPPNativeMethods.NPPi.FilterGaussPyramid.nppiGetFilterGaussPyramidLayerUpBorderDstROI(_sizeRoi.width, _sizeRoi.height, ref pDstSizeROIMin, ref pDstSizeROIMax, nRate);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGetFilterGaussPyramidLayerUpBorderDstROI", status));
            NPPException.CheckNppStatus(status, this);
        }


        #endregion



        //New in Cuda 9.0
        #region New Cuda9
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr420 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void RGBToYCbCr420_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiRGBToYCbCr420_JPEG_8u_P3R(src, src0.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr420_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void RGBToYCbCr422_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiRGBToYCbCr422_JPEG_8u_P3R(src, src0.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr422_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void RGBToYCbCr411_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiRGBToYCbCr411_JPEG_8u_P3R(src, src0.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr411_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void RGBToYCbCr444_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiRGBToYCbCr444_JPEG_8u_P3R(src, src0.Pitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr444_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }





        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YCbCr420 color conversion.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public void RGBToYCbCr420_JPEG(NPPImage_8uC3 src, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiRGBToYCbCr420_JPEG_8u_C3P3R(src.DevicePointerRoi, src.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr420_JPEG_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public void RGBToYCbCr422_JPEG(NPPImage_8uC3 src, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiRGBToYCbCr422_JPEG_8u_C3P3R(src.DevicePointerRoi, src.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr422_JPEG_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public void RGBToYCbCr411_JPEG(NPPImage_8uC3 src, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiRGBToYCbCr411_JPEG_8u_C3P3R(src.DevicePointerRoi, src.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr411_JPEG_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned packed RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public void RGBToYCbCr444_JPEG(NPPImage_8uC3 src, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiRGBToYCbCr444_JPEG_8u_C3P3R(src.DevicePointerRoi, src.Pitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRGBToYCbCr444_JPEG_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned planar YCbCr420 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void BGRToYCbCr420_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiBGRToYCbCr420_JPEG_8u_P3R(src, src0.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr420_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void BGRToYCbCr422_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiBGRToYCbCr422_JPEG_8u_P3R(src, src0.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr422_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void BGRToYCbCr411_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiBGRToYCbCr411_JPEG_8u_P3R(src, src0.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr411_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void BGRToYCbCr444_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiBGRToYCbCr444_JPEG_8u_P3R(src, src0.Pitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr444_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }





        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr420 color conversion.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public void BGRToYCbCr420_JPEG(NPPImage_8uC3 src, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiBGRToYCbCr420_JPEG_8u_C3P3R(src.DevicePointerRoi, src.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr420_JPEG_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public void BGRToYCbCr422_JPEG(NPPImage_8uC3 src, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiBGRToYCbCr422_JPEG_8u_C3P3R(src.DevicePointerRoi, src.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr422_JPEG_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public void BGRToYCbCr411_JPEG(NPPImage_8uC3 src, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] dstPitch = new int[] { dest0.Pitch, dest1.Pitch, dest2.Pitch };
            status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiBGRToYCbCr411_JPEG_8u_C3P3R(src.DevicePointerRoi, src.Pitch, dst, dstPitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr411_JPEG_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned packed BGR to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public void BGRToYCbCr444_JPEG(NPPImage_8uC3 src, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiBGRToYCbCr444_JPEG_8u_C3P3R(src.DevicePointerRoi, src.Pitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiBGRToYCbCr444_JPEG_8u_C3P3R", status));
            NPPException.CheckNppStatus(status, this);
        }





        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr420 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void YCbCr420ToRGB_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr420ToRGB_JPEG_8u_P3R(src, srcPitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToRGB_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void YCbCr422ToRGB_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr422ToRGB_JPEG_8u_P3R(src, srcPitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToRGB_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void YCbCr411ToRGB_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr411ToRGB_JPEG_8u_P3R(src, srcPitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToRGB_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void YCbCr444ToRGB_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr444ToRGB_JPEG_8u_P3R(src, src0.Pitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr444ToRGB_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }




        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned packed YCbCr420 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr420ToRGB_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr420ToRGB_JPEG_8u_P3C3R(src, srcPitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToRGB_JPEG_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr422ToRGB_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr422ToRGB_JPEG_8u_P3C3R(src, srcPitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToRGB_JPEG_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr411ToRGB_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr411ToRGB_JPEG_8u_P3C3R(src, srcPitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToRGB_JPEG_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr444ToRGB_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr444ToRGB_JPEG_8u_P3C3R(src, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr444ToRGB_JPEG_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }







        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr420 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void YCbCr420ToBGR_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr420ToBGR_JPEG_8u_P3R(src, srcPitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToBGR_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void YCbCr422ToBGR_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr422ToBGR_JPEG_8u_P3R(src, srcPitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToBGR_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void YCbCr411ToBGR_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr411ToBGR_JPEG_8u_P3R(src, srcPitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToBGR_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar RGB to 3 channel 8-bit unsigned planar YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest0">Destination image (Channel 0)</param>
        /// <param name="dest1">Destination image (Channel 1)</param>
        /// <param name="dest2">Destination image (Channel 2)</param>
        public static void YCbCr444ToBGR_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC1 dest0, NPPImage_8uC1 dest1, NPPImage_8uC1 dest2)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            CUdeviceptr[] dst = new CUdeviceptr[] { dest0.DevicePointerRoi, dest1.DevicePointerRoi, dest2.DevicePointerRoi };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr444ToBGR_JPEG_8u_P3R(src, src0.Pitch, dst, dest0.Pitch, dest0.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr444ToBGR_JPEG_8u_P3R", status));
            NPPException.CheckNppStatus(status, null);
        }








        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned packed YCbCr420 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr420ToBGR_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr420ToBGR_JPEG_8u_P3C3R(src, srcPitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToBGR_JPEG_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr422ToBGR_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr422ToBGR_JPEG_8u_P3C3R(src, srcPitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToBGR_JPEG_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr411ToBGR_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            int[] srcPitch = new int[] { src0.Pitch, src1.Pitch, src2.Pitch };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr411ToBGR_JPEG_8u_P3C3R(src, srcPitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr411ToBGR_JPEG_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }
        /// <summary>
        /// 3 channel 8-bit unsigned planar BGR to 3 channel 8-bit unsigned packed YCbCr422 color conversion.
        /// </summary>
        /// <param name="src0">Source image (Channel 0)</param>
        /// <param name="src1">Source image (Channel 1)</param>
        /// <param name="src2">Source image (Channel 2)</param>
        /// <param name="dest">Destination image</param>
        public static void YCbCr444ToBGR_JPEG(NPPImage_8uC1 src0, NPPImage_8uC1 src1, NPPImage_8uC1 src2, NPPImage_8uC3 dest)
        {
            CUdeviceptr[] src = new CUdeviceptr[] { src0.DevicePointer, src1.DevicePointer, src2.DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.RGBToYCbCr_JPEG.nppiYCbCr444ToBGR_JPEG_8u_P3C3R(src, src0.Pitch, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr444ToBGR_JPEG_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        #endregion

        #region new in Cuda 9.1




        /// <summary>
        /// Calculate scratch buffer size needed for 3 channel 8-bit unsigned integer MorphCloseBorder, MorphOpenBorder, MorphTopHatBorder, 
        /// MorphBlackHatBorder, or MorphGradientBorder function based on destination image oSizeROI width and height.
        /// </summary>
        /// <returns>Required buffer size in bytes.</returns>
        public int MorphGetBufferSize()
        {
            int ret = 0;
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphGetBufferSize_8u_C3R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphGetBufferSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }




        /// <summary>
        /// 3 channel 8-bit unsigned integer morphological close with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphCloseBorder(NPPImage_8uC3 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphCloseBorder_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphCloseBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// 3 channel 8-bit unsigned integer morphological open with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphOpenBorder(NPPImage_8uC3 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphOpenBorder_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphOpenBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned integer morphological top hat with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphTopHatBorder(NPPImage_8uC3 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphTopHatBorder_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphTopHatBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned integer morphological black hat with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphBlackHatBorder(NPPImage_8uC3 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphBlackHatBorder_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphBlackHatBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned integer morphological gradient with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphGradientBorder(NPPImage_8uC3 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphGradientBorder_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphGradientBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion


        #region New in Cuda 10.2


        /// <summary>
        /// 3 channel 8-bit unsigned packed YUV to 3 channel 8-bit unsigned packed RGB batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input and output images passed in pSrcBatchList and pSrcBatchList
        /// arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">source_batch_images_pointer</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YUVToRGBBatch(CudaDeviceVariable<NppiImageDescriptor> pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            NppStatus status = NPPNativeMethods.NPPi.YUVToRGB.nppiYUVToRGBBatch_8u_C3R(pSrcBatchList.DevicePointer, pDstBatchList.DevicePointer, pSrcBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToRGBBatch_8u_C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YUV to 3 channel 8-bit unsigned packed BGR batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input and output images passed in pSrcBatchList and pSrcBatchList
        /// arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">source_batch_images_pointer</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YUVToBGRBatch(CudaDeviceVariable<NppiImageDescriptor> pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            NppStatus status = NPPNativeMethods.NPPi.YUVToBGR.nppiYUVToBGRBatch_8u_C3R(pSrcBatchList.DevicePointer, pDstBatchList.DevicePointer, pSrcBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToBGRBatch_8u_C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YCbCr to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair of input/output images has own ROI.
        /// Provided oMaxSizeROI must contain the maximum width and the maximum height of all ROIs defined in pDstBatchList.API user must ensure that
        /// ROI from pDstBatchList for each pair of input and output images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">source_batch_images_pointer</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YCbCrToRGBBatch(CudaDeviceVariable<NppiImageDescriptor> pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCrToRGBBatch_8u_C3R(pSrcBatchList.DevicePointer, pDstBatchList.DevicePointer, pSrcBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToRGBBatch_8u_C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YCbCr to 3 channel 8-bit unsigned packed BGR batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input and output images passed in pSrcBatchList and pSrcBatchList
        /// arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">source_batch_images_pointer</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YCbCrToBGRBatch(CudaDeviceVariable<NppiImageDescriptor> pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCrToBGRBatch_8u_C3R(pSrcBatchList.DevicePointer, pDstBatchList.DevicePointer, pSrcBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToBGRBatch_8u_C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YUV to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair of input/output images has own ROI.
        /// Provided oMaxSizeROI must contain the maximum width and the maximum height of all ROIs defined in pDstBatchList.API user must ensure that
        /// ROI from pDstBatchList for each pair of input and output images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">source_batch_images_pointer</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YUVToRGBBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor> pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            NppStatus status = NPPNativeMethods.NPPi.YUVToRGB.nppiYUVToRGBBatch_8u_C3R_Advanced(pSrcBatchList.DevicePointer, pDstBatchList.DevicePointer, pSrcBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToRGBBatch_8u_C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YCbCr to 3 channel 8-bit unsigned packed BGR batch color conversion where each pair of input/output images has own ROI.
        /// Provided oMaxSizeROI must contain the maximum width and the maximum height of all ROIs defined in pDstBatchList.API user must ensure that
        /// ROI from pDstBatchList for each pair of input and output images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">source_batch_images_pointer</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YCbCrToBGRBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor> pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCrToBGRBatch_8u_C3R_Advanced(pSrcBatchList.DevicePointer, pDstBatchList.DevicePointer, pSrcBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToBGRBatch_8u_C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YUV to 3 channel 8-bit unsigned packed BGR batch color conversion where each pair of input/output images has own ROI.
        /// Provided oMaxSizeROI must contain the maximum width and the maximum height of all ROIs defined in pDstBatchList.API user must ensure that
        /// ROI from pDstBatchList for each pair of input and output images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">source_batch_images_pointer</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YUVToBGRBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor> pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            NppStatus status = NPPNativeMethods.NPPi.YUVToBGR.nppiYUVToBGRBatch_8u_C3R_Advanced(pSrcBatchList.DevicePointer, pDstBatchList.DevicePointer, pSrcBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToBGRBatch_8u_C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned packed YCbCr to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair of input/output images has own ROI.
        /// Provided oMaxSizeROI must contain the maximum width and the maximum height of all ROIs defined in pDstBatchList.API user must ensure that
        /// ROI from pDstBatchList for each pair of input and output images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">source_batch_images_pointer</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YCbCrToRGBBatch_8u_C3R_Advanced(CudaDeviceVariable<NppiImageDescriptor> pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCrToRGBBatch_8u_C3R_Advanced(pSrcBatchList.DevicePointer, pDstBatchList.DevicePointer, pSrcBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToRGBBatch_8u_C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV to 3 channel 8-bit unsigned packed RGB batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YUVToRGBBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUVToRGB.nppiYUVToRGBBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToRGBBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV to 3 channel 8-bit unsigned packed BGR batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YUVToBGRBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUVToBGR.nppiYUVToBGRBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToBGRBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV422 to 3 channel 8-bit unsigned packed RGB batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YUV422ToRGBBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUV422ToRGB.nppiYUV422ToRGBBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV422ToRGBBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV422 to 3 channel 8-bit unsigned packed BGR batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YUV422ToBGRBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUV422ToRGB.nppiYUV422ToBGRBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV422ToBGRBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 3 channel 8-bit unsigned packed RGB batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YUV420ToRGBBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToRGB.nppiYUV420ToRGBBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToRGBBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 3 channel 8-bit unsigned packed BGR batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YUV420ToBGRBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToBGR.nppiYUV420ToBGRBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToBGRBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 3 channel 8-bit unsigned packed RGB batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YCbCrToRGBBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCrToRGBBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToRGBBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 3 channel 8-bit unsigned packed BGR batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YCbCrToBGRBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCrToBGRBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToBGRBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 3 channel 8-bit unsigned packed RGB batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YCbCr422ToRGBBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCr422ToRGBBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToRGBBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 3 channel 8-bit unsigned packed BGR batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YCbCr422ToBGRBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr422ToBGRBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToBGRBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 3 channel 8-bit unsigned packed RGB batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.	
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YCbCr420ToRGBBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCr420ToRGBBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToRGBBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 3 channel 8-bit unsigned packed BGR batch color conversion for a single ROI.
        /// Provided oSizeROI will be used for all pairs of input planes making input images and output packed images passed in
        /// pSrcBatchList and pSrcBatchList arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the
        /// borders of any of provided images.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oSizeROI">Region-of-Interest (ROI).</param>
        public static void YCbCr420ToBGRBatch(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr420ToBGRBatch_8u_P3C3R(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToBGRBatch_8u_P3C3R", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }




        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YUVToRGBBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUVToRGB.nppiYUVToRGBBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToRGBBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV to 3 channel 8-bit unsigned packed BGR batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YUVToBGRBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUVToBGR.nppiYUVToBGRBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUVToBGRBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV422 to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YUV422ToRGBBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUV422ToRGB.nppiYUV422ToRGBBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV422ToRGBBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV422 to 3 channel 8-bit unsigned packed BGR batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YUV422ToBGRBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUV422ToRGB.nppiYUV422ToBGRBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV422ToBGRBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YUV420ToRGBBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToRGB.nppiYUV420ToRGBBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToRGBBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YUV420 to 3 channel 8-bit unsigned packed BGR batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YUV420ToBGRBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YUV420ToBGR.nppiYUV420ToBGRBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYUV420ToBGRBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YCbCrToRGBBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCrToRGBBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToRGBBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr to 3 channel 8-bit unsigned packed BGR batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YCbCrToBGRBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCrToBGRBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCrToBGRBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YCbCr422ToRGBBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCr422ToRGBBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToRGBBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr422 to 3 channel 8-bit unsigned packed BGR batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YCbCr422ToBGRBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr422ToBGRBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr422ToBGRBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 3 channel 8-bit unsigned packed RGB batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YCbCr420ToRGBBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToRGB.nppiYCbCr420ToRGBBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToRGBBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned planar YCbCr420 to 3 channel 8-bit unsigned packed BGR batch color conversion where each pair
        /// of input/output images has own ROI.Provided oMaxSizeROI must contain the maximum width and the maximum height of all
        /// ROIs defined in pDstBatchList.API user must ensure that ROI from pDstBatchList for each pair of input and output
        /// images does not go beyond the borders of images in each pair.
        /// </summary>
        /// <param name="pSrcBatchList">An array where each element is a batch of images representing one of planes in planar images, 
        /// \ref source_batch_images_pointer.The first element of array (pSrcBatchList[0]) represents a batch of Y planes. 
        /// The second element of array (pSrcBatchList[1]) represents a batch of Cb planes.The third element of array
        ///  (pSrcBatchList[2]) represents a batch of Cr planes.</param>
        /// <param name="pDstBatchList">destination_batch_images_pointer</param>
        /// <param name="oMaxSizeROI">Region-of-Interest (ROI), must contain the maximum width and the maximum height from all destination ROIs used for processing data.</param>
        public static void YCbCr420ToBGRBatchAdvanced(CudaDeviceVariable<NppiImageDescriptor>[] pSrcBatchList, CudaDeviceVariable<NppiImageDescriptor> pDstBatchList, NppiSize oMaxSizeROI)
        {
            CUdeviceptr[] srcList = new CUdeviceptr[] { pSrcBatchList[0].DevicePointer, pSrcBatchList[1].DevicePointer, pSrcBatchList[2].DevicePointer };
            NppStatus status = NPPNativeMethods.NPPi.YCbCrToBGR.nppiYCbCr420ToBGRBatch_8u_P3C3R_Advanced(srcList, pDstBatchList.DevicePointer, pDstBatchList.Size, oMaxSizeROI);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiYCbCr420ToBGRBatch_8u_P3C3R_Advanced", status));
            NPPException.CheckNppStatus(status, pSrcBatchList);
        }


        /// <summary>
        /// color twist batch
        /// An input color twist matrix with floating-point coefficient values is applied
        /// within the ROI for each image in batch. Color twist matrix can vary per image. The same ROI is applied to each image.
        /// </summary>
        /// <param name="nMin">Minimum clamp value.</param>
        /// <param name="nMax">Maximum saturation and clamp value.</param>
        /// <param name="oSizeROI"></param>
        /// <param name="pBatchList">Device memory pointer to nBatchSize list of NppiColorTwistBatchCXR structures.</param>
        public static void ColorTwistBatch(float nMin, float nMax, NppiSize oSizeROI, CudaDeviceVariable<NppiColorTwistBatchCXR> pBatchList)
        {
            NppStatus status = NPPNativeMethods.NPPi.ColorTwistBatch.nppiColorTwistBatch32f_8u_C3R(nMin, nMax, oSizeROI, pBatchList.DevicePointer, pBatchList.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwistBatch32f_8u_C3R", status));
            NPPException.CheckNppStatus(status, pBatchList);
        }


        /// <summary>
        /// in place color twist batch
        /// An input color twist matrix with floating-point coefficient values is applied
        /// within the ROI for each image in batch. Color twist matrix can vary per image. The same ROI is applied to each image.
        /// </summary>
        /// <param name="nMin">Minimum clamp value.</param>
        /// <param name="nMax">Maximum saturation and clamp value.</param>
        /// <param name="oSizeROI"></param>
        /// <param name="pBatchList">Device memory pointer to nBatchSize list of NppiColorTwistBatchCXR structures.</param>
        public static void ColorTwistBatchI(float nMin, float nMax, NppiSize oSizeROI, CudaDeviceVariable<NppiColorTwistBatchCXR> pBatchList)
        {
            NppStatus status = NPPNativeMethods.NPPi.ColorTwistBatch.nppiColorTwistBatch32f_8u_C3IR(nMin, nMax, oSizeROI, pBatchList.DevicePointer, pBatchList.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwistBatch32f_8u_C3IR", status));
            NPPException.CheckNppStatus(status, pBatchList);
        }
        #endregion

        #region New in Cuda 11.2

        /// <summary>
        /// Three-channel 8-bit unsigned image MSE.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pMSE">Pointer to the computed MSE of two images. </param>
        /// <param name="pDeviceBuffer">Pointer to the required device memory allocation. Use \ref nppiMSEGetBufferHostSize_8u_C3R to compute the required size (in bytes).</param>
        public void MSE(NPPImage_8uC3 src2, CudaDeviceVariable<float> pMSE, CudaDeviceVariable<byte> pDeviceBuffer)
        {
            status = NPPNativeMethods.NPPi.IQA.nppiMSE_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pMSE.DevicePointer, pDeviceBuffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMSE_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Three-channel 8-bit unsigned image PSNR.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pPSNR">Pointer to the computed PSNR of two images. </param>
        /// <param name="pDeviceBuffer">Pointer to the required device memory allocation. Use \ref nppiPSNRGetBufferHostSize_8u_C3R to compute the required size (in bytes).</param>
        public void PSNR(NPPImage_8uC3 src2, CudaDeviceVariable<float> pPSNR, CudaDeviceVariable<byte> pDeviceBuffer)
        {
            status = NPPNativeMethods.NPPi.IQA.nppiPSNR_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pPSNR.DevicePointer, pDeviceBuffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiPSNR_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Three-channel 8-bit unsigned image SSIM.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pSSIM">Pointer to the computed SSIM of two images. </param>
        /// <param name="pDeviceBuffer">Pointer to the required device memory allocation. Use \ref nppiSSIMGetBufferHostSize_8u_C3R to compute the required size (in bytes).</param>
        public void SSIM(NPPImage_8uC3 src2, CudaDeviceVariable<float> pSSIM, CudaDeviceVariable<byte> pDeviceBuffer)
        {
            status = NPPNativeMethods.NPPi.IQA.nppiSSIM_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pSSIM.DevicePointer, pDeviceBuffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSSIM_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Three-channel 8-bit unsigned image MS-SSIM*.
        /// This function uses the algorithm described in the paper by Wang et.al.
        /// Wang, Z., Simoncelli, E.P., Bovik, A.C.Multiscale Structural Similarity for Image Quality Assessment.In: The
        /// Thirty-Seventh Asilomar Conference on Signals, Systems and Computers, 2003, 13981402. Pacific Grove, CA, USA: IEEE,
        /// 2003. https://doi.org/10.1109/ACSSC.2003.1292216. <para/>
        /// NOTE: this API call can only process oSizeROI dimensions 16px by 16px and above.Any oSizeROI dimensions less than 16px by 16px
        /// will result in undefined behaviour.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pMSSSIM">Pointer to the computed SSIM of two images. </param>
        /// <param name="pDeviceBuffer">Pointer to the required device memory allocation. Use \ref nppiWMSSSIMGetBufferHostSize_8u_C3R to compute the required size (in bytes).</param>
        public void WMSSSIM(NPPImage_8uC3 src2, CudaDeviceVariable<float> pMSSSIM, CudaDeviceVariable<byte> pDeviceBuffer)
        {
            status = NPPNativeMethods.NPPi.IQA.nppiWMSSSIM_8u_C3R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pMSSSIM.DevicePointer, pDeviceBuffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiWMSSSIM_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Buffer size for \ref nppiMSE_8u_C3R.
        /// </summary>
        public SizeT MSEGetBufferHostSize()
        {
            SizeT ret = 0;
            status = NPPNativeMethods.NPPi.IQA.nppiMSEGetBufferHostSize_8u_C3R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMSEGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }

        /// <summary>
        /// Buffer size for \ref nppiPSNR_8u_C3R.
        /// </summary>
        public SizeT PSNRGetBufferHostSize()
        {
            SizeT ret = 0;
            status = NPPNativeMethods.NPPi.IQA.nppiPSNRGetBufferHostSize_8u_C3R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiPSNRGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }

        /// <summary>
        /// Buffer size for \ref nppiSSIM_8u_C3R.
        /// </summary>
        public SizeT SSIMGetBufferHostSize()
        {
            SizeT ret = 0;
            status = NPPNativeMethods.NPPi.IQA.nppiSSIMGetBufferHostSize_8u_C3R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSSIMGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }

        /// <summary>
        /// Buffer size for \ref nppiWMSSSIM_8u_C3R.
        /// </summary>
        public SizeT WMSSSIMGetBufferHostSize()
        {
            SizeT ret = 0;
            status = NPPNativeMethods.NPPi.IQA.nppiWMSSSIMGetBufferHostSize_8u_C3R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiWMSSSIMGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }



        /// <summary>
        /// 3 channel 8-bit unsigned MSE for a batch of image pairs for a single ROI.
        /// Provided oSizeROI will be used for all images passed in pSrc1BatchList and pSrc2BatchList
        /// arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the borders of any of provided images.
        /// </summary>
        /// <param name="oSizeROI"></param>
        /// <param name="pBatchSrc1">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pBatchSrc2">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pMSE">Device memory pointer to output array of the computed MSE for nBatchSize* sizeof(Npp32f)* 3 image pairs.</param>
        /// <param name="pDeviceBufferList">Device memory pointer to the list of NppiBufferDescriptor buffer descriptors specifying per image device
        /// memory buffer pointers and size as returned by at least one MSEBatchGetBufferHostSize call.</param>
        public static void MSEBatch(NppiSize oSizeROI, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc1, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc2, CudaDeviceVariable<float> pMSE,
                                        CudaDeviceVariable<NppiBufferDescriptor> pDeviceBufferList)
        {
            NppStatus status = NPPNativeMethods.NPPi.IQA.nppiMSEBatch_8u_C3R(pBatchSrc1.DevicePointer, pBatchSrc2.DevicePointer, pBatchSrc1.Size, oSizeROI, pMSE.DevicePointer,
                pDeviceBufferList.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMSEBatch_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned MSE for a batch of image pairs with per-image ROI
        /// </summary>
        /// <param name="oMaxSizeROI">maximum ROI width and height of ALL images in the batch.</param>
        /// <param name="pBatchSrc1">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pBatchSrc2">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pMSE">Device memory pointer to output array of the computed MSE for nBatchSize* sizeof(Npp32f)* 3 image pairs.</param>
        /// <param name="pDeviceBufferList">Device memory pointer to the list of NppiBufferDescriptor buffer descriptors specifying per image device
        /// memory buffer pointers and size as returned by at least one MSEBatchGetBufferHostSize call.</param>
        public static void MSEBatchAdvanced(NppiSize oMaxSizeROI, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc1, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc2, CudaDeviceVariable<float> pMSE,
                                        CudaDeviceVariable<NppiBufferDescriptor> pDeviceBufferList)
        {
            NppStatus status = NPPNativeMethods.NPPi.IQA.nppiMSEBatch_8u_C3R_Advanced(pBatchSrc1.DevicePointer, pBatchSrc2.DevicePointer, pBatchSrc1.Size, oMaxSizeROI, pMSE.DevicePointer,
                pDeviceBufferList.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMSEBatch_8u_C3R_Advanced", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// Buffer size for \ref nppiMSEBatch_8u_C3R.
        /// </summary>
        public SizeT MSEBatchGetBufferHostSize()
        {
            SizeT ret = 0;
            status = NPPNativeMethods.NPPi.IQA.nppiMSEBatchGetBufferHostSize_8u_C3R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMSEBatchGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }

        /// <summary>
        /// 3 channel 8-bit unsigned PSNR for a batch of image pairs for a single ROI.
        /// Provided oSizeROI will be used for all images passed in pSrc1BatchList and pSrc2BatchList
        /// arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the borders of any of provided images.
        /// </summary>
        /// <param name="oSizeROI"></param>
        /// <param name="pBatchSrc1">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pBatchSrc2">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pPSNR">Device memory pointer to output array of the computed PSNR for nBatchSize* sizeof(Npp32f)* 3 image pairs.</param>
        /// <param name="pDeviceBufferList">Device memory pointer to the list of NppiBufferDescriptor buffer descriptors specifying per image device
        /// memory buffer pointers and size as returned by at least one PSNRBatchGetBufferHostSize call.</param>
        public static void PSNRBatch(NppiSize oSizeROI, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc1, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc2, CudaDeviceVariable<float> pPSNR,
                                        CudaDeviceVariable<NppiBufferDescriptor> pDeviceBufferList)
        {
            NppStatus status = NPPNativeMethods.NPPi.IQA.nppiPSNRBatch_8u_C3R(pBatchSrc1.DevicePointer, pBatchSrc2.DevicePointer, pBatchSrc1.Size, oSizeROI, pPSNR.DevicePointer,
                pDeviceBufferList.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiPSNRBatch_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned PSNR for a batch of image pairs with per-image ROI
        /// </summary>
        /// <param name="oMaxSizeROI">maximum ROI width and height of ALL images in the batch.</param>
        /// <param name="pBatchSrc1">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pBatchSrc2">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pPSNR">Device memory pointer to output array of the computed PSNR for nBatchSize* sizeof(Npp32f)* 3 image pairs.</param>
        /// <param name="pDeviceBufferList">Device memory pointer to the list of NppiBufferDescriptor buffer descriptors specifying per image device
        /// memory buffer pointers and size as returned by at least one PSNRBatchGetBufferHostSize call.</param>
        public static void PSNRBatchAdvanced(NppiSize oMaxSizeROI, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc1, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc2, CudaDeviceVariable<float> pPSNR,
                                        CudaDeviceVariable<NppiBufferDescriptor> pDeviceBufferList)
        {
            NppStatus status = NPPNativeMethods.NPPi.IQA.nppiPSNRBatch_8u_C3R_Advanced(pBatchSrc1.DevicePointer, pBatchSrc2.DevicePointer, pBatchSrc1.Size, oMaxSizeROI, pPSNR.DevicePointer,
                pDeviceBufferList.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiPSNRBatch_8u_C3R_Advanced", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// Buffer size for \ref nppiPSNRBatch_8u_C3R.
        /// </summary>
        public SizeT PSNRBatchGetBufferHostSize()
        {
            SizeT ret = 0;
            status = NPPNativeMethods.NPPi.IQA.nppiPSNRBatchGetBufferHostSize_8u_C3R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiPSNRBatchGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }

        /// <summary>
        /// 3 channel 8-bit unsigned WMSSSIM for a batch of image pairs for a single ROI.
        /// Provided oSizeROI will be used for all images passed in pSrc1BatchList and pSrc2BatchList
        /// arguments.API user must ensure that provided ROI (oSizeROI) does not go beyond the borders of any of provided images.
        /// </summary>
        /// <param name="oSizeROI"></param>
        /// <param name="pBatchSrc1">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pBatchSrc2">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pWMSSSIM">Device memory pointer to output array of the computed WMSSSIM for nBatchSize* sizeof(Npp32f)* 3 image pairs.</param>
        /// <param name="pDeviceBufferList">Device memory pointer to the list of NppiBufferDescriptor buffer descriptors specifying per image device
        /// memory buffer pointers and size as returned by at least one WMSSSIMBatchGetBufferHostSize call.</param>
        public static void WMSSSIMBatch(NppiSize oSizeROI, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc1, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc2, CudaDeviceVariable<float> pWMSSSIM,
                                        CudaDeviceVariable<NppiBufferDescriptor> pDeviceBufferList)
        {
            NppStatus status = NPPNativeMethods.NPPi.IQA.nppiWMSSSIMBatch_8u_C3R(pBatchSrc1.DevicePointer, pBatchSrc2.DevicePointer, pBatchSrc1.Size, oSizeROI, pWMSSSIM.DevicePointer,
                pDeviceBufferList.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiWMSSSIMBatch_8u_C3R", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// 3 channel 8-bit unsigned WMSSSIM for a batch of image pairs with per-image ROI
        /// </summary>
        /// <param name="oMaxSizeROI">maximum ROI width and height of ALL images in the batch.</param>
        /// <param name="pBatchSrc1">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pBatchSrc2">device memory pointer to the list of device memory image descriptors, per image oSize must be initialized.</param>
        /// <param name="pWMSSSIM">Device memory pointer to output array of the computed WMSSSIM for nBatchSize* sizeof(Npp32f)* 3 image pairs.</param>
        /// <param name="pDeviceBufferList">Device memory pointer to the list of NppiBufferDescriptor buffer descriptors specifying per image device
        /// memory buffer pointers and size as returned by at least one WMSSSIMBatchGetBufferHostSize call.</param>
        public static void WMSSSIMBatchAdvanced(NppiSize oMaxSizeROI, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc1, CudaDeviceVariable<NppiImageDescriptor> pBatchSrc2, CudaDeviceVariable<float> pWMSSSIM,
                                        CudaDeviceVariable<NppiBufferDescriptor> pDeviceBufferList)
        {
            NppStatus status = NPPNativeMethods.NPPi.IQA.nppiWMSSSIMBatch_8u_C3R_Advanced(pBatchSrc1.DevicePointer, pBatchSrc2.DevicePointer, pBatchSrc1.Size, oMaxSizeROI, pWMSSSIM.DevicePointer,
                pDeviceBufferList.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiWMSSSIMBatch_8u_C3R_Advanced", status));
            NPPException.CheckNppStatus(status, null);
        }

        /// <summary>
        /// Buffer size for \ref nppiWMSSSIMBatch_8u_C3R.
        /// </summary>
        public SizeT WMSSSIMBatchGetBufferHostSize()
        {
            SizeT ret = 0;
            status = NPPNativeMethods.NPPi.IQA.nppiWMSSSIMBatchGetBufferHostSize_8u_C3R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiWMSSSIMBatchGetBufferHostSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }
        #endregion

        #region New in Cuda 12.0

        /// <summary>
        /// Returns the required size of host memory buffer needed by most nppiFilterBoxBorderAdvanced functions.
        /// </summary>
        public int FilterBoxBorderAdvancedGetDeviceBufferSize()
        {
            int bufferSize = 0;
            status = NPPNativeMethods.NPPi.LinearFixedFilters2D.nppiFilterBoxBorderAdvancedGetDeviceBufferSize(_sizeRoi, _channels, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterBoxBorderAdvancedGetDeviceBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Buffer size (in bytes) for nppiCrossCorrFull_NormLevelAdvanced functions.
        /// </summary>
        /// <param name="oTplRoiSize">Region-of-Interest (ROI) size of the template image.</param>
        public SizeT CrossCorrFull_NormLevel_GetAdvancedScratchBufferSize(NppiSize oTplRoiSize)
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevel_GetAdvancedScratchBufferSize(_sizeRoi, oTplRoiSize, sizeof(float), _channels, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevel_GetAdvancedScratchBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Buffer size (in bytes) for nppiCrossCorrSame_NormLevelAdvanced functions.
        /// </summary>
        /// <param name="oTplRoiSize">Region-of-Interest (ROI) size of the template image.</param>
        public SizeT CrossCorrSame_NormLevel_GetAdvancedScratchBufferSize(NppiSize oTplRoiSize)
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevel_GetAdvancedScratchBufferSize(_sizeRoi, oTplRoiSize, sizeof(float), _channels, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevel_GetAdvancedScratchBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Buffer size (in bytes) for nppiCrossCorrValid_NormLevelAdvanced functions.
        /// </summary>
        /// <param name="oTplRoiSize">Region-of-Interest (ROI) size of the template image.</param>
        public SizeT CrossCorrValid_NormLevel_GetAdvancedScratchBufferSize(NppiSize oTplRoiSize)
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevel_GetAdvancedScratchBufferSize(_sizeRoi, oTplRoiSize, sizeof(float), _channels, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevel_GetAdvancedScratchBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
#if ADD_MISSING_CTX

        /// <summary>
        /// median filter scratch memory size.
        /// </summary>
        /// <param name="oMaskSize">Width and Height of the neighborhood region for the local Avg operation.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        public uint FilterMedianBorderGetBufferSize(NppiSize oMaskSize, NppiBorderType eBorderType)
        {
            uint bufferSize = 0;
            status = NPPNativeMethods.NPPi.ImageMedianFilter.nppiFilterMedianBorderGetBufferSize_8u_C3R(_sizeRoi, oMaskSize, ref bufferSize, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterMedianBorderGetBufferSize_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        #region Add
        /// <summary>
        /// Add constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nConstant">Value to add</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Add(CudaDeviceVariable<byte> nConstant, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.AddDeviceConst.nppiAddDeviceC_8u_C3RSfs(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddDeviceC_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Add constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value. Inplace.
        /// </summary>
        /// <param name="nConstant">Value to add</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Add(CudaDeviceVariable<byte> nConstant, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.AddDeviceConst.nppiAddDeviceC_8u_C3IRSfs(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddDeviceC_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sub

        /// <summary>
        /// Subtract constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sub(CudaDeviceVariable<byte> nConstant, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.SubDeviceConst.nppiSubDeviceC_8u_C3RSfs(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSubDeviceC_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Subtract constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value. Inplace.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Sub(CudaDeviceVariable<byte> nConstant, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.SubDeviceConst.nppiSubDeviceC_8u_C3IRSfs(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSubDeviceC_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Mul

        /// <summary>
        /// Multiply constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Mul(CudaDeviceVariable<byte> nConstant, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.MulDeviceConst.nppiMulDeviceC_8u_C3RSfs(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulDeviceC_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Multiply constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value. Inplace.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Mul(CudaDeviceVariable<byte> nConstant, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.MulDeviceConst.nppiMulDeviceC_8u_C3IRSfs(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulDeviceC_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Multiply constant to image and scale by max bit width value
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        public void Mul(CudaDeviceVariable<byte> nConstant, NPPImage_8uC3 dest)
        {
            status = NPPNativeMethods.NPPi.MulDeviceConstScale.nppiMulDeviceCScale_8u_C3R(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulDeviceCScale_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Multiply constant to image and scale by max bit width value
        /// </summary>
        /// <param name="nConstant">Value</param>
        public void Mul(CudaDeviceVariable<byte> nConstant)
        {
            status = NPPNativeMethods.NPPi.MulDeviceConstScale.nppiMulDeviceCScale_8u_C3IR(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulDeviceCScale_8u_C3IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Div

        /// <summary>
        /// Divide constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Div(CudaDeviceVariable<byte> nConstant, NPPImage_8uC3 dest, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.DivDeviceConst.nppiDivDeviceC_8u_C3RSfs(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDivDeviceC_8u_C3RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Divide constant to image, scale by 2^(-nScaleFactor), then clamp to saturated value. Inplace.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="nScaleFactor">scaling factor</param>
        public void Div(CudaDeviceVariable<byte> nConstant, int nScaleFactor)
        {
            status = NPPNativeMethods.NPPi.DivDeviceConst.nppiDivDeviceC_8u_C3IRSfs(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi, nScaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDivDeviceC_8u_C3IRSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion


        /// <summary>
        /// median filter with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="oMaskSize">Width and Height of the neighborhood region for the local Avg operation.</param>
        /// <param name="oAnchor">X and Y offsets of the kernel origin frame of reference w.r.t the source pixel.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="pBuffer">Pointer to the user-allocated scratch buffer required for the Median operation.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void FilterMedianBorder(NPPImage_8uC3 dest, NppiSize oMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, CudaDeviceVariable<byte> pBuffer, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ImageMedianFilter.nppiFilterMedianBorder_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi,
                                                    dest.Pitch, dest.SizeRoi, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterMedianBorder_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// Box filter with border control. 
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="oMaskSize">Width and Height of the neighborhood region for the local Avg operation.</param>
        /// <param name="oAnchor">X and Y offsets of the kernel origin frame of reference w.r.t the source pixel.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="pBuffer">Pointer to the user-allocated scratch buffer required for the Median operation.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void FilterBoxBorderAdvanced(NPPImage_8uC3 dest, NppiSize oMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, CudaDeviceVariable<byte> pBuffer, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.LinearFixedFilters2D.nppiFilterBoxBorderAdvanced_8u_C3R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi,
                                                    dest.Pitch, dest.SizeRoi, oMaskSize, oAnchor, eBorderType, pBuffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterBoxBorderAdvanced_8u_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrFull_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Pointer to the required device memory allocation. </param>
        /// <param name="bufferAdvanced">Pointer to the required device memory allocation. See nppiCrossCorrFull_NormLevel_GetAdvancedScratchBufferSize</param>
        public void CrossCorrFull_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst, CudaDeviceVariable<byte> buffer, CudaDeviceVariable<byte> bufferAdvanced)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevelAdvanced_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer, bufferAdvanced.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevelAdvanced_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// CrossCorrSame_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Pointer to the required device memory allocation. </param>
        /// <param name="bufferAdvanced">Pointer to the required device memory allocation. See nppiCrossCorrSame_NormLevel_GetAdvancedScratchBufferSize</param>
        public void CrossCorrSame_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst, CudaDeviceVariable<byte> buffer, CudaDeviceVariable<byte> bufferAdvanced)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevelAdvanced_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer, bufferAdvanced.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevelAdvanced_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// CrossCorrValid_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Pointer to the required device memory allocation. </param>
        /// <param name="bufferAdvanced">Pointer to the required device memory allocation. See nppiCrossCorrValid_NormLevel_GetAdvancedScratchBufferSize</param>
        public void CrossCorrValid_NormLevel(NPPImage_8uC3 tpl, NPPImage_32fC3 dst, CudaDeviceVariable<byte> buffer, CudaDeviceVariable<byte> bufferAdvanced)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevelAdvanced_8u32f_C3R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer, bufferAdvanced.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevelAdvanced_8u32f_C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
#endif
        #endregion

    }
}
