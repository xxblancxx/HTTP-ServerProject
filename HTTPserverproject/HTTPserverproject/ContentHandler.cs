using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTPserverproject
{

    class ContentHandler
    {
        public string GetContentType(string filepath)
        {
            string fileExtension = Path.GetExtension(filepath);

            switch (fileExtension.ToLower())
            {
                case ".txt":
                    return "text/plain";

                case ".html":
                case ".htm":
                    return "text/html";

                case ".jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp2";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                case ".docx":
                    return "application/msword";
                default:
                   throw new ArgumentException();

            }

        }
    }
}
