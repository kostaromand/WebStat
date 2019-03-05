using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.FileIO;
namespace WebStat
{
    public class CSVDataReader : DataReader
    {
        static int groupIndex = 5;
        static int requestIndex = 3;
        static int phraseFreqIndex = 11;
        static int accFreqIndex = 12;

        public CSVDataReader(string path)
        {
            this.path = path;
        }

        public override IEnumerable<RequestObject> ReadData()
        {
            var requestObjects = new List<RequestObject>();
            using (TextFieldParser tfp = new TextFieldParser(path, Encoding.Default))
            {
                tfp.TextFieldType = FieldType.Delimited;
                tfp.SetDelimiters(";");
                tfp.ReadFields();
                string[] fields;
                string group;
                string request;
                int phraseFreq;
                int accFreq;
                while (!tfp.EndOfData)
                {
                    fields = tfp.ReadFields();
                    group = fields[groupIndex];
                    request = fields[requestIndex];
                    phraseFreq = int.Parse(fields[phraseFreqIndex]);
                    accFreq = int.Parse(fields[accFreqIndex]);
                    requestObjects.Add(new RequestObject(group, request, phraseFreq, accFreq));
                }
                return requestObjects;
            }
        }
    }
}