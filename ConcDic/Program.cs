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
        private static readonly ConcurrentDictionary<Type, Lazy<List<MethodInfo>>> dict = new ConcurrentDictionary<Type, Lazy<List<MethodInfo>>>();
        public static Lazy<List<MethodInfo>> GetMethodList(Type type)
        {
            var result = dict.GetOrAdd(type, 
                t => new Lazy<List<MethodInfo>>(()=>
                {
                    return t.GetProperties().Select(property => property.GetGetMethod()).ToList();
                }));
            return result;
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
                Console.WriteLine(list.Value);
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
