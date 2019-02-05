using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RH.Entidades;

namespace RH.BLL
{
    public class DocumentosEmpleado
    {
        RHEntities ctx = null;

        public DocumentosEmpleado()
        {
            ctx = new RHEntities();
        }

        public bool DeleteDocumento(int IdDocumento)
        {
            var documento = ctx.DocumentosEmpleados.FirstOrDefault(x => x.IdDocumento == IdDocumento);
            ctx.DocumentosEmpleados.Remove(documento);
            var status = ctx.SaveChanges();
            return status > 0 ? true : false;
        }

        public bool DeleteDocumento(DocumentosEmpleados documento)
        {
            ctx.DocumentosEmpleados.Remove(documento);
            var status = ctx.SaveChanges();
            return status > 0 ? true : false;
        }

        public List<Documento> GetDocumentosEmpleado(int IdEmpleado)
        {
            return (from docs in ctx.DocumentosEmpleados
             join tipos in ctx.C_DocumentosTipo on docs.IdTipoDocumento equals tipos.IdTipoDocumento
             where docs.IdEmpleado == IdEmpleado
             select new Documento
             {
                 IdDocumento = docs.IdDocumento,
                 Descripcion = tipos.Descripcion,
                 NombreDocumento = docs.NombreDocumento,
                 IdTipoDocumento = docs.IdTipoDocumento
             }).ToList();
        }

        public List<C_DocumentosTipo> GetTiposDocumento()
        {
            return ctx.C_DocumentosTipo.ToList();
        }

        public String GetNombreTipoDocumento(int IdTipoDocumento)
        {
            return ctx.C_DocumentosTipo.Where(x => x.IdTipoDocumento == IdTipoDocumento).Select(x => x.Descripcion).FirstOrDefault();
        }

        public bool NewDocument(DocumentosEmpleados document)
        {
            ctx.DocumentosEmpleados.Add(document);
            var response = ctx.SaveChanges();
            return response > 0 ? true : false;
        }

        public DocumentosEmpleados GetDocumentoById(int IdDocumento)
        {
            return ctx.DocumentosEmpleados.FirstOrDefault(x => x.IdDocumento == IdDocumento);
        }
    }

    public class Documento
    {
        public int IdDocumento { get; set; }
        public string Descripcion { get; set; }
        public string NombreDocumento { get; set; }
        public int IdTipoDocumento { get; set; }
    }
}
