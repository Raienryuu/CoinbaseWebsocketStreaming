using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingWithBackpressure.Connections.DataModels
{
    // Summary:
    //     Websocket payload described in RFC 6455
    public enum Opcode
    {
        ContinuationFrame = 0x0,
        TextFrame = 0x1,
        BinaryFrame = 0x2,
        // 0x3-0x7 are reserved for further non-control frames
        ConnectionClose = 0x8,
        Ping = 0x9,
        Pong = 0xA,
        // 0xB-0xF are reserved for further control frames
    }
    public class SocketPayload
    {
        bool FinBit { get; set; } = false;
        private bool RSV1 { get; set; } = false;
        private bool RSV2 { get; set; } = false;
        private bool RSV3 { get; set; } = false;
        public byte Opcode { get; private set; }
        private bool Mask { get; set; } = true;
        public UInt64 PayloadLength { get; private set; } = 0;
        private byte[] MaskBytes { get; set; }
        private byte[] ExtensionBytes { get; set; }
        private byte[] ApplicationBytes { get; set; }

        private byte[] Message;
        private int currentByteIndex;


        public SocketPayload() 
        {
            Random random = new Random();
            MaskBytes = new byte[4];
            random.NextBytes(MaskBytes);
            ApplicationBytes = new byte[0];
            ExtensionBytes = new byte[0];
            Message = new byte[24];
        }


        public bool GetFinBit()
        {
            return FinBit;
        }
        public void SetFinBit(bool finBit)
        {
            FinBit = finBit;
        }
        public void SetOpcode(byte Opcode)
        {
            if (Opcode > 15)
            {
                return;
            }
            else if (Opcode < 16) {
                this.Opcode = Opcode;
            }
        }

        public void SetApplicationData(string message)
        {
            if(message.Length > 0)
            {
                ApplicationBytes = Encoding.UTF8.GetBytes(message);
                PayloadLength = (ulong)ExtensionBytes.LongLength
                    + (ulong)ApplicationBytes.LongLength;
            }
        }
        public void SetExtensionData(string message)
        {
            if(message.Length > 0)
            {
                ExtensionBytes = Encoding.UTF8.GetBytes(message);
                PayloadLength = (ulong)ExtensionBytes.LongLength 
                    + (ulong)ApplicationBytes.LongLength;
            }
        }

        public byte[] GetMessageBytes()
        {
            byte[] message = new byte[24];
            SetFirstPart(ref message);
            SetPayloadLengthPart(ref message);
            SetFinalPart(ref message);
            return message;
        }

        private void SetFinalPart(ref byte[] message)
        {
            if (Mask)
            {
                Array.Copy(MaskBytes, 0, message, currentByteIndex, 4);
                currentByteIndex += 4;
            }

            message = ExtendMessageLength(message);

            if (ExtensionBytes.Length > 0)
            {
                Array.Copy(ExtensionBytes, 0, message, currentByteIndex,
                    ExtensionBytes.Length);
                currentByteIndex += ExtensionBytes.Length;
            }

            if (ApplicationBytes.Length > 0)
            {
                Array.Copy(ApplicationBytes, 0, message, currentByteIndex,
                    ApplicationBytes.Length);
            }
        }

        private byte[] ExtendMessageLength(byte[] message)
        {
            byte[] extendedMessage = new byte[(ulong)message.Length
                + PayloadLength];
            Array.Copy(message, extendedMessage, message.Length);

            return extendedMessage;
        }

        private void SetPayloadLengthPart(ref byte[] message)
        {
            if (PayloadLength < 126)
            {
                message[1] += (byte)PayloadLength;
                currentByteIndex = 2;
            } else if ((byte)PayloadLength == 126)
            {
                message[3] = 0x7E;
                currentByteIndex = 4;
            }
            else if (PayloadLength < UInt64.MaxValue)
            {
                byte[] lengthBytes = BitConverter.GetBytes(PayloadLength);
                for (int i = 2; i < 10; i++)
                {
                    message[i] = lengthBytes[i - 2];
                }
                currentByteIndex = 10;
            }
            else
            {
                throw new Exception("Value not supported");
            }
        }

        private void SetFirstPart(ref byte[] message)
        {
            message[0] = 0;
            if (FinBit)
            {
                message[0] = message[0] |= 0x80;
            }
            if (RSV1)
            {
                message[0] = message[0] |= 0x40;
            }
            if (RSV2)
            {
                message[0] = message[0] |= 0x20;
            }
            if (RSV3)
            {
                message[0] = message[0] |= 0x10;
            }
            message[0] |= Opcode;
            if (Mask)
            {
                message[1] = message[1] |= 0x80;
            }
        }
    }
}
