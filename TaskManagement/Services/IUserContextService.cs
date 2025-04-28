namespace TaskManagement.Services
{
    public interface IUserContextService
    {
        public Guid GetUserId();
        public bool IsAdmin();
    }
}
