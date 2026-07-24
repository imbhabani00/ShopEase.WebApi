namespace ShopEase.Application.DTOs.Response.Lookup
{
    #region LookupResponse
    public class LookupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    #endregion

    #region LookupResponseList
    public class LookupResponseList
    {
        public List<LookupResponse> LookupData { get; set; }
        public int ReturnValue { get; set; }
    }
    #endregion
}