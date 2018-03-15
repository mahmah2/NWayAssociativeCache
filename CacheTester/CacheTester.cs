using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheTester
{
    [TestClass]
    public class CacheTester
    {
        [TestMethod]
        public void CacheTester1()
        {
            var cache = new SetAssociativeCache.NWayAssociateCache<int, int>(1, 16 * 1024, 4, 256);

            

            Assert.IsNotNull(cache);
        }
    }
}
