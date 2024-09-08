namespace Identity.Core.Domain
{
    public class BaseEntityDomain
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }

        public BaseEntityDomain()
        {
            CreationDate = DateTime.Now;
        }
    }
}
