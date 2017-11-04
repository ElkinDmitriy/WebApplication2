using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionRepositories
{
    public interface IRepositiry <T> where T: class
    {

        IEnumerable<T> GetAll(); // будет получать список всех записей
        T GetForId(int id); // вернет запись по конкретному id
        void Update(T entity);
        void Create(T entity);
        void Delete(int id);
        void SaveChange();

    }
}
