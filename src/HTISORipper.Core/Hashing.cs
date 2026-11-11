using System;
using System.Security.Cryptography;

namespace HTISORipper.Core
{
    public static class Hashing
    {
        /// <summary>
        /// SHA-256 backed by Windows CNG (Vista+, FIPS-validated, considerably faster than SHA256Managed).
        /// Falls back to the managed implementation if CNG is unavailable.
        /// </summary>
        public static SHA256 CreateSha256()
        {
            try
            {
                return new SHA256Cng();
            }
            catch (PlatformNotSupportedException)
            {
                return new SHA256Managed();
            }
            catch (CryptographicException)
            {
                return new SHA256Managed();
            }
        }
    }
}
