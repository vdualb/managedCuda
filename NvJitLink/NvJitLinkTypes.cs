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


using System;
using System.Runtime.InteropServices;

namespace ManagedCuda.NvJitLink
{
    /// <summary>
    /// The enumerated type nvJitLinkResult defines API call result codes.
	/// nvJitLink APIs return nvJitLinkResult codes to indicate the result.
	/// </summary>
	public enum nvJitLinkResult
    {
        /// <summary/>
        Success = 0,
        /// <summary>
        /// Unrecognized Option
        /// </summary>
        ErrorUnrecognizedOption,
        /// <summary>
        /// -arch=sm_NN option not specified
        /// </summary>
        ErrorMissingArch,
        /// <summary>
        /// Invalid Input
        /// </summary>
        ErrorInvalidInput,
        /// <summary>
        /// Issue during PTX Compilation
        /// </summary>
        ErrorPtxCompile,
        /// <summary>
        /// Issue during NVVM Compilation
        /// </summary>
        ErrorNVVMCompile,
        /// <summary>
        /// Internal Error
        /// </summary>
        ErrorInternal,
        /// <summary>
        /// Issue with Thread Pool
        /// </summary>
        ErrorThreadPool,
        /// <summary>
        /// Unrecognized Input
        /// </summary>
        UnrecognizedInput,
        /// <summary>
        /// Finalizer Error
        /// </summary>
        ErrorFinalize,
        /// <summary>
        /// Null Input
        /// </summary>
        NullInput,
        /// <summary>
        /// Incompatible Options
        /// </summary>
        IncompatibleOptions,
        /// <summary>
        /// Incorrect Input Type
        /// </summary>
        IncorrectInputType,
        /// <summary>
        /// Arch Mismatch
        /// </summary>
        ArchMismatch,
        /// <summary>
        /// Outdated Library
        /// </summary>
        OutdatedLibrary,
        /// <summary>
        /// Missing Fatbin
        /// </summary>
        MissingFatBin,
        /// <summary>
        /// Unrecognized -arch value
        /// </summary>
        UnrecognizedArch,
        /// <summary>
        /// Unsupported -arch value
        /// </summary>
        UnsupportedArch,
        /// <summary>
        /// Requires -lto
        /// </summary>
        LTONotEnabled
    }

    /// <summary>
    /// The enumerated type nvJitLinkInputType defines the kind of inputs
    /// that can be passed to nvJitLinkAdd* APIs.
	/// </summary>
	public enum nvJitLinkInputType
    {
        /// <summary>
        /// Error Type
        /// </summary>
        None = 0,
        /// <summary>
        /// For CUDA Binaries
        /// </summary>
        Cubin = 1,
        /// <summary>
        /// For PTX
        /// </summary>
        Ptx,
        /// <summary>
        /// For LTO-IR
        /// </summary>
        LTOIR,
        /// <summary>
        /// For Fatbin
        /// </summary>
        FatBin,
        /// <summary>
        /// For Host Object
        /// </summary>
        Object,
        /// <summary>
        /// For Host Library
        /// </summary>
        Library,
        /// <summary>
        /// For Index File
        /// </summary>
        Index,
        /// <summary>
        /// Dynamically chooses from the valid types
        /// </summary>
        Any = 10
    }

    /// <summary>
    /// nvJitLinkHandle is the unit of linking, and an opaque handle for a program.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct nvJitLinkHandle
    {
        /// <summary>
        /// 
        /// </summary>
        public IntPtr Pointer;
    }
}
