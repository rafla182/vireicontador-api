using System;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace VireiContadorP.API.Custom.Extensions
{
    public static class AttributesExtensions
    {
        public static bool HasCustomAttribute(this ActionDescriptor actionDescriptor, Type attribute)
        {
            var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
            
            if (controllerActionDescriptor != null)
            {
                return controllerActionDescriptor.MethodInfo.IsDefined(attribute, false) || controllerActionDescriptor.ControllerTypeInfo.IsDefined(attribute, false);
            }

            return false;
        }
    }
}