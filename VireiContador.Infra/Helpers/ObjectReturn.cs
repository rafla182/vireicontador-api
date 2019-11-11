
namespace VireiContador.Infra.Helpers
{
    public abstract class ObjectReturn
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        
        protected ObjectReturn() { }

        public ObjectReturn(int code, string message, object result)
        {
            this.Code = code;
            this.Message = message;
            this.Result = result;
        }

        public abstract string GetMessage(int code);
    }
}