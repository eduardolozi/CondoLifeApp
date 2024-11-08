using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Filters;
using Domain.Utils;
using Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services {
    public class CondominiumService(CondoLifeContext dbContext)
    {
        public Condominium? GetById(int id) {
            return dbContext.Condominium.Include(x => x.Address).FirstOrDefault(x => x.Id == id);
        }
        
        public List<Condominium>? GetAll(CondominiumFilter? filter) {
            var query = dbContext.Condominium.Include(x => x.Address).AsNoTracking().AsQueryable();

            Address? address = null;
            if(filter.HasValue()) 
                address = filter!.Address;

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
            return condos.IsEmpty() ? null : condos;
        }

        public void Insert(Condominium condominium) {
            dbContext.Condominium.Add(condominium);
            dbContext.SaveChanges();
        }
        
        public void Update(int id, Condominium condominium) {
            condominium.Id = id;
            var condo = GetById(id);
            
            if(condo.HasValue()) {
                condo!.Name = condominium.Name;

                if(condo.Address.HasValue() && condominium.Address.HasValue()) {
                    condo.Address.Country = condominium.Address.Country;
                    condo.Address.State = condominium.Address.State;
                    condo.Address.City = condominium.Address.City;
                    condo.Address.PostalCode = condominium.Address.PostalCode;
                }

                dbContext.SaveChanges();
                return;
            }
            throw new ResourceNotFoundException("Condomínio não encontrado");
        }
        
        public void Delete(int id) {
            var condo = GetById(id)
                ?? throw new ResourceNotFoundException("Condomínio não encontrado");
            dbContext.Condominium.Remove(condo);
            dbContext.SaveChanges();
        }
    }
}
