using System;
using System.Collections.Generic;
using System.Linq;


namespace diylazy
{
    class baseObject
    {
        public static int setSize = 500000;
        public string simpleString { get; set; }
        public int simpleInt { get; set; }

        protected IEnumerable<Tuple<double, string>> _complexList;

        public virtual IEnumerable<Tuple<double, string>> complexList
        {
            get
            {
                return (_complexList);
            }
        }

        protected IEnumerable<Tuple<double, string>> fakeDBCall()
        {
            // faux database call to consume time
            var rand = new Random(((int)DateTime.Now.Ticks % int.MaxValue));
            var result = new double[setSize];
            for (int x = 0; x < setSize; x++)
            {
                result[x] = rand.NextDouble() * double.MaxValue;
            }
            return ((result.Select(x => new Tuple<double, string>(x, x.ToString()))).ToList());
        }
    }

    class complexObject : baseObject
    {
        public complexObject()
        {
            simpleString = "Some string";
            simpleInt = 42;
            _complexList = fakeDBCall();
        }
    }

    class complexLazyObject : baseObject
    {
        private Lazy<IEnumerable<Tuple<double, string>>> _lazyList;

        public override IEnumerable<Tuple<double, string>> complexList
        {
            get
            {
                return (_lazyList.Value);
            }
        }

        public complexLazyObject()
        {
            simpleString = "Some string";
            simpleInt = 16309;
            _lazyList = new Lazy<IEnumerable<Tuple<double, string>>>(fakeDBCall);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Bench start");
            var nt1 = DateTime.Now;
            var normal = new complexObject();
            var nt2 = DateTime.Now;
            var normalDelta = nt2.Subtract(nt1);

            var lt1 = DateTime.Now;
            var lazy = new complexLazyObject();
            var lt2 = DateTime.Now;
            var lazyDelta = lt2.Subtract(lt1);

            var nt3 = DateTime.Now;
            var normalAccess = normal.complexList;
            var nt4 = DateTime.Now;
            if (normalAccess.Count() != baseObject.setSize) { throw new Exception("Data failure"); }
            var normalAccessDelta = nt4.Subtract(nt3);

            var lt3 = DateTime.Now;
            var lazyAccess = lazy.complexList;
            var lt4 = DateTime.Now;
            if (lazyAccess.Count() != baseObject.setSize) { throw new Exception("Data failure"); }
            var lazyAccessDelta = lt4.Subtract(lt3);

            var nt5 = DateTime.Now;
            var normalAccess2 = normal.complexList;
            var nt6 = DateTime.Now;
            if (normalAccess2.Count() != baseObject.setSize) { throw new Exception("Data failure"); }
            var normalAccessDelta2 = nt6.Subtract(nt5);

            var lt5 = DateTime.Now;
            var lazyAccess2 = lazy.complexList;
            var lt6 = DateTime.Now;
            if (lazyAccess2.Count() != baseObject.setSize) { throw new Exception("Data failure"); }
            var lazyAccessDelta2 = lt6.Subtract(lt5);


            Console.Out.WriteLine("Bench finish, results");
            Console.Out.WriteLine("Normal Constructor :\t\t{0:0.000} ms", normalDelta.TotalMilliseconds);
            Console.Out.WriteLine("Lazy Constructor :\t\t{0:0.000} ms", lazyDelta.TotalMilliseconds);
            Console.Out.WriteLine("Normal First Access :\t\t{0:0.000} ms", normalAccessDelta.TotalMilliseconds);
            Console.Out.WriteLine("Lazy First Access :\t\t{0:0.000} ms", lazyAccessDelta.TotalMilliseconds);
            Console.Out.WriteLine("Normal Second Access :\t\t{0:0.000} ms", normalAccessDelta2.TotalMilliseconds);
            Console.Out.WriteLine("Lazy Second Access :\t\t{0:0.000} ms", lazyAccessDelta2.TotalMilliseconds);
            Console.Out.Write("");
            Console.Out.Write("Press any key...");
            Console.In.Read();
        }
    }
}
