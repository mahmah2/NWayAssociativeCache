using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheTester
{
    public class Student :  IComparable<Student>
    {
        public Student(string name, decimal age)
        {
            Name = name;
            Age = age;
        }
        public string Name { get; set; }
        public decimal Age { get; set; }

        public int CompareTo(Student other)
        {
            return  Name == other.Name &&
                    Age == other.Age ? 0 : 1; // 1 is controversial but we only interested in the case that they are equal
        }

        public override string ToString()
        {
            return $"{{{Name}, {Age}}}";
        }
    }
}
