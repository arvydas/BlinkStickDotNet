using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.Runtime.InteropServices;

namespace HidSharp
{
	public class LibusbHidDevice : HidDevice
	{
		private UsbRegistry deviceRegistry;

		string _manufacturer;
        string _productName;
        string _serialNumber;
        byte[] _reportDescriptor;
        int _vid, _pid, _version;
        int _maxInput = 0, _maxOutput = 0, _maxFeature = 0;
        bool _reportsUseID;
        string _path;


		public LibusbHidDevice (UsbRegistry key)
		{
			this.deviceRegistry = key;
		}

		public override HidStream Open()
        {
            var stream = new LibusbHidStream();
            try { stream.Init(this, this.deviceRegistry); return stream; }
            catch { stream.Close(); throw; }
        }

        public override byte[] GetReportDescriptor()
        {
            return (byte[])_reportDescriptor.Clone();
        }

        internal unsafe bool GetInfo ()
		{
			_vid = deviceRegistry.Vid;
            _pid = deviceRegistry.Pid;
            _version = deviceRegistry.Rev;
            _manufacturer = (String)deviceRegistry.DeviceProperties["Mfg"];
            _productName = (String)deviceRegistry.DeviceProperties["DeviceDesc"];
            _serialNumber = (String)deviceRegistry.DeviceProperties["SerialNumber"];

            return true;
        }

        public override string DevicePath
        {
            get { return _path; }
        }

        public override int MaxInputReportLength
        {
            get { return _maxInput; }
        }

        public override int MaxOutputReportLength
        {
            get { return _maxOutput; }
        }

        public override int MaxFeatureReportLength
        {
            get { return _maxFeature; }
        }

        internal bool ReportsUseID
        {
            get { return _reportsUseID; }
        }

        public override string Manufacturer
        {
            get { return _manufacturer; }
        }

        public override int ProductID
        {
            get { return _pid; }
        }

        public override string ProductName
        {
            get { return _productName; }
        }

        public override int ProductVersion
        {
            get { return _version; }
        }

        public override string SerialNumber
        {
            get { return _serialNumber; }
        }

        public override int VendorID
        {
            get { return _vid; }
        }
	}
}

