using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Diagnostics;
using SetAssociativeCache;
using System.Threading.Tasks;
using System.Threading;

namespace CacheTester
{
    [TestClass]
    public class CacheTester
    {
        [TestMethod]
        public void CacheTesterInt1()
        {
            //Testing 1 Way cache with 16 entries and integer key and integer value 
            var cache = new NWayAssociateCache<int, int>(1, 16, new IntKeyMapper());
            cache.SetRemoveAlgorithm(AlgorithmTypeEnum.Custom, new FirstEntrySelector<int>()); //always remove the first entry in a set


            Assert.IsNotNull(cache);

            cache.SetValue(1, 78);
            int intValue = 0;
            cache.ReadValue(1, out intValue);

            Trace.WriteLine(cache.ToString());

            Assert.AreEqual(intValue, 78);

            Trace.WriteLine(cache.ToString());
        }

        [TestMethod]
        public void CacheTesterInt2()
        {
            //Testing 1 Way cache with 2 entries and integer key and integer value 
            //Testing miss count
            var cache = new NWayAssociateCache<int, int>(1, 2, new IntKeyMapper());

            Assert.IsNotNull(cache);

            int missCount = 0;
            cache.OnMiss += (s,e) => { missCount++; };

            //We will put three values in a size of 2 cache so we should have one miss
            cache.SetValue(1, 10);
            cache.SetValue(2, 20);
            cache.SetValue(3, 30);

            Assert.AreEqual(missCount, 1);


            int intValue = 0;
            cache.ReadValue(2, out intValue);

            Assert.AreEqual(intValue, 20);
            Trace.WriteLine(cache.ToString());
        }

        [TestMethod]
        public void CacheTesterString1()
        {
            //Testing 1 Way cache with 16 entries and string key and integer value 
            var cache = new NWayAssociateCache<string, int>(1, 16, new StringKeyMapper()); 
            cache.SetRemoveAlgorithm(AlgorithmTypeEnum.Custom, new FirstEntrySelector<string>()); //always remove the first entry

            Assert.IsNotNull(cache);

            cache.SetValue("Key1", 154);
            int intValue = 0;
            cache.ReadValue("Key1", out intValue);

            Assert.AreEqual(intValue, 154);
            Trace.WriteLine(cache.ToString());
        }

        [TestMethod]
        public void CacheTesterStudent1()
        {
            //Testing 3 Way cache with 2 entries in each set and string key and Student class value 
            var cache = new NWayAssociateCache<string, Student>(3, 2,
                new  StringKeyMapper());  

            Assert.IsNotNull(cache);

            int missCount = 0;
            cache.OnMiss += (s, e) => { missCount++; };

            cache.SetValue("S01", new Student("Student01", 10)); //Fills first entry
            cache.SetValue("S02", new Student("Student02", 11)); //Fills second place
            cache.SetValue("S01", new Student("Student01", 13)); //Uppdates S01 timestamp and pushes S02 back in the list ordered by time 
            cache.SetValue("S03", new Student("Student03", 14)); //Insert new entry S02 should be removed

            cache.SetValue("S001", new Student("Student04", 20.05M));
            cache.SetValue("S002", new Student("Student05", 22.05M));
            cache.SetValue("S003", new Student("Student06", 23.05M));
            cache.SetValue("S004", new Student("Student07", 25.15M));

            cache.SetValue("S0001", new Student("Student08", 15.9M));
            cache.SetValue("S0002", new Student("Student09", 17.9M));
            cache.SetValue("S0003", new Student("Student10", 18.9M));

            Trace.WriteLine(cache.ToString());
            //Check output to see cache internals

            Assert.AreEqual(4, missCount);

            Student studentValue = default(Student);
            cache.ReadValue("S01", out studentValue);
            Assert.AreEqual(studentValue.Age, 13);

            cache.ReadValue("S003", out studentValue);
            Assert.AreEqual(studentValue.Age, 23.05M);

            cache.ReadValue("S0003", out studentValue);
            Assert.AreEqual(studentValue.Age, 18.9M);
        }

        [TestMethod]
        public void StudentKeyTest()
        {
            //Testing 3 Way cache with 2 entries in each set and Student key and int value 
            var cache = new NWayAssociateCache<Student, int>(3, 2, new StudentKeyMapper());

            cache.SetValue(new Student("S01", 10), 13); //Fills first entry
            cache.SetValue(new Student("S02", 11), 54); //Fills second place
            cache.SetValue(new Student("S01", 13), 66); //Uppdates S01 timestamp and pushes S02 back in the list ordered by time 
            cache.SetValue(new Student("S03", 14), 3); //Insert new entry S02 should be removed

            cache.SetValue(new Student("S001", 20.05M), 246);
            cache.SetValue(new Student("S002", 22.05M), 76);
            cache.SetValue(new Student("S003", 23.05M), 87);
            cache.SetValue(new Student("S004", 25.15M), 8);

            cache.SetValue(new Student("S0001", 15.9M), 2900);
            cache.SetValue(new Student("S0002", 17.9M), 155);
            cache.SetValue(new Student("S0003", 18.9M), 322);

            Trace.WriteLine(cache.ToString());

            int i = 0;
            cache.ReadValue(new Student("S01", 13), out i);
            Assert.AreEqual(66, i);

            cache.ReadValue(new Student("S003", 23.05M), out i);
            Assert.AreEqual(87, i);

            cache.ReadValue(new Student("S0003", 18.9M), out i);
            Assert.AreEqual(322, i);
        }

        [TestMethod]
        public void MultiThreadTest1()
        {
            //Testing 4 Way cache with 2 entries and integer key and integer value 
            var cache = new NWayAssociateCache<int, int>(5, 100, new IntKeyMapper());

            Assert.IsNotNull(cache);

            Task[] tasks = new Task[cache.NumbertOfSets];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() => {
                    int threadNo = i;

                    Random r = new Random();

                    for (int j = 0; j < cache.SetCapacity; j++)
                    {
                        var key = threadNo * cache.SetCapacity + j;

                        cache.SetValue(key , key);

                        Thread.Sleep(r.Next(5, 50));

                        int value;
                        var result = cache.ReadValue(key, out value);

                        Assert.IsTrue(result);

                        Assert.AreEqual(key, value);
                    }
                }
                );
            }

            Task.WaitAll(tasks);
        }
        
    }
}
