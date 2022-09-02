using System;

namespace ProviderContract.Data
{
    /// <summary>
    /// API-ключи
    /// </summary>
    public class KeysData
    {
        /// <summary>
        /// Публичный ключ
        /// </summary>
        public string PublicKey { get; }
        /// <summary>
        /// Секретный ключ
        /// </summary>
        public string SecretKey { get; }
        /// <summary>
        /// Кодовая фраза - пароль (если требуется)
        /// </summary>
        public string PassPhrase { get; }

        /// <summary>
        /// API-ключи
        /// </summary>
        /// <param name="publicKey">Публичный ключ</param>
        /// <param name="secretKey">Секретный ключ</param>
        /// <param name="passPhrase">Кодовая фраза - пароль (если требуется)</param>
        public KeysData(string publicKey, string secretKey, string passPhrase = null)
        {
            PublicKey = publicKey;
            SecretKey = secretKey;
            PassPhrase = passPhrase;
        }
    }
}
