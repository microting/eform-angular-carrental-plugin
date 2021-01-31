using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using eFormCore;
using Microsoft.EntityFrameworkCore;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormRentableItemBase.Infrastructure.Data;
using Rebus.Bus;
using RentableItems.Pn.Abstractions;
using RentableItems.Pn.Installers;

namespace RentableItems.Pn.Services
{
    public class RebusService : IRebusService
    {
        private IBus _bus;
        private IWindsorContainer _container;
        private string _connectionString;
        private readonly IEFormCoreService _coreHelper;

        public RebusService(IEFormCoreService coreHelper)
        {            
            //_dbContext = dbContext;
            _coreHelper = coreHelper;
        }

        public async Task Start(string connectionString)
        {
            _connectionString = connectionString;   
            _container = new WindsorContainer();
            _container.Install(
                new RebusHandlerInstaller()
                , new RebusInstaller(connectionString, 1, 1)
            );
            
            Core _core = await _coreHelper.GetCore();
            _container.Register(Component.For<Core>().Instance(_core));
            _container.Register(Component.For<eFormRentableItemPnDbContext>().Instance(GetContext()));
            _bus = _container.Resolve<IBus>();
        }

        public IBus GetBus()
        {
            return _bus;
        }
        
        private eFormRentableItemPnDbContext GetContext()
        {

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<eFormRentableItemPnDbContext>();

            if (_connectionString.ToLower().Contains("convert zero datetime"))
            {
                dbContextOptionsBuilder.UseMySql(_connectionString);
            }
            else
            {
                dbContextOptionsBuilder.UseSqlServer(_connectionString);
            }
            dbContextOptionsBuilder.UseLazyLoadingProxies();
            return new eFormRentableItemPnDbContext(dbContextOptionsBuilder.Options);

        }        
    }
}