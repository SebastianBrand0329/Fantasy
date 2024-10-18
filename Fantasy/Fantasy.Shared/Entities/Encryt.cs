using System.Security.Cryptography;
using System.Text;

namespace Fantasy.Shared.Entities
{
	public class Encryt
	{
		private enum EncryptMode
		{
			ENCRYPT,
			DECRYPT
		}

		private static int IV_LENGTH = 16;

		private static readonly char[] CharacterMatrixForRandomIVStringGeneration = new char[64]
		{
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
		'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
		'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
		'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
		'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
		'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7',
		'8', '9', '-', '_'
		};

		private static byte[] _iv { get; set; } = new byte[16];

		private static byte[] _key { get; set; } = new byte[32];

		public static string GenerateRandomIV()
		{
			Random random = new Random();
			using RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			byte[] data = new byte[IV_LENGTH];
			rNGCryptoServiceProvider.GetBytes(data);
			return new string((from s in Enumerable.Repeat(CharacterMatrixForRandomIVStringGeneration, IV_LENGTH)
							   select s[random.Next(s.Length)]).ToArray());
		}

		public static string GetHashSha256(string text, int length)
		{
			using SHA256 sHA = SHA256.Create();
			byte[] array = sHA.ComputeHash(Encoding.UTF8.GetBytes(text));
			string text2 = string.Empty;
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				text2 += $"{b:x2}";
			}

			if (length != 0)
			{
				if (length > text2.Length)
				{
					return text2;
				}

				return text2.Substring(0, length);
			}

			return text2;
		}

		private static string EncryptDecrypt(string _inputText, string Key, EncryptMode _mode, string iv)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			string result = string.Empty;
			byte[] bytes = uTF8Encoding.GetBytes(GetHashSha256(Key, 32));
			byte[] array = ((iv != null) ? uTF8Encoding.GetBytes(iv) : uTF8Encoding.GetBytes(GetHashSha256(Key.Substring(0, IV_LENGTH), IV_LENGTH)));
			int num = bytes.Length;
			if (num > _key.Length)
			{
				num = _key.Length;
			}

			int num2 = array.Length;
			if (num2 > _iv.Length)
			{
				num2 = _iv.Length;
			}

			Array.Copy(bytes, _key, num);
			Array.Copy(array, _iv, num2);
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.CBC;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			rijndaelManaged.Key = _key;
			rijndaelManaged.IV = _iv;
			if (_mode.Equals(EncryptMode.ENCRYPT))
			{
				byte[] inArray = rijndaelManaged.CreateEncryptor().TransformFinalBlock(uTF8Encoding.GetBytes(_inputText), 0, _inputText.Length);
				result = uTF8Encoding.GetString(_iv) + Convert.ToBase64String(inArray);
			}

			if (_mode.Equals(EncryptMode.DECRYPT))
			{
				byte[] bytes2 = rijndaelManaged.CreateDecryptor().TransformFinalBlock(Convert.FromBase64String(_inputText), 0, Convert.FromBase64String(_inputText).Length);
				result = uTF8Encoding.GetString(bytes2);
			}

			return result;
		}

		private static string Encrypt(string _plainText, string _key, string iv)
		{
			return EncryptDecrypt(_plainText, _key, EncryptMode.ENCRYPT, iv);
		}

		private static string Decrypt(string _encryptedText, string _key, string iv)
		{
			return EncryptDecrypt(_encryptedText, _key, EncryptMode.DECRYPT, iv);
		}

		public static string Encrypt(string text, string key)
		{
			try
			{
				return Encrypt(text, key, null);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static string Decrypt(string text, string key)
		{
			try
			{
				return Decrypt(text.Substring(16), key, text.Substring(0, 16));
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static string EncryptString(string plainText, string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(GetHashSha256(key, 32));
			byte[] inArray;
			using (Aes aes = Aes.Create())
			{
				aes.Key = bytes;
				aes.IV = _iv;
				ICryptoTransform transform = aes.CreateEncryptor();
				using MemoryStream memoryStream = new MemoryStream();
				using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					streamWriter.Write(plainText);
				}

				inArray = memoryStream.ToArray();
			}

			return Convert.ToBase64String(inArray);
		}

		public static string DecryptString(string cipherText, string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(GetHashSha256(key, 32));
			byte[] buffer = Convert.FromBase64String(cipherText);
			using Aes aes = Aes.Create();
			aes.Key = bytes;
			aes.IV = _iv;
			ICryptoTransform transform = aes.CreateDecryptor();
			using MemoryStream stream = new MemoryStream(buffer);
			using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
			using StreamReader streamReader = new StreamReader(stream2);
			return streamReader.ReadToEnd();
		}

		public static string DecryptHelp(string _inputText, string SECRET_KEY)
		{
			return EncryptionDecryption(_inputText.Substring(16), SECRET_KEY, EncryptMode.DECRYPT, _inputText.Substring(0, 16));
		}

		public static string EncryptHelp(string _inputText, string SECRET_KEY)
		{
			return EncryptionDecryption(_inputText, SECRET_KEY, EncryptMode.ENCRYPT, null);
		}

		private static string EncryptionDecryption(string _inputText, string key, EncryptMode _mode, string iv)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			string result = string.Empty;
			string hashSha = GetHashSha256(key, 16);
			byte[] bytes = uTF8Encoding.GetBytes(hashSha);
			byte[] array = ((iv != null) ? uTF8Encoding.GetBytes(iv) : uTF8Encoding.GetBytes(GenerateRandomIV()));
			int num = bytes.Length;
			byte[] array2 = new byte[16];
			byte[] array3 = new byte[16];
			if (num > array2.Length)
			{
				num = array2.Length;
			}

			int num2 = array.Length;
			if (num2 > array3.Length)
			{
				num2 = array3.Length;
			}

			Array.Copy(bytes, array2, num);
			Array.Copy(array, array3, num2);
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.CBC;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			rijndaelManaged.KeySize = 128;
			rijndaelManaged.BlockSize = 128;
			rijndaelManaged.Key = array2;
			rijndaelManaged.IV = array3;
			if (_mode.Equals(EncryptMode.ENCRYPT))
			{
				byte[] inArray = rijndaelManaged.CreateEncryptor().TransformFinalBlock(uTF8Encoding.GetBytes(_inputText), 0, _inputText.Length);
				result = uTF8Encoding.GetString(array3) + Convert.ToBase64String(inArray);
			}

			if (_mode.Equals(EncryptMode.DECRYPT))
			{
				byte[] array4 = Convert.FromBase64String(_inputText);
				byte[] bytes2 = rijndaelManaged.CreateDecryptor().TransformFinalBlock(array4, 0, array4.Length);
				result = uTF8Encoding.GetString(bytes2);
			}

			return result;
		}
	}
}