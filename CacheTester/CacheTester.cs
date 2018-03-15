using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace CacheTester
{
    [TestClass]
    public class CacheTester
    {
        [TestMethod]
        public void CacheTesterInt1()
        {
            var cache = new SetAssociativeCache.NWayAssociateCache<int, int>(1, 16, 
                (i,j)=> i-j , 
                (i,j)=>i-j, 
                i => 0,  //Select N 
                l => 0); //Select index 0 to delete in a set

            Assert.IsNotNull(cache);

            cache.SetValue(1, 78);
            int intValue = 0;
            cache.ReadValue(1, out intValue);

            Assert.AreEqual(intValue, 78);
        }

        [TestMethod]
        public void CacheTesterInt2()
        {
            var cache = new SetAssociativeCache.NWayAssociateCache<int, int>(1, 2,
                (i, j) => i - j,
                (i, j) => i - j,
                i => 0  //Select N 
                ); 

            Assert.IsNotNull(cache);

            cache.SetValue(1, 10);
            cache.SetValue(2, 20);
            cache.SetValue(3, 30);

            int intValue = 0;
            cache.ReadValue(2, out intValue);

            Assert.AreEqual(intValue, 20);
        }



        [TestMethod]
        public void CacheTesterString1()
        {
            var cache = new SetAssociativeCache.NWayAssociateCache<string, int>(1, 16,
                (i, j) => string.Compare(i,j),
                (i, j) => i - j,
                i => 0,  //Select N 
                l => l[0].Key ); //Select index 0 to delete in a set

            Assert.IsNotNull(cache);

            cache.SetValue("Key1", 154);
            int intValue = 0;
            cache.ReadValue("Key1", out intValue);

            Assert.AreEqual(intValue, 154);
        }
    }
}
