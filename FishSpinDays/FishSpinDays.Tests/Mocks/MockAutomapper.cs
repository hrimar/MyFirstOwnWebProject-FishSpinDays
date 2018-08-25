using AutoMapper;
using FishSpinDays.Web.Mapping;

namespace FishSpinDays.Tests.Mocks
{
    public static class MockAutomapper
    {
        static MockAutomapper() 
        {
            Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());
        }

        public static IMapper GetMapper()
        {
            return Mapper.Instance;
        }
    }
}
