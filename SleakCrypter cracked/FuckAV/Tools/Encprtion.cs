using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FuckAV.Tools
{
    internal class Encprtion
    {
        public static byte[] EncryptTripleDES(byte[] CShellcode, byte[] key, byte[] iv)
        {
            using (var des = TripleDES.Create())
            {
                des.KeySize = 128;
                des.BlockSize = 64;
                des.Padding = PaddingMode.PKCS7;
                des.Mode = CipherMode.CBC;

                des.Key = key;
                des.IV = iv;

                using (var encryptor = des.CreateEncryptor(des.Key, des.IV))
                {
                    using (var msEncShellCode = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(msEncShellCode, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(CShellcode, 0, CShellcode.Length);
                        cryptoStream.FlushFinalBlock();

                        return msEncShellCode.ToArray();
                    };
                }
            }
        }
        public static byte[] EncryptAES(byte[] CShellcode, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var msEncShellCode = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(msEncShellCode, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(CShellcode, 0, CShellcode.Length);
                        cryptoStream.FlushFinalBlock();

                        return msEncShellCode.ToArray();
                    };
                }
            }
        }
    }
}
