namespace VireiContador.Infra.Extensions
{
    public static class ValidacaoExtensions
    {
        public static bool Obrigatorio(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}