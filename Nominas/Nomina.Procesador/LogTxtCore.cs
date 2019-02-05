using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomina.Procesador
{
    public static class LogTxtCore
    {

        //Escribir en el archivo
        public static async Task EscribirLogAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }

        public static void EscribirLog(string filePath, string text)
        {
            try
            {


                byte[] encodedText = Encoding.Unicode.GetBytes(text);

                using (FileStream sourceStream = new FileStream(filePath,
                    FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: false))
                {
                    sourceStream.Write(encodedText, 0, encodedText.Length);
                };
            }
            catch (Exception)
            {


            }
        }

        //Leer contenido
        public static async Task<string> LeerArchivoLogAsync(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true))
            {
                StringBuilder sb = new StringBuilder();

                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                return sb.ToString();
            }
        }

        //validar directorio
        public static bool ValidarDirectorio(string filePath)
        {
            var result = false;
            try
            {

                string dirName = new DirectoryInfo(filePath).FullName;

                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                    result = true;
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception)
            {

            }
            return result;
        }
    }
}
