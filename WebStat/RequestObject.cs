namespace WebStat
{
    //Объект, описывающий стуктуру запроса
    public class RequestObject
    {
        public string Domain { get; set; }
        public string[] Groups { get; set; } //группа
        public string Request { get; set; } //запрос
        public int PhraseFrequency { get; set; } //фразовая частота
        public int AccurateFrequency { get; set; } //точная частота
        public string URL { get; set; } //ulr
        public int Position { get; set; } //ulr
        public RequestObject(string domain,string groups,string request,int phraseFrequency, int accurateFrequency,int position, string url)
        {
            this.Domain = domain;
            this.Groups = groups.Split('.');
            this.Request = request;
            this.PhraseFrequency = phraseFrequency;
            this.AccurateFrequency = accurateFrequency;
            this.URL = url;
            this.Position = position;
        }
    }
}
