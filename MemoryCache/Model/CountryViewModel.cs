namespace MemoryCache.Model
{
    public class CountryViewModel
    {
        public NameData Name { get; set; }
        public object Capital { get; set; }
        public string Region { get; set; }
        public Dictionary<string, string> Languages { get; set; }

        public class NameData
        {
            public string Common { get; set; }
        }
    }
}
