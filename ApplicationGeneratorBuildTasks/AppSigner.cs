using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationGeneratorBuildTasks
{
    internal static class AppSigner
    {
        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct SIGNER_SUBJECT_INFO
        {
            public uint cbSize;
            public IntPtr pdwIndex;
            public uint dwSubjectChoice;
            public SubjectChoiceUnion Union1;
            [StructLayoutAttribute(LayoutKind.Explicit)]
            internal struct SubjectChoiceUnion
            {
                [FieldOffsetAttribute(0)]
                public System.IntPtr pSignerFileInfo;
                [FieldOffsetAttribute(0)]
                public System.IntPtr pSignerBlobInfo;
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct SIGNER_CERT
        {
            public uint cbSize;
            public uint dwCertChoice;
            public SignerCertUnion Union1;
            [StructLayoutAttribute(LayoutKind.Explicit)]
            internal struct SignerCertUnion
            {
                [FieldOffsetAttribute(0)]
                public IntPtr pwszSpcFile;
                [FieldOffsetAttribute(0)]
                public IntPtr pCertStoreInfo;
                [FieldOffsetAttribute(0)]
                public IntPtr pSpcChainInfo;
            };
            public IntPtr hwnd;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct SIGNER_SIGNATURE_INFO
        {
            public uint cbSize;
            public uint algidHash; // ALG_ID
            public uint dwAttrChoice;
            public IntPtr pAttrAuthCode;
            public IntPtr psAuthenticated; // PCRYPT_ATTRIBUTES
            public IntPtr psUnauthenticated; // PCRYPT_ATTRIBUTES
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct SIGNER_FILE_INFO
        {
            public uint cbSize;
            public IntPtr pwszFileName;
            public IntPtr hFile;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct SIGNER_CERT_STORE_INFO
        {
            public uint cbSize;
            public IntPtr pSigningCert; // CERT_CONTEXT
            public uint dwCertPolicy;
            public IntPtr hCertStore;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct SIGNER_PROVIDER_INFO
        {
            public uint cbSize;
            public IntPtr pwszProviderName;
            public uint dwProviderType;
            public uint dwKeySpec;
            public uint dwPvkChoice;
            public SignerProviderUnion providerUnion;
            [StructLayoutAttribute(LayoutKind.Explicit)]
            internal struct SignerProviderUnion
            {
                [FieldOffset(0)]
                public IntPtr pwszPvkFileName;
                [FieldOffset(0)]
                public IntPtr pwszKeyContainer;
            };
        }

        [DllImport("Mssign32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SignerSign(
            IntPtr pSubjectInfo,        // SIGNER_SUBJECT_INFO
            IntPtr pSignerCert,         // SIGNER_CERT
            IntPtr pSignatureInfo,      // SIGNER_SIGNATURE_INFO
            IntPtr pProviderInfo,       // SIGNER_PROVIDER_INFO
            string pwszHttpTimeStamp,   // LPCWSTR
            IntPtr psRequest,           // PCRYPT_ATTRIBUTES
            IntPtr pSipData            // LPVOID 
            );

        [DllImport("Mssign32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SignerTimeStamp(
            IntPtr pSubjectInfo,        // SIGNER_SUBJECT_INFO
            string pwszHttpTimeStamp,   // LPCWSTR
            IntPtr psRequest,           // PCRYPT_ATTRIBUTES
            IntPtr pSipData            // LPVOID 
            );

        [DllImport("Crypt32.DLL", EntryPoint = "CertCreateCertificateContext", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr CertCreateCertificateContext(
            int dwCertEncodingType,
            byte[] pbCertEncoded,
            int cbCertEncoded);

        public static void Sign(string appPath, X509Certificate2 certificate)
        {
            IntPtr pSignerCert = IntPtr.Zero;
            IntPtr pSubjectInfo = IntPtr.Zero;
            IntPtr pSignatureInfo = IntPtr.Zero;
            IntPtr pProviderInfo = IntPtr.Zero;
            try
            {
                // Grab the X509Certificate from the .pfx file.
                pSignerCert = CreateSignerCert(certificate);
                pSubjectInfo = CreateSignerSubjectInfo(appPath);
                pSignatureInfo = CreateSignerSignatureInfo();
                pProviderInfo = GetProviderInfo(certificate);
                SignCode(pSubjectInfo, pSignerCert, pSignatureInfo, pProviderInfo);
            }
            catch (CryptographicException ce)
            {
                string exception;
                // do anything with this useful information?
                switch (Marshal.GetHRForException(ce))
                {
                    case -2146885623:
                        exception = string.Format(@"An error occurred while attempting to load the signing certificate. ""{0}"" does not appear to contain a valid certificate.");
                        break;
                    case -2147024810:
                        exception = string.Format(@"An error occurred while attempting to load the signing certificate. The specified password was incorrect.");
                        break;
                    default:
                        exception = string.Format(@"An error occurred while attempting to load the signing certificate. {0}", ce.Message);
                        break;
                }
            }
            catch (Exception e)
            {
                // do anything with this useful information?
                string exception = e.Message;
            }
            finally
            {
                if (pSignerCert != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSignerCert, typeof(SIGNER_CERT));
                }
                if (pSubjectInfo != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSubjectInfo, typeof(SIGNER_SUBJECT_INFO));
                }
                if (pSignatureInfo != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSignatureInfo, typeof(SIGNER_SIGNATURE_INFO));
                }
                if (pProviderInfo != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSignatureInfo, typeof(SIGNER_PROVIDER_INFO));
                }
            }
        }

        public static void Sign(string appPath, string thumbprint)
        {
            var pSignerCert = IntPtr.Zero;
            var pSubjectInfo = IntPtr.Zero;
            var pSignatureInfo = IntPtr.Zero;
            var pProviderInfo = IntPtr.Zero;
            var certificate = FindCertByThumbprint(thumbprint);

            try
            {
                pSignerCert = CreateSignerCert(thumbprint);
                pSubjectInfo = CreateSignerSubjectInfo(appPath);
                pSignatureInfo = CreateSignerSignatureInfo();
                //pProviderInfo = GetProviderInfo(certificate);

                SignCode(pSubjectInfo, pSignerCert, pSignatureInfo, IntPtr.Zero);

                //if (tsaServer != null)
                //{
                //    TimeStampSignedCode(pSubjectInfo, tsaServer);
                //}
            }
            finally
            {
                if (pSignerCert != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSignerCert, typeof(SIGNER_CERT));
                }
                if (pSubjectInfo != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSubjectInfo, typeof(SIGNER_SUBJECT_INFO));
                }
                if (pSignatureInfo != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSignatureInfo, typeof(SIGNER_SIGNATURE_INFO));
                }
            }
        }

        public static void Sign(string appPath, X509Certificate2 certificate, string pwszPvkFileName)
        {
            var pSignerCert = IntPtr.Zero;
            var pSubjectInfo = IntPtr.Zero;
            var pSignatureInfo = IntPtr.Zero;
            var pProviderInfo = IntPtr.Zero;

            try
            {
                pSignerCert = CreateSignerCert(certificate);
                pSubjectInfo = CreateSignerSubjectInfo(appPath);
                pSignatureInfo = CreateSignerSignatureInfo();
                pProviderInfo = IntPtr.Zero;

                SignCode(pSubjectInfo, pSignerCert, pSignatureInfo, pProviderInfo);
            }
            finally
            {
                if (pSignerCert != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSignerCert, typeof(SIGNER_CERT));
                }
                if (pSubjectInfo != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSubjectInfo, typeof(SIGNER_SUBJECT_INFO));
                }
                if (pSignatureInfo != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pSignatureInfo, typeof(SIGNER_SIGNATURE_INFO));
                }
            }
        }

        private static IntPtr GetProviderInfo(X509Certificate2 cert)
        {
            if (cert == null || !cert.HasPrivateKey)
            {
                return IntPtr.Zero;
            }

            ICspAsymmetricAlgorithm key = (ICspAsymmetricAlgorithm)cert.PrivateKey;
            const int PVK_TYPE_KEYCONTAINER = 2;

            if (key == null)
            {
                return IntPtr.Zero;
            }

            SIGNER_PROVIDER_INFO providerInfo = new SIGNER_PROVIDER_INFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_PROVIDER_INFO)),
                pwszProviderName = Marshal.StringToHGlobalUni(key.CspKeyContainerInfo.ProviderName),
                dwProviderType = (uint)key.CspKeyContainerInfo.ProviderType,
                dwPvkChoice = PVK_TYPE_KEYCONTAINER,
                providerUnion = new SIGNER_PROVIDER_INFO.SignerProviderUnion
                {
                    pwszKeyContainer = Marshal.StringToHGlobalUni(key.CspKeyContainerInfo.KeyContainerName)
                },
            };

            IntPtr pProviderInfo = Marshal.AllocHGlobal(Marshal.SizeOf(providerInfo));
            Marshal.StructureToPtr(providerInfo, pProviderInfo, false);
            
            return pProviderInfo;
        }

        private static IntPtr CreateSignerSubjectInfo(string pathToAssembly)
        {
            var info = new SIGNER_SUBJECT_INFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_SUBJECT_INFO)),
                pdwIndex = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(uint)))
            };
            var index = 0;
            Marshal.StructureToPtr(index, info.pdwIndex, false);

            info.dwSubjectChoice = 0x1; //SIGNER_SUBJECT_FILE
            var assemblyFilePtr = Marshal.StringToHGlobalUni(pathToAssembly);

            var fileInfo = new SIGNER_FILE_INFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_FILE_INFO)),
                pwszFileName = assemblyFilePtr,
                hFile = IntPtr.Zero
            };

            info.Union1 = new SIGNER_SUBJECT_INFO.SubjectChoiceUnion
            {
                pSignerFileInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SIGNER_FILE_INFO)))
            };

            Marshal.StructureToPtr(fileInfo, info.Union1.pSignerFileInfo, false);

            IntPtr pSubjectInfo = Marshal.AllocHGlobal(Marshal.SizeOf(info));
            Marshal.StructureToPtr(info, pSubjectInfo, false);

            return pSubjectInfo;
        }

        private static X509Certificate2 FindCertByThumbprint(string thumbprint)
        {
            try
            {
                var store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (certs.Count == 0)
                {
                    throw new Exception(string.Format("Unable to find certificate with thumbprint '{0}'", thumbprint));
                }
                if (certs.Count > 1) // Can this happen?
                {
                    throw new Exception(string.Format("More than one certificate with thumbprint '{0}'", thumbprint));
                }
                store.Close();
                return certs[0];
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error locating certificate", e));
            }
        }

        private static IntPtr CreateSignerCert(X509Certificate2 cert)
        {
            SIGNER_CERT signerCert = new SIGNER_CERT
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_CERT)),
                dwCertChoice = 0x2,
                Union1 = new SIGNER_CERT.SignerCertUnion
                {
                    pCertStoreInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SIGNER_CERT_STORE_INFO)))
                },
                hwnd = IntPtr.Zero
            };

            const int X509_ASN_ENCODING = 0x00000001;
            const int PKCS_7_ASN_ENCODING = 0x00010000;

            IntPtr pCertContext = CertCreateCertificateContext(
                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                cert.GetRawCertData(),
                cert.GetRawCertData().Length);

            SIGNER_CERT_STORE_INFO certStoreInfo = new SIGNER_CERT_STORE_INFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_CERT_STORE_INFO)),
                pSigningCert = pCertContext,
                dwCertPolicy = 0x2, // SIGNER_CERT_POLICY_CHAIN
                hCertStore = IntPtr.Zero
            };

            Marshal.StructureToPtr(certStoreInfo, signerCert.Union1.pCertStoreInfo, false);

            IntPtr pSignerCert = Marshal.AllocHGlobal(Marshal.SizeOf(signerCert));
            Marshal.StructureToPtr(signerCert, pSignerCert, false);

            return pSignerCert;
        }

        private static IntPtr CreateSignerCert(string thumbprint)
        {
            var signerCert = new SIGNER_CERT
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_CERT)),
                dwCertChoice = 0x2,
                Union1 = new SIGNER_CERT.SignerCertUnion
                {
                    pCertStoreInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SIGNER_CERT_STORE_INFO)))
                },
                hwnd = IntPtr.Zero
            };

            const int X509_ASN_ENCODING = 0x00000001;
            const int PKCS_7_ASN_ENCODING = 0x00010000;

            var cert = FindCertByThumbprint(thumbprint);

            var pCertContext = CertCreateCertificateContext(
                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
                cert.GetRawCertData(),
                cert.GetRawCertData().Length);

            var certStoreInfo = new SIGNER_CERT_STORE_INFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_CERT_STORE_INFO)),
                pSigningCert = pCertContext,
                dwCertPolicy = 0x2, // SIGNER_CERT_POLICY_CHAIN
                hCertStore = IntPtr.Zero
            };

            Marshal.StructureToPtr(certStoreInfo, signerCert.Union1.pCertStoreInfo, false);

            IntPtr pSignerCert = Marshal.AllocHGlobal(Marshal.SizeOf(signerCert));
            Marshal.StructureToPtr(signerCert, pSignerCert, false);

            return pSignerCert;
        }

        private static IntPtr CreateSignerSignatureInfo()
        {
            var signatureInfo = new SIGNER_SIGNATURE_INFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_SIGNATURE_INFO)),
                algidHash = 0x00008004, // CALG_SHA1
                dwAttrChoice = 0x0, // SIGNER_NO_ATTR
                pAttrAuthCode = IntPtr.Zero,
                psAuthenticated = IntPtr.Zero,
                psUnauthenticated = IntPtr.Zero
            };
            IntPtr pSignatureInfo = Marshal.AllocHGlobal(Marshal.SizeOf(signatureInfo));
            Marshal.StructureToPtr(signatureInfo, pSignatureInfo, false);

            return pSignatureInfo;
        }

        private static void TimeStampSignedCode(IntPtr pSubjectInfo, string tsaServer)
        {
            var hResult = SignerTimeStamp(
                pSubjectInfo,
                tsaServer,
                IntPtr.Zero,
                IntPtr.Zero
                );

            if (hResult != 0)
            {
                throw new Exception(string.Format("Error timestamping signed installer - Error code 0x{0:X}", hResult));
            }
        }

        private static void SignCode(IntPtr pSubjectInfo, IntPtr pSignerCert, IntPtr pSignatureInfo, IntPtr pProviderInfo)
        {
            var hResult = SignerSign(pSubjectInfo, pSignerCert, pSignatureInfo, pProviderInfo, null, IntPtr.Zero, IntPtr.Zero);

            if (hResult != 0)
            {
                throw new Exception(string.Format("Error timestamping signed installer - Error code 0x{0:X}", hResult));
            }
        }
    }
}
