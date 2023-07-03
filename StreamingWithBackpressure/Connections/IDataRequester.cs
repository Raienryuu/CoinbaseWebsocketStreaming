namespace StreamingWithBackpressure.Connections
{
    public interface IDataRequester
    {
        public Task TryGetDataFromWebSocketAsync();

    }
}