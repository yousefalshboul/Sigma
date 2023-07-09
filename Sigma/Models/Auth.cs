using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigma.Models
{
    public class Auth
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }   
        public string LastName { get; set; }

        public string encode(string pass)
        {
            byte[] EncDataByte = new byte[pass.Length];
            EncDataByte = System.Text.Encoding.UTF8.GetBytes(pass);
            string EncryptedData = Convert.ToBase64String(EncDataByte);
            return EncryptedData;
        }
    }

}