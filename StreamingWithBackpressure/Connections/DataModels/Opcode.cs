namespace StreamingWithBackpressure.Connections.DataModels
{
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
}