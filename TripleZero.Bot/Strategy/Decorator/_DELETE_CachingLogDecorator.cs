//using System;
//using System.Collections.Generic;
//using System.Text;
//using TripleZero.Helper;
//using TripleZero.Helper.Enum;
//using TripleZero.Infrastructure.DI;

//namespace TripleZero.Strategy.Decorator
//{
//    public class CachingLogDecorator : ICachingStrategy
//    {
//        private readonly ICachingStrategy _decorated;
//        public CachingLogDecorator(ICachingStrategy decorated)
//        {
//            this._decorated = decorated;
//        }
//        public bool CacheAdd(string key, object obj, short minutesBeforeExpiration)
//        {
//            Consoler.WriteLineInColor(string.Format("Cache add. Key : {0} - minutes : {1}", key, minutesBeforeExpiration), ConsoleColor.Yellow);
//            var res = _decorated.CacheAdd(key, obj, minutesBeforeExpiration);
//            if (res)
//                Consoler.WriteLineInColor(string.Format("Cache added. Key : {0} - minutes : {1}", key, minutesBeforeExpiration), ConsoleColor.Yellow);
//            else
//                Consoler.WriteLineInColor(string.Format("Cache not added! Key : {0} - minutes : {1}", key, minutesBeforeExpiration), ConsoleColor.Yellow);

//            return res;
//        }

//        public bool CacheAdd(string key, object obj)
//        {
//            Consoler.WriteLineInColor(string.Format("Cache add. Key : {0} - minutes : default", key), ConsoleColor.Yellow);
//            var res = _decorated.CacheAdd(key, obj);
//            if (res)
//                Consoler.WriteLineInColor(string.Format("Cache added. Key : {0} - minutes : default", key), ConsoleColor.Yellow);
//            else
//                Consoler.WriteLineInColor(string.Format("Cache not added! Key : {0} - minutes : default", key), ConsoleColor.Yellow);

//            return res;
//        }

//        public object CacheGetFromKey(string key)
//        {
//            var res = _decorated.CacheGetFromKey(key);
//            if (res == null)
//            {
//                Consoler.WriteLineInColor(string.Format("Didn't find results from cache for Key : {0}", key), ConsoleColor.Yellow);
//            }
//            else
//            {
//                Consoler.WriteLineInColor(string.Format("Found results from cache for Key : {0}", key), ConsoleColor.Yellow);
//            }
//            return res;
//        }

//        //public override bool CacheAdd(string key, object obj, short minutesBeforeExpiration)
//        //{
//        //    Consoler.WriteLineInColor(string.Format("Cache add. Key : {0} - minutes : {1}", key, minutesBeforeExpiration), ConsoleColor.Yellow);
//        //    var res = _decorated.CacheAdd(key, obj, minutesBeforeExpiration);
//        //    if (res)
//        //        Consoler.WriteLineInColor(string.Format("Cache added. Key : {0} - minutes : {1}", key, minutesBeforeExpiration), ConsoleColor.Yellow);
//        //    else
//        //        Consoler.WriteLineInColor(string.Format("Cache not added! Key : {0} - minutes : {1}", key, minutesBeforeExpiration), ConsoleColor.Yellow);

//        //    return res;
//        //}

//        //public override bool CacheAdd(string key, object obj)
//        //{
//        //    Consoler.WriteLineInColor(string.Format("Cache add. Key : {0} - minutes : default", key), ConsoleColor.Yellow);
//        //    var res = _decorated.CacheAdd(key, obj);
//        //    if (res)
//        //        Consoler.WriteLineInColor(string.Format("Cache added. Key : {0} - minutes : default", key), ConsoleColor.Yellow);
//        //    else
//        //        Consoler.WriteLineInColor(string.Format("Cache not added! Key : {0} - minutes : default", key), ConsoleColor.Yellow);

//        //    return res;
//        //}



//        //public override object CacheGetFromKey(string key)
//        //{
//        //    var res = _decorated.CacheGetFromKey(key);
//        //    if (res == null)
//        //    {
//        //        Consoler.WriteLineInColor(string.Format("Didn't find results from cache for Key : {0}", key), ConsoleColor.Yellow);
//        //    }
//        //    else
//        //    {
//        //        Consoler.WriteLineInColor(string.Format("Found results from cache for Key : {0}", key), ConsoleColor.Yellow);
//        //    }
//        //    return res;
//        //}
//    }
//}
