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
    public partial class NPPImage_32fC1 : NPPImageBase
    {
        #region Constructors
        /// <summary>
        /// Allocates new memory on device using NPP-Api.
        /// </summary>
        /// <param name="nWidthPixels">Image width in pixels</param>
        /// <param name="nHeightPixels">Image height in pixels</param>
        public NPPImage_32fC1(int nWidthPixels, int nHeightPixels)
        {
            _sizeOriginal.width = nWidthPixels;
            _sizeOriginal.height = nHeightPixels;
            _sizeRoi.width = nWidthPixels;
            _sizeRoi.height = nHeightPixels;
            _channels = 1;
            _isOwner = true;
            _typeSize = sizeof(float);
            _dataType = NppDataType.NPP_32F;
            _nppChannels = NppiChannels.NPP_CH_1;

            _devPtr = NPPNativeMethods.NPPi.MemAlloc.nppiMalloc_32f_C1(nWidthPixels, nHeightPixels, ref _pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}, Pitch is: {3}, Number of color channels: {4}", DateTime.Now, "nppiMalloc_32f_C1", res, _pitch, _channels));

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
        public NPPImage_32fC1(CUdeviceptr devPtr, int width, int height, int pitch, bool isOwner)
        {
            _devPtr = devPtr;
            _devPtrRoi = _devPtr;
            _sizeOriginal.width = width;
            _sizeOriginal.height = height;
            _sizeRoi.width = width;
            _sizeRoi.height = height;
            _pitch = pitch;
            _channels = 1;
            _isOwner = isOwner;
            _typeSize = sizeof(float);
            _dataType = NppDataType.NPP_32F;
            _nppChannels = NppiChannels.NPP_CH_1;
        }

        /// <summary>
        /// Creates a new NPPImage from allocated device ptr. Does not take ownership of decPtr.
        /// </summary>
        /// <param name="devPtr">Already allocated device ptr.</param>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="pitch">Pitch / Line step</param>
        public NPPImage_32fC1(CUdeviceptr devPtr, int width, int height, int pitch)
            : this(devPtr, width, height, pitch, false)
        {

        }

        /// <summary>
        /// Creates a new NPPImage from allocated device ptr. Does not take ownership of inner image device pointer.
        /// </summary>
        /// <param name="image">NPP image</param>
        public NPPImage_32fC1(NPPImageBase image)
            : this(image.DevicePointer, image.Width, image.Height, image.Pitch, false)
        {

        }

        /// <summary>
        /// Allocates new memory on device using NPP-Api.
        /// </summary>
        /// <param name="size">Image size</param>
        public NPPImage_32fC1(NppiSize size)
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
        public NPPImage_32fC1(CUdeviceptr devPtr, NppiSize size, int pitch, bool isOwner)
            : this(devPtr, size.width, size.height, pitch, isOwner)
        {

        }

        /// <summary>
        /// Creates a new NPPImage from allocated device ptr.
        /// </summary>
        /// <param name="devPtr">Already allocated device ptr.</param>
        /// <param name="size">Image size</param>
        /// <param name="pitch">Pitch / Line step</param>
        public NPPImage_32fC1(CUdeviceptr devPtr, NppiSize size, int pitch)
            : this(devPtr, size.width, size.height, pitch)
        {

        }

        /// <summary>
        /// For dispose
        /// </summary>
        ~NPPImage_32fC1()
        {
            Dispose(false);
        }
        #endregion

        #region Converter operators

        /// <summary>
        /// Converts a NPPImage to a CudaPitchedDeviceVariable
        /// </summary>
        public CudaPitchedDeviceVariable<float> ToCudaPitchedDeviceVariable()
        {
            return new CudaPitchedDeviceVariable<float>(_devPtr, _sizeOriginal.width, _sizeOriginal.height, _pitch);
        }

        /// <summary>
        /// Converts a NPPImage to a CudaPitchedDeviceVariable
        /// </summary>
        /// <param name="img">NPPImage</param>
        /// <returns>CudaPitchedDeviceVariable with the same device pointer and size of NPPImage without ROI information</returns>
        public static implicit operator CudaPitchedDeviceVariable<float>(NPPImage_32fC1 img)
        {
            return img.ToCudaPitchedDeviceVariable();
        }

        /// <summary>
        /// Converts a CudaPitchedDeviceVariable to a NPPImage 
        /// </summary>
        /// <param name="img">CudaPitchedDeviceVariable</param>
        /// <returns>NPPImage with the same device pointer and size of CudaPitchedDeviceVariable with ROI set to full image</returns>
        public static implicit operator NPPImage_32fC1(CudaPitchedDeviceVariable<float> img)
        {
            return img.ToNPPImage();
        }
        #endregion

        #region Copy
        /// <summary>
        /// Image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        public void Copy(NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_32f_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Masked Operation 8-bit unsigned image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="mask">Mask image</param>
        public void Copy(NPPImage_32fC1 dst, NPPImage_8uC1 mask)
        {
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_32f_C1MR(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, mask.DevicePointerRoi, mask.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="channel">Channel number. This number is added to the dst pointer</param>
        public void Copy(NPPImage_32fC2 dst, int channel)
        {
            if (channel < 0 | channel >= dst.Channels) throw new ArgumentOutOfRangeException("channel", "channel must be in range [0..1].");
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_32f_C1C2R(_devPtrRoi, _pitch, dst.DevicePointerRoi + channel * _typeSize, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_32f_C1C2R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="channel">Channel indicator (real or imaginary part of the complex number)</param>
        public void Copy(NPPImage_32fcC1 dst, ComplexChannel channel)
        {
            int c = (int)channel;
            //typesize is sizeof(float), so only the real or imag part of the complex number!
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_32f_C1C2R(_devPtrRoi, _pitch, dst.DevicePointerRoi + c * _typeSize, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_32f_C1C2R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="channel">Channel number. This number is added to the dst pointer</param>
        public void Copy(NPPImage_32fC3 dst, int channel)
        {
            if (channel < 0 | channel >= dst.Channels) throw new ArgumentOutOfRangeException("channel", "channel must be in range [0..2].");
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_32f_C1C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi + channel * _typeSize, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_32f_C1C3R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image copy.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="channel">Channel number. This number is added to the dst pointer</param>
        public void Copy(NPPImage_32fC4 dst, int channel)
        {
            if (channel < 0 | channel >= dst.Channels) throw new ArgumentOutOfRangeException("channel", "channel must be in range [0..3].");
            status = NPPNativeMethods.NPPi.MemCopy.nppiCopy_32f_C1C4R(_devPtrRoi, _pitch, dst.DevicePointerRoi + channel * _typeSize, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopy_32f_C1C4R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Set
        /// <summary>
        /// Set pixel values to nValue.
        /// </summary>
        /// <param name="nValue">Value to be set</param>
        public void Set(float nValue)
        {
            status = NPPNativeMethods.NPPi.MemSet.nppiSet_32f_C1R(nValue, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSet_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Set pixel values to nValue. <para/>
        /// The 8-bit mask image affects setting of the respective pixels in the destination image. <para/>
        /// If the mask value is zero (0) the pixel is not set, if the mask is non-zero, the corresponding
        /// destination pixel is set to specified value.
        /// </summary>
        /// <param name="nValue">Value to be set</param>
        /// <param name="mask">Mask image</param>
        public void Set(float nValue, NPPImage_8uC1 mask)
        {
            status = NPPNativeMethods.NPPi.MemSet.nppiSet_32f_C1MR(nValue, _devPtrRoi, _pitch, _sizeRoi, mask.DevicePointerRoi, mask.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSet_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Convert
        /// <summary>
        /// 32-bit floating point to 16-bit conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        public void Convert(NPPImage_16uC1 dst, NppRoundMode roundMode)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f16u_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f16u_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 32-bit floating point to 16-bit conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        public void Convert(NPPImage_16fC1 dst, NppRoundMode roundMode)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f16f_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f16f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 16-bit conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        public void Convert(NPPImage_16sC1 dst, NppRoundMode roundMode)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f16s_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f16s_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 8-bit unsigned conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        public void Convert(NPPImage_8uC1 dst, NppRoundMode roundMode)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f8u_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f8u_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 8-bit signed conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        public void Convert(NPPImage_8sC1 dst, NppRoundMode roundMode)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f8s_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f8s_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 8-bit unsigned conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        /// <param name="scaleFactor">Integer Result Scaling.</param>
        public void Convert(NPPImage_8uC1 dst, NppRoundMode roundMode, int scaleFactor)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f8u_C1RSfs(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode, scaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f8u_C1RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 8-bit signed conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        /// <param name="scaleFactor">Integer Result Scaling.</param>
        public void Convert(NPPImage_8sC1 dst, NppRoundMode roundMode, int scaleFactor)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f8s_C1RSfs(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode, scaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f8s_C1RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 16-bit unsigned conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        /// <param name="scaleFactor">Integer Result Scaling.</param>
        public void Convert(NPPImage_16uC1 dst, NppRoundMode roundMode, int scaleFactor)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f16u_C1RSfs(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode, scaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f16u_C1RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 16-bit signed conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        /// <param name="scaleFactor">Integer Result Scaling.</param>
        public void Convert(NPPImage_16sC1 dst, NppRoundMode roundMode, int scaleFactor)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f16s_C1RSfs(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode, scaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f16s_C1RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 32-bit unsigned conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        /// <param name="scaleFactor">Integer Result Scaling.</param>
        public void Convert(NPPImage_32uC1 dst, NppRoundMode roundMode, int scaleFactor)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f32u_C1RSfs(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode, scaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f32u_C1RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 32-bit floating point to 32-bit signed conversion.
        /// </summary>
        /// <param name="dst">Destination image</param>
        /// <param name="roundMode">Flag specifying how fractional float values are rounded to integer values.</param>
        /// <param name="scaleFactor">Integer Result Scaling.</param>
        public void Convert(NPPImage_32sC1 dst, NppRoundMode roundMode, int scaleFactor)
        {
            status = NPPNativeMethods.NPPi.BitDepthConversion.nppiConvert_32f32s_C1RSfs(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, roundMode, scaleFactor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiConvert_32f32s_C1RSfs", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Add
        /// <summary>
        /// Image addition.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void Add(NPPImage_32fC1 src2, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Add.nppiAdd_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAdd_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image addition.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        public void Add(NPPImage_32fC1 src2)
        {
            status = NPPNativeMethods.NPPi.Add.nppiAdd_32f_C1IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAdd_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Add constant to image.
        /// </summary>
        /// <param name="nConstant">Value to add</param>
        /// <param name="dest">Destination image</param>
        public void Add(float nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.AddConst.nppiAddC_32f_C1R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Add constant to image. Inplace.
        /// </summary>
        /// <param name="nConstant">Value to add</param>
        public void Add(float nConstant)
        {
            status = NPPNativeMethods.NPPi.AddConst.nppiAddC_32f_C1IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddC_32f_C1IR", status));
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

        #region Sub
        /// <summary>
        /// Image subtraction.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void Sub(NPPImage_32fC1 src2, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Sub.nppiSub_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSub_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image subtraction.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        public void Sub(NPPImage_32fC1 src2)
        {
            status = NPPNativeMethods.NPPi.Sub.nppiSub_32f_C1IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSub_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Subtract constant to image.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        /// <param name="dest">Destination image</param>
        public void Sub(float nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.SubConst.nppiSubC_32f_C1R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSubC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Subtract constant to image. Inplace.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        public void Sub(float nConstant)
        {
            status = NPPNativeMethods.NPPi.SubConst.nppiSubC_32f_C1IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSubC_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Mul
        /// <summary>
        /// Image multiplication.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void Mul(NPPImage_32fC1 src2, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Mul.nppiMul_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMul_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image multiplication.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        public void Mul(NPPImage_32fC1 src2)
        {
            status = NPPNativeMethods.NPPi.Mul.nppiMul_32f_C1IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMul_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Multiply constant to image.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        public void Mul(float nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.MulConst.nppiMulC_32f_C1R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Multiply constant to image. Inplace.
        /// </summary>
        /// <param name="nConstant">Value</param>
        public void Mul(float nConstant)
        {
            status = NPPNativeMethods.NPPi.MulConst.nppiMulC_32f_C1IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulC_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Div
        /// <summary>
        /// Image division.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void Div(NPPImage_32fC1 src2, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Div.nppiDiv_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDiv_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image division.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        public void Div(NPPImage_32fC1 src2)
        {
            status = NPPNativeMethods.NPPi.Div.nppiDiv_32f_C1IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDiv_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Divide constant to image.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        public void Div(float nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.DivConst.nppiDivC_32f_C1R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDivC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Divide constant to image. Inplace.
        /// </summary>
        /// <param name="nConstant">Value</param>
        public void Div(float nConstant)
        {
            status = NPPNativeMethods.NPPi.DivConst.nppiDivC_32f_C1IR(nConstant, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDivC_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Exp
        /// <summary>
        /// Exponential.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Exp(NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Exp.nppiExp_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiExp_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace exponential.
        /// </summary>
        public void Exp()
        {
            status = NPPNativeMethods.NPPi.Exp.nppiExp_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiExp_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Ln
        /// <summary>
        /// Natural logarithm.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Ln(NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Ln.nppiLn_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLn_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Natural logarithm.
        /// </summary>
        public void Ln()
        {
            status = NPPNativeMethods.NPPi.Ln.nppiLn_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLn_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sqr
        /// <summary>
        /// Image squared.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Sqr(NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Sqr.nppiSqr_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqr_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace image squared.
        /// </summary>
        public void Sqr()
        {
            status = NPPNativeMethods.NPPi.Sqr.nppiSqr_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqr_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sqrt
        /// <summary>
        /// Image square root.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Sqrt(NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Sqrt.nppiSqrt_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrt_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Inplace image square root.
        /// </summary>
        public void Sqrt()
        {
            status = NPPNativeMethods.NPPi.Sqrt.nppiSqrt_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrt_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
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
        public void AlphaComp(float alpha1, NPPImage_32fC1 src2, float alpha2, NPPImage_32fC1 dest, NppiAlphaOp nppAlphaOp)
        {
            status = NPPNativeMethods.NPPi.AlphaCompConst.nppiAlphaCompC_32f_C1R(_devPtrRoi, _pitch, alpha1, src2.DevicePointerRoi, src2.Pitch, alpha2, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nppAlphaOp);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAlphaCompC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
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

        #region AbsDiff
        /// <summary>
        /// Absolute difference of this minus src2.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void AbsDiff(NPPImage_32fC1 src2, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.AbsDiff.nppiAbsDiff_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAbsDiff_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Absolute difference with constant.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        /// <param name="dest">Destination image</param>
        public void AbsDiff(float nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.AbsDiffConst.nppiAbsDiffC_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nConstant);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAbsDiffC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Abs
        /// <summary>
        /// Image absolute value.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Abs(NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Abs.nppiAbs_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAbs_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image absolute value. In place.
        /// </summary>
        public void Abs()
        {
            status = NPPNativeMethods.NPPi.Abs.nppiAbs_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAbs_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region AddProduct
        /// <summary>
        /// Image product added to in place floating point destination image.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        public void AddProduct(NPPImage_32fC1 src2, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.AddProduct.nppiAddProduct_32f_C1IR(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddProduct_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image product added to in place floating point destination image using filter mask (updates destination when mask is non-zero).
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        /// <param name="mask">Mask image</param>
        public void AddProduct(NPPImage_32fC1 src2, NPPImage_32fC1 dest, NPPImage_8uC1 mask)
        {
            status = NPPNativeMethods.NPPi.AddProduct.nppiAddProduct_32f_C1IMR(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, mask.DevicePointerRoi, mask.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddProduct_32f_C1IMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region AddSquare
        /// <summary>
        /// Image squared then added to in place floating point destination image.
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void AddSquare(NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.AddSquare.nppiAddSquare_32f_C1IR(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddSquare_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image squared then added to in place floating point destination image using filter mask (updates destination when mask is non-zero).
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="mask">Mask image</param>
        public void AddSquare(NPPImage_32fC1 dest, NPPImage_8uC1 mask)
        {
            status = NPPNativeMethods.NPPi.AddSquare.nppiAddSquare_32f_C1IMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddSquare_32f_C1IMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region AddWeighted
        /// <summary>
        /// Channel alpha weighted image added to in place floating point destination image.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="nAlpha">Alpha weight to be applied to source image pixels (0.0F to 1.0F)</param>
        public void AddWeighted(NPPImage_32fC1 dest, float nAlpha)
        {
            status = NPPNativeMethods.NPPi.AddWeighted.nppiAddWeighted_32f_C1IR(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nAlpha);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddWeighted_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Channel alpha weighted image added to in place floating point destination image using filter mask (updates destination when mask is non-zero).
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="mask">Mask image</param>
        /// <param name="nAlpha">Alpha weight to be applied to source image pixels (0.0F to 1.0F)</param>
        public void AddWeighted(NPPImage_32fC1 dest, NPPImage_8uC1 mask, float nAlpha)
        {
            status = NPPNativeMethods.NPPi.AddWeighted.nppiAddWeighted_32f_C1IMR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nAlpha);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddWeighted_32f_C1IMR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Histogram

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
        /// Scratch-buffer size for HistogramRange.
        /// </summary>
        /// <param name="nLevels"></param>
        /// <returns></returns>
        public SizeT HistogramRangeGetBufferSize(int nLevels)
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramRangeGetBufferSize_32f_C1R(_sizeRoi, nLevels, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramRangeGetBufferSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Histogram with bins determined by pLevels array. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="histogram">array that receives the computed histogram. The array must be of size nLevels-1.</param>
        /// <param name="pLevels">Array in device memory containing the level sizes of the bins. The array must be of size nLevels</param>
        public void HistogramRange(CudaDeviceVariable<int> histogram, CudaDeviceVariable<float> pLevels)
        {
            SizeT bufferSize = HistogramRangeGetBufferSize(histogram.Size);
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramRange_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, histogram.DevicePointer, pLevels.DevicePointer, pLevels.Size, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramRange_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Histogram with bins determined by pLevels array. No additional buffer is allocated.
        /// </summary>
        /// <param name="histogram">array that receives the computed histogram. The array must be of size nLevels-1.</param>
        /// <param name="pLevels">Array in device memory containing the level sizes of the bins. The array must be of size nLevels</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="HistogramRangeGetBufferSize(int)"/></param>
        public void HistogramRange(CudaDeviceVariable<int> histogram, CudaDeviceVariable<float> pLevels, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = HistogramRangeGetBufferSize(histogram.Size);
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Histogram.nppiHistogramRange_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, histogram.DevicePointer, pLevels.DevicePointer, pLevels.Size, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiHistogramRange_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sum
        /// <summary>
        /// Scratch-buffer size for nppiSum_32f_C1R.
        /// </summary>
        /// <returns></returns>
        public SizeT SumGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.Sum.nppiSumGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSumGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image sum with 64-bit double precision result. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="result">Allocated device memory with size of at least 1 * sizeof(double)</param>
        public void Sum(CudaDeviceVariable<double> result)
        {
            SizeT bufferSize = SumGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Sum.nppiSum_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, result.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSum_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image sum with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="result">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="SumGetBufferHostSize()"/></param>
        public void Sum(CudaDeviceVariable<double> result, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = SumGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Sum.nppiSum_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, result.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSum_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.Min.nppiMinGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel minimum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        public void Min(CudaDeviceVariable<float> min)
        {
            SizeT bufferSize = MinGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Min.nppiMin_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, min.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMin_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum. No additional buffer is allocated.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinGetBufferHostSize()"/></param>
        public void Min(CudaDeviceVariable<float> min, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Min.nppiMin_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, min.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMin_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.MinIdx.nppiMinIndxGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinIndxGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel minimum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="indexX">Allocated device memory with size of at least 1 * sizeof(int)</param>
        /// <param name="indexY">Allocated device memory with size of at least 1 * sizeof(int)</param>
        public void MinIndex(CudaDeviceVariable<float> min, CudaDeviceVariable<int> indexX, CudaDeviceVariable<int> indexY)
        {
            SizeT bufferSize = MinIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MinIdx.nppiMinIndx_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, min.DevicePointer, indexX.DevicePointer, indexY.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinIndx_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum. No additional buffer is allocated.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="indexX">Allocated device memory with size of at least 1 * sizeof(int)</param>
        /// <param name="indexY">Allocated device memory with size of at least 1 * sizeof(int)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinIndexGetBufferHostSize()"/></param>
        public void MinIndex(CudaDeviceVariable<float> min, CudaDeviceVariable<int> indexX, CudaDeviceVariable<int> indexY, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MinIdx.nppiMinIndx_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, min.DevicePointer, indexX.DevicePointer, indexY.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinIndx_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.Max.nppiMaxGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel maximum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        public void Max(CudaDeviceVariable<float> max)
        {
            SizeT bufferSize = MaxGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.Max.nppiMax_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, max.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMax_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel maximum. No additional buffer is allocated.
        /// </summary>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MaxGetBufferHostSize()"/></param>
        public void Max(CudaDeviceVariable<float> max, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MaxGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.Max.nppiMax_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, max.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMax_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.MaxIdx.nppiMaxIndxGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxIndxGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel maximum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="indexX">Allocated device memory with size of at least 1 * sizeof(int)</param>
        /// <param name="indexY">Allocated device memory with size of at least 1 * sizeof(int)</param>
        public void MaxIndex(CudaDeviceVariable<float> max, CudaDeviceVariable<int> indexX, CudaDeviceVariable<int> indexY)
        {
            SizeT bufferSize = MaxIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MaxIdx.nppiMaxIndx_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, max.DevicePointer, indexX.DevicePointer, indexY.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxIndx_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum. No additional buffer is allocated.
        /// </summary>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="indexX">Allocated device memory with size of at least 1 * sizeof(int)</param>
        /// <param name="indexY">Allocated device memory with size of at least 1 * sizeof(int)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MaxIndexGetBufferHostSize()"/></param>
        public void MaxIndex(CudaDeviceVariable<float> max, CudaDeviceVariable<int> indexX, CudaDeviceVariable<int> indexY, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MaxIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MaxIdx.nppiMaxIndx_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, max.DevicePointer, indexX.DevicePointer, indexY.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxIndx_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.MinMaxNew.nppiMinMaxGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel minimum and maximum. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        public void MinMax(CudaDeviceVariable<float> min, CudaDeviceVariable<float> max)
        {
            SizeT bufferSize = MinMaxGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MinMaxNew.nppiMinMax_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, min.DevicePointer, max.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMax_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum and maximum. No additional buffer is allocated.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinMaxGetBufferHostSize()"/></param>
        public void MinMax(CudaDeviceVariable<float> min, CudaDeviceVariable<float> max, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinMaxGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MinMaxNew.nppiMinMax_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, min.DevicePointer, max.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMax_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndxGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndxGetBufferHostSize_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndxGetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndxGetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// Image pixel minimum and maximum values with their indices. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="minIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="maxIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        public void MinMaxIndex(CudaDeviceVariable<float> min, CudaDeviceVariable<float> max, CudaDeviceVariable<NppiPoint> minIndex, CudaDeviceVariable<NppiPoint> maxIndex)
        {
            SizeT bufferSize = MinMaxIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndx_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, min.DevicePointer, max.DevicePointer, minIndex.DevicePointer, maxIndex.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndx_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum and maximum values with their indices. No additional buffer is allocated.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="minIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="maxIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinMaxIndexGetBufferHostSize()"/></param>
        public void MinMaxIndex(CudaDeviceVariable<float> min, CudaDeviceVariable<float> max, CudaDeviceVariable<NppiPoint> minIndex, CudaDeviceVariable<NppiPoint> maxIndex, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinMaxIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndx_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, min.DevicePointer, max.DevicePointer, minIndex.DevicePointer, maxIndex.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndx_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum and maximum values with their indices. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="minIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="maxIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="mask">If the mask is filled with zeros, then all the returned values are zeros, i.e., pMinIndex = {0, 0}, pMaxIndex = {0, 0}, pMinValue = 0, pMaxValue = 0.</param>
        public void MinMaxIndex(CudaDeviceVariable<float> min, CudaDeviceVariable<float> max, CudaDeviceVariable<NppiPoint> minIndex, CudaDeviceVariable<NppiPoint> maxIndex, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = MinMaxIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndx_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, min.DevicePointer, max.DevicePointer, minIndex.DevicePointer, maxIndex.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndx_32f_C1MR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Image pixel minimum and maximum values with their indices. No additional buffer is allocated.
        /// </summary>
        /// <param name="min">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="max">Allocated device memory with size of at least 1 * sizeof(float)</param>
        /// <param name="minIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="maxIndex">Allocated device memory with size of at least 1 * sizeof(NppiPoint)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MinMaxIndexMaskedGetBufferHostSize()"/></param>
        /// <param name="mask">If the mask is filled with zeros, then all the returned values are zeros, i.e., pMinIndex = {0, 0}, pMaxIndex = {0, 0}, pMinValue = 0, pMaxValue = 0.</param>
        public void MinMaxIndex(CudaDeviceVariable<float> min, CudaDeviceVariable<float> max, CudaDeviceVariable<NppiPoint> minIndex, CudaDeviceVariable<NppiPoint> maxIndex, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MinMaxIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MinMaxIndxNew.nppiMinMaxIndx_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, min.DevicePointer, max.DevicePointer, minIndex.DevicePointer, maxIndex.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinMaxIndx_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.MeanNew.nppiMeanGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMeanGetBufferHostSize_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.MeanNew.nppiMeanGetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMeanGetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image mean with 64-bit double precision result. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 1 * sizeof(double)</param>
        public void Mean(CudaDeviceVariable<double> mean)
        {
            SizeT bufferSize = MeanGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MeanNew.nppiMean_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image mean with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MeanGetBufferHostSize()"/></param>
        public void Mean(CudaDeviceVariable<double> mean, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MeanGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MeanNew.nppiMean_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image mean with 64-bit double precision result. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">If the mask is filled with zeros, then all the returned values are zeros, i.e., pMinIndex = {0, 0}, pMaxIndex = {0, 0}, pMinValue = 0, pMaxValue = 0.</param>
        public void Mean(CudaDeviceVariable<double> mean, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = MeanMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MeanNew.nppiMean_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_32f_C1MR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image mean with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MeanMaskedGetBufferHostSize()"/></param>
        /// <param name="mask">If the mask is filled with zeros, then all the returned values are zeros, i.e., pMinIndex = {0, 0}, pMaxIndex = {0, 0}, pMinValue = 0, pMaxValue = 0.</param>
        public void Mean(CudaDeviceVariable<double> mean, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MeanMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MeanNew.nppiMean_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMeanStdDevGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMeanStdDevGetBufferHostSize_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMeanStdDevGetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMeanStdDevGetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image mean and standard deviation. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="stdDev">Allocated device memory with size of at least 1 * sizeof(double)</param>
        public void MeanStdDev(CudaDeviceVariable<double> mean, CudaDeviceVariable<double> stdDev)
        {
            SizeT bufferSize = MeanStdDevGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMean_StdDev_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer, stdDev.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_StdDev_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image sum with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="stdDev">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MeanStdDevGetBufferHostSize()"/></param>
        public void MeanStdDev(CudaDeviceVariable<double> mean, CudaDeviceVariable<double> stdDev, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MeanStdDevGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMean_StdDev_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer, stdDev.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_StdDev_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image mean and standard deviation. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="stdDev">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        public void MeanStdDev(CudaDeviceVariable<double> mean, CudaDeviceVariable<double> stdDev, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = MeanStdDevMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMean_StdDev_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer, stdDev.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_StdDev_32f_C1MR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image sum with 64-bit double precision result. No additional buffer is allocated.
        /// </summary>
        /// <param name="mean">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="stdDev">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="MeanStdDevMaskedGetBufferHostSize()"/></param>
        public void MeanStdDev(CudaDeviceVariable<double> mean, CudaDeviceVariable<double> stdDev, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MeanStdDevMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MeanStdDevNew.nppiMean_StdDev_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, buffer.DevicePointer, mean.DevicePointer, stdDev.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMean_StdDev_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.NormInf.nppiNormInfGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormInfGetBufferHostSize_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormInf.nppiNormInfGetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormInfGetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image infinity norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        public void NormInf(CudaDeviceVariable<double> norm)
        {
            SizeT bufferSize = NormInfGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormInf.nppiNorm_Inf_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_Inf_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image infinity norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormInfGetBufferHostSize()"/></param>
        public void NormInf(CudaDeviceVariable<double> norm, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormInfGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormInf.nppiNorm_Inf_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_Inf_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image infinity norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        public void NormInf(CudaDeviceVariable<double> norm, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = NormInfMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormInf.nppiNorm_Inf_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_Inf_32f_C1MR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image infinity norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormInfMaskedGetBufferHostSize()"/></param>
        public void NormInf(CudaDeviceVariable<double> norm, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormInfMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormInf.nppiNorm_Inf_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_Inf_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.NormL1.nppiNormL1GetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormL1GetBufferHostSize_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormL1.nppiNormL1GetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormL1GetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image L1 norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        public void NormL1(CudaDeviceVariable<double> norm)
        {
            SizeT bufferSize = NormL1GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormL1.nppiNorm_L1_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L1_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L1 norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormL1GetBufferHostSize()"/></param>
        public void NormL1(CudaDeviceVariable<double> norm, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormL1GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormL1.nppiNorm_L1_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L1_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L1 norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        public void NormL1(CudaDeviceVariable<double> norm, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = NormL1MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormL1.nppiNorm_L1_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L1_32f_C1MR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L1 norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormL1MaskedGetBufferHostSize()"/></param>
        public void NormL1(CudaDeviceVariable<double> norm, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormL1MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormL1.nppiNorm_L1_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L1_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.NormL2.nppiNormL2GetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormL2GetBufferHostSize_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormL2.nppiNormL2GetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormL2GetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image L2 norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        public void NormL2(CudaDeviceVariable<double> norm)
        {
            SizeT bufferSize = NormL2GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormL2.nppiNorm_L2_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L2_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L2 norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormL2GetBufferHostSize()"/></param>
        public void NormL2(CudaDeviceVariable<double> norm, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormL2GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormL2.nppiNorm_L2_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L2_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L2 norm. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        public void NormL2(CudaDeviceVariable<double> norm, NPPImage_8uC1 mask)
        {
            SizeT bufferSize = NormL2MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormL2.nppiNorm_L2_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L2_32f_C1MR", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image L2 norm. No additional buffer is allocated.
        /// </summary>
        /// <param name="norm">Allocated device memory with size of at least 1 * sizeof(double)</param>
        /// <param name="mask">mask</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormL2MaskedGetBufferHostSize()"/></param>
        public void NormL2(CudaDeviceVariable<double> norm, NPPImage_8uC1 mask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormL2MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormL2.nppiNorm_L2_32f_C1MR(_devPtrRoi, _pitch, mask.DevicePointerRoi, mask.Pitch, _sizeRoi, norm.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNorm_L2_32f_C1MR", status));
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
        public void Threshold(NPPImage_32fC1 dest, float nThreshold, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations OP the predicate (sourcePixel OP nThreshold) is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="eComparisonOperation">eComparisonOperation. Only allowed values are <see cref="NppCmpOp.Less"/> and <see cref="NppCmpOp.Greater"/></param>
        public void Threshold(float nThreshold, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_32f_C1IR", status));
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
        public void ThresholdGT(NPPImage_32fC1 dest, float nThreshold)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_GT_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_GT_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is greater than nThreshold is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        public void ThresholdGT(float nThreshold)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_GT_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_GT_32f_C1IR", status));
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
        public void ThresholdLT(NPPImage_32fC1 dest, float nThreshold)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LT_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LT_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is less than nThreshold is true, the pixel is set
        /// to nThreshold, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        public void ThresholdLT(float nThreshold)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LT_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LT_32f_C1IR", status));
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
        public void Threshold(NPPImage_32fC1 dest, float nThreshold, float nValue, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_Val_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold, nValue, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_Val_32f_C1R", status));
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
        public void Threshold(float nThreshold, float nValue, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_Val_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold, nValue, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_Val_32f_C1IR", status));
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
        public void ThresholdGT(NPPImage_32fC1 dest, float nThreshold, float nValue)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_GTVal_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_GTVal_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is greater than nThreshold is true, the pixel is set
        /// to nValue, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="nValue">The threshold replacement value.</param>
        public void ThresholdGT(float nThreshold, float nValue)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_GTVal_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_GTVal_32f_C1IR", status));
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
        public void ThresholdLT(NPPImage_32fC1 dest, float nThreshold, float nValue)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LTVal_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThreshold, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LTVal_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// In place image threshold.<para/>
        /// If for a comparison operations sourcePixel is less than nThreshold is true, the pixel is set
        /// to nValue, otherwise it is set to sourcePixel.
        /// </summary>
        /// <param name="nThreshold">The threshold value.</param>
        /// <param name="nValue">The threshold replacement value.</param>
        public void ThresholdLT(float nThreshold, float nValue)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LTVal_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, nThreshold, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LTVal_32f_C1IR", status));
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
        public void ThresholdLTGT(NPPImage_32fC1 dest, float nThresholdLT, float nValueLT, float nThresholdGT, float nValueGT)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LTValGTVal_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nThresholdLT, nValueLT, nThresholdGT, nValueGT);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LTValGTVal_32f_C1R", status));
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
        public void ThresholdLTGT(float nThresholdLT, float nValueLT, float nThresholdGT, float nValueGT)
        {
            status = NPPNativeMethods.NPPi.Threshold.nppiThreshold_LTValGTVal_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, nThresholdLT, nValueLT, nThresholdGT, nValueGT);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiThreshold_LTValGTVal_32f_C1IR", status));
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
        public void Compare(NPPImage_32fC1 src2, NPPImage_8uC1 dest, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Compare.nppiCompare_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCompare_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Compare pSrc's pixels with constant value.
        /// </summary>
        /// <param name="nConstant">constant value</param>
        /// <param name="dest">Destination image</param>
        /// <param name="eComparisonOperation">Specifies the comparison operation to be used in the pixel comparison.</param>
        public void Compare(float nConstant, NPPImage_8uC1 dest, NppCmpOp eComparisonOperation)
        {
            status = NPPNativeMethods.NPPi.Compare.nppiCompareC_32f_C1R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, eComparisonOperation);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCompareC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region CompareEqualEps
        /// <summary>
        /// Compare pSrc1's pixels with corresponding pixels in pSrc2.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dest">Destination image</param>
        /// <param name="epsilon">epsilon tolerance value to compare to pixel absolute differences.</param>
        public void CompareEqualEps(NPPImage_32fC1 src2, NPPImage_8uC1 dest, float epsilon)
        {
            status = NPPNativeMethods.NPPi.Compare.nppiCompareEqualEps_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, epsilon);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCompareEqualEps_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Compare pSrc's pixels with constant value.
        /// </summary>
        /// <param name="nConstant">constant value</param>
        /// <param name="dest">Destination image</param>
        /// <param name="epsilon">epsilon tolerance value to compare to pixel absolute differences.</param>
        public void CompareEqualEps(float nConstant, NPPImage_8uC1 dest, float epsilon)
        {
            status = NPPNativeMethods.NPPi.Compare.nppiCompareEqualEpsC_32f_C1R(_devPtrRoi, _pitch, nConstant, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, epsilon);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCompareEqualEpsC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        //new in Cuda 5.5
        #region DotProduct
        /// <summary>
        /// Device scratch buffer size (in bytes) for nppiDotProd_32f64f_C1R.
        /// </summary>
        /// <returns></returns>
        public SizeT DotProdGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.DotProd.nppiDotProdGetBufferHostSize_32f64f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDotProdGetBufferHostSize_32f64f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// One-channel 32-bit floating point image DotProd.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pDp">Pointer to the computed dot product of the two images. (1 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="DotProdGetBufferHostSize()"/></param>
        public void DotProduct(NPPImage_32fC1 src2, CudaDeviceVariable<double> pDp, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = DotProdGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.DotProd.nppiDotProd_32f64f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pDp.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDotProd_32f64f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// One-channel 32-bit floating point DotProd. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pDp">Pointer to the computed dot product of the two images. (1 * sizeof(double))</param>
        public void DotProduct(NPPImage_32fC1 src2, CudaDeviceVariable<double> pDp)
        {
            SizeT bufferSize = DotProdGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.DotProd.nppiDotProd_32f64f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pDp.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDotProd_32f64f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        #endregion

        #region CopyNew

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
        public void Copy(NPPImage_32fC1 dst, int nTopBorderHeight, int nLeftBorderWidth, byte nValue)
        {
            status = NPPNativeMethods.NPPi.CopyConstBorder.nppiCopyConstBorder_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, dst.DevicePointerRoi, dst.Pitch, dst.SizeRoi, nTopBorderHeight, nLeftBorderWidth, nValue);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopyConstBorder_32f_C1R", status));
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
        public void CopyReplicateBorder(NPPImage_32fC1 dst, int nTopBorderHeight, int nLeftBorderWidth)
        {
            status = NPPNativeMethods.NPPi.CopyReplicateBorder.nppiCopyReplicateBorder_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, dst.DevicePointerRoi, dst.Pitch, dst.SizeRoi, nTopBorderHeight, nLeftBorderWidth);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopyReplicateBorder_32f_C1R", status));
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
        public void CopyWrapBorder(NPPImage_32fC1 dst, int nTopBorderHeight, int nLeftBorderWidth)
        {
            status = NPPNativeMethods.NPPi.CopyWrapBorder.nppiCopyWrapBorder_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, dst.DevicePointerRoi, dst.Pitch, dst.SizeRoi, nTopBorderHeight, nLeftBorderWidth);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopyWrapBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// linearly interpolated source image subpixel coordinate color copy.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nDx">Fractional part of source image X coordinate.</param>
        /// <param name="nDy">Fractional part of source image Y coordinate.</param>
        public void CopySubpix(NPPImage_32fC1 dst, float nDx, float nDy)
        {
            status = NPPNativeMethods.NPPi.CopySubpix.nppiCopySubpix_32f_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, nDx, nDy);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCopySubpix_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region LUTNew
        /// <summary>
        /// look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points with no interpolation.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="pValues">Pointer to an array of user defined OUTPUT values</param>
        /// <param name="pLevels">Pointer to an array of user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUT(NPPImage_32fC1 dst, CudaDeviceVariable<float> pValues, CudaDeviceVariable<float> pLevels)
        {
            status = NPPNativeMethods.NPPi.ColorLUT.nppiLUT_32f_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, pValues.DevicePointer, pLevels.DevicePointer, pLevels.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// cubic interpolated look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points through cubic interpolation. 
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="pValues">Pointer to an array of user defined OUTPUT values</param>
        /// <param name="pLevels">Pointer to an array of user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUTCubic(NPPImage_32fC1 dst, CudaDeviceVariable<float> pValues, CudaDeviceVariable<float> pLevels)
        {
            status = NPPNativeMethods.NPPi.ColorLUTCubic.nppiLUT_Cubic_32f_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, pValues.DevicePointer, pLevels.DevicePointer, pLevels.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_Cubic_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// Inplace look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points with no interpolation.
        /// </summary>
        /// <param name="pValues">Pointer to an array of user defined OUTPUT values</param>
        /// <param name="pLevels">Pointer to an array of user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUT(CudaDeviceVariable<float> pValues, CudaDeviceVariable<float> pLevels)
        {
            status = NPPNativeMethods.NPPi.ColorLUT.nppiLUT_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, pValues.DevicePointer, pLevels.DevicePointer, pLevels.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Inplace cubic interpolated look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points through cubic interpolation. 
        /// </summary>
        /// <param name="pValues">Pointer to an array of user defined OUTPUT values</param>
        /// <param name="pLevels">Pointer to an array of user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUTCubic(CudaDeviceVariable<float> pValues, CudaDeviceVariable<float> pLevels)
        {
            status = NPPNativeMethods.NPPi.ColorLUTCubic.nppiLUT_Cubic_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, pValues.DevicePointer, pLevels.DevicePointer, pLevels.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_Cubic_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Inplace linear interpolated look-up-table color conversion.
        /// The LUT is derived from a set of user defined mapping points through cubic interpolation. 
        /// </summary>
        /// <param name="pValues">Pointer to an array of user defined OUTPUT values</param>
        /// <param name="pLevels">Pointer to an array of user defined INPUT values. pLevels.Size gives nLevels.</param>
        public void LUTLinear(CudaDeviceVariable<float> pValues, CudaDeviceVariable<float> pLevels)
        {
            status = NPPNativeMethods.NPPi.ColorLUTLinear.nppiLUT_Linear_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, pValues.DevicePointer, pLevels.DevicePointer, pLevels.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_Linear_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }

        //nppiLUT_Linear_32f_C1R

        /// <summary>
        /// look-up-table color conversion.<para/>
        /// The LUT is derived from a set of user defined mapping points through linear interpolation.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="values">array of user defined OUTPUT values</param>
        /// <param name="levels">array of user defined INPUT values</param>
        public void LUTLinear(NPPImage_32fC1 dest, CudaDeviceVariable<float> values, CudaDeviceVariable<float> levels)
        {
            if (values.Size != levels.Size) throw new ArgumentException("values and levels must have same size.");

            status = NPPNativeMethods.NPPi.ColorLUTLinear.nppiLUT_Linear_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, values.DevicePointer, levels.DevicePointer, (int)levels.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiLUT_Linear_32f_C1R", status));
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
        public void Dilate(NPPImage_32fC1 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor)
        {
            status = NPPNativeMethods.NPPi.MorphologyFilter2D.nppiDilate_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, Mask.DevicePointer, aMaskSize, oAnchor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDilate_32f_C1R", status));
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
        public void Erode(NPPImage_32fC1 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor)
        {
            status = NPPNativeMethods.NPPi.MorphologyFilter2D.nppiErode_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, Mask.DevicePointer, aMaskSize, oAnchor);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiErode_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// 3x3 dilation.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Dilate3x3(NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.MorphologyFilter2D.nppiDilate3x3_32f_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDilate3x3_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3x3 erosion.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Erode3x3(NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.MorphologyFilter2D.nppiErode3x3_32f_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiErode3x3_32f_C1R", status));
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
        public void DilateBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.DilationWithBorderControl.nppiDilateBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, Mask.DevicePointer, aMaskSize, oAnchor, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDilateBorder_32f_C1R", status));
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
        public void ErodeBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ErosionWithBorderControl.nppiErodeBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, Mask.DevicePointer, aMaskSize, oAnchor, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiErodeBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3x3 dilation with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void Dilate3x3Border(NPPImage_32fC1 dest, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.Dilate3x3Border.nppiDilate3x3Border_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDilate3x3Border_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 3x3 erosion with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void Erode3x3Border(NPPImage_32fC1 dest, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.Erode3x3Border.nppiErode3x3Border_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiErode3x3Border_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffInfGetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffInfGetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_Inf.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed Inf-norm of differences. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffInfMaskedGetBufferHostSize()"/></param>
        public void NormDiff_Inf(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffInfMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_Inf_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_Inf_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_Inf. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed Inf-norm of differences. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        public void NormDiff_Inf(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormDiffInfMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_Inf_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_Inf_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffL1GetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffL1GetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_L1.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L1-norm of differences. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffL1MaskedGetBufferHostSize()"/></param>
        public void NormDiff_L1(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffL1MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L1_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L1_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_L1. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L1-norm of differences. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        public void NormDiff_L1(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormDiffL1MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L1_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L1_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffL2GetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffL2GetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_L2.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L2-norm of differences. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffL2MaskedGetBufferHostSize()"/></param>
        public void NormDiff_L2(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffL2MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L2_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L2_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_L2. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L2-norm of differences. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        public void NormDiff_L2(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormDiffL2MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L2_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L2_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelInfGetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelInfGetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_Inf.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelInfMaskedGetBufferHostSize()"/></param>
        public void NormRel_Inf(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelInfMaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_Inf_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_Inf_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_Inf. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        public void NormRel_Inf(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormRelInfMaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_Inf_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_Inf_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelL1GetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelL1GetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_L1.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelL1MaskedGetBufferHostSize()"/></param>
        public void NormRel_L1(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelL1MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L1_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L1_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_L1. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        public void NormRel_L1(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormRelL1MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L1_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L1_32f_C1MR", status));
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
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelL2GetBufferHostSize_32f_C1MR(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelL2GetBufferHostSize_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_L2.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelL2MaskedGetBufferHostSize()"/></param>
        public void NormRel_L2(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, NPPImage_8uC1 pMask, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelL2MaskedGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L2_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L2_32f_C1MR", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_L2. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="pMask">Mask image.</param>
        public void NormRel_L2(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, NPPImage_8uC1 pMask)
        {
            SizeT bufferSize = NormRelL2MaskedGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L2_32f_C1MR(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, pMask.DevicePointerRoi, pMask.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L2_32f_C1MR", status));
            buffer.Dispose();
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
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffInfGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffInfGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_Inf.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed Inf-norm of differences. (1 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffInfGetBufferHostSize()"/></param>
        public void NormDiff_Inf(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffInfGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_Inf_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_Inf_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_Inf. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed Inf-norm of differences. (1 * sizeof(double))</param>
        public void NormDiff_Inf(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff)
        {
            SizeT bufferSize = NormDiffInfGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_Inf_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_Inf_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffL1GetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffL1GetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_L1.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L1-norm of differences. (1 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffL1GetBufferHostSize()"/></param>
        public void NormDiff_L1(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffL1GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L1_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L1_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_L1. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L1-norm of differences. (1 * sizeof(double))</param>
        public void NormDiff_L1(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff)
        {
            SizeT bufferSize = NormDiffL1GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L1_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L1_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiffL2GetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiffL2GetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormDiff_L2.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L2-norm of differences. (1 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormDiffL2GetBufferHostSize()"/></param>
        public void NormDiff_L2(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormDiffL2GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L2_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L2_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormDiff_L2. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormDiff">Pointer to the computed L2-norm of differences. (1 * sizeof(double))</param>
        public void NormDiff_L2(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormDiff)
        {
            SizeT bufferSize = NormDiffL2GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormDiff.nppiNormDiff_L2_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormDiff.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormDiff_L2_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelInfGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelInfGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_Inf.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelInfGetBufferHostSize()"/></param>
        public void NormRel_Inf(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelInfGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_Inf_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_Inf_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_Inf. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        public void NormRel_Inf(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel)
        {
            SizeT bufferSize = NormRelInfGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_Inf_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_Inf_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelL1GetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelL1GetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_L1.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelL1GetBufferHostSize()"/></param>
        public void NormRel_L1(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelL1GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L1_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L1_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_L1. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        public void NormRel_L1(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel)
        {
            SizeT bufferSize = NormRelL1GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L1_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L1_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.NormRel.nppiNormRelL2GetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRelL2GetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image NormRel_L2.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="NormRelL2GetBufferHostSize()"/></param>
        public void NormRel_L2(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = NormRelL2GetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L2_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L2_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image NormRel_L2. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="pNormRel">Pointer to the computed relative error for the infinity norm of two images. (1 * sizeof(double))</param>
        public void NormRel_L2(NPPImage_32fC1 tpl, CudaDeviceVariable<double> pNormRel)
        {
            SizeT bufferSize = NormRelL2GetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.NormRel.nppiNormRel_L2_32f_C1R(_devPtrRoi, _pitch, tpl.DevicePointerRoi, tpl.Pitch, _sizeRoi, pNormRel.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiNormRel_L2_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.ImageProximity.nppiFullNormLevelGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFullNormLevelGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrFull_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="FullNormLevelGetBufferHostSize()"/></param>
        public void CrossCorrFull_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = FullNormLevelGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevel_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevel_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrFull_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        public void CrossCorrFull_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            SizeT bufferSize = FullNormLevelGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevel_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevel_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSameNormLevelGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSameNormLevelGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrSame_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="SameNormLevelGetBufferHostSize()"/></param>
        public void CrossCorrSame_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = SameNormLevelGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevel_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevel_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrSame_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        public void CrossCorrSame_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            SizeT bufferSize = SameNormLevelGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevel_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevel_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.ImageProximity.nppiValidNormLevelGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiValidNormLevelGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// CrossCorrValid_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="ValidNormLevelGetBufferHostSize()"/></param>
        public void CrossCorrValid_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = ValidNormLevelGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevel_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevel_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrValid_NormLevel. Buffer is internally allocated and freed.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        public void CrossCorrValid_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            SizeT bufferSize = ValidNormLevelGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevel_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevel_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }








        /// <summary>
        /// image SqrDistanceFull_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void SqrDistanceFull_Norm(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceFull_Norm_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceFull_Norm_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image SqrDistanceSame_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void SqrDistanceSame_Norm(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceSame_Norm_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceSame_Norm_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image SqrDistanceValid_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void SqrDistanceValid_Norm(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiSqrDistanceValid_Norm_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSqrDistanceValid_Norm_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// image CrossCorrFull_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void CrossCorrFull_Norm(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_Norm_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_Norm_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image CrossCorrSame_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void CrossCorrSame_Norm(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_Norm_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_Norm_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image CrossCorrValid_Norm.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void CrossCorrValid_Norm(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_Norm_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_Norm_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image CrossCorrValid.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination-Image</param>
        public void CrossCorrValid(NPPImage_32fC1 tpl, NPPImage_32fC1 dst)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointerRoi, dst.Pitch);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.CountInRange.nppiCountInRangeGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCountInRangeGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image CountInRange.
        /// </summary>
        /// <param name="pCounts">Pointer to the number of pixels that fall into the specified range. (1 * sizeof(int))</param>
        /// <param name="nLowerBound">Lower bound of the specified range.</param>
        /// <param name="nUpperBound">Upper bound of the specified range.</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="CountInRangeGetBufferHostSize()"/></param>
        public void CountInRange(CudaDeviceVariable<int> pCounts, float nLowerBound, float nUpperBound, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = CountInRangeGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.CountInRange.nppiCountInRange_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, pCounts.DevicePointer, nLowerBound, nUpperBound, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCountInRange_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image CountInRange.
        /// </summary>
        /// <param name="pCounts">Pointer to the number of pixels that fall into the specified range. (1 * sizeof(int))</param>
        /// <param name="nLowerBound">Lower bound of the specified range.</param>
        /// <param name="nUpperBound">Upper bound of the specified range.</param>
        public void CountInRange(CudaDeviceVariable<int> pCounts, float nLowerBound, float nUpperBound)
        {
            SizeT bufferSize = CountInRangeGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.CountInRange.nppiCountInRange_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, pCounts.DevicePointer, nLowerBound, nUpperBound, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCountInRange_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.QualityIndex.nppiQualityIndexGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiQualityIndexGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }

        /// <summary>
        /// image QualityIndex.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dst">Pointer to the quality index. (1 * sizeof(float))</param>
        /// <param name="buffer">Allocated device memory with size of at <see cref="QualityIndexGetBufferHostSize()"/></param>
        public void QualityIndex(NPPImage_32fC1 src2, CudaDeviceVariable<float> dst, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = QualityIndexGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.QualityIndex.nppiQualityIndex_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, dst.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiQualityIndex_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image QualityIndex.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="dst">Pointer to the quality index. (1 * sizeof(float))</param>
        public void QualityIndex(NPPImage_32fC1 src2, CudaDeviceVariable<float> dst)
        {
            SizeT bufferSize = QualityIndexGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);

            status = NPPNativeMethods.NPPi.QualityIndex.nppiQualityIndex_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, dst.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiQualityIndex_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region MinMaxEveryNew
        /// <summary>
        /// image MinEvery
        /// </summary>
        /// <param name="src2">Source-Image</param>
        public void MinEvery(NPPImage_32fC1 src2)
        {
            status = NPPNativeMethods.NPPi.MinMaxEvery.nppiMinEvery_32f_C1IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMinEvery_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// image MaxEvery
        /// </summary>
        /// <param name="src2">Source-Image</param>
        public void MaxEvery(NPPImage_32fC1 src2)
        {
            status = NPPNativeMethods.NPPi.MinMaxEvery.nppiMaxEvery_32f_C1IR(src2.DevicePointerRoi, src2.Pitch, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaxEvery_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region DupNew
        /// <summary>
        /// source image duplicated in all 3 channels of destination image.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Dup(NPPImage_32fC3 dst)
        {
            status = NPPNativeMethods.NPPi.Dup.nppiDup_32f_C1C3R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDup_32f_C1C3R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// source image duplicated in all 4 channels of destination image.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void Dup(NPPImage_32fC4 dst)
        {
            status = NPPNativeMethods.NPPi.Dup.nppiDup_32f_C1C4R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDup_32f_C1C4R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// source image duplicated in 3 channels of 4 channel destination image with alpha channel unaffected.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        public void DupA(NPPImage_32fC4 dst)
        {
            status = NPPNativeMethods.NPPi.Dup.nppiDup_32f_C1AC4R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDup_32f_C1AC4R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Transpose
        /// <summary>
        /// image transpose
        /// </summary>
        /// <param name="dest">Destination image</param>
        public void Transpose(NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.Transpose.nppiTranspose_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiTranspose_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region GeometryNew

        /// <summary>
        /// image conversion.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="nMin">specifies the minimum saturation value to which every output value will be clamped.</param>
        /// <param name="nMax">specifies the maximum saturation value to which every output value will be clamped.</param>
        public void Scale(NPPImage_32fC1 dst, float nMin, float nMax)
        {
            NppiRect srcRect = new NppiRect(_pointRoi, _sizeRoi);
            status = NPPNativeMethods.NPPi.Scale.nppiScale_32f8u_C1R(_devPtrRoi, _pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, nMin, nMax);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiScale_32f8u_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region RectStdDev


        /// <summary>
        ///  RectStdDev.
        /// </summary>
        /// <param name="dst">Destination-Image</param>
        /// <param name="sqr">Destination-Image</param>
        /// <param name="oRect">rectangular window</param>
        public void RectStdDev(NPPImage_32fC1 dst, NPPImage_32fC1 sqr, NppiRect oRect)
        {
            status = NPPNativeMethods.NPPi.Integral.nppiRectStdDev_32f_C1R(_devPtrRoi, _pitch, sqr.DevicePointerRoi, sqr.Pitch, dst.DevicePointerRoi, dst.Pitch, _sizeRoi, oRect);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiRectStdDev_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        //New in Cuda 6.0

        #region MaxError
        /// <summary>
        /// image maximum error. User buffer is internally allocated and freed.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        public void MaxError(NPPImage_32fC1 src2, CudaDeviceVariable<double> pError)
        {
            SizeT bufferSize = MaxErrorGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);
            status = NPPNativeMethods.NPPi.MaximumError.nppiMaximumError_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumError_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image maximum error.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        /// <param name="buffer">Pointer to the user-allocated scratch buffer required for the MaxError operation.</param>
        public void MaxError(NPPImage_32fC1 src2, CudaDeviceVariable<double> pError, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MaxErrorGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MaximumError.nppiMaximumError_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumError_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Device scratch buffer size (in bytes) for MaxError.
        /// </summary>
        /// <returns></returns>
        public SizeT MaxErrorGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MaximumError.nppiMaximumErrorGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumErrorGetBufferHostSize_32f_C1R", status));
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
        public void AverageError(NPPImage_32fC1 src2, CudaDeviceVariable<double> pError)
        {
            SizeT bufferSize = AverageErrorGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);
            status = NPPNativeMethods.NPPi.AverageError.nppiAverageError_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageError_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image average error.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        /// <param name="buffer">Pointer to the user-allocated scratch buffer required for the AverageError operation.</param>
        public void AverageError(NPPImage_32fC1 src2, CudaDeviceVariable<double> pError, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = AverageErrorGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.AverageError.nppiAverageError_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageError_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Device scratch buffer size (in bytes) for AverageError.
        /// </summary>
        /// <returns></returns>
        public SizeT AverageErrorGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.AverageError.nppiAverageErrorGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageErrorGetBufferHostSize_32f_C1R", status));
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
        public void MaximumRelativeError(NPPImage_32fC1 src2, CudaDeviceVariable<double> pError)
        {
            SizeT bufferSize = MaximumRelativeErrorGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);
            status = NPPNativeMethods.NPPi.MaximumRelativeError.nppiMaximumRelativeError_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumRelativeError_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image maximum relative error.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        /// <param name="buffer">Pointer to the user-allocated scratch buffer required for the MaximumRelativeError operation.</param>
        public void MaximumRelativeError(NPPImage_32fC1 src2, CudaDeviceVariable<double> pError, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = MaximumRelativeErrorGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.MaximumRelativeError.nppiMaximumRelativeError_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumRelativeError_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Device scratch buffer size (in bytes) for MaximumRelativeError.
        /// </summary>
        /// <returns></returns>
        public SizeT MaximumRelativeErrorGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.MaximumRelativeError.nppiMaximumRelativeErrorGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMaximumRelativeErrorGetBufferHostSize_32f_C1R", status));
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
        public void AverageRelativeError(NPPImage_32fC1 src2, CudaDeviceVariable<double> pError)
        {
            SizeT bufferSize = AverageRelativeErrorGetBufferHostSize();
            CudaDeviceVariable<byte> buffer = new CudaDeviceVariable<byte>(bufferSize);
            status = NPPNativeMethods.NPPi.AverageRelativeError.nppiAverageRelativeError_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageRelativeError_32f_C1R", status));
            buffer.Dispose();
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// image average relative error.
        /// </summary>
        /// <param name="src2">2nd source image</param>
        /// <param name="pError">Pointer to the computed error.</param>
        /// <param name="buffer">Pointer to the user-allocated scratch buffer required for the AverageRelativeError operation.</param>
        public void AverageRelativeError(NPPImage_32fC1 src2, CudaDeviceVariable<double> pError, CudaDeviceVariable<byte> buffer)
        {
            SizeT bufferSize = AverageRelativeErrorGetBufferHostSize();
            if (bufferSize > buffer.Size) throw new NPPException("Provided buffer is too small.");

            status = NPPNativeMethods.NPPi.AverageRelativeError.nppiAverageRelativeError_32f_C1R(_devPtrRoi, _pitch, src2.DevicePointerRoi, src2.Pitch, _sizeRoi, pError.DevicePointer, buffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageRelativeError_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Device scratch buffer size (in bytes) for AverageRelativeError.
        /// </summary>
        /// <returns></returns>
        public SizeT AverageRelativeErrorGetBufferHostSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.AverageRelativeError.nppiAverageRelativeErrorGetBufferHostSize_32f_C1R(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAverageRelativeErrorGetBufferHostSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        #endregion

        #region ColorTwist
        /// <summary>
        /// An input color twist matrix with floating-point pixel values is applied
        /// within ROI.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="twistMatrix">The color twist matrix with floating-point pixel values [3,4].</param>
        public void ColorTwist(NPPImage_32fC1 dest, float[,] twistMatrix)
        {
            status = NPPNativeMethods.NPPi.ColorProcessing.nppiColorTwist_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, twistMatrix);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwist_32f_C1R", status));
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
            status = NPPNativeMethods.NPPi.ColorProcessing.nppiColorTwist_32f_C1IR(_devPtrRoi, _pitch, _sizeRoi, aTwist);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwist_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
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
            status = NPPNativeMethods.NPPi.FixedFilters.nppiFilterUnsharpGetBufferSize_32f_C1R(nRadius, nSigma, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterUnsharpGetBufferSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        #endregion



        //New in Cuda 8.0

        #region FilterGaussAdvancedBorder


        /// <summary>
        /// Calculate destination image SizeROI width and height from source image ROI width and height and downsampling rate.
        /// It is highly recommended that this function be use to determine the destination image ROI for consistent results.
        /// </summary>
        /// <param name="nRate">The downsampling rate to be used.  For integer equivalent rates unnecessary source pixels are just skipped. For non-integer rates the source image is bilinear interpolated. nRate must be > 1.0F and &lt;= 10.0F. </param>
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
        /// <param name="nRate">The downsampling rate to be used.  For integer equivalent rates unnecessary source pixels are just skipped. For non-integer rates the source image is bilinear interpolated. nRate must be > 1.0F and &lt;= 10.0F. </param>
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
        #region New in Cuda 9.0
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
            NppStatus status = NPPNativeMethods.NPPi.ColorTwistBatch.nppiColorTwistBatch_32f_C1R(nMin, nMax, oSizeROI, pBatchList.DevicePointer, pBatchList.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwistBatch_32f_C1R", status));
            NPPException.CheckNppStatus(status, pBatchList);
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
        public static void ColorTwistBatchI(float nMin, float nMax, NppiSize oSizeROI, CudaDeviceVariable<NppiColorTwistBatchCXR> pBatchList)
        {
            NppStatus status = NPPNativeMethods.NPPi.ColorTwistBatch.nppiColorTwistBatch_32f_C1IR(nMin, nMax, oSizeROI, pBatchList.DevicePointer, pBatchList.Size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiColorTwistBatch_32f_C1IR", status));
            NPPException.CheckNppStatus(status, pBatchList);
        }


        /// <summary>
        /// Gray scale dilation with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="Mask">Pointer to the start address of the mask array.</param>
        /// <param name="aMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void GrayDilateBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.DilationWithBorderControl.nppiGrayDilateBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, Mask.DevicePointer, aMaskSize, oAnchor, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGrayDilateBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// Gray scale erosion with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="Mask">Pointer to the start address of the mask array.</param>
        /// <param name="aMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void GrayErodeBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> Mask, NppiSize aMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ErosionWithBorderControl.nppiGrayErodeBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, Mask.DevicePointer, aMaskSize, oAnchor, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiGrayErodeBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }




        /// <summary>
        /// Calculate scratch buffer size needed for 1 channel 32-bit floating point MorphCloseBorder, MorphOpenBorder, MorphTopHatBorder, 
        /// MorphBlackHatBorder, or MorphGradientBorder function based on destination image oSizeROI width and height.
        /// </summary>
        /// <returns>Required buffer size in bytes.</returns>
        public int MorphGetBufferSize()
        {
            int ret = 0;
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphGetBufferSize_32f_C1R(_sizeRoi, ref ret);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphGetBufferSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return ret;
        }




        /// <summary>
        /// 1 channel 32-bit floating point morphological close with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphCloseBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphCloseBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphCloseBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// 1 channel 32-bit floating point morphological open with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphOpenBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphOpenBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphOpenBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 1 channel 32-bit floating point morphological top hat with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphTopHatBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphTopHatBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphTopHatBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 1 channel 32-bit floating point morphological black hat with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphBlackHatBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphBlackHatBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphBlackHatBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 1 channel 32-bit floating point morphological gradient with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="pMask">Pointer to the start address of the mask array</param>
        /// <param name="oMaskSize">Width and Height mask array.</param>
        /// <param name="oAnchor">X and Y offsets of the mask origin frame of reference w.r.t the source pixel.</param>
        /// <param name="pBuffer">Pointer to device memory scratch buffer at least as large as value returned by the corresponding MorphGetBufferSize call.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void MorphGradientBorder(NPPImage_32fC1 dest, CudaDeviceVariable<byte> pMask, NppiSize oMaskSize, NppiPoint oAnchor, CudaDeviceVariable<byte> pBuffer, NppiBorderType eBorderType, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ComplexImageMorphology.nppiMorphGradientBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi, dest.Pitch, dest.SizeRoi, pMask.DevicePointer, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMorphGradientBorder_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region new in Cuda 11.4

        /// <summary>
        /// Scratch-buffer size for SignedDistanceTransformPBA.
        /// </summary>
        /// <returns></returns>
        public SizeT SignedDistanceTransformPBAGetBufferSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiSignedDistanceTransformPBAGetBufferSize(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSignedDistanceTransformPBAGetBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }


#if ADD_MISSING_CTX
        /// <summary>
		/// 1 channel 32-bit floating point grayscale to optional 1 channel 16-bit signed integer euclidean distance voronoi diagram 
		/// and 32-bit floating point transform with optional sub-pixel shifts. 
		/// <para/>
		/// For this particular version of the function acceptable input pixel intensities are less than or equal to 0.0f for those fully outside of connected 
		/// pixel regions, intensities with fractional parts between 0.0f and 1.0f representing the percentage of connected pixel region sub-pixel coverage within a 
		/// particular pixel (region contour), and intensities greater than or equal to 1.0f for pixels that are fully contained within closed connected pixel regions. 
		/// This function executes in two passes, the first pass prioritizes pixels outside of closed regions, the second pass 
		/// prioritizes pixels within closed regions.  The two passes are then merged on output. The function assumes that fully 
		/// covered pixels have centers located at sub-pixel locations of .5,.5 . 
        /// </summary>
		/// <param name="nCutoffValue">source image pixel values &lt; nCutoffValue will be considered fully outside of pixel regions (and set to -1).</param>
		/// <param name="nSubPixelXShift">final transform distances will be shifted in the X direction by this sub-pixel fraction. </param>
		/// <param name="nSubPixelYShift">final transform distances will be shifted in the Y direction by this sub-pixel fraction. </param>
		/// <param name="pDstVoronoi">device memory voronoi diagram destination_image_pointer or NULL for no voronoi output.</param>
		/// <param name="pDstVoronoiIndices">device memory voronoi diagram destination_image_pointer or NULL for no voronoi indices output.</param>
		/// <param name="pDstVoronoiManhattanRelativeDistances">device memory voronoi relative Manhattan distances destination_image_pointer or NULL for no voronoi Manhattan output.</param>
		/// <param name="pDstTransform">device memory true euclidean distance transform destination_image_pointer or NULL for no transform output.</param>
		/// <param name="pBuffer">pointer to scratch DEVICE memory buffer of size hpBufferSize (see nppiSignedDistanceTransformPBAGetBufferSize() above)</param>
        public void SignedDistanceTransformPBA(float nCutoffValue,
                                           float nSubPixelXShift, float nSubPixelYShift, NPPImage_16sC1 pDstVoronoi, NPPImage_16sC1 pDstVoronoiIndices,
                                                        NPPImage_16sC1 pDstVoronoiManhattanRelativeDistances, NPPImage_32fC1 pDstTransform, CudaDeviceVariable<byte> pBuffer)
        {
            CUdeviceptr dstVoronoi = new CUdeviceptr();
            CUdeviceptr dstTransform = new CUdeviceptr();
            CUdeviceptr dstVoronoiIndices = new CUdeviceptr();
            CUdeviceptr dstVoronoiManhattenDistances = new CUdeviceptr();
            int pitchVoronoi = 0;
            int pitchTransform = 0;
            int pitchVoronoiIndices = 0;
            int pitchVoronoiManhattenDistances = 0;

            if (pDstVoronoi != null)
            {
                dstVoronoi = pDstVoronoi.DevicePointerRoi;
                pitchVoronoi = pDstVoronoi.Pitch;
            }
            if (pDstTransform != null)
            {
                dstTransform = pDstTransform.DevicePointerRoi;
                pitchTransform = pDstTransform.Pitch;
            }
            if (pDstVoronoiIndices != null)
            {
                dstVoronoiIndices = pDstVoronoiIndices.DevicePointerRoi;
                pitchVoronoiIndices = pDstVoronoiIndices.Pitch;
            }
            if (pDstVoronoiManhattanRelativeDistances != null)
            {
                dstVoronoiManhattenDistances = pDstVoronoiManhattanRelativeDistances.DevicePointerRoi;
                pitchVoronoiManhattenDistances = pDstVoronoiManhattanRelativeDistances.Pitch;
            }

            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiSignedDistanceTransformPBA_32f_C1R(_devPtrRoi, _pitch, nCutoffValue, nSubPixelXShift, nSubPixelYShift, dstVoronoi, pitchVoronoi, dstVoronoiIndices, pitchVoronoiIndices, dstVoronoiManhattenDistances, pitchVoronoiManhattenDistances, dstTransform, pitchTransform, _sizeRoi, pBuffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSignedDistanceTransformPBA_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

#endif
        #endregion


        #region New in Cuda 12.0
        /// <summary>
        /// Scratch-buffer size for SignedDistanceTransformPBA 64 bit floating point output.
        /// </summary>
        /// <returns></returns>
        public SizeT SignedDistanceTransformPBAGet64BufferSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiSignedDistanceTransformPBAGet64fBufferSize(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSignedDistanceTransformPBAGet64fBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Scratch-buffer size for DistanceTransformPBA.
        /// </summary>
        /// <returns></returns>
        public SizeT DistanceTransformPBAGetBufferSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiDistanceTransformPBAGetBufferSize(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDistanceTransformPBAGetBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Calculate scratch buffer size needed for the DistanceTransformPBA function based antialiasing on destination image SizeROI width and height.
        /// </summary>
        /// <returns></returns>
        public SizeT DistanceTransformPBAGetAntialiasingBufferSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiDistanceTransformPBAGetAntialiasingBufferSize(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDistanceTransformPBAGetAntialiasingBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        /// <summary>
        /// Calculate scratch buffer size needed for the SignedDistanceTransformPBA function based antialiasing on destination image SizeROI width and height.
        /// </summary>
        /// <returns></returns>
        public SizeT SignedDistanceTransformPBAGetAntialiasingBufferSize()
        {
            SizeT bufferSize = 0;
            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiSignedDistanceTransformPBAGetAntialiasingBufferSize(_sizeRoi, ref bufferSize);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSignedDistanceTransformPBAGetAntialiasingBufferSize", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
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
            status = NPPNativeMethods.NPPi.ImageMedianFilter.nppiFilterMedianBorderGetBufferSize_32f_C1R(_sizeRoi, oMaskSize, ref bufferSize, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterMedianBorderGetBufferSize_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
            return bufferSize;
        }
        #region Add
        /// <summary>
        /// Add constant to image.
        /// </summary>
        /// <param name="nConstant">Value to add</param>
        /// <param name="dest">Destination image</param>
        public void Add(CudaDeviceVariable<float> nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.AddDeviceConst.nppiAddDeviceC_32f_C1R(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddDeviceC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Add constant to image. Inplace.
        /// </summary>
        /// <param name="nConstant">Value to add</param>
        public void Add(CudaDeviceVariable<float> nConstant)
        {
            status = NPPNativeMethods.NPPi.AddDeviceConst.nppiAddDeviceC_32f_C1IR(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAddDeviceC_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Sub

        /// <summary>
        /// Subtract constant to image.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        /// <param name="dest">Destination image</param>
        public void Sub(CudaDeviceVariable<float> nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.SubDeviceConst.nppiSubDeviceC_32f_C1R(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSubDeviceC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Subtract constant to image. Inplace.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        public void Sub(CudaDeviceVariable<float> nConstant)
        {
            status = NPPNativeMethods.NPPi.SubDeviceConst.nppiSubDeviceC_32f_C1IR(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSubDeviceC_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Mul

        /// <summary>
        /// Multiply constant to image.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        public void Mul(CudaDeviceVariable<float> nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.MulDeviceConst.nppiMulDeviceC_32f_C1R(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulDeviceC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Multiply constant to image. Inplace.
        /// </summary>
        /// <param name="nConstant">Value</param>
        public void Mul(CudaDeviceVariable<float> nConstant)
        {
            status = NPPNativeMethods.NPPi.MulDeviceConst.nppiMulDeviceC_32f_C1IR(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiMulDeviceC_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region Div

        /// <summary>
        /// Divide constant to image.
        /// </summary>
        /// <param name="nConstant">Value</param>
        /// <param name="dest">Destination image</param>
        public void Div(CudaDeviceVariable<float> nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.DivDeviceConst.nppiDivDeviceC_32f_C1R(_devPtrRoi, _pitch, nConstant.DevicePointer, dest.DevicePointerRoi, dest.Pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDivDeviceC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// Divide constant to image. Inplace.
        /// </summary>
        /// <param name="nConstant">Value</param>
        public void Div(CudaDeviceVariable<float> nConstant)
        {
            status = NPPNativeMethods.NPPi.DivDeviceConst.nppiDivDeviceC_32f_C1IR(nConstant.DevicePointer, _devPtrRoi, _pitch, _sizeRoi);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDivDeviceC_32f_C1IR", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion

        #region AbsDiff

        /// <summary>
        /// Absolute difference with constant.
        /// </summary>
        /// <param name="nConstant">Value to subtract</param>
        /// <param name="dest">Destination image</param>
        public void AbsDiff(CudaDeviceVariable<float> nConstant, NPPImage_32fC1 dest)
        {
            status = NPPNativeMethods.NPPi.AbsDiffDeviceConst.nppiAbsDiffDeviceC_32f_C1R(_devPtrRoi, _pitch, dest.DevicePointerRoi, dest.Pitch, _sizeRoi, nConstant.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiAbsDiffDeviceC_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        #endregion


        /// <summary>
        /// 1 channel 32-bit floating point grayscale to optional 1 channel 16-bit signed integer euclidean distance voronoi diagram 
        /// and 32-bit floating point transform with optional sub-pixel shifts. 
        /// <para/>
        /// For this particular version of the function acceptable input pixel intensities are less than or equal to 0.0f for those fully outside of connected 
        /// pixel regions, intensities with fractional parts between 0.0f and 1.0f representing the percentage of connected pixel region sub-pixel coverage within a 
        /// particular pixel (region contour), and intensities greater than or equal to 1.0f for pixels that are fully contained within closed connected pixel regions. 
        /// This function executes in two passes, the first pass prioritizes pixels outside of closed regions, the second pass 
        /// prioritizes pixels within closed regions.  The two passes are then merged on output. The function assumes that fully 
        /// covered pixels have centers located at sub-pixel locations of .5,.5. In general, object exterior distances are output as negative 
        /// numbers progressing to positive and object interior distances are output as positive numbers progressing to negative. 
        /// </summary>
        /// <param name="nCutoffValue">source image pixel values &lt; nCutoffValue will be considered fully outside of pixel regions (and set to -1).</param>
        /// <param name="nSubPixelXShift">final transform distances will be shifted in the X direction by this sub-pixel fraction. </param>
        /// <param name="nSubPixelYShift">final transform distances will be shifted in the Y direction by this sub-pixel fraction. </param>
        /// <param name="pDstVoronoi">device memory voronoi diagram destination_image_pointer or NULL for no voronoi output.</param>
        /// <param name="pDstVoronoiIndices">device memory voronoi diagram destination_image_pointer or NULL for no voronoi indices output.</param>
        /// <param name="pDstVoronoiAbsoluteManhattanDistances">device memory voronoi relative Manhattan distances destination_image_pointer or NULL for no voronoi Manhattan output.</param>
        /// <param name="pDstTransform">device memory true euclidean distance transform destination_image_pointer or NULL for no transform output.</param>
        /// <param name="pBuffer">pointer to scratch DEVICE memory buffer of size hpBufferSize (see nppiSignedDistanceTransformPBAGetBufferSize() above)</param>
        public void SignedDistanceTransformAbsPBA(float nCutoffValue, float nSubPixelXShift, float nSubPixelYShift, NPPImage_16sC1 pDstVoronoi, NPPImage_16sC1 pDstVoronoiIndices,
                                                  NPPImage_16sC1 pDstVoronoiAbsoluteManhattanDistances, NPPImage_32fC1 pDstTransform, CudaDeviceVariable<byte> pBuffer)
        {
            CUdeviceptr dstVoronoi = new CUdeviceptr();
            CUdeviceptr dstTransform = new CUdeviceptr();
            CUdeviceptr dstVoronoiIndices = new CUdeviceptr();
            CUdeviceptr dstVoronoiManhattenDistances = new CUdeviceptr();
            int pitchVoronoi = 0;
            int pitchTransform = 0;
            int pitchVoronoiIndices = 0;
            int pitchVoronoiManhattenDistances = 0;

            if (pDstVoronoi != null)
            {
                dstVoronoi = pDstVoronoi.DevicePointerRoi;
                pitchVoronoi = pDstVoronoi.Pitch;
            }
            if (pDstTransform != null)
            {
                dstTransform = pDstTransform.DevicePointerRoi;
                pitchTransform = pDstTransform.Pitch;
            }
            if (pDstVoronoiIndices != null)
            {
                dstVoronoiIndices = pDstVoronoiIndices.DevicePointerRoi;
                pitchVoronoiIndices = pDstVoronoiIndices.Pitch;
            }
            if (pDstVoronoiAbsoluteManhattanDistances != null)
            {
                dstVoronoiManhattenDistances = pDstVoronoiAbsoluteManhattanDistances.DevicePointerRoi;
                pitchVoronoiManhattenDistances = pDstVoronoiAbsoluteManhattanDistances.Pitch;
            }

            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiSignedDistanceTransformAbsPBA_32f_C1R(_devPtrRoi, _pitch, nCutoffValue, nSubPixelXShift, nSubPixelYShift, dstVoronoi, pitchVoronoi, dstVoronoiIndices, pitchVoronoiIndices, dstVoronoiManhattenDistances, pitchVoronoiManhattenDistances, dstTransform, pitchTransform, _sizeRoi, pBuffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSignedDistanceTransformAbsPBA_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// 1 channel 32-bit floating point grayscale to optional 1 channel 16-bit signed integer euclidean distance voronoi diagram 
        /// and 64-bit floating point transform with optional sub-pixel shifts.
        /// <para/>
        /// For this particular version of the function acceptable input pixel intensities are less than or equal to 0.0f for those fully outside of connected 
        /// pixel regions, intensities with fractional parts between 0.0f and 1.0f representing the percentage of connected pixel region sub-pixel coverage within a 
        /// particular pixel (region contour), and intensities greater than or equal to 1.0f for pixels that are fully contained within closed connected pixel regions. 
        /// This function executes in two passes, the first pass prioritizes pixels outside of closed regions, the second pass 
        /// prioritizes pixels within closed regions.  The two passes are then merged on output. The function assumes that fully 
        /// covered pixels have centers located at sub-pixel locations of .5,.5. In general, object exterior distances are output as negative 
        /// numbers progressing to positive and object interior distances are output as positive numbers progressing to negative.
        /// </summary>
        /// <param name="nCutoffValue">source image pixel values &lt; nCutoffValue will be considered fully outside of pixel regions (and set to -1).</param>
        /// <param name="nSubPixelXShift">final transform distances will be shifted in the X direction by this sub-pixel fraction. </param>
        /// <param name="nSubPixelYShift">final transform distances will be shifted in the Y direction by this sub-pixel fraction. </param>
        /// <param name="pDstVoronoi">device memory voronoi diagram destination_image_pointer or NULL for no voronoi output.</param>
        /// <param name="pDstVoronoiIndices">device memory voronoi diagram destination_image_pointer or NULL for no voronoi indices output.</param>
        /// <param name="pDstVoronoiManhattanRelativeDistances">device memory voronoi relative Manhattan distances destination_image_pointer or NULL for no voronoi Manhattan output.</param>
        /// <param name="pDstTransform">device memory true euclidean distance transform destination_image_pointer or NULL for no transform output.</param>
        /// <param name="pBuffer">pointer to scratch DEVICE memory buffer of size hpBufferSize (see nppiSignedDistanceTransformPBAGet64fBufferSize() above)</param>
        /// <param name="pAntialiasingDeviceBuffer">pointer to scratch DEVICE memory buffer of size hpAntialiasingBufferSize (see nppiSignedDistanceTransformPBAGetAntialiasingBufferSize() above) or NULL if not Antialiasing</param>
        public void SignedDistanceTransformPBA(float nCutoffValue,
                                           double nSubPixelXShift, double nSubPixelYShift, NPPImage_16sC1 pDstVoronoi, NPPImage_16sC1 pDstVoronoiIndices,
                                                        NPPImage_16sC1 pDstVoronoiManhattanRelativeDistances, NPPImage_64fC1 pDstTransform, CudaDeviceVariable<byte> pBuffer, CudaDeviceVariable<byte> pAntialiasingDeviceBuffer)
        {
            CUdeviceptr dstVoronoi = new CUdeviceptr();
            CUdeviceptr dstTransform = new CUdeviceptr();
            CUdeviceptr dstVoronoiIndices = new CUdeviceptr();
            CUdeviceptr dstVoronoiManhattenDistances = new CUdeviceptr();
            CUdeviceptr antiAlias = new CUdeviceptr();
            int pitchVoronoi = 0;
            int pitchTransform = 0;
            int pitchVoronoiIndices = 0;
            int pitchVoronoiManhattenDistances = 0;

            if (pDstVoronoi != null)
            {
                dstVoronoi = pDstVoronoi.DevicePointerRoi;
                pitchVoronoi = pDstVoronoi.Pitch;
            }
            if (pDstTransform != null)
            {
                dstTransform = pDstTransform.DevicePointerRoi;
                pitchTransform = pDstTransform.Pitch;
            }
            if (pDstVoronoiIndices != null)
            {
                dstVoronoiIndices = pDstVoronoiIndices.DevicePointerRoi;
                pitchVoronoiIndices = pDstVoronoiIndices.Pitch;
            }
            if (pDstVoronoiManhattanRelativeDistances != null)
            {
                dstVoronoiManhattenDistances = pDstVoronoiManhattanRelativeDistances.DevicePointerRoi;
                pitchVoronoiManhattenDistances = pDstVoronoiManhattanRelativeDistances.Pitch;
            }
            if (pAntialiasingDeviceBuffer != null)
            {
                antiAlias = pAntialiasingDeviceBuffer.DevicePointer;
            }

            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiSignedDistanceTransformPBA_32f64f_C1R(_devPtrRoi, _pitch, nCutoffValue, nSubPixelXShift, nSubPixelYShift, dstVoronoi, pitchVoronoi, dstVoronoiIndices, pitchVoronoiIndices, dstVoronoiManhattenDistances, pitchVoronoiManhattenDistances, dstTransform, pitchTransform, _sizeRoi, pBuffer.DevicePointer, antiAlias);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSignedDistanceTransformPBA_32f64f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// 1 channel 32-bit floating point grayscale to optional 1 channel 16-bit signed integer euclidean distance voronoi diagram 
        /// and 64-bit floating point transform with optional sub-pixel shifts. 
        /// <para/>
        /// For this particular version of the function acceptable input pixel intensities are less than or equal to 0.0f for those fully outside of connected 
        /// pixel regions, intensities with fractional parts between 0.0f and 1.0f representing the percentage of connected pixel region sub-pixel coverage within a 
        /// particular pixel (region contour), and intensities greater than or equal to 1.0f for pixels that are fully contained within closed connected pixel regions. 
        /// This function executes in two passes, the first pass prioritizes pixels outside of closed regions, the second pass 
        /// prioritizes pixels within closed regions.  The two passes are then merged on output. The function assumes that fully 
        /// covered pixels have centers located at sub-pixel locations of .5,.5. In general, object exterior distances are output as negative 
        /// numbers progressing to positive and object interior distances are output as positive numbers progressing to negative. 
        /// </summary>
        /// <param name="nCutoffValue">source image pixel values &lt; nCutoffValue will be considered fully outside of pixel regions (and set to -1).</param>
        /// <param name="nSubPixelXShift">final transform distances will be shifted in the X direction by this sub-pixel fraction. </param>
        /// <param name="nSubPixelYShift">final transform distances will be shifted in the Y direction by this sub-pixel fraction. </param>
        /// <param name="pDstVoronoi">device memory voronoi diagram destination_image_pointer or NULL for no voronoi output.</param>
        /// <param name="pDstVoronoiIndices">device memory voronoi diagram destination_image_pointer or NULL for no voronoi indices output.</param>
        /// <param name="pDstVoronoiAbsoluteManhattanDistances">device memory voronoi relative Manhattan distances destination_image_pointer or NULL for no voronoi Manhattan output.</param>
        /// <param name="pDstTransform">device memory true euclidean distance transform destination_image_pointer or NULL for no transform output.</param>
        /// <param name="pBuffer">pointer to scratch DEVICE memory buffer of size hpBufferSize (see nppiSignedDistanceTransformPBAGet64fBufferSize() above)</param>
        /// <param name="pAntialiasingDeviceBuffer">pointer to scratch DEVICE memory buffer of size hpAntialiasingBufferSize (see nppiSignedDistanceTransformPBAGetAntialiasingBufferSize() above) or NULL if not Antialiasing</param>
        public void SignedDistanceTransformAbsPBA(float nCutoffValue, double nSubPixelXShift, double nSubPixelYShift, NPPImage_16sC1 pDstVoronoi, NPPImage_16sC1 pDstVoronoiIndices,
                                                  NPPImage_16sC1 pDstVoronoiAbsoluteManhattanDistances, NPPImage_64fC1 pDstTransform, CudaDeviceVariable<byte> pBuffer, CudaDeviceVariable<byte> pAntialiasingDeviceBuffer)
        {
            CUdeviceptr dstVoronoi = new CUdeviceptr();
            CUdeviceptr dstTransform = new CUdeviceptr();
            CUdeviceptr dstVoronoiIndices = new CUdeviceptr();
            CUdeviceptr dstVoronoiManhattenDistances = new CUdeviceptr();
            CUdeviceptr antiAlias = new CUdeviceptr();
            int pitchVoronoi = 0;
            int pitchTransform = 0;
            int pitchVoronoiIndices = 0;
            int pitchVoronoiManhattenDistances = 0;

            if (pDstVoronoi != null)
            {
                dstVoronoi = pDstVoronoi.DevicePointerRoi;
                pitchVoronoi = pDstVoronoi.Pitch;
            }
            if (pDstTransform != null)
            {
                dstTransform = pDstTransform.DevicePointerRoi;
                pitchTransform = pDstTransform.Pitch;
            }
            if (pDstVoronoiIndices != null)
            {
                dstVoronoiIndices = pDstVoronoiIndices.DevicePointerRoi;
                pitchVoronoiIndices = pDstVoronoiIndices.Pitch;
            }
            if (pDstVoronoiAbsoluteManhattanDistances != null)
            {
                dstVoronoiManhattenDistances = pDstVoronoiAbsoluteManhattanDistances.DevicePointerRoi;
                pitchVoronoiManhattenDistances = pDstVoronoiAbsoluteManhattanDistances.Pitch;
            }
            if (pAntialiasingDeviceBuffer != null)
            {
                antiAlias = pAntialiasingDeviceBuffer.DevicePointer;
            }

            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiSignedDistanceTransformAbsPBA_32f64f_C1R(_devPtrRoi, _pitch, nCutoffValue, nSubPixelXShift, nSubPixelYShift, dstVoronoi, pitchVoronoi, dstVoronoiIndices, pitchVoronoiIndices, dstVoronoiManhattenDistances, pitchVoronoiManhattenDistances, dstTransform, pitchTransform, _sizeRoi, pBuffer.DevicePointer, antiAlias);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiSignedDistanceTransformAbsPBA_32f64f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }



        /// <summary>
        /// 1 channel 32-bit floating point grayscale to optional 1 channel 16-bit signed integer euclidean distance voronoi diagram output and/or 
        /// optional 64-bit floating point transform with optional relative Manhattan distances.
        /// </summary>
        /// <param name="nMinSiteValue">source image pixel values >= nMinSiteValue and &lt;= nMaxSiteValue are considered sites (traditionally 0s)</param>
        /// <param name="nMaxSiteValue">source image pixel values >= nMinSiteValue and &lt;= nMaxSiteValue are considered sites (traditionally 0s)</param>
        /// <param name="pDstVoronoi">device memory voronoi diagram destination_image_pointer or NULL for no voronoi output.</param>
        /// <param name="pDstVoronoiIndices">device memory voronoi diagram destination_image_pointer or NULL for no voronoi indices output.</param>
        /// <param name="pDstVoronoiManhattanRelativeDistances">device memory voronoi relative Manhattan distances destination_image_pointer or NULL for no voronoi Manhattan output.</param>
        /// <param name="pDstTransform">device memory true euclidean distance transform destination_image_pointer or NULL for no transform output.</param>
        /// <param name="pBuffer">pointer to scratch DEVICE memory buffer of size hpBufferSize (see nppiDistanceTransformPBAGet64fBufferSize() above)</param>
        /// <param name="pAntialiasingDeviceBuffer">pointer to scratch DEVICE memory buffer of size hpAntialiasingBufferSize (see nppiDistanceTransformPBAGetAntialiasingBufferSize() above) or NULL if not Antialiasing</param>
        public void DistanceTransformPBA(float nMinSiteValue, float nMaxSiteValue, NPPImage_16sC1 pDstVoronoi, NPPImage_16sC1 pDstVoronoiIndices,
                                                        NPPImage_16sC1 pDstVoronoiManhattanRelativeDistances, NPPImage_64fC1 pDstTransform, CudaDeviceVariable<byte> pBuffer, CudaDeviceVariable<byte> pAntialiasingDeviceBuffer)
        {
            CUdeviceptr dstVoronoi = new CUdeviceptr();
            CUdeviceptr dstTransform = new CUdeviceptr();
            CUdeviceptr dstVoronoiIndices = new CUdeviceptr();
            CUdeviceptr dstVoronoiManhattenDistances = new CUdeviceptr();
            CUdeviceptr antiAlias = new CUdeviceptr();
            int pitchVoronoi = 0;
            int pitchTransform = 0;
            int pitchVoronoiIndices = 0;
            int pitchVoronoiManhattenDistances = 0;

            if (pDstVoronoi != null)
            {
                dstVoronoi = pDstVoronoi.DevicePointerRoi;
                pitchVoronoi = pDstVoronoi.Pitch;
            }
            if (pDstTransform != null)
            {
                dstTransform = pDstTransform.DevicePointerRoi;
                pitchTransform = pDstTransform.Pitch;
            }
            if (pDstVoronoiIndices != null)
            {
                dstVoronoiIndices = pDstVoronoiIndices.DevicePointerRoi;
                pitchVoronoiIndices = pDstVoronoiIndices.Pitch;
            }
            if (pDstVoronoiManhattanRelativeDistances != null)
            {
                dstVoronoiManhattenDistances = pDstVoronoiManhattanRelativeDistances.DevicePointerRoi;
                pitchVoronoiManhattenDistances = pDstVoronoiManhattanRelativeDistances.Pitch;
            }
            if (pAntialiasingDeviceBuffer != null)
            {
                antiAlias = pAntialiasingDeviceBuffer.DevicePointer;
            }

            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiDistanceTransformPBA_32f64f_C1R(_devPtrRoi, _pitch, nMinSiteValue, nMaxSiteValue, dstVoronoi, pitchVoronoi, dstVoronoiIndices, pitchVoronoiIndices, dstVoronoiManhattenDistances, pitchVoronoiManhattenDistances, dstTransform, pitchTransform, _sizeRoi, pBuffer.DevicePointer, antiAlias);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDistanceTransformPBA_32f64f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// 1 channel 32-bit floating point grayscale to optional 1 channel 16-bit signed integer euclidean distance voronoi diagram output and/or  
        /// optional 64-bit floating point transform with optional absolute Manhattan distances
        /// </summary>
        /// <param name="nMinSiteValue">source image pixel values >= nMinSiteValue and &lt;= nMaxSiteValue are considered sites (traditionally 0s)</param>
        /// <param name="nMaxSiteValue">source image pixel values >= nMinSiteValue and &lt;= nMaxSiteValue are considered sites (traditionally 0s)</param>
        /// <param name="pDstVoronoi">device memory voronoi diagram destination_image_pointer or NULL for no voronoi output.</param>
        /// <param name="pDstVoronoiIndices">device memory voronoi diagram destination_image_pointer or NULL for no voronoi indices output.</param>
        /// <param name="pDstVoronoiAbsoluteManhattanDistances">device memory voronoi relative Manhattan distances destination_image_pointer or NULL for no voronoi Manhattan output.</param>
        /// <param name="pDstTransform">device memory true euclidean distance transform destination_image_pointer or NULL for no transform output.</param>
        /// <param name="pBuffer">pointer to scratch DEVICE memory buffer of size hpBufferSize (see nppiDistanceTransformPBAGet64fBufferSize() above)</param>
        /// <param name="pAntialiasingDeviceBuffer">pointer to scratch DEVICE memory buffer of size hpAntialiasingBufferSize (see nppiDistanceTransformPBAGetAntialiasingBufferSize() above) or NULL if not Antialiasing</param>
        public void DistanceTransformAbsPBA(float nMinSiteValue, float nMaxSiteValue, NPPImage_16sC1 pDstVoronoi, NPPImage_16sC1 pDstVoronoiIndices,
                                                  NPPImage_16sC1 pDstVoronoiAbsoluteManhattanDistances, NPPImage_64fC1 pDstTransform, CudaDeviceVariable<byte> pBuffer, CudaDeviceVariable<byte> pAntialiasingDeviceBuffer)
        {
            CUdeviceptr dstVoronoi = new CUdeviceptr();
            CUdeviceptr dstTransform = new CUdeviceptr();
            CUdeviceptr dstVoronoiIndices = new CUdeviceptr();
            CUdeviceptr dstVoronoiManhattenDistances = new CUdeviceptr();
            CUdeviceptr antiAlias = new CUdeviceptr();
            int pitchVoronoi = 0;
            int pitchTransform = 0;
            int pitchVoronoiIndices = 0;
            int pitchVoronoiManhattenDistances = 0;

            if (pDstVoronoi != null)
            {
                dstVoronoi = pDstVoronoi.DevicePointerRoi;
                pitchVoronoi = pDstVoronoi.Pitch;
            }
            if (pDstTransform != null)
            {
                dstTransform = pDstTransform.DevicePointerRoi;
                pitchTransform = pDstTransform.Pitch;
            }
            if (pDstVoronoiIndices != null)
            {
                dstVoronoiIndices = pDstVoronoiIndices.DevicePointerRoi;
                pitchVoronoiIndices = pDstVoronoiIndices.Pitch;
            }
            if (pDstVoronoiAbsoluteManhattanDistances != null)
            {
                dstVoronoiManhattenDistances = pDstVoronoiAbsoluteManhattanDistances.DevicePointerRoi;
                pitchVoronoiManhattenDistances = pDstVoronoiAbsoluteManhattanDistances.Pitch;
            }
            if (pAntialiasingDeviceBuffer != null)
            {
                antiAlias = pAntialiasingDeviceBuffer.DevicePointer;
            }

            status = NPPNativeMethods.NPPi.FilterDistanceTransform.nppiDistanceTransformAbsPBA_32f64f_C1R(_devPtrRoi, _pitch, nMinSiteValue, nMaxSiteValue, dstVoronoi, pitchVoronoi, dstVoronoiIndices, pitchVoronoiIndices, dstVoronoiManhattenDistances, pitchVoronoiManhattenDistances, dstTransform, pitchTransform, _sizeRoi, pBuffer.DevicePointer, antiAlias);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiDistanceTransformAbsPBA_32f64f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }


        /// <summary>
        /// median filter with border control.
        /// </summary>
        /// <param name="dest">Destination image</param>
        /// <param name="oMaskSize">Width and Height of the neighborhood region for the local Avg operation.</param>
        /// <param name="oAnchor">X and Y offsets of the kernel origin frame of reference w.r.t the source pixel.</param>
        /// <param name="eBorderType">The border type operation to be applied at source image border boundaries.</param>
        /// <param name="pBuffer">Pointer to the user-allocated scratch buffer required for the Median operation.</param>
        /// <param name="filterArea">The area where the filter is allowed to read pixels. The point is relative to the ROI set to source image, the size is the total size starting from the filterArea point. Default value is the set ROI.</param>
        public void FilterMedianBorder(NPPImage_32fC1 dest, NppiSize oMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, CudaDeviceVariable<byte> pBuffer, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.ImageMedianFilter.nppiFilterMedianBorder_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi,
                                                    dest.Pitch, dest.SizeRoi, oMaskSize, oAnchor, pBuffer.DevicePointer, eBorderType);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterMedianBorder_32f_C1R", status));
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
        public void FilterBoxBorderAdvanced(NPPImage_32fC1 dest, NppiSize oMaskSize, NppiPoint oAnchor, NppiBorderType eBorderType, CudaDeviceVariable<byte> pBuffer, NppiRect filterArea = new NppiRect())
        {
            if (filterArea.Size == new NppiSize())
            {
                filterArea.Size = _sizeRoi;
            }
            status = NPPNativeMethods.NPPi.LinearFixedFilters2D.nppiFilterBoxBorderAdvanced_32f_C1R(_devPtrRoi, _pitch, filterArea.Size, filterArea.Location, dest.DevicePointerRoi,
                                                    dest.Pitch, dest.SizeRoi, oMaskSize, oAnchor, eBorderType, pBuffer.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiFilterBoxBorderAdvanced_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }

        /// <summary>
        /// CrossCorrFull_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Pointer to the required device memory allocation. </param>
        /// <param name="bufferAdvanced">Pointer to the required device memory allocation. See nppiCrossCorrFull_NormLevel_GetAdvancedScratchBufferSize</param>
        public void CrossCorrFull_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst, CudaDeviceVariable<byte> buffer, CudaDeviceVariable<byte> bufferAdvanced)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrFull_NormLevelAdvanced_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer, bufferAdvanced.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrFull_NormLevelAdvanced_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// CrossCorrSame_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Pointer to the required device memory allocation. </param>
        /// <param name="bufferAdvanced">Pointer to the required device memory allocation. See nppiCrossCorrSame_NormLevel_GetAdvancedScratchBufferSize</param>
        public void CrossCorrSame_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst, CudaDeviceVariable<byte> buffer, CudaDeviceVariable<byte> bufferAdvanced)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrSame_NormLevelAdvanced_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer, bufferAdvanced.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrSame_NormLevelAdvanced_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
        /// <summary>
        /// CrossCorrValid_NormLevel.
        /// </summary>
        /// <param name="tpl">template image.</param>
        /// <param name="dst">Destination image</param>
        /// <param name="buffer">Pointer to the required device memory allocation. </param>
        /// <param name="bufferAdvanced">Pointer to the required device memory allocation. See nppiCrossCorrValid_NormLevel_GetAdvancedScratchBufferSize</param>
        public void CrossCorrValid_NormLevel(NPPImage_32fC1 tpl, NPPImage_32fC1 dst, CudaDeviceVariable<byte> buffer, CudaDeviceVariable<byte> bufferAdvanced)
        {
            status = NPPNativeMethods.NPPi.ImageProximity.nppiCrossCorrValid_NormLevelAdvanced_32f_C1R(_devPtrRoi, _pitch, _sizeRoi, tpl.DevicePointerRoi, tpl.Pitch, tpl.SizeRoi, dst.DevicePointer, dst.Pitch, buffer.DevicePointer, bufferAdvanced.DevicePointer);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppiCrossCorrValid_NormLevelAdvanced_32f_C1R", status));
            NPPException.CheckNppStatus(status, this);
        }
#endif
        #endregion
    }
}
