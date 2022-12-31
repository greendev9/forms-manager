using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SharedAssemblies.Models;

namespace SharedAssemblies.DAL
{
  public class MyDbInitializer : CreateDatabaseIfNotExists<MyDbContext>
  {
    protected override void Seed(MyDbContext context)
    {

    }
  }
}