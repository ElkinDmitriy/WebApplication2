using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using DefinitionRepositories;

namespace Implementation
{
    public class UsersRepository : DefinitionRepositories.IRepositiry<Data.User>, IDisposable
    {
        BlogContext db;

        public UsersRepository()
        {
            db = new BlogContext();
        }

        public void Create(User entity)
        {
            db.Users.Add(entity);
        }

        public void Delete(int id)
        {
            Data.User tmpUser = db.Users.Find(id);
            if(tmpUser != null)
            {
                db.Users.Remove(tmpUser);
            }
        }

        public IEnumerable<User> GetAll()
        {
            return db.Users.ToList();
        }

        public User GetForId(int id)
        {
            return db.Users.Find(id);
        }

        public void SaveChange()
        {
            db.SaveChanges();
        }

        public void Update(Data.User entity)
        {
            try
            {
                Data.User tmpUser = db.Users.Single(u => u.Id == entity.Id);
                if (tmpUser != null)
                {
                    tmpUser.FirstName = entity.FirstName;
                    tmpUser.LastName = entity.LastName;
                    tmpUser.Email = entity.Email;
                    tmpUser.DateBirth = entity.DateBirth;
                    tmpUser.AboutMe = entity.AboutMe;
                    tmpUser.IsConfirm = entity.IsConfirm;
                    tmpUser.Password = entity.Password;
                    tmpUser.PhoneNumber = entity.PhoneNumber;
                }
            }
            catch (Exception ex) { }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~UsersRepository() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
