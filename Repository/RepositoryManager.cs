using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _context;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;
        public ICompanyRepository Company
        {
            get
            {
                if(_companyRepository == null)
                {
                    _companyRepository = new CompanyRepository(_context);
                }
                return _companyRepository;
            }
        }
        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository == null)
                {
                    _employeeRepository = new EmployeeRepository(_context);
                }
                return _employeeRepository;
            }
        }
        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
        }
        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
