namespace WebStat
{
    //Объект, описывающий стуктуру запроса
    public class RequestObject
    {
        public string Group { get; set; } //группа
        public string Request { get; set; } //запрос
        public int PhraseFrequency { get; set; } //фразовая частота
        public int AccurateFrequency { get; set; } //точная частота
        
        public RequestObject(string group,string request,int phraseFrequency, int accurateFrequency)
        {
            this.Group = group;
            this.Request = request;
            this.PhraseFrequency = phraseFrequency;
            this.AccurateFrequency = accurateFrequency;
        }
    }
}
