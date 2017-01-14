using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Main
{
    class Program
    {

        class simple
        {
            public int a { get; set; }
        }
        class twostring
        {
            public string a { get; set; }
            public string b { get; set; }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Size int :{0}", TestSize<int>.SizeOf(1));
            Console.WriteLine("Size long :{0}", TestSize<long>.SizeOf(long.MaxValue));
            Console.WriteLine("Size int?null :{0}", TestSize<int?>.SizeOf(null));
            Console.WriteLine("Size int? :{0}", TestSize<int?>.SizeOf(2));


            Console.WriteLine("Size string[10] :{0}", TestSize<string>.SizeOf("0123456789"));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 100; i++)
                sb.Append("0123456789");
            Console.WriteLine("Size stringbulder[10*100] :{0}", TestSize<StringBuilder>.SizeOf(sb));


            sb = new StringBuilder();
            for (int i = 0; i < 100; i++)
                sb.Append("わたしわたしわたしわ");
            Console.WriteLine("Size stringbulder U[10*100] :{0}", TestSize<StringBuilder>.SizeOf(sb));


            Console.WriteLine("Size C:simple :{0}", TestSize<simple>.SizeOf(new simple()));
            Console.WriteLine("Size C:strings :{0}", TestSize<twostring>.SizeOf(new twostring()));
            Console.WriteLine("Size C:strings[setted]:{0}", TestSize<twostring>.SizeOf(new twostring() { a = "0123456789", b = "0123456789" }));
            var arrayint = new int[100];
            Console.WriteLine("Size arrayint[100] :{0}", TestSize<int[]>.SizeOf(arrayint));

            var arraysimple = new simple[3];
            Console.WriteLine("Size simple[3] :{0}", TestSize<simple[]>.SizeOf(arraysimple));

            var list = new List<int>();
            Console.WriteLine("Size list Empty :{0}", TestSize<List<int>>.SizeOf(list));
            for (int i = 0; i < 100; i++) list.Add(i);
            Console.WriteLine("Size list<int>[100] :{0}", TestSize<List<int>>.SizeOf(list));

            Console.ReadLine();
        }
    }

    class TestSize<T>
    {

        static private int SizeOfObj(Type T, object thevalue)
        {
            var type = T;
            int returnval = 0;
            if (type.IsValueType)
            {
                var nulltype = Nullable.GetUnderlyingType(type);
                returnval = System.Runtime.InteropServices.Marshal.SizeOf(nulltype ?? type);
            }
            else if (thevalue == null)
                return 0;
            else if (thevalue is string)
                returnval = Encoding.Default.GetByteCount(thevalue as string);
            else if (type.IsArray && type.GetElementType().IsValueType)
            {
                returnval = ((Array)thevalue).GetLength(0) * System.Runtime.InteropServices.Marshal.SizeOf(type.GetElementType());
            }
            else if (thevalue is Stream)
            {
                Stream thestram = thevalue as Stream;
                returnval = (int)thestram.Length;
            }
            else if (type.IsSerializable)
            {
                try
                {
                    using (Stream s = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(s, thevalue);
                        returnval = (int)s.Length;
                    }
                }
                catch { }
            }
            else
            {
                var fields = type.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                for (int i = 0; i < fields.Length; i++)
                {
                    Type t = fields[i].FieldType;
                    Object v = fields[i].GetValue(thevalue);
                    returnval += 4 + SizeOfObj(t, v);
                }
            }
            if (returnval == 0)
                try
                {
                    returnval = System.Runtime.InteropServices.Marshal.SizeOf(thevalue);
                }
                catch { }
            return returnval;
        }
        static public int SizeOf(T value)
        {
            return SizeOfObj(typeof(T), value);
        }
    }
}
