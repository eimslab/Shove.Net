using System.Collections.Generic;

namespace Shove.Drawing.PNGConvertCore
{
    internal class Lookup
    {
        public int Alpha { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }

    internal class LookupData
    {
        public LookupData(int granularity)
        {
            Lookups = new List<Lookup>();
            Tags = new int[granularity, granularity, granularity, granularity];
        }

        public IList<Lookup> Lookups { get; private set; }
        public int[, , ,] Tags { get; private set; }

    }
}