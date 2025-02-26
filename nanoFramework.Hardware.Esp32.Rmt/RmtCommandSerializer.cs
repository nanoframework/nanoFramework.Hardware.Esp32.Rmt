using System;
using System.Collections;
using System.Runtime.CompilerServices;

#nullable enable
namespace nanoFramework.Hardware.Esp32.Rmt
{
    [ExcludeType]
    internal static class RmtCommandSerializer
    {
        /// <summary>
        /// Serialize commands to rmt_item32_t native byte format
        /// </summary>
        /// <returns>The serialized commands</returns>
        public static byte[] SerializeCommands(ArrayList commands)
        {
            int i = 0;
            int remaining;
            byte[] binaryCommands = new byte[commands.Count * 4];
            foreach (var cmd in commands)
            {
                // First pair
                if ((cmd as RmtCommand).Duration0 <= 255)
                {
                    binaryCommands[0 + i] = (byte)(cmd as RmtCommand).Duration0;
                    binaryCommands[1 + i] = (byte)((cmd as RmtCommand).Level0 == true ? 128 : 0);
                }
                else
                {
                    remaining = (cmd as RmtCommand).Duration0 % 256;
                    binaryCommands[0 + i] = (byte)(remaining);
                    binaryCommands[1 + i] = (byte)(((cmd as RmtCommand).Level0 ? 128 : 0) + (((cmd as RmtCommand).Duration0 - remaining) / 256));
                }

                // Second pair
                if ((cmd as RmtCommand).Duration1 <= 255)
                {
                    binaryCommands[2 + i] = (byte)(cmd as RmtCommand).Duration1;
                    binaryCommands[3 + i] = (byte)((cmd as RmtCommand).Level1 ? 128 : 0);
                }
                else
                {
                    remaining = (cmd as RmtCommand).Duration1 % 256;
                    binaryCommands[2 + i] = (byte)(remaining);
                    binaryCommands[3 + i] = (byte)(((cmd as RmtCommand).Level1 ? 128 : 0) + (((cmd as RmtCommand).Duration1 - remaining) / 256));
                }
                i += 4;
            }
            return binaryCommands;
        }
    }
}
