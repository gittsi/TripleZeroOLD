using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication1.SWGOH_Help_Api_C_Sharp_master.connectionHelper
{
    using System.Collections.ObjectModel;
    using Newtonsoft.Json.Serialization;
    public class ExpandoJSONConverter : JsonConverter 
    {
        //public override object Deserialize(IDictionary<string, object> dictionary, Type type, JsonSerializer serializer)
        //{
        //    throw new NotImplementedException();
        //}
        //public override IDictionary<string, object> Serialize(object obj, JsonSerializer serializer)
        //{
        //    var result = new Dictionary<string, object>();
        //    var dictionary = obj as IDictionary<string, object>;
        //    foreach (var item in dictionary)
        //        result.Add(item.Key, item.Value);
        //    return result;
        //}

        public IDictionary<string, object> Serialize(object obj, JsonSerializer serializer)
        {
            var result = new Dictionary<string, object>();
            var dictionary = obj as IDictionary<string, object>;
            foreach (var item in dictionary)
                result.Add(item.Key, item.Value);
            return result;
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        //public override IEnumerable<Type> SupportedTypes
        //{
        //    get
        //    {
        //        return new ReadOnlyCollection<Type>(new Type[] { typeof(System.Dynamic.ExpandoObject) });
        //    }
        //}
    }
}