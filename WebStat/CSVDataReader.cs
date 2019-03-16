using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.FileIO;
namespace WebStat
{
    public class CSVDataReader : DataReader
    {
        int groupIndex = 5;
        int requestIndex = 3;
        int phraseFreqIndex = 11;
        int accFreqIndex = 12;
        int urlIndex = 14;
        int positionIndex = 13;
        int domainIndex = 2;
        public CSVDataReader(string path)
        {
            this.path = path;
        }

        public override IEnumerable<RequestObject> ReadData()
        {
            var requestObjects = new List<RequestObject>();
            string[] fields;
            using (TextFieldParser tfp = new TextFieldParser(path, Encoding.Default))
            {
                tfp.TextFieldType = FieldType.Delimited;
                tfp.SetDelimiters(";");
                tfp.ReadFields();
                while (!tfp.EndOfData)
                {
                    fields = tfp.ReadFields();

                    requestObjects.Add(new RequestObject(
                        fields[domainIndex],
                        fields[groupIndex],
                        fields[requestIndex],
                        int.Parse(fields[phraseFreqIndex]),
                        int.Parse(fields[accFreqIndex]),
                        int.Parse(fields[positionIndex]),
                        fields[urlIndex])
                        );
                }
                return requestObjects;
            }
        }
    }
}