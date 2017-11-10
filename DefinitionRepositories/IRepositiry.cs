using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionRepositories
{
    public interface IRepositiry <T> where T: class
    {

        /*
         * GetAllWhere returned list records where func is true
         * IF func is null returned All records
         */
        IEnumerable<T> GetAllWhere(Func<T, bool> func = null);  
        T GetForId(int id); // вернет запись по конкретному id
        void Update(T entity);
        void Create(T entity);
        void Delete(int id);
        void SaveChange();

    }
}
