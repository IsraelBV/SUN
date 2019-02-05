using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Zip;

namespace Common.Helpers
{
public class ZipResult : ActionResult
    {
        private IEnumerable<string> _files;
        private string _fileName;

        public string FileName
        {
            get
            {
                return _fileName ?? "archivoCfdi.zip";
            }
            set { _fileName = value; }
        }

        public ZipResult( string nombreArchivo, params string[] files)
        {
            this._files = files;
            this._fileName = nombreArchivo;

        }

        public ZipResult(IEnumerable<string> files)
        {
            this._files = files;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            
            context.HttpContext.Response.ContentType = "application/zip";
            context.HttpContext.Response.AppendHeader("content-disposition", "attachment; filename=\""+ _fileName + "\"");
            context.HttpContext.Response.CacheControl = "Private";
            context.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddMinutes(3));

            byte[] buffer = new byte[4096];

            ZipOutputStream outputStream = new ZipOutputStream(context.HttpContext.Response.OutputStream);
            outputStream.SetLevel(0);

            foreach (var item in _files)
            {
                Stream fs = File.OpenRead(item);
                var fileName = Path.GetFileName(item);
                ZipEntry entry = new ZipEntry(ZipEntry.CleanName(fileName));
                entry.Size = fs.Length;

                outputStream.PutNextEntry(entry);

                int count = fs.Read(buffer, 0, buffer.Length);
                while (count > 0)
                {
                    outputStream.Write(buffer, 0, count);
                    count = fs.Read(buffer, 0, buffer.Length);
                    if (!context.HttpContext.Response.IsClientConnected)
                    {
                        break;
                    }
                    context.HttpContext.Response.Flush();
                }
                fs.Close();

                //using (ZipFile zf = new ZipFile())
                //{
                //    zf.AddFiles(_files, false, "");
                //    context.HttpContext.Response.ContentType = "application/zip";
                //    context.HttpContext.Response.AppendHeader("content-disposition", "attachment; filename=" + FileName);
                //    zf.Save(context.HttpContext.Response.OutputStream);
                //}
            }

            outputStream.Close();

            context.HttpContext.Response.Flush();
            context.HttpContext.Response.End();

        }
    }
}
