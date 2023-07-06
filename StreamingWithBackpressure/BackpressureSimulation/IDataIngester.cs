using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingWithBackpressure.BackpressureSimulation
{
    public interface IDataIngester
    {
        public Task IngestData();
    }
}
