using AutoMapper;
using CentralNegocio.Application.Interfaces.Persistence;
using CentralNegocio.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace CentralNegocio.Persistence.Repositories.EFCore
{
    public class ClienteRespository: IClienteRepository
    {
        ApplicationDbContext _context;
        private readonly IMapper mapper;
        private List<string> _listExclude;
        public ClienteRespository(ApplicationDbContext dbcontext, IMapper mapper)
        {
            string PropiedadesGenerales = "Status,Edit,Add,View,Grid,migration";
            _context = dbcontext;
            this.mapper = mapper;
            _listExclude = PropiedadesGenerales.Split(',')?.ToList()!;
        }

        public DbSet<T> GetMore<T>() where T : class
        {
            return _context.Set<T>();
        }

        public IQueryable<T> GetIncludesAsNoTraking<T>(params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.AsNoTracking();
        }


        public IQueryable<T> GetAsNoTracking<T>() where T : class
        {
            return _context.Set<T>().AsNoTracking();
        }


        public async Task<T> Save<T>(T model) where T : class
        {
            _context.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<List<T>> SaveRange<T>(List<T> model) where T : class
        {
            await _context.AddRangeAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<T> Update<T>(int Id, T model, List<string>? PropiedadesExcluir = null) where T : class
        {
            PropiedadesExcluir = PropiedadesExcluir ?? new List<string>();
            var modelUpdate = await _context.Set<T>().FindAsync(Id)!;
            if (modelUpdate != null)
            {
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    var value = propertyInfo.GetValue(model);
                    if (value != null && !IsNullOrDefault(value))
                        propertyInfo.SetValue(modelUpdate, value);

                    if (_listExclude.Contains(propertyInfo.Name) || PropiedadesExcluir.Contains(propertyInfo.Name))
                        propertyInfo.SetValue(modelUpdate, value);
                }
                await _context.SaveChangesAsync();
                return modelUpdate;
            }
            return model;
        }


        public async Task<T> SaveUpdate<T>(int Id, T model) where T : class
        {
            var modelUpdate = await _context.Set<T>().FindAsync(Id);
            if (modelUpdate != null)
            {
                //modelUpdate = model;
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    var value = propertyInfo.GetValue(model);

                    if (value != null && !IsNullOrDefault(value))
                    {
                        propertyInfo.SetValue(modelUpdate, value);
                    }
                }
                await _context.SaveChangesAsync();
                return modelUpdate;
            }
            else
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return model;
            }
        }
        private bool IsNullOrDefault(object value)
        {
            if (value == null) return true;
            if (value.GetType().IsValueType)
            {
                return value.Equals(Activator.CreateInstance(value.GetType()));
            }
            return false;
        }

        public async Task DeletePhysical<T>(int Id) where T : class
        {
            var modelDelete = await _context.Set<T>().FindAsync(Id);

            if (modelDelete != null)
            {
                _context.Remove(modelDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteLogic<T>(int Id, int? deletedStatus = 0, string? NameFieldStatus = "status") where T : class
        {
            var modelDelete = await _context.Set<T>().FindAsync(Id);

            if (modelDelete != null)
            {
                var property = typeof(T).GetProperty(NameFieldStatus!); // Busca la propiedad Status
                if (property != null && property.PropertyType == typeof(int?))
                {
                    property.SetValue(modelDelete, deletedStatus); // Asigna el estado de eliminado
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException($"La entidad no tiene una propiedad {NameFieldStatus} de tipo int.");
                }
            }
        }
        public async Task DeletePhysicalRange<T>(List<int> Ids) where T : class
        {
            foreach (int Id in Ids)
            {
                var modelDelete = await _context.Set<T>().FindAsync(Id);

                if (modelDelete != null)
                {
                    _context.Set<T>().Remove(modelDelete);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLogicRange<T>(List<int> Ids, string nameId, int? deletedStatus = 0, string? NameFieldStatus = "status") where T : class
        {
            var entities = await _context.Set<T>().Where(e => Ids.Contains((int)e.GetType().GetProperty(nameId)!.GetValue(e)!)).ToListAsync();

            if (entities.Any())
            {
                var property = typeof(T).GetProperty(NameFieldStatus!); // Busca la propiedad Status
                if (property != null && property.PropertyType == typeof(int?))
                {
                    foreach (var entity in entities)
                    {
                        property.SetValue(entity, deletedStatus); // Asigna el estado de eliminado
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException($"La entidad no tiene una propiedad {NameFieldStatus} de tipo int.");
                }
            }
        }

        public async Task TruncateTable(string Table, string? Schema = "public")
        {
            await _context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE ONLY {Schema}.{Table} RESTART IDENTITY;");
        }
    }
}
