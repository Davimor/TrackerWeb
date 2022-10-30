using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Documento
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ContentType { get; set; } = "";
        public byte[] Data { get; set; }
        public string UploadUser { get; set; } = "";
        public DateTime? UploadDate { get; set; }
        public bool Deleted { get; set; }
        public string DeleteUser { get; set; } = "";
        public DateTime? DeleteDate { get; set; }

        public Documento() { }

        public Documento(IFormFile f, Empleado user)
        {
            using (var ms = new MemoryStream())
            {
                f.CopyTo(ms);
                var fileBytes = ms.ToArray();
                Name = f.FileName;
                ContentType = MimeMapping.MimeUtility.GetMimeMapping(f.FileName);
                Data = fileBytes;
                UploadUser = user.IdUser;
                UploadDate = DateTime.Now;
            }
        }


    }
}
