using System.Collections.Generic;
using FluentValidator;

namespace VireiContadorP.API.Models.Responses
{
    public class ResponseBase
    {
        public bool Sucesso { get; set; }
        public object Resultado { get; set; }
        public IReadOnlyCollection<Notification> Erros { get; set; }
    }
}