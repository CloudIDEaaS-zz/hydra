using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AsymmetricKeyExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rsa = new RSACryptoServiceProvider();
            var publicKey = rsa.ToXmlString(false); // false to get the public key   
            var privateKey = rsa.ToXmlString(true); // true to get the private key   
        }
    }
}
