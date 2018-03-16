using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcDic
{
    using System.Collections.Concurrent;

    static class DictUtils
    {
        public static void AddEntryToList<T1, T2>(ConcurrentDictionary<T1, List<T2>> d, T1 key, T2 listEntry)
        {
            if (d.TryAdd(key, new List<T2>()))
            {
                d[key].Add(listEntry);
            }
        }
    }

    class ReflectionCache
    {
        private static readonly ConcurrentDictionary<Type, List<MethodInfo>> dict = new ConcurrentDictionary<Type, List<MethodInfo>>();
        private static readonly ConcurrentBag<Type> done = new ConcurrentBag<Type>();
        public static List<MethodInfo> GetMethodList(Type type)
        {
            if (dict.TryAdd(type, new List<MethodInfo>()))
            {
                foreach (var property in type.GetProperties())
                {
                    dict[type].Add(property.GetGetMethod());
                }
                done.Add(type);
                return dict[type];
            }
            while (!done.Contains(type))
            {
            }
            return dict[type];
        }
    }

    internal class Program
    {
        class X
        {
            public int I { get; set; }
            public string S { get; set; }
        }
        class Y
        {
            public int I { get; set; }
            public string S { get; set; }
        }
        private static void Main(string[] args)
        {
            try
            {
                var list = ReflectionCache.GetMethodList(new X().GetType());
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
