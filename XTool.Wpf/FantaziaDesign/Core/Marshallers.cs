using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Core
{
	public interface IMarshaller<T>
	{
		IntPtr MarshalAsPointer(T target, int optionCode = 0);
		T MarshalFromPointer(IntPtr pointer, int optionCode = 0);
		bool TryMarshalToPointer(T target, ref IntPtr pointer);
		void FreePointer(IntPtr pointer, int optionCode = 0);
	}

	public sealed class StringMarshaller : IMarshaller<string>
	{
		public readonly int HGlobalAuto = 0;
		public readonly int HGlobalUnicode = 1;
		public readonly int HGlobalAnsi = 2;
		public readonly int BSTR = 3;
		public readonly int CoTaskMemAuto = 4;
		public readonly int CoTaskMemUnicode = 5;
		public readonly int CoTaskMemAnsi = 6;
#if NETSTANDARD2_1_OR_GREATER
		public readonly int CoTaskMemUTF8 = 7;
#endif

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			if (optionCode == BSTR)
			{
				Marshal.FreeBSTR(pointer);
			}
            else if (optionCode > BSTR && optionCode <=
#if NETSTANDARD2_1_OR_GREATER
				CoTaskMemUTF8
#else
				CoTaskMemAnsi
#endif
				)
			{
				Marshal.FreeCoTaskMem(pointer);
			}
			else
			{
				Marshal.FreeHGlobal(pointer);
			}
		}

		public IntPtr MarshalAsPointer(string target, int optionCode = 0)
		{
			switch (optionCode)
			{
				case 0: return Marshal.StringToHGlobalAuto(target);
				case 1: return Marshal.StringToHGlobalUni(target);
				case 2: return Marshal.StringToHGlobalAnsi(target);
				case 3: return Marshal.StringToBSTR(target);
				case 4: return Marshal.StringToCoTaskMemAuto(target);
				case 5: return Marshal.StringToCoTaskMemUni(target);
				case 6: return Marshal.StringToCoTaskMemAnsi(target);
#if NETSTANDARD2_1_OR_GREATER
				case 7: return Marshal.StringToCoTaskMemUTF8(target);
#endif
				default: return Marshal.StringToHGlobalAuto(target);
			}
		}

		public string MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			switch (optionCode)
			{
				case 0: return Marshal.PtrToStringAuto(pointer);
				case 1: return Marshal.PtrToStringUni(pointer);
				case 2: return Marshal.PtrToStringAnsi(pointer);
				case 3: return Marshal.PtrToStringBSTR(pointer);
				case 4: return Marshal.PtrToStringAuto(pointer);
				case 5: return Marshal.PtrToStringUni(pointer);
				case 6: return Marshal.PtrToStringAnsi(pointer);
#if NETSTANDARD2_1_OR_GREATER
				case 7: return Marshal.PtrToStringUTF8(pointer);
#endif
				default: return Marshal.PtrToStringAuto(pointer);
			}
		}

		public bool TryMarshalToPointer(string target, ref IntPtr pointer)
		{
			if (pointer != IntPtr.Zero)
			{
				Marshal.StructureToPtr(target, pointer, true);
				return true;
			}
			return false;
		}

	}

	public sealed class ByteMarshaller : IMarshaller<byte>
	{
		private static readonly int s_marshallerSize = sizeof(byte);

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			if (optionCode == 0)
			{
				Marshal.FreeHGlobal(pointer);
			}
			else
			{
				Marshal.FreeCoTaskMem(pointer);
			}
		}

		public IntPtr MarshalAsPointer(byte target, int optionCode = 0)
		{
			var pointer = optionCode == 0 
				? Marshal.AllocHGlobal(s_marshallerSize) 
				: Marshal.AllocCoTaskMem(s_marshallerSize);
			Marshal.WriteByte(pointer, target);
			return pointer;
		}

		public byte MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return Marshal.ReadByte(pointer);
		}

		public bool TryMarshalToPointer(byte target, ref IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return false;
			}
			Marshal.WriteByte(pointer, target);
			return true;
		}
	}

	public sealed class SByteMarshaller : IMarshaller<sbyte>
	{
		private readonly ByteMarshaller m_compatible = new ByteMarshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer, optionCode);
		}

		public IntPtr MarshalAsPointer(sbyte target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer((CompatibleByte)target, optionCode);
		}

		public sbyte MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return new CompatibleByte(m_compatible.MarshalFromPointer(pointer, optionCode)).SValue;
		}

		public bool TryMarshalToPointer(sbyte target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer((CompatibleByte)target, ref pointer);
		}
	}

	public sealed class Int16Marshaller : IMarshaller<short>
	{
		private static readonly int s_marshallerSize = sizeof(short);

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			if (optionCode == 0)
			{
				Marshal.FreeHGlobal(pointer);
			}
			else
			{
				Marshal.FreeCoTaskMem(pointer);
			}
		}

		public IntPtr MarshalAsPointer(short target, int optionCode = 0)
		{
			var pointer = optionCode == 0
				? Marshal.AllocHGlobal(s_marshallerSize)
				: Marshal.AllocCoTaskMem(s_marshallerSize);
			Marshal.WriteInt16(pointer, target);
			return pointer;
		}

		public short MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return Marshal.ReadInt16(pointer);
		}

		public bool TryMarshalToPointer(short target, ref IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return false;
			}
			Marshal.WriteInt16(pointer, target);
			return true;
		}
	}

	public sealed class UInt16Marshaller : IMarshaller<ushort>
	{
		private readonly Int16Marshaller m_compatible = new Int16Marshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer);
		}

		public IntPtr MarshalAsPointer(ushort target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer((CompatibleInt16)target, optionCode);
		}

		public ushort MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return new CompatibleInt16(m_compatible.MarshalFromPointer(pointer, optionCode)).UValue;
		}

		public bool TryMarshalToPointer(ushort target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer((CompatibleInt16)target, ref pointer);
		}
	}

	public sealed class CharMarshaller : IMarshaller<char>
	{
		private readonly Int16Marshaller m_compatible = new Int16Marshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer);
		}

		public IntPtr MarshalAsPointer(char target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer((CompatibleInt16)target, optionCode);
		}

		public char MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return new CompatibleInt16(m_compatible.MarshalFromPointer(pointer, optionCode)).CValue;
		}

		public bool TryMarshalToPointer(char target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer((CompatibleInt16)target, ref pointer);
		}
	}

	public sealed class Int32Marshaller : IMarshaller<int>
	{
		private static readonly int s_marshallerSize = sizeof(int);

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			if (optionCode == 0)
			{
				Marshal.FreeHGlobal(pointer);
			}
			else
			{
				Marshal.FreeCoTaskMem(pointer);
			}
		}

		public IntPtr MarshalAsPointer(int target, int optionCode = 0)
		{
			var pointer = optionCode == 0
				? Marshal.AllocHGlobal(s_marshallerSize)
				: Marshal.AllocCoTaskMem(s_marshallerSize);
			Marshal.WriteInt32(pointer, target);
			return pointer;
		}

		public int MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return Marshal.ReadInt32(pointer);
		}

		public bool TryMarshalToPointer(int target, ref IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return false;
			}
			Marshal.WriteInt32(pointer, target);
			return true;
		}
	}

	public sealed class UInt32Marshaller : IMarshaller<uint>
	{
		private readonly Int32Marshaller m_compatible = new Int32Marshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer);
		}

		public IntPtr MarshalAsPointer(uint target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer((CompatibleInt32)target, optionCode);
		}

		public uint MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return new CompatibleInt32(m_compatible.MarshalFromPointer(pointer, optionCode)).UValue;
		}

		public bool TryMarshalToPointer(uint target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer((CompatibleInt32)target, ref pointer);
		}
	}

	public sealed class BooleanMarshaller : IMarshaller<bool>
	{
		private readonly Int32Marshaller m_compatible = new Int32Marshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer);
		}

		public IntPtr MarshalAsPointer(bool target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer(target ? 1 : 0, optionCode);
		}

		public bool MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return m_compatible.MarshalFromPointer(pointer, optionCode) > 0;
		}

		public bool TryMarshalToPointer(bool target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer(target ? 1 : 0, ref pointer);
		}
	}

	public sealed class Float32Marshaller : IMarshaller<float>
	{
		private readonly Int32Marshaller m_compatible = new Int32Marshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer);
		}

		public IntPtr MarshalAsPointer(float target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer((CompatibleInt32)target, optionCode);
		}

		public float MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return new CompatibleInt32(m_compatible.MarshalFromPointer(pointer, optionCode)).FValue;
		}

		public bool TryMarshalToPointer(float target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer((CompatibleInt32)target, ref pointer);
		}
	}

	public sealed class Int64Marshaller : IMarshaller<long>
	{
		private static readonly int s_marshallerSize = sizeof(long);

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			if (optionCode == 0)
			{
				Marshal.FreeHGlobal(pointer);
			}
			else
			{
				Marshal.FreeCoTaskMem(pointer);
			}
		}

		public IntPtr MarshalAsPointer(long target, int optionCode = 0)
		{
			var pointer = optionCode == 0
				? Marshal.AllocHGlobal(s_marshallerSize)
				: Marshal.AllocCoTaskMem(s_marshallerSize);
			Marshal.WriteInt64(pointer, target);
			return pointer;
		}

		public long MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return Marshal.ReadInt64(pointer);
		}

		public bool TryMarshalToPointer(long target, ref IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return false;
			}
			Marshal.WriteInt64(pointer, target);
			return true;
		}
	}

	public sealed class UInt64Marshaller : IMarshaller<ulong>
	{
		private readonly Int64Marshaller m_compatible = new Int64Marshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer);
		}

		public IntPtr MarshalAsPointer(ulong target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer((CompatibleInt64)target, optionCode);
		}

		public ulong MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return new CompatibleInt64(m_compatible.MarshalFromPointer(pointer, optionCode)).UValue;
		}

		public bool TryMarshalToPointer(ulong target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer((CompatibleInt64)target, ref pointer);
		}
	}

	public sealed class Float64Marshaller : IMarshaller<double>
	{
		private readonly Int64Marshaller m_compatible = new Int64Marshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer);
		}

		public IntPtr MarshalAsPointer(double target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer((CompatibleInt64)target, optionCode);
		}

		public double MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return new CompatibleInt64(m_compatible.MarshalFromPointer(pointer, optionCode)).FValue;
		}

		public bool TryMarshalToPointer(double target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer((CompatibleInt64)target, ref pointer);
		}
	}

	public sealed class DateTimeMarshaller : IMarshaller<DateTime>
	{
		private readonly Int64Marshaller m_compatible = new Int64Marshaller();

		public void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			m_compatible.FreePointer(pointer);
		}

		public IntPtr MarshalAsPointer(DateTime target, int optionCode = 0)
		{
			return m_compatible.MarshalAsPointer(target.Ticks, optionCode);
		}

		public DateTime MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return new DateTime(m_compatible.MarshalFromPointer(pointer, optionCode));
		}

		public bool TryMarshalToPointer(DateTime target, ref IntPtr pointer)
		{
			return m_compatible.TryMarshalToPointer(target.Ticks, ref pointer);
		}
	}

	public class GenericMarshaller<T> : IMarshaller<T>
	{
		public GenericMarshaller()
		{
		}

		public virtual void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			if (optionCode == 0)
			{
				Marshal.FreeHGlobal(pointer);
			}
			else
			{
				Marshal.FreeCoTaskMem(pointer);
			}
		}

		public virtual IntPtr MarshalAsPointer(T target, int optionCode = 0)
		{
			var marshallerSize = Marshal.SizeOf<T>();
			var pointer = optionCode == 0
				? Marshal.AllocHGlobal(marshallerSize)
				: Marshal.AllocCoTaskMem(marshallerSize);
			Marshal.StructureToPtr(target, pointer, true);
			return pointer;
		}

		public virtual T MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			return Marshal.PtrToStructure<T>(pointer);
		}

		public virtual bool TryMarshalToPointer(T target, ref IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return false;
			}
			Marshal.StructureToPtr(target, pointer, true);
			return true;
		}
	}

	public class CompatibleGenericMarshaller<T> : GenericMarshaller<T>
	{
		protected IMarshaller<T> m_compatible;

		public CompatibleGenericMarshaller()
		{
		}

		public CompatibleGenericMarshaller(IMarshaller<T> compatible)
		{
			m_compatible = compatible;
		}

		public CompatibleGenericMarshaller(bool useDefaultIfExist = false)
		{
			if (useDefaultIfExist)
			{
				m_compatible = Marshallers.GetDefaultMarshaller<T>();
			}
		}

		public override void FreePointer(IntPtr pointer, int optionCode = 0)
		{
			if (m_compatible != null)
			{
				m_compatible.FreePointer(pointer, optionCode);
			}
			else
			{
				base.FreePointer(pointer, optionCode);
			}
		}

		public override IntPtr MarshalAsPointer(T target, int optionCode = 0)
		{
			if (m_compatible != null)
			{
				return m_compatible.MarshalAsPointer(target, optionCode);
			}
			return base.MarshalAsPointer(target, optionCode);
		}

		public override T MarshalFromPointer(IntPtr pointer, int optionCode = 0)
		{
			if (m_compatible != null)
			{
				return m_compatible.MarshalFromPointer(pointer, optionCode);
			}
			return base.MarshalFromPointer(pointer, optionCode);
		}

		public override bool TryMarshalToPointer(T target, ref IntPtr pointer)
		{
			if (m_compatible != null)
			{
				return m_compatible.TryMarshalToPointer(target, ref pointer);
			}
			return base.TryMarshalToPointer(target, ref pointer);
		}
	}

	public static class Marshallers
	{
		private static readonly Dictionary<Type, object> s_marshallers = new Dictionary<Type, object>();

		public static IMarshaller<T> GetDefaultMarshaller<T>()
		{
			if (s_marshallers.Count == 0)
			{
				InitBuiltInTypeMarshallers();
			}
			var type = typeof(T);
			if (s_marshallers.TryGetValue(type, out var obj))
			{
				return obj as IMarshaller<T>;
			}
			return null;
		}

		public static IMarshaller<T> GetMarshallerFromSource<T>(IReadOnlyDictionary<Type, object> source)
		{
			if (source != null && source.Count > 0)
			{
				var type = typeof(T);
				if (source.TryGetValue(type, out var obj))
				{
					return obj as IMarshaller<T>;
				}
			}
			return GetDefaultMarshaller<T>();
		}

		private static void InitBuiltInTypeMarshallers()
		{
			RegisterDefaultMarshaller(new StringMarshaller());
			RegisterDefaultMarshaller(new ByteMarshaller());
			RegisterDefaultMarshaller(new SByteMarshaller());
			RegisterDefaultMarshaller(new Int16Marshaller());
			RegisterDefaultMarshaller(new UInt16Marshaller());
			RegisterDefaultMarshaller(new CharMarshaller());
			RegisterDefaultMarshaller(new Int32Marshaller());
			RegisterDefaultMarshaller(new UInt32Marshaller());
			RegisterDefaultMarshaller(new BooleanMarshaller());
			RegisterDefaultMarshaller(new Float32Marshaller());
			RegisterDefaultMarshaller(new Int64Marshaller());
			RegisterDefaultMarshaller(new UInt64Marshaller());
			RegisterDefaultMarshaller(new Float64Marshaller());
			RegisterDefaultMarshaller(new DateTimeMarshaller());
		}

		public static bool RegisterDefaultMarshaller<T>(IMarshaller<T> marshaller)
		{
			var type = typeof(T);
			if (s_marshallers.TryGetValue(type, out var obj))
			{
				var msl = obj as IMarshaller<T>;
				if (msl is null)
				{
					s_marshallers[type] = marshaller;
					return true;
				}
				return false;
			}
			else
			{
				s_marshallers.Add(type, marshaller);
				return true;
			}
		}
	}

}
