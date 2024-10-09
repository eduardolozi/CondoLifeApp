using Domain.Exceptions;
using Domain.Models;
using Domain.Utils;
using Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services {
    public class CondominiumService
    {
        private readonly CondoLifeContext _dbContext;
        public CondominiumService(CondoLifeContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Condominium? GetById(int id) {
            return _dbContext.Condominium.Include(x => x.Address).FirstOrDefault(x => x.Id == id);
        }
        
        public List<Condominium>? GetAll(CondominiumFilter filter) {
            var query = _dbContext.Condominium.Include(x => x.Address).AsQueryable();
            var address = filter.Address;

            if(address.HasValue()) {
                if (address!.Country.HasValue())
                    query = query.Where(x => EF.Functions.Like(x.Address.Country, $"%{address.Country}%"));

                if (address!.State.HasValue())
                    query = query.Where(x => EF.Functions.Like(x.Address.State, $"%{address.State}%"));

                if (address!.City.HasValue())
                    query = query.Where(x => EF.Functions.Like(x.Address.City, $"%{address.City}%"));

                if (address!.PostalCode.HasValue())
                    query = query.Where(x => EF.Functions.Like(x.Address.PostalCode, $"%{address.PostalCode}%"));
            }

            if (filter.Name.HasValue()) 
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{filter.Name}%"));

            var condos = query.ToList();
            return condos.Count > 0 ? condos : null;
        }

        public void Insert(Condominium condominium) {
            _dbContext.Condominium.Add(condominium);
            _dbContext.SaveChanges();
        }
        
        public void Update(int id, Condominium condominium) {
            condominium.Id = id;
            _dbContext.Condominium.Update(condominium);
            _dbContext.SaveChanges();
        }
        
        public void Delete(int id) {
            var condo = GetById(id)
                ?? throw new ResourceNotFoundException("Condomínio não encontrado");
            _dbContext.Condominium.Remove(condo);
            _dbContext.SaveChanges();
        }
    }
}
