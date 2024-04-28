using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop.COM
{
	[Flags]
	public enum ClsCtx
	{
		INPROC_SERVER = 1,
		INPROC_HANDLER = 2,
		LOCAL_SERVER = 4,
		INPROC_SERVER16 = 8,
		REMOTE_SERVER = 16,
		INPROC_HANDLER16 = 32,
		NO_CODE_DOWNLOAD = 1024,
		NO_CUSTOM_MARSHAL = 4096,
		ENABLE_CODE_DOWNLOAD = 8192,
		NO_FAILURE_LOG = 16384,
		DISABLE_AAA = 32768,
		ENABLE_AAA = 65536,
		FROM_DEFAULT_CONTEXT = 131072,
		ACTIVATE_32_BIT_SERVER = 262144,
		ACTIVATE_64_BIT_SERVER = 524288,
		ENABLE_CLOAKING = 1048576,
		PS_DLL = -2147483648,
		INPROC = 3,
		SERVER = 21,
		ALL = 23
	}

	public class PropVariantNative
	{
		[DllImport("ole32.dll")]
		public static extern int PropVariantClear(ref PropVariant pvar);

		[DllImport("ole32.dll")]
		public static extern int PropVariantClear(IntPtr pvar);
	}

	public struct Blob
	{
		/// <summary>
		/// Length of binary object.
		/// </summary>
		public int Length;
		/// <summary>
		/// Pointer to buffer storing data.
		/// </summary>
		public IntPtr Data;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct PropVariant
	{
		/// <summary>
		/// Value type tag.
		/// </summary>
		[FieldOffset(0)] public short vt;
		/// <summary>
		/// Reserved1.
		/// </summary>
		[FieldOffset(2)] public short wReserved1;
		/// <summary>
		/// Reserved2.
		/// </summary>
		[FieldOffset(4)] public short wReserved2;
		/// <summary>
		/// Reserved3.
		/// </summary>
		[FieldOffset(6)] public short wReserved3;
		/// <summary>
		/// cVal.
		/// </summary>
		[FieldOffset(8)] public sbyte cVal;
		/// <summary>
		/// bVal.
		/// </summary>
		[FieldOffset(8)] public byte bVal;
		/// <summary>
		/// iVal.
		/// </summary>
		[FieldOffset(8)] public short iVal;
		/// <summary>
		/// uiVal.
		/// </summary>
		[FieldOffset(8)] public ushort uiVal;
		/// <summary>
		/// lVal.
		/// </summary>
		[FieldOffset(8)] public int lVal;
		/// <summary>
		/// ulVal.
		/// </summary>
		[FieldOffset(8)] public uint ulVal;
		/// <summary>
		/// intVal.
		/// </summary>
		[FieldOffset(8)] public int intVal;
		/// <summary>
		/// uintVal.
		/// </summary>
		[FieldOffset(8)] public uint uintVal;
		/// <summary>
		/// hVal.
		/// </summary>
		[FieldOffset(8)] public long hVal;
		/// <summary>
		/// uhVal.
		/// </summary>
		[FieldOffset(8)] public long uhVal;
		/// <summary>
		/// fltVal.
		/// </summary>
		[FieldOffset(8)] public float fltVal;
		/// <summary>
		/// dblVal.
		/// </summary>
		[FieldOffset(8)] public double dblVal;
		//VARIANT_BOOL boolVal;
		/// <summary>
		/// boolVal.
		/// </summary>
		[FieldOffset(8)] public short boolVal;
		/// <summary>
		/// scode.
		/// </summary>
		[FieldOffset(8)] public int scode;
		//CY cyVal;
		//[FieldOffset(8)] private DateTime date; - can cause issues with invalid value
		/// <summary>
		/// Date time.
		/// </summary>
		[FieldOffset(8)] public System.Runtime.InteropServices.ComTypes.FILETIME filetime;
		//CLSID* puuid;
		//CLIPDATA* pclipdata;
		//BSTR bstrVal;
		//BSTRBLOB bstrblobVal;
		/// <summary>
		/// Binary large object.
		/// </summary>
		[FieldOffset(8)] public Blob blobVal;
		//LPSTR pszVal;
		/// <summary>
		/// Pointer value.
		/// </summary>
		[FieldOffset(8)] public IntPtr pointerValue; //LPWSTR 
													 //IUnknown* punkVal;
		/*IDispatch* pdispVal;
        IStream* pStream;
        IStorage* pStorage;
        LPVERSIONEDSTREAM pVersionedStream;
        LPSAFEARRAY parray;
        CAC cac;
        CAUB caub;
        CAI cai;
        CAUI caui;
        CAL cal;
        CAUL caul;
        CAH cah;
        CAUH cauh;
        CAFLT caflt;
        CADBL cadbl;
        CABOOL cabool;
        CASCODE cascode;
        CACY cacy;
        CADATE cadate;
        CAFILETIME cafiletime;
        CACLSID cauuid;
        CACLIPDATA caclipdata;
        CABSTR cabstr;
        CABSTRBLOB cabstrblob;
        CALPSTR calpstr;
        CALPWSTR calpwstr;
        CAPROPVARIANT capropvar;
        CHAR* pcVal;
        UCHAR* pbVal;
        SHORT* piVal;
        USHORT* puiVal;
        LONG* plVal;
        ULONG* pulVal;
        INT* pintVal;
        UINT* puintVal;
        FLOAT* pfltVal;
        DOUBLE* pdblVal;
        VARIANT_BOOL* pboolVal;
        DECIMAL* pdecVal;
        SCODE* pscode;
        CY* pcyVal;
        DATE* pdate;
        BSTR* pbstrVal;
        IUnknown** ppunkVal;
        IDispatch** ppdispVal;
        LPSAFEARRAY* pparray;
        PROPVARIANT* pvarVal;
        */

		/// <summary>
		/// Creates a new PropVariant containing a long value
		/// </summary>
		public static PropVariant FromLong(long value)
		{
			return new PropVariant() { vt = (short)VarEnum.VT_I8, hVal = value };
		}

		/// <summary>
		/// Helper method to gets blob data
		/// </summary>
		private byte[] GetBlob()
		{
			var blob = new byte[blobVal.Length];
			Marshal.Copy(blobVal.Data, blob, 0, blob.Length);
			return blob;
		}

		/// <summary>
		/// Interprets a blob as an array of structs
		/// </summary>
		public T[] GetBlobAsArrayOf<T>()
		{
			var blobByteLength = blobVal.Length;
			var singleInstance = (T)Activator.CreateInstance(typeof(T));
			var structSize = Marshal.SizeOf(singleInstance);
			if (blobByteLength % structSize != 0)
			{
				throw new InvalidDataException(String.Format("Blob size {0} not a multiple of struct size {1}", blobByteLength, structSize));
			}
			var items = blobByteLength / structSize;
			var array = new T[items];
			for (int n = 0; n < items; n++)
			{
				array[n] = (T)Activator.CreateInstance(typeof(T));
				Marshal.PtrToStructure(new IntPtr((long)blobVal.Data + n * structSize), array[n]);
			}
			return array;
		}

		/// <summary>
		/// Gets the type of data in this PropVariant
		/// </summary>
		public VarEnum DataType => (VarEnum)vt;

		/// <summary>
		/// Property value
		/// </summary>
		public object Value
		{
			get
			{
				VarEnum ve = DataType;
				switch (ve)
				{
					case VarEnum.VT_I1:
						return bVal;
					case VarEnum.VT_I2:
						return iVal;
					case VarEnum.VT_I4:
						return lVal;
					case VarEnum.VT_I8:
						return hVal;
					case VarEnum.VT_INT:
						return iVal;
					case VarEnum.VT_UI4:
						return ulVal;
					case VarEnum.VT_UI8:
						return uhVal;
					case VarEnum.VT_LPWSTR:
						return Marshal.PtrToStringUni(pointerValue);
					case VarEnum.VT_BLOB:
					case VarEnum.VT_VECTOR | VarEnum.VT_UI1:
						return GetBlob();
					case VarEnum.VT_CLSID:
						return (Guid)Marshal.PtrToStructure(pointerValue, typeof(Guid));
					case VarEnum.VT_BOOL:
						switch (boolVal)
						{
							case -1:
								return true;
							case 0:
								return false;
							default:
								throw new NotSupportedException("PropVariant VT_BOOL must be either -1 or 0");
						}
					case VarEnum.VT_FILETIME:
						return DateTime.FromFileTime((((long)filetime.dwHighDateTime) << 32) + filetime.dwLowDateTime);
				}
				throw new NotImplementedException("PropVariant " + ve);
			}
		}

		/// <summary>
		/// Clears with a known pointer
		/// </summary>
		public static void Clear(IntPtr ptr)
		{
			PropVariantNative.PropVariantClear(ptr);
		}
	}
	public struct PropertyKey : IEquatable<PropertyKey>
	{
		public PropertyKey(Guid formatId, int propertyId)
		{
			fmtid = formatId;
			pid = propertyId;
		}

		private Guid fmtid;

		private int pid;

		public Guid FormatId { get => fmtid; set => fmtid = value; }
		public int PropertyId { get => pid; set => pid = value; }

		public bool Equals(PropertyKey other)
		{
			return fmtid == other.fmtid && pid == other.pid;
		}

		public override int GetHashCode()
		{
			return fmtid.GetHashCode() ^ pid.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is PropertyKey key)
			{
				return Equals(key);
			}
			return false;
		}

		public static bool operator ==(PropertyKey left, PropertyKey right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PropertyKey left, PropertyKey right)
		{
			return !(left == right);
		}
	}

	[Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IPropertyStore
	{
		int GetCount(out int propCount);

		int GetAt(int property, out PropertyKey key);

		int GetValue(ref PropertyKey key, out PropVariant value);

		int SetValue(ref PropertyKey key, ref PropVariant value);

		int Commit();
	}

	public class PropertyStorePair
	{
		internal PropertyStorePair(PropertyKey key, PropVariant value)
		{
			this.Key = key;
			this.propertyValue = value;
		}

		public PropertyKey Key { get; }

		public object Value
		{
			get
			{
				return this.propertyValue.Value;
			}
		}

		private PropVariant propertyValue;
	}

	public class PropertyStore
	{
		public int Count
		{
			get
			{
				int result;
				Marshal.ThrowExceptionForHR(_storeInterface.GetCount(out result));
				return result;
			}
		}

		public PropertyStorePair this[int index]
		{
			get
			{
				PropertyKey key = this.GetKey(index);
				PropVariant result;
				Marshal.ThrowExceptionForHR(_storeInterface.GetValue(ref key, out result));
				return new PropertyStorePair(key, result);
			}
		}

		public bool Contains(PropertyKey key)
		{
			for (int i = 0; i < this.Count; i++)
			{
				PropertyKey ikey = this.GetKey(i);
				if (ikey == key)
				{
					return true;
				}
			}
			return false;
		}

		public PropertyStorePair this[PropertyKey key]
		{
			get
			{
				var contains = TryGetPropertyStorePair(key, out PropertyStorePair propStorePair, out int errorCode);
				if (errorCode == 0)
				{
					if (contains)
					{
						return propStorePair;
					}
					else
					{
						throw new IndexOutOfRangeException($"Cannot find key = {key}");
					}
				}
				Marshal.ThrowExceptionForHR(errorCode);
				return null;
			}
		}

		public bool TryGetPropertyStorePair(PropertyKey key, out PropertyStorePair propStorePair, out int errorCode)
		{
			errorCode = 0;
			propStorePair = null;
			errorCode = _storeInterface.GetCount(out int count);
			if (errorCode != 0)
			{
				return false;
			}
			PropVariant curValue;
			PropertyKey curKey;
			for (int i = 0; i < count; i++)
			{
				errorCode = _storeInterface.GetAt(i, out curKey);
				if (errorCode != 0)
				{
					return false;
				}
				if (curKey == key)
				{
					try
					{
						errorCode = _storeInterface.GetValue(ref curKey, out curValue);
						if (errorCode == 0)
						{
							propStorePair = new PropertyStorePair(curKey, curValue);
							return true;
						}
					}
					catch (Exception exp)
					{
						//Media.CoreAudio.PropertyKeys.NameDictionary.TryGetValue(curKey, out string value);
						//var parent = _parent as Media.CoreAudio.MMDevice;
						//System.Diagnostics.Debug.WriteLine("## Writing Error");
						//System.Diagnostics.Debug.Indent();
						//System.Diagnostics.Debug.WriteLine("Id = " + parent.DeviceInfo.Id);
						//System.Diagnostics.Debug.WriteLine("IconPath = " + parent.DeviceInfo.IconPath);
						//System.Diagnostics.Debug.WriteLine("Error In GetProperty : key = " + value);
						//System.Diagnostics.Debug.WriteLine(exp.Message);
						//System.Diagnostics.Debug.WriteLine(exp.HResult);
						//System.Diagnostics.Debug.Unindent();
					}

				}
			}
			return false;
		}

		public bool TryGetPropertyStorePair(PropertyKey key, out PropertyStorePair propStorePair)
		{
			return TryGetPropertyStorePair(key, out propStorePair, out int errorCode) && errorCode == 0;
		}

		public PropertyKey GetKey(int index)
		{
			PropertyKey key;
			Marshal.ThrowExceptionForHR(_storeInterface.GetAt(index, out key));
			return key;
		}

		public int TryGetKey(int index, out PropertyKey key)
		{
			return _storeInterface.GetAt(index, out key);
		}

		public PropVariant GetValue(int index)
		{
			PropertyKey key = this.GetKey(index);
			PropVariant result;
			Marshal.ThrowExceptionForHR(_storeInterface.GetValue(ref key, out result));
			return result;
		}

		public int TryGetValue(int index, out PropVariant value)
		{
			value = default(PropVariant);
			PropertyKey key;
			int errorCode = _storeInterface.GetAt(index, out key);
			if (errorCode != 0)
			{
				return errorCode;
			}
			return _storeInterface.GetValue(ref key, out value);
		}

		public void SetValue(PropertyKey key, PropVariant value)
		{
			Marshal.ThrowExceptionForHR(_storeInterface.SetValue(ref key, ref value));
		}

		public void Commit()
		{
			Marshal.ThrowExceptionForHR(_storeInterface.Commit());
		}

		internal PropertyStore(IPropertyStore store)
		{
			_storeInterface = store;
		}

		private readonly IPropertyStore _storeInterface;

	}


}
