using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Cryptography
{
	class Application
	{
		public static void Main(string[] args)
		{
			//TestRSA();
			TestSignScheme();
			Console.Read();
		}

		public static void TestRSA()
		{
			RSA.Keys.Generate();
			RSA.Reader = new BinaryReader(File.Open("Tests/dog.jpg", FileMode.Open));
			RSA.Writer = new BinaryWriter(File.Open("Tests/dog_encrypted.jpg", FileMode.Create));
			RSA.Encrypt();
			RSA.Reader = new BinaryReader(File.Open("Tests/dog_encrypted.jpg", FileMode.Open));
			RSA.Writer = new BinaryWriter(File.Open("Tests/dog_decrypted.jpg", FileMode.Create));
			RSA.Decrypt();
		}

		public static void TestSignScheme()
		{
			RSA.Keys.Generate();
			RSA.Reader = new BinaryReader(File.Open("Tests/super_important.txt", FileMode.Open));
			RSA.Writer = new BinaryWriter(File.Open("Tests/super_important_encrypted.txt", FileMode.Create));
			RSA.Encrypt();

			var reader = new BinaryReader(File.Open("Tests/super_important_encrypted.txt", FileMode.Open));
			var message = reader.ReadBytes((int)reader.BaseStream.Length);
			var (g, p, A) = DiffieHellman.Alice.Pass();
			var B = DiffieHellman.Bob.Pass((g, p, A));

			ElGamal.Keys.Generate();
			var sign = ElGamal.Sign(message);
			var writer = new BinaryWriter(File.Open("Tests/super_important_encrypted_signed.txt", FileMode.Create));
			writer.Write(ElGamal.PackMessage(message, sign).Xor(DiffieHellman.Alice.K.ToByteArray()));
			writer.Close();

			reader = new BinaryReader(File.Open("Tests/super_important_encrypted_signed.txt", FileMode.Open));
			var bytes = reader.ReadBytes((int)reader.BaseStream.Length);
			var ans = bytes.Xor(DiffieHellman.Bob.K.ToByteArray());
			var r = ans.Take(32).ToArray();
			var s = ans.Skip(32).Take(32).ToArray();
			var m = ans.Skip(32 * 2).ToArray();
			Console.Write(ElGamal.Verify(m, (new BigInteger(r), new BigInteger(s))));
		}
	}
}
