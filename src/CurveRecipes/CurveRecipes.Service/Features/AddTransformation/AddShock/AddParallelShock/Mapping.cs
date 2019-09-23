using AutoMapper;
using CurveRecipes.Domain;

namespace CurveRecipes.Service.Features.AddTransformation.AddShock.AddParallelShock
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Command, ParallelShock>().ConstructUsing(x => new ParallelShock(x.ShockTarget, new Shift(x.Shift)));
        }
    }
}
