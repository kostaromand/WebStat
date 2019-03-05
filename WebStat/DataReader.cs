
using System.Collections.Generic;

namespace WebStat
{
    public abstract class DataReader
    {
        protected string path;
        public abstract IEnumerable<RequestObject> ReadData();
    }
}