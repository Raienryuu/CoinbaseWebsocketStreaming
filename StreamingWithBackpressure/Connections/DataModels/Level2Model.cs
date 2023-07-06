using System.Collections;

namespace StreamingWithBackpressure.Connections.DataModels
{
    public class Level2Model
    {
        public string? type { get; set; }
        public string? product_id { get; set; }
        public required ArrayList changes { get; set; }
        public DateTime? time { get; set; }
    }
}
