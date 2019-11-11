using System.Collections.Generic;
using System.Linq;

namespace VireiContador.Infra.Helpers
{
    public class ObjectReturnSuccess : ObjectReturn
    {
        IDictionary<int, string> successMessages = new Dictionary<int, string>
        {
            { 1, "Success" }
        };

        public ObjectReturnSuccess(int code, object result)
        {
            base.Code = code;
            base.Message = GetMessage(code);
            base.Result = result;
        }

        public override string GetMessage(int code)
        {
            return successMessages.FirstOrDefault(d => d.Key == code).Value;
        }
    }
}