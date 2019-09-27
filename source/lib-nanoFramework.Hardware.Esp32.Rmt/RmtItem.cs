namespace nanoFramework.Hardware.Esp32.Rmt
{
    using System;

    public struct RmtItem
    {
        internal UInt32 data;

        public RmtItem(UInt32 item)
        {
            data = item;
        }

        public UInt16 Duration0
        {
            get
            {
                return (UInt16)(data >> 17);
            }
            set
            {
                data = ((data & ~((UInt32)0xfffe0000)) | (UInt32)(value << 17));
            }
        }

        public bool Level0
        {
            get
            {
                return (((data & 0x10000) >> 16) == 1) ? true : false;
            }
            set
            {
                UInt32 valueInt = (UInt32)((value) ? 1 : 0) << 16;
                data = (data & ~((UInt32)0x10000) | valueInt);
            }
        }

        public UInt16 Duration1
        {
            get
            {
                return (UInt16)(data >> 1);
            }
            set
            {
                data = (data & ~((UInt32)0xfffe)) | (UInt32)(value << 1);
            }
        }

        public bool Level1
        {
            get
            {
                return ((data & 0x1) == 1) ? true : false;
            }
            set
            {
                UInt32 valueInt = (UInt32)(value ? 1 : 0);
                data = (data & ~(UInt32)0x1) | valueInt;
            }
        }
    }
}