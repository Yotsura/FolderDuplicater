using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace FileMirroringTool.Utils
{
    internal static class EncryptUtils
    {
        const int KEY_SIZE = 256;
        const int BLOCK_SIZE = 128;
        const int BUFFER_SIZE = BLOCK_SIZE * 64;  // バッファーサイズはBlockSizeの倍数にする
        const int SALT_SIZE = 25; // 8以上

        private static AesManaged GetAesManaged() => new AesManaged
        {
            KeySize = KEY_SIZE,
            BlockSize = BLOCK_SIZE,
            Mode = CipherMode.CBC,
            Padding = PaddingMode.ISO10126
        };

        private static byte[] GetSalt()
            => new Rfc2898DeriveBytes(DateTime.Now.Ticks.ToString(), SALT_SIZE).Salt;

        //パスワードから暗号キーを作成する
        private static byte[] GetKeyFromPassword(string password, byte[] salt)
        {
            //Rfc2898DeriveBytesオブジェクトを作成する
            var deriveBytes = new Rfc2898DeriveBytes(password, salt);
            //反復処理回数を指定する デフォルトで1000回
            deriveBytes.IterationCount = 1000;
            //キーを生成する
            return deriveBytes.GetBytes(KEY_SIZE / 8);
        }

        //パスワードから初期化ベクトルを作成する
        private static byte[] GetIVFromPassword(string password)
        {
            var deriveBytes = new Rfc2898DeriveBytes(password + DateTime.Now.Ticks.ToString(), 100);
            deriveBytes.IterationCount = 1000;
            return deriveBytes.GetBytes(BLOCK_SIZE / 8);
        }

        public static void EncryptFile(string srcFilePath, string destFilePath, string password)
        {
            byte[] salt = GetSalt();
            byte[] iv = GetIVFromPassword(password);
            byte[] key = GetKeyFromPassword(password, salt);
            try
            {
                using (var dst_fs = new FileStream(destFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    dst_fs.Write(salt, 0, salt.Length);
                    dst_fs.Write(iv, 0, iv.Length);

                    using (var src_fs = new FileStream(srcFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (AesManaged aes = GetAesManaged())
                    using (var cs = new CryptoStream(dst_fs, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                    using (var ds = new DeflateStream(cs, CompressionMode.Compress))
                    {
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int len = 0;
                        while ((len = src_fs.Read(buffer, 0, buffer.Length)) > 0)
                            ds.Write(buffer, 0, len);   //圧縮→暗号化→書き込み
                    }
                }
                //比較に使用している最終更新日時をあわせる。
                File.SetLastWriteTime(destFilePath, File.GetLastWriteTime(srcFilePath));
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public static void DecryptFile(string srcFilePath, string destFilePath, string password)
        {
            try
            {
                var decryptedData = new List<byte>();
                using (var src_fs = new FileStream(srcFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] salt = src_fs.ReadBytes(SALT_SIZE);
                    byte[] key = GetKeyFromPassword(password, salt);
                    byte[] iv = src_fs.ReadBytes(BLOCK_SIZE / 8);

                    using (AesManaged aes = GetAesManaged())
                    using (var cs = new CryptoStream(src_fs, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    using (var ds = new DeflateStream(cs, CompressionMode.Decompress))
                    {
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int len = 0;
                        while ((len = ds.Read(buffer, 0, buffer.Length)) > 0)
                            decryptedData.AddRange(buffer.Take(len));
                    }
                }
                using (var dst_fs = new FileStream(destFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    dst_fs.Write(decryptedData.ToArray(), 0, decryptedData.Count);

                //比較に使用している最終更新日時をあわせる。
                File.SetLastWriteTime(destFilePath, File.GetLastWriteTime(srcFilePath));
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        static byte[] ReadBytes(this FileStream fs, int datalength)
        {
            var data = new byte[datalength];
            fs.Read(data, 0, datalength);
            return data;
        }
    }
}
