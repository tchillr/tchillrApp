using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure;

namespace TchillrREST.Data
{
    public class MigrationsContextFactory : IDbContextFactory<TchillrDBContext>
    {
        public TchillrDBContext Create()
        {
            return new TchillrDBContext("Server=tcp:myuc6ta27d.database.windows.net,1433;Database=TchillrDB;User ID=TchillrSGBD@myuc6ta27d;Password=Tch1llrInTown;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;");
        }
    }
}