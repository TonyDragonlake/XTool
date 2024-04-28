using System;
using FantaziaDesign.Core;

namespace XTool.Excel.ObjectModels
{
	public struct XAddress : IEquatable<XAddress>, IDeepCopyable<XAddress>
	{
		private XIndex m_row;
		private XIndex m_col;

		public XIndex Row => m_row;
		public XIndex Column => m_col;

		public XAddress(XIndex row, XIndex col)
		{
			m_row = row;
			m_col = col;
		}

		public XAddress(int rowIndex, bool isRowAbsoluted, int colIndex, bool isColAbsoluted)
		{
			m_row = new XIndex(rowIndex, isRowAbsoluted);
			m_col = new XIndex(colIndex, isColAbsoluted);
		}

		public XAddress(string xlRef)
		{
			m_row = new XIndex();
			m_col = new XIndex();
			SetAddress(xlRef);
		}

		public void SetAddress(string xlRef)
		{
			if (string.IsNullOrWhiteSpace(xlRef))
			{
				throw new ArgumentException($"{nameof(xlRef)} cannot be null or whitespace", nameof(xlRef));
			}
			SetAddressInternal(ref xlRef, ref m_row, ref m_col);
		}

		internal static void SetAddressInternal(ref string xlRef, ref XIndex row, ref XIndex col)
		{
			int rPos, cPos;
			FindPosition(xlRef, out rPos, out cPos);
			var colStr = xlRef.Substring(cPos, rPos - cPos);
			var rowStr = xlRef.Substring(rPos);
			row.SetXIndex(rowStr);
			col.SetXIndex(colStr);
		}

		private static void FindPosition(string xlRef, out int rPos, out int cPos)
		{
			int[] symbolPos = new int[2] { -1, -1 };
			rPos = -1;
			cPos = -1;
			int sPos = 0;
			for (int i = 0; i < xlRef.Length; i++)
			{
				var item = xlRef[i];
				if (char.IsWhiteSpace(item))
				{
					continue;
				}
				if (item == '$' && sPos < 2)
				{
					symbolPos[sPos] = i;
					sPos++;
					continue;
				}
				if (cPos < 0 && ((item >= 'a' && item <= 'z') || (item >= 'A' && item <= 'Z')))
				{
					cPos = i;
					continue;
				}
				if (rPos < 0 && (item >= '0' && item <= '9'))
				{
					rPos = i;
					continue;
				}
			}

			if (cPos > symbolPos[0])
			{
				// has col prefix
				cPos = symbolPos[0];
				var s1 = symbolPos[1];
				if (s1 > 0 && rPos > s1)
				{
					rPos = s1;
				}
			}
			else
			{
				var s0 = symbolPos[0];
				if (rPos > s0)
				{
					rPos = s0;
				}
			}
			if (cPos < 0)
			{
				cPos = 0;
			}
			if (cPos > rPos)
			{
				throw new InvalidCastException("Unknown format");
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is XAddress xAddress)
			{
				return Equals(xAddress);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_col.GetHashCode() ^ m_row.GetHashCode();
		}

		public override string ToString()
		{
			return $"{m_col.ToString(XIndexFormat.Letter)}{m_row}";
		}

		public bool Equals(XAddress other)
		{
			return m_col.Equals(other.m_col) && m_row.Equals(other.m_row);
		}

		public void ZeroAddress()
		{
			m_row.ZeroIndex();
			m_col.ZeroIndex();
		}

		public XAddress DeepCopy()
		{
			return new XAddress(m_row.Index, m_row.IsAbsoluted, m_col.Index, m_col.IsAbsoluted);
		}

		public void DeepCopyValueFrom(XAddress obj)
		{
			m_row.DeepCopyValueFrom(obj.Row);
			m_col.DeepCopyValueFrom(obj.Column);
		}

		public object Clone()
		{
			return DeepCopy();
		}
	}
}
