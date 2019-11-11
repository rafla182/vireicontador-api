using System.Collections.Generic;
using System.Linq;

namespace VireiContador.Infra.Helpers
{
    public class ObjectReturnError : ObjectReturn
    {
        IDictionary<int, string> errorMessages = new Dictionary<int, string>
        {
            { -1, "Error" },
            { -2, "Operation not allowed" },
            { -3, "Invalid Token" },
            { -4, "Channel is null" },
            { -5, "Invalid Email Address" },
            { -6, "User Already Registered" },
            { -7, "UserAccount not found" },
            { -8, "Authentication Error" },
            { -9, "Missing Parameter" },
            { -10, "EmailSent is null" },
            { -11, "Wrong parameter format" },
            { -12, "Invalid AppSecret" },
            { -13, "AppSecret not allowed" },
            { -14, "Discontinued Method" },
            { -15, "Mobile Device not registered" },
            { -17, "Mobile Device Operating System not known" },
            { -18, "Region not found or not available" },
            { -19, "Deal not found" }
        };

        public ObjectReturnError(int code)
        {
            base.Code = code;
            base.Message = GetMessage(code);
            base.Result = null;
        }

        public ObjectReturnError(int code, object result)
        {
            base.Code = code;
            base.Message = GetMessage(code);
            base.Result = result;
        }

        public override string GetMessage(int code)
        {
            return errorMessages.FirstOrDefault(d => d.Key == code).Value;
        }
    }
}