using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.VFPacket
{
    class VFHead
    {
        public const int LENGTH = 6;
        private ushort frameOrder;
        private ushort partsNum;
        private ushort partOrder;

        public ushort FrameOrder
        {
            get
            {
                return frameOrder;
            }

            set
            {
                frameOrder = value;
            }
        }

        public ushort PartsNum
        {
            get
            {
                return partsNum;
            }

            set
            {
                partsNum = value;
            }
        }

        public ushort PartOrder
        {
            get
            {
                return partOrder;
            }

            set
            {
                partOrder = value;
            }
        }

        public VFHead(ushort frameOrder, ushort partsNum, ushort partOrder) {
            this.frameOrder = frameOrder;
            this.partsNum = partsNum;
            this.partOrder = partOrder;
        }

        public byte[] getBytes() {
            return BytesUtil.catBytes(BitConverter.GetBytes(frameOrder), BitConverter.GetBytes(partsNum), BitConverter.GetBytes(partOrder));
        }
        public static VFHead readBytes(byte[] head) {
            return new VFHead(BitConverter.ToUInt16(head, 0), BitConverter.ToUInt16(head, 2), BitConverter.ToUInt16(head, 4));
        }
    }
}
