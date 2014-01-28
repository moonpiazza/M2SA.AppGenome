using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace M2SA.AppGenome.Cache.MemCached
{
	/// <summary>
	/// Fowler-Noll-Vo hash, variant 1, 32-bit version.
	/// http://www.isthe.com/chongo/tech/comp/fnv/
	/// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    [CLSCompliant(false)]
    public class FNV1_32 : HashAlgorithm
	{
		private static readonly uint FNV_prime = 16777619;
		private static readonly uint offset_basis = 2166136261;

        /// <summary>
        /// 
        /// </summary>
		protected uint hash;

        /// <summary>
        /// 
        /// </summary>
		public FNV1_32() {
			HashSizeValue = 32;
		}

        /// <summary>
        /// 
        /// </summary>
		public override void Initialize() {
			hash = offset_basis;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="ibStart"></param>
        /// <param name="cbSize"></param>
		protected override void HashCore(byte[] array, int ibStart, int cbSize) {
			int length = ibStart + cbSize;
			for (int i = ibStart; i < length; i++) {
				hash = (hash * FNV_prime)^array[i];
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		protected override byte[] HashFinal() {
			return BitConverter.GetBytes(hash);
		}
	}

	/// <summary>
	/// Fowler-Noll-Vo hash, variant 1a, 32-bit version.
	/// http://www.isthe.com/chongo/tech/comp/fnv/
	/// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    [CLSCompliant(false)]
    public class FNV1a_32 : HashAlgorithm
	{
		private static readonly uint FNV_prime = 16777619;
		private static readonly uint offset_basis = 2166136261;
        /// <summary>
        /// 
        /// </summary>
		protected uint hash;

        /// <summary>
        /// 
        /// </summary>
		public FNV1a_32() {
			HashSizeValue = 32;
		}

        /// <summary>
        /// 
        /// </summary>
		public override void Initialize() {
			hash = offset_basis;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="ibStart"></param>
        /// <param name="cbSize"></param>
		protected override void HashCore(byte[] array, int ibStart, int cbSize) {
			int length = ibStart + cbSize;
			for (int i = ibStart; i < length; i++) {
				hash = (hash^array[i]) * FNV_prime;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		protected override byte[] HashFinal() {
			return BitConverter.GetBytes(hash);
		}
	}

	/// <summary>
	/// Modified Fowler-Noll-Vo hash, 32-bit version.
	/// http://home.comcast.net/~bretm/hash/6.html
	/// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
    [CLSCompliant(false)]
    public class ModifiedFNV1_32 : FNV1_32
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		protected override byte[] HashFinal() {
			hash += hash << 13;
			hash ^= hash >> 7;
			hash += hash << 3;
			hash ^= hash >> 17;
			hash += hash << 5;
			return BitConverter.GetBytes(hash);
		}
	}
}