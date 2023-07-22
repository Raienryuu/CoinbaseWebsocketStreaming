using System.Collections;

namespace StreamingWithBackpressure.Connections.DataModels
{
    public class Level2Model
    {
        public string? type;
        public string? product_id;
        public required ArrayList changes;
        public DateTime? time;
    }
}
