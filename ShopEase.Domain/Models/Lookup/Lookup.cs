namespace ShopEase.Domain.Models.Lookup
{
    #region Lookup
    public class Lookup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    #endregion

    #region LookupList
    public class LookupList
    {
        public List<Lookup> LookupData { get; set; }
        public int ReturnValue { get; set; }
    }
    #endregion
}