using SearchEngineTask.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngineTask.Models
{
    public interface ITestInterface
    {
        int SomeMethod();

    }

    public class TestClass1 : ITestInterface
    {
        public int SomeMethod()
        {
            return 2;
        }

        public int SomeMethod2()
        {
            throw new NotImplementedException();
        }
    }

    public class TestClass2 : ITestInterface
    {
        public int SomeMethod()
        {
            var a = Task.Factory.StartNew(() => VAsync());

            return 1;
        }

        public async Task<int> VAsync()
        {
            SearchEngineDbContext db = new SearchEngineDbContext();
            var a = await db.SaveChangesAsync();

            return 0;
        }
    }
}
