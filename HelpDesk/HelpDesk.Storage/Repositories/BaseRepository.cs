using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Storage.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly DataContext Context;

        public BaseRepository(DataContext dataContext) 
        { 
            Context = dataContext;
        }

        public void Add(T entity) 
        { 
            Context.Set<T>().Add(entity);
        }
        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
        }
    }
}
