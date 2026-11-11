using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HTISORipper.Core;

namespace HTISORipper.UnitTests
{
    /// <summary>
    /// In-memory optical disc for automated tests. Can simulate transient and permanent medium errors,
    /// disc removal at a given LBA, and a callback per read (used to trigger cancellation).
    /// </summary>
    internal sealed class MockSectorSource : ISectorSource
    {
        private readonly byte[] _data;
        public readonly Dictionary<long, int> TransientFailures = new Dictionary<long, int>();
        public readonly HashSet<long> PermanentlyBad = new HashSet<long>();
        public long RemovedAtLba = -1;
        public Action<long> OnRead;
        public int ReadCalls;
        /// <summary>When set, sector contents returned by reads differ from the image at these LBAs (simulates a changed disc).</summary>
        public readonly HashSet<long> CorruptOnRead = new HashSet<long>();

        public MockSectorSource(byte[] data)
        {
            if (data.Length % 2048 != 0) throw new ArgumentException("data must be whole sectors");
            _data = data;
        }

        public int SectorSize { get { return 2048; } }
        public long SectorCount { get { return _data.Length / 2048; } }
        public byte[] Data { get { return _data; } }

        public SectorReadResult Read(long lba, int count, byte[] buffer, int offset)
        {
            ReadCalls++;
            if (OnRead != null) OnRead(lba);
            for (long s = lba; s < lba + count; s++)
            {
                if (RemovedAtLba >= 0 && s >= RemovedAtLba)
                    return new SectorReadResult(SectorReadStatus.NotReady, 21, "medium removed");
                if (s >= SectorCount || PermanentlyBad.Contains(s))
                    return new SectorReadResult(SectorReadStatus.MediumError, 23, "CRC error");
                int remaining;
                if (TransientFailures.TryGetValue(s, out remaining) && remaining > 0)
                {
                    TransientFailures[s] = remaining - 1;
                    return new SectorReadResult(SectorReadStatus.MediumError, 23, "transient CRC error");
                }
            }
            Buffer.BlockCopy(_data, (int)(lba * 2048), buffer, offset, count * 2048);
            foreach (long bad in CorruptOnRead)
                if (bad >= lba && bad < lba + count) buffer[offset + (int)(bad - lba) * 2048] ^= 0xFF;
            return SectorReadResult.Success;
        }
    }

    /// <summary>Builds small synthetic disc images with real ISO 9660 / Joliet / El Torito / UDF descriptors.</summary>
    internal static class IsoBuilder
    {
        public static byte[] Build(int sectors, string volumeId = "TEST_DISC", bool joliet = false, bool elTorito = false,
            bool udf = false, long? declaredVolumeBlocks = null, int seed = 1234)
        {
            var data = new byte[sectors * 2048];
            new Random(seed).NextBytes(data);   // non-trivial content so copy errors are detectable
            Array.Clear(data, 16 * 2048, 2048 * 8);

            int lba = 16;
            // Primary volume descriptor
            byte[] pvd = Descriptor(1);
            WriteAscii(pvd, 40, volumeId.PadRight(32));
            uint blocks = (uint)(declaredVolumeBlocks ?? sectors);
            BitConverter.GetBytes(blocks).CopyTo(pvd, 80);
            WriteBigEndian(pvd, 84, blocks);
            BitConverter.GetBytes((ushort)2048).CopyTo(pvd, 128);
            Put(data, lba++, pvd);

            if (elTorito)
            {
                byte[] boot = Descriptor(0);
                WriteAscii(boot, 7, "EL TORITO SPECIFICATION");
                Put(data, lba++, boot);
            }
            if (joliet)
            {
                byte[] svd = Descriptor(2);
                svd[88] = 0x25; svd[89] = 0x2F; svd[90] = 0x45;
                Put(data, lba++, svd);
            }
            Put(data, lba++, Descriptor(255));

            if (udf)
            {
                Put(data, lba++, Recognition("BEA01"));
                Put(data, lba++, Recognition("NSR02"));
                Put(data, lba++, Recognition("TEA01"));
                if (sectors > 256)
                {
                    byte[] anchor = new byte[2048];
                    BitConverter.GetBytes((ushort)2).CopyTo(anchor, 0);
                    int sum = 0;
                    for (int i = 0; i < 16; i++) if (i != 4) sum += anchor[i];
                    anchor[4] = (byte)sum;
                    Put(data, 256, anchor);
                }
            }
            else
            {
                // A sector that is not a recognition descriptor ends the scan.
                Put(data, lba, new byte[2048]);
            }
            return data;
        }

        private static byte[] Descriptor(byte type)
        {
            var d = new byte[2048];
            d[0] = type;
            WriteAscii(d, 1, "CD001");
            d[6] = 1;
            return d;
        }

        private static byte[] Recognition(string id)
        {
            var d = new byte[2048];
            WriteAscii(d, 1, id);
            d[6] = 1;
            return d;
        }

        private static void Put(byte[] image, int lba, byte[] sector)
        {
            Buffer.BlockCopy(sector, 0, image, lba * 2048, 2048);
        }

        private static void WriteAscii(byte[] target, int offset, string text)
        {
            Encoding.ASCII.GetBytes(text).CopyTo(target, offset);
        }

        private static void WriteBigEndian(byte[] target, int offset, uint value)
        {
            target[offset] = (byte)(value >> 24);
            target[offset + 1] = (byte)(value >> 16);
            target[offset + 2] = (byte)(value >> 8);
            target[offset + 3] = (byte)value;
        }
    }

    internal sealed class TempDir : IDisposable
    {
        public TempDir()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "htisoripper-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; private set; }
        public string File(string name) { return System.IO.Path.Combine(Path, name); }

        public void Dispose()
        {
            try { Directory.Delete(Path, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
        }
    }

    internal static class Sha
    {
        public static string Of(byte[] data)
        {
            using (var sha = Hashing.CreateSha256()) return Format.Hex(sha.ComputeHash(data));
        }
    }
}
