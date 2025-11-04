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

namespace ManagedCuda.NPP.NPPsExtensions
{
    /// <summary>
    /// Extensions methods extending CudaDeviceVariable with NPPs features.
    /// </summary>
    public static class NPPsExtensionMethods
    {
        #region Filtering
        /// <summary>
        /// IntegralGetBufferSize
        /// </summary>
        public static SizeT IntegralGetBufferSize(this CudaDeviceVariable<int> devVar)
        {
            SizeT size = 0;
            NppStatus status = NPPNativeMethods.NPPs.FilteringFunctions.nppsIntegralGetBufferSize_32s(devVar.Size, ref size);
            Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "nppsIntegralGetBufferSize_32s", status));
            NPPException.CheckNppStatus(status, devVar);
            return size;
        }

        #endregion
    }
}
