namespace FishSpinDays.Services
{
    using AutoMapper;
    using FishSpinDays.Data;

    public abstract class BaseService
    {
        protected BaseService(FishSpinDaysDbContext dbContex, IMapper mapper)
        {
            this.DbContext = dbContex;
            this.Mapper = mapper;
        }

        public FishSpinDaysDbContext DbContext { get; private set; }

        public IMapper Mapper { get; private set; }
    }
}
