namespace MagicVilla_VillaAPI.Models
{
    public class JWT
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audienece { get; set; }
        public string DurationInDays { get; set; }
    }
}
