using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Tools
{
    public static class AESHelper
    {
        /// <summary>
        /// 128位处理key 
        /// </summary>
        /// <param name="keyArray">原字节</param>
        /// <param name="key">处理key</param>
        /// <returns></returns>
        /// 如果key字节组大于16位则使用全0字节组进行加密
        private static byte[] GetAesKey(byte[] keyArray, string key)
        {
            byte[] newArray = new byte[16];
            if (keyArray.Length < 16)
            {
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i >= keyArray.Length)
                    {
                        newArray[i] = 0;
                    }
                    else
                    {
                        newArray[i] = keyArray[i];
                    }
                }
            }
            return newArray;
        }
        /// <summary>
        /// 使用AES加密字符串,按128位处理key
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>Base64字符串结果</returns>
        public static string AesEncrypt(string content, string key, bool autoHandle = true)
        {
            //生成key的字节组
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle)
            {
                keyArray = GetAesKey(keyArray, key);
            }
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);

            //生成创建对称加密算法对象
            SymmetricAlgorithm des = Aes.Create();
            //设置加密key子杰组
            des.Key = keyArray;
            //ecb加密模式
            des.Mode = CipherMode.ECB;
            //设置填充模式
            des.Padding = PaddingMode.PKCS7;
            //创建加密对象
            ICryptoTransform cTransform = des.CreateEncryptor();
            //加密
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            //返回加密后的字符串
            return Convert.ToBase64String(resultArray);
        }
        /// <summary>
        /// 使用AES解密字符串,按128位处理key
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>UTF8解密结果</returns>
        public static string AesDecrypt(string content, string key, bool autoHandle = true)
        {
            //生成key字节组
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle)
            {
                keyArray = GetAesKey(keyArray, key);
            }
            //解密对象字节组
            byte[] toEncryptArray = Convert.FromBase64String(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            //创建解密对象
            ICryptoTransform cTransform = des.CreateDecryptor();
            //解密
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            //返回解密后的字符串
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
