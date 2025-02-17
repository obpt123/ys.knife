﻿using System;

namespace YS.Knife.Data
{
    public static class SequentialKey
    {
        private static readonly Random _random = new Random(unchecked((int)DateTime.Now.Ticks));
        private static int sequenceValue = 0;
        private static long lastTicks = 0;
        /// <summary>
        /// Create a new string key of length 24.
        /// </summary>
        /// <returns>A new key of length 24.</returns>
        public static string NewString()
        {
            // 15+3+6 
            lock (_random)
            {
                long timestamp = DateTimeOffset.UtcNow.Ticks;
                if (timestamp != lastTicks)
                {
                    lastTicks = timestamp;
                    sequenceValue = 0;
                }
                else
                {
                    sequenceValue++;
                }
                return $"{timestamp:x15}{(sequenceValue & 0x00000fff):x3}{_random.Next(0xffffff):x6}";
            }
        }

    }

}
