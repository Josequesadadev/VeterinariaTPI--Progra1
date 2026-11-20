using System.Security.Cryptography;

namespace Veterinaria.Logica.Seguridad
{
    // Las contraseñas nunca se guardan en texto plano.
    // Se usa PBKDF2 (SHA-256) con una "sal" aleatoria por usuario.
    // Formato guardado en la base: iteraciones.sal.hash (sal y hash en Base64)
    public static class PasswordHasher
    {
        private const int TamanioSal = 16;
        private const int TamanioHash = 32;
        private const int Iteraciones = 100000;

        public static string Hashear(string password)
        {
            byte[] sal = RandomNumberGenerator.GetBytes(TamanioSal);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, sal, Iteraciones, HashAlgorithmName.SHA256, TamanioHash);
            return $"{Iteraciones}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string password, string hashGuardado)
        {
            string[] partes = hashGuardado.Split('.');
            if (partes.Length != 3 || !int.TryParse(partes[0], out int iteraciones))
            {
                return false;
            }

            byte[] sal = Convert.FromBase64String(partes[1]);
            byte[] hashEsperado = Convert.FromBase64String(partes[2]);
            byte[] hashIngresado = Rfc2898DeriveBytes.Pbkdf2(password, sal, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

            // Comparacion en tiempo constante
            return CryptographicOperations.FixedTimeEquals(hashIngresado, hashEsperado);
        }
    }
}
