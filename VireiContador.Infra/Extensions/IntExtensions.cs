namespace VireiContador.Infra.Extensions
{
    public static class IntExtensions
    {
        public static int Inicio(this int pagina, int quantidade)
        {
            return (pagina - 1) * quantidade;
        }
    }
}
