using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace VireiContador.Infra.Helpers
{
    public class JsonLowerCaseUnderscoreContractResolver : DefaultContractResolver
    {
        private readonly Regex regex = new Regex("(?!(^[A-Z]))([A-Z])");

        protected override string ResolvePropertyName(string propertyName)
        {
            return regex.Replace(propertyName, "_$2").ToLower();
        }
    }
}
