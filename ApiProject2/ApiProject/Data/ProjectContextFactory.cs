using Microsoft.EntityFrameworkCore;

namespace ApiProject.Data
{
    public class ProjectContextFactory
    {

        //private const string ConnectionString = "Server=Srv2\\pupils;DataBase=StoreDataBase2;Integrated Security=SSPI;Persist Security Info=False;TrustServerCertificate=True;";
        private const string ConnectionString = "Server=HomeJerPC;DataBase=ProjectApiDB;Integrated Security=SSPI;Persist Security Info=False;TrustServerCertificate=True;";

        //private const string ConnectionString = "Server=Srv2\\pupils;DataBase=3411111912_ProjectAPI;Integrated Security=SSPI;Persist Security Info=False;TrustServerCertificate=True;";

        public static ProjectContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectContext>();
            optionsBuilder.UseSqlServer(ConnectionString);
            return new ProjectContext(optionsBuilder.Options);
        }
    }
}