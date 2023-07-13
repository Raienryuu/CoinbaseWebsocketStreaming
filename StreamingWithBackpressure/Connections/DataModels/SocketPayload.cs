using System.Collections;
using System.Text;

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
        bool FinBit { get; set; } = true;
        private bool RSV1 { get; set; } = false;
        private bool RSV2 { get; set; } = false;
        private bool RSV3 { get; set; } = false;
        public Opcode Opcode { get; private set; }
        private bool Mask { get; set; } = true;
        public ulong PayloadLength { get; private set; } = 0;
        private byte[] MaskBytes { get; set; }
        private byte[] ExtensionBytes { get; set; }
        private byte[] ApplicationBytes { get; set; }

        private byte[] Message;
        private int currentByteIndex;


        public SocketPayload() 
        {
            MaskBytes = new byte[4];
            new Random().NextBytes(MaskBytes);
            ApplicationBytes = Array.Empty<byte>();
            ExtensionBytes = Array.Empty<byte>();
            Message = new byte[14];
        }

        public bool GetFinBit()
        {
            return FinBit;
        }
        public void SetFinBit(bool finBit)
        {
            FinBit = finBit;
        }
        public void SetOpcode(Opcode opcode)
        {
            Opcode = opcode;
        }

        public void SetApplicationData(string applicationData)
        {
            if(applicationData.Length > 0)
            {
                ApplicationBytes = Encoding.UTF8.GetBytes(applicationData);
                PayloadLength = (ulong)ExtensionBytes.Length
                    + (ulong)ApplicationBytes.Length;
            }
        }
        public void SetExtensionData(string extensionData)
        {
            if(extensionData.Length > 0)
            {
                ExtensionBytes = Encoding.UTF8.GetBytes(extensionData);
                PayloadLength = (ulong)ExtensionBytes.Length 
                    + (ulong)ApplicationBytes.Length;
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
                message[1] = (byte)(message[1] | 0x80);
                MaskBytes.CopyTo(message, currentByteIndex);
                currentByteIndex += 4;
            }
            LoadPayloadData(ref message);
        }

        private void LoadPayloadData(ref byte[] message)
        {
            int i;
            for (i = 0; i < ExtensionBytes.Length; i++)
            {
                message[i + currentByteIndex] = (byte)(ExtensionBytes[i] ^ MaskBytes[i % 4]);
            }
            currentByteIndex += i;
            for (i = 0; i < ApplicationBytes.Length; i++)
            {
                message[i + currentByteIndex] = (byte)(ApplicationBytes[i] ^ MaskBytes[i % 4]);
            }
            currentByteIndex += i;
        }

        private void ExtendMessageLength(ref byte[] message, ulong newLength)
        {
            byte[] extendedMessage = new byte[newLength];
            Array.Copy(message, extendedMessage, message.Length);
            message = extendedMessage;
        }

        private void SetPayloadLengthPart(ref byte[] message)
        {
            if (PayloadLength < 126)
            {
                message[1] += (byte)PayloadLength;
                currentByteIndex = 2;
                ExtendMessageLength(ref message, PayloadLength + 6);
            } else if (PayloadLength <= UInt16.MaxValue)
            {
                message[1] = 0x7E;
                byte[] lengthBytes = BitConverter.GetBytes(PayloadLength);
                for (int i = 2; i < 4; i++)
                {
                    message[i] = lengthBytes[i - 2];
                }
                currentByteIndex = 4;
                ExtendMessageLength(ref message, PayloadLength + 8);
            }
            else if (PayloadLength <= UInt64.MaxValue)
            {
                message[1] = 0x7F;

                byte[] lengthBytes = BitConverter.GetBytes(PayloadLength);
                for (int i = 2; i < 10; i++)
                {
                    message[i] = lengthBytes[i - 2];
                }
                currentByteIndex = 10;
                ExtendMessageLength(ref message, PayloadLength + 14);
            }
            else
            {
                throw new Exception("Value not supported");
            }
        }

        private void SetFirstPart(ref byte[] message)
        {
            message[0] = 0;
            message[0] |= (byte)Opcode;
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
        }

        public void SetMask(byte[] mask)
        {
            MaskBytes = mask;
        }
    }
}
