using DotvvmBlog.DAL;

namespace DotvvmBlog.BL.Services
{
    public class BaseService
    {

        protected BlogDbContext CreateDbContext()
        {
            return new BlogDbContext();
        }

    }
}