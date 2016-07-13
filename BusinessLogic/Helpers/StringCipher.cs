namespace Business_Logic.Helpers
{
    public static class StringCipher
    {
        #region Public Methods

        public static string Encrypt(string plainText)
        {
            return CryptographyHelper.GetProtectedBase64String(plainText);
        }

        public static string Decrypt(string cipherText)
        {
            return CryptographyHelper.GetUnpotectedStringFromBase64(cipherText);
        }

        #endregion
    }
}
