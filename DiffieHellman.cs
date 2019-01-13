using System.Numerics;

namespace Cryptography
{
	public static class DiffieHellman
	{
		private static BigInteger a;
		private static BigInteger b;

		public static BigInteger g;
		public static BigInteger p;
		public static BigInteger A;
		public static BigInteger B;

		public static class Alice
		{
			public static BigInteger K
			{
				get => BigInteger.ModPow(B, a, p);
			}

			public static (BigInteger, BigInteger, BigInteger) Pass()
			{
				var n = Utils.GeneratePrime(32);
				p = Utils.GeneratePrimeRange(10, n);
				g = Utils.GetGroupGenerator(p, true);
				a = Utils.GeneratePrimeRange(10, n);
				A = BigInteger.ModPow(g, a, p);
				return (g, p, A);
			}
		}

		public static class Bob
		{
			public static BigInteger K
			{
				get => BigInteger.ModPow(A, b, p);
			}

			public static BigInteger Pass((BigInteger, BigInteger, BigInteger) gpA)
			{
				(g, p, A) = gpA;
				var n = Utils.GeneratePrime(32);
				b = Utils.GeneratePrimeRange(10, n);
				B = BigInteger.ModPow(g, b, p);
				return B;
			}
		}
	}
}
