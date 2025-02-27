using System.Collections;
using System.Runtime.CompilerServices;

#nullable enable
namespace nanoFramework.Hardware.Esp32.Rmt
{
    internal static class RmtCommandSerializer
    {
        /// <summary>
        /// Serialize commands to rmt_item32_t native byte format
        /// </summary>
        /// <returns>The serialized commands</returns>
        public static byte[] SerializeCommands(ArrayList commands)
        {
            var index = 0;
            var serializedCommands = new byte[commands.Count * 4];

            for (var i = 0; i < commands.Count; i++)
            {
                var command = (RmtCommand) commands[i];

                var highByte1 = (byte)(command.Duration0 >> 8);
                var lowByte1 = (byte)(command.Duration0 & 0xFF);
                var highByte2 = (byte)(command.Duration1 >> 8);
                var lowByte2 = (byte)(command.Duration1 & 0xFF);
                var level1 = (byte)(command.Level0 ? 0x80 : 0);
                var level2 = (byte)(command.Level1 ? 0x80 : 0);

                serializedCommands[index++] = lowByte1;
                serializedCommands[index++] = (byte)(highByte1 | level1);
                serializedCommands[index++] = lowByte2;
                serializedCommands[index++] = (byte)(highByte2 | level2);
            }

            return serializedCommands;
        }
    }
}
