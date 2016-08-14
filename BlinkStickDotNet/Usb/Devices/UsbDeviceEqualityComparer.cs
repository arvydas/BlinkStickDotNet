using System.Collections.Generic;

namespace BlinkStickDotNet.Usb
{

    /// <summary>
    /// Provides equality comparers for USB devices.
    /// </summary>
    public static class UsbDeviceEquality
    {
        private readonly static IEqualityComparer<IInternalUsbDevice> internalComparer = new InternalUsbDeviceEqualityComparer();
        private readonly static IEqualityComparer<IUsbDevice> comparer = new UsbDeviceEqualityComparer();

        /// <summary>
        /// Gets the internal comparer.
        /// </summary>
        /// <value>
        /// The internal comparer.
        /// </value>
        internal static IEqualityComparer<IInternalUsbDevice> InternalComparer
        {
            get { return internalComparer; }
        }

        /// <summary>
        /// Gets the comparer.
        /// </summary>
        /// <value>
        /// The comparer.
        /// </value>
        public static IEqualityComparer<IUsbDevice> Comparer
        {
            get { return comparer; }
        }
    }

    /// <summary>
    /// Compares internal IInternalUsbDevice objects.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{BlinkStickDotNet.Usb.IInternalUsbDevice}" />
    internal class InternalUsbDeviceEqualityComparer : IEqualityComparer<IInternalUsbDevice>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IInternalUsbDevice x, IInternalUsbDevice y)
        {
            return UsbDeviceEquality.Comparer.Equals(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(IInternalUsbDevice obj)
        {
            return UsbDeviceEquality.Comparer.GetHashCode(obj);
        }
    }

    /// <summary>
    /// Compares IUsbDevice objects.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{BlinkStickDotNet.Usb.IUsbDevice}" />
    internal class UsbDeviceEqualityComparer : IEqualityComparer<IUsbDevice>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsbDeviceEqualityComparer"/> class.
        /// </summary>
        internal UsbDeviceEqualityComparer()
        {
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IUsbDevice x, IUsbDevice y)
        {
            if (x == null)
            {
                return y == null;
            }

            if (y == null)
            {
                return false;

            }

            return
                x.Manufacturer == y.Manufacturer &&
                x.ProductId == y.ProductId &&
                x.ProductName == y.ProductName &&
                x.ProductVersion == y.ProductVersion &&
                x.SerialNumber == y.SerialNumber &&
                x.VendorId == y.VendorId &&
                x.Path == y.Path;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(IUsbDevice obj)
        {
            return RSHash(
                   obj.Manufacturer,
                   obj.ProductId,
                   obj.ProductName,
                   obj.ProductVersion,
                   obj.SerialNumber,
                   obj.VendorId,
                   obj.Path
            );
        }

        /// <summary> 
        /// This is a simple hashing function from Robert Sedgwicks Hashing in C book.
        /// Also, some simple optimizations to the algorithm in order to speed up
        /// its hashing process have been added. from: www.partow.net
        /// </summary>
        /// <param name="input">array of objects, parameters combination that you need
        /// to get a unique hash code for them</param>
        /// <returns>Hash code</returns>
        private static int RSHash(params object[] input)
        {
            const int b = 378551;
            int a = 63689;
            int hash = 0;

            // If it overflows then just wrap around
            unchecked
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] != null)
                    {
                        hash = hash * a + input[i].GetHashCode();
                        a = a * b;
                    }
                }
            }

            return hash;
        }
    }
}