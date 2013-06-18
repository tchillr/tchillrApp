using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace TchillrREST.Contract
{
    public class ContractResolver : DefaultContractResolver
    {
        public ContractResolver()
        {
        }
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            // only serializer properties that start with the specified character
            properties = properties.Where(p => !p.PropertyName.Contains("Entity") ).ToList();

            return properties;
        }
    }
}
