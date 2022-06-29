using JWTDecoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class JwtToken
    {
        public JwtHeader Header { get; set; }
        public string Payload { get; set; }
        public string Verification { get; set; }
    }

    public static class SecurityExtensions
    {
        public static JwtToken DecodeJwtToken(this string str)
        {
            var decodedToken = JWTDecoder.Decoder.DecodeToken(str);
            var header = decodedToken.Header;
            var payload = decodedToken.Payload;
            var verification = decodedToken.Verification;

            return new JwtToken
            {
                Header = header, 
                Payload = payload, 
                Verification = verification
            };
        }
    }
}
