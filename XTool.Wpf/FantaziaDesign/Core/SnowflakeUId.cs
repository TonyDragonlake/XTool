using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace FantaziaDesign.Core
{
	public static class SnowflakeUId
	{
		private static int s_workerIdShift = 12;
		private static int s_timestampShift = 22;
		private static long s_sequenceMask = -1L ^ (-1L << 12);

		private static long s_startTimestamp;
		private static long s_workerId;
		private static long s_sequence = 0L;
		private static long s_lastTimestamp = -1L;

		public static void ConfigureSettings(int workerIdBits, int sequenceBits, long workerId)
		{
			var maxWorkerId = -1L ^ (-1L << workerIdBits);
			if (workerId > maxWorkerId || workerId < 0)
			{
				throw new ArgumentOutOfRangeException($"workerId can't be greater than {maxWorkerId} or less than 0");
			}
			s_workerIdShift = sequenceBits;
			s_timestampShift = sequenceBits + workerIdBits;
			s_sequenceMask = -1L ^ (-1L << sequenceBits);
			s_workerId = workerId;
		}

		public static long Next()
		{
			var nowTimestamp = Stopwatch.GetTimestamp();
			if (s_startTimestamp == 0L)
			{
				Interlocked.Exchange(ref s_startTimestamp, nowTimestamp - 10000L);
				Interlocked.Exchange(ref s_lastTimestamp, nowTimestamp);
				return (10000L << s_timestampShift) | (s_workerId << s_workerIdShift);
			}
			if (nowTimestamp < s_lastTimestamp)
			{
				throw new InvalidOperationException($"Clock moved backwards. Refusing to generate id for {s_lastTimestamp - nowTimestamp} milliseconds");
			}
			var nextSeq = s_sequence + 1;
			if (s_lastTimestamp == nowTimestamp)
			{
				nextSeq &= s_sequenceMask;
				if (nextSeq == 0)
				{
					do{
						nowTimestamp = Stopwatch.GetTimestamp();
					} while (nowTimestamp <= s_lastTimestamp);
				}
			}
			else
			{
				nextSeq = 0L;
			}
			Interlocked.Exchange(ref s_lastTimestamp, nowTimestamp);
			Interlocked.Exchange(ref s_sequence, nextSeq);
			return ((nowTimestamp - s_startTimestamp) << s_timestampShift)
					| (s_workerId << s_workerIdShift)
					| nextSeq;
		}
	}
}
