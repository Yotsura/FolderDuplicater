using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System;
using System.Linq;

namespace FileMirroringTool.Utils
{
    internal class EncryptUtils
    {
        internal EncryptUtils(string pass)
        {
            var key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            var passByte = Encoding.UTF8.GetBytes(pass);
            for (var i = 0; i < 32  ; i++)
            {
                if (passByte.Length <= i) break;
                key[i] = passByte[i];
            }
            _encryptKey = key;
        }

        readonly byte[] _encryptKey = new byte[32];
        readonly byte[] _dummy = new byte[16];
        readonly byte[] _buffer = new byte[8192];

        private AesManaged GetAesManaged() => new AesManaged
        {
            KeySize = 256,
            BlockSize = 128,
            Mode = CipherMode.CBC,
            IV = _dummy,
            Key = _encryptKey,
            Padding = PaddingMode.PKCS7
        };

        internal void EncryptFile(FileInfo sourceFile, FileInfo saveFile)
        {
            using (ICryptoTransform encryptor = GetAesManaged().CreateEncryptor())
            using (var src_fs = new FileStream(sourceFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var dst_fs = new FileStream(saveFile.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            // 一定サイズずつ暗号化して出力ファイルストリームに書き出す
            using (var cs = new CryptoStream(dst_fs, encryptor, CryptoStreamMode.Write))
            {
                // 先頭16バイトをdummyで埋める
                cs.Write(_dummy, 0, _dummy.Length);

                int len = 0;
                while ((len = src_fs.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    cs.Write(_buffer, 0, len);
                }
            }

            //比較に使用している最終更新日時をあわせる。
            saveFile.LastWriteTime = sourceFile.LastWriteTime;
        }

        internal void DecryptFile(FileInfo encriptedFile, FileInfo saveFile)
        {
            using (var decryptor = GetAesManaged().CreateDecryptor())
            using (var src_fs = new FileStream(encriptedFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var dst_fs = new FileStream(saveFile.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var cs = new CryptoStream(src_fs, decryptor, CryptoStreamMode.Read))
            {
                // 先頭16バイトは不要なのでまず復号して破棄
                cs.Read(_dummy, 0, _dummy.Length);

                int len = 0;
                while ((len = cs.Read(_buffer, 0, _buffer.Length)) > 0)
                {
                    dst_fs.Write(_buffer, 0, len);
                }
            }
            //比較に使用している最終更新日時をあわせる。
            saveFile.LastWriteTime = encriptedFile.LastWriteTime;
        }
    }
}
