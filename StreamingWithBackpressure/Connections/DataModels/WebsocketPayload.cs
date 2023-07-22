using System.Text;

namespace StreamingWithBackpressure.Connections.DataModels
{

    // Summary:
    //     Websocket payload described in RFC 6455
    public class WebSocketPayload
    {
        private int currentByteIndex;
        protected byte[] Message;
        public bool FinBit = false;
        public bool Mask = true;
        public Opcode Opcode;
        public ulong PayloadLength { get; protected set; } = 0;
        public bool RSV1 = false;
        public bool RSV2 = false;
        public bool RSV3 = false;
        protected byte[] ApplicationBytes;
        protected byte[] ExtensionBytes;
        protected byte[] MaskBytes;

        public WebSocketPayload()
        {
            MaskBytes = new byte[4];
            new Random().NextBytes(MaskBytes);
            ApplicationBytes = Array.Empty<byte>();
            ExtensionBytes = Array.Empty<byte>();
            Message = new byte[14];
        }

        public WebSocketPayload(byte[] message)
        {
            MaskBytes = new byte[4];
            ApplicationBytes = Array.Empty<byte>();
            ExtensionBytes = Array.Empty<byte>();
            Message = message;
            DecodeMessage();
        }

        public void SetApplicationData(string applicationData)
        {
            if (applicationData.Length > 0)
            {
                ApplicationBytes = Encoding.UTF8.GetBytes(applicationData);
                PayloadLength = (ulong)ExtensionBytes.Length
                    + (ulong)ApplicationBytes.Length;
            }
        }
        public void SetExtensionData(string extensionData)
        {
            if (extensionData.Length > 0)
            {
                ExtensionBytes = Encoding.UTF8.GetBytes(extensionData);
                PayloadLength = (ulong)ExtensionBytes.Length
                    + (ulong)ApplicationBytes.Length;
            }
        }

        public byte[] GetMessageBytes()
        {
            EncodeFirstByte();
            EncodePayloadLength();
            EncodeMaskAndPayload();
            return Message;
        }

        private void EncodeMaskAndPayload()
        {
            if (Mask)
            {
                Message[1] = (byte)(Message[1] | 0x80);
                MaskBytes.CopyTo(Message, currentByteIndex);
                currentByteIndex += 4;
            }
            LoadPayloadData();
        }

        private void LoadPayloadData()
        {
            int i;
            for (i = 0; i < ExtensionBytes.Length; i++)
            {
                Message[i + currentByteIndex] = (byte)(ExtensionBytes[i] ^ MaskBytes[i % 4]);
            }
            currentByteIndex += i;
            for (i = 0; i < ApplicationBytes.Length; i++)
            {
                Message[i + currentByteIndex] = (byte)(ApplicationBytes[i] ^ MaskBytes[i % 4]);
            }
            currentByteIndex += i;
        }

        private void ExtendMessageLength(ulong newLength)
        {
            byte[] extendedMessage = new byte[newLength];
            Array.Copy(Message, extendedMessage, Message.Length);
            Message = extendedMessage;
        }

        private void EncodeSingleByteLength()
        {
            Message[1] += (byte)PayloadLength;
            currentByteIndex = 2;
            ExtendMessageLength(PayloadLength + 6);
        }

        private void EncodeTripleByteLength() {
            Message[1] = 0x7E;

            byte[] lengthBytes = BitConverter.GetBytes(PayloadLength);
            for (int i = 2; i < 4; i++)
            {
                Message[i] = lengthBytes[i - 2];
            }
            currentByteIndex = 4;
            ExtendMessageLength(PayloadLength + 8);
        }

        private void EncodeBig()
        {
            Message[1] = 0x7F;

            byte[] lengthBytes = BitConverter.GetBytes(PayloadLength);
            for (int i = 2; i < 10; i++)
            {
                Message[i] = lengthBytes[i - 2];
            }
            currentByteIndex = 10;
            ExtendMessageLength(PayloadLength + 14);
        }

        private void EncodePayloadLength()
        {
            if (PayloadLength < 126)
            {
                EncodeSingleByteLength();
            }
            else if (PayloadLength <= UInt16.MaxValue)
            {
                EncodeTripleByteLength();
            }
            else if (PayloadLength <= UInt64.MaxValue)
            {
                EncodeBig();
            }
            else
            {
                throw new Exception("Value not supported");
            }
        }

        private void EncodeFirstByte()
        {
            Message[0] = 0;
            Message[0] |= (byte)Opcode;
            if (FinBit)
            {
                Message[0] = Message[0] |= 0x80;
            }
            if (RSV1)
            {
                Message[0] = Message[0] |= 0x40;
            }
            if (RSV2)
            {
                Message[0] = Message[0] |= 0x20;
            }
            if (RSV3)
            {
                Message[0] = Message[0] |= 0x10;
            }
        }

        private void DecodeFirstByte()
        {
            var opcode = Message[0];
            if ((Message[0] & 0x80) == 0x80)
            {
                FinBit = true;
            }
            if ((Message[0] & 0x40) == 0x40)
            {
                RSV1 = true;
            }
            if ((Message[0] & 0x20) == 0x20) {
                RSV2 = true;
            }
            if ((Message[0] & 0x10) == 0x10)
            {
                RSV3 = true;
            }
            
            opcode = (byte)(Message[0] & ~0xF0);
            Opcode = (Opcode)opcode;
        }

        private void DecodeLengthBytes()
        {
            if ((Message[1] & 0x80) == 0x80)
            {
                PayloadLength = (ulong)(Message[1] + 128);

                if (PayloadLength > 125 && PayloadLength <= UInt16.MaxValue)
                {
                    EncodeTripleByteLength();
                }
                else if (PayloadLength <= UInt64.MaxValue)
                {
                    EncodeBig();
                }
                else
                {
                    throw new Exception("Value not supported");
                }
            }

        }

        private void DecodeSingleByteLength()
        {
            throw new NotImplementedException();
        }

        private void DecodePayload()
        {
            throw new NotImplementedException();
        }

        private void DecodeMessage()
        {
            DecodeFirstByte();
            DecodeLengthBytes();
            DecodePayload();
        }

        
    }
}
