using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Cryptography
{
	public static class ElGamal
	{
		public class KeyBinding
		{
			internal BigInteger g;
			internal BigInteger p;
			internal BigInteger x;
			internal BigInteger y;

			public KeyBinding()
			{
				Generate();
			}

			public (BigInteger, BigInteger, BigInteger, BigInteger) Generate()
			{
				var p = Utils.GeneratePrime(8);
				var g = Utils.GetGroupGenerator(p, isPrime: true);
				var x = Utils.RandomRange(1, p);
				var y = BigInteger.ModPow(g, x, p);
				return (p, g, x, y);
			}

			public override string ToString() => $"g: {g}\np: {p}\nx: {x}\ny: {y}";
		}

		public static KeyBinding Keys { get; private set; } = new KeyBinding();

		public static BinaryReader Reader { get; set; }
		public static BinaryWriter Writer { get; set; }

		public static (BigInteger, BigInteger) Sign(byte[] m)
		{
			var k = Utils.GeneratePrimeRange(1, Keys.p - 1);
			var r = BigInteger.ModPow(Keys.g, k, Keys.p);
			var s = ((H(m) - Keys.x * r) * k.ModInverse(Keys.p - 1)).EuclidianMod(Keys.p - 1);
			return (r, s);
		}

		public static bool Verify(byte[] m, (BigInteger, BigInteger) sign)
		{
			var (r, s) = sign;
			if (0 >= r || r >= Keys.p || 0 >= s || s >= Keys.p - 1)
			{
				return false;
			}
			return 
				(BigInteger.ModPow(Keys.y, r, Keys.p) * BigInteger.ModPow(r, s, Keys.p)) % Keys.p == BigInteger.ModPow(Keys.g, H(m), Keys.p);
		}

		private static BigInteger H(byte[] m)
		{
			return new BigInteger(new SHA256Managed().ComputeHash(m).Extend());
		}
	}
}
