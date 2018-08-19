namespace FishSpinDays.Services
{
    using AutoMapper;
    using FishSpinDays.Data;

    public class BaseService
    {
        public BaseService(FishSpinDaysDbContext dbContex, IMapper mapper)
        {
            this.DbContext = dbContex;
            this.Mapper = mapper;
        }

        public FishSpinDaysDbContext DbContext { get; private set; }

        public IMapper Mapper { get; private set; }
    }
}
