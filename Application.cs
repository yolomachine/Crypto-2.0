using System.IO;

namespace Cryptography
{
	class Application
	{
		public static void Main(string[] args)
		{
			//TestRSA();
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
	}
}
