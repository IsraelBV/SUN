using System.Collections.Generic;
using System.Linq;
using RH.Entidades;

namespace RH.BLL
{
    public class Bancos
    {
        readonly RHEntities _ctx;

        public Bancos()
        {
            _ctx = new RHEntities();
        }


        /// <summary>
        /// Retorna una lista de bancos
        /// </summary>
        /// <returns></returns>
        public List<C_Banco_SAT> GetBancos()
        {
            var lista = _ctx.C_Banco_SAT.ToList();

            return lista;
        }

        /// <summary>
        /// Retorna un banco encontrado por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public C_Banco_SAT GetBancoById(int id)
        {
            var banco = _ctx.C_Banco_SAT.FirstOrDefault(x => x.IdBanco == id);

            return banco;
        }

        /// <summary>
        /// Actualiza los datos del objeto banco
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bn"></param>
        /// <returns></returns>
        public bool UpdateBanco(int id, C_Banco_SAT bn)
        {
            var result = false;

            var item = _ctx.C_Banco_SAT.FirstOrDefault(x => x.IdBanco == id);
            
            //si no se encontro un registro con el id,
            // se detiene la actualizacion
            if (item == null) return false;

            item.Descripcion = bn.Descripcion;
            item.Status = bn.Status;

            var r = _ctx.SaveChanges();

            if( r > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Agregar un nuevo banco al catalogo
        /// </summary>
        /// <param name="bn"></param>
        /// <returns></returns>
        public bool CreateBanco(C_Banco_SAT bn)
        {
            var result = false;

            _ctx.C_Banco_SAT.Add(bn);
            var r = _ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Elimina un banco del catalogo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bn"></param>
        /// <returns></returns>
        public bool DeleteBanco(int id, C_Banco_SAT bn)
        {
            var result = false;

            var item = _ctx.C_Banco_SAT.FirstOrDefault(x => x.IdBanco == id);

            //si no se encontro un registro con ese id,
            // retornamos false 
            if (item == null) return false;

            _ctx.C_Banco_SAT.Attach(item);
            _ctx.C_Banco_SAT.Remove(item);

            var r = _ctx.SaveChanges();

            if (r > 0)
                result = true;

            return result;
        }

        public List<C_Banco_SAT> ObtenerBancosDisponibles()
        {
            return _ctx.C_Banco_SAT.Where(s => s.Status == true).ToList();
        }

        public List<string> ObtenerBancosDescripcion()
        {
            return _ctx.C_Banco_SAT.Where(s => s.Status == true).Select(s => s.Descripcion).ToList();
        }

        public int ObtenerIdBancoPorDescripcion(string descripcion)
        {
            return (from b in _ctx.C_Banco_SAT where b.Descripcion.Equals(descripcion) select b.IdBanco).FirstOrDefault();
        }
    }
}
