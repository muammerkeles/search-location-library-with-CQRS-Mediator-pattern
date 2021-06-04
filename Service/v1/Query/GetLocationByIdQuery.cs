using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Trevental.Common.Model.Location;
using Trevental.DAL.Repository;

namespace Trevental.DAL.Location.Service.Query
{
    public class GetLocationByIdQuery : IRequest<LocationModel>
    {
        public int Id { get; set; }
        
        public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, LocationModel>
        {

            private readonly ILocationRepository _locationRepository;

            public GetLocationByIdQueryHandler(ILocationRepository _Repository)
            {
                _locationRepository = _Repository;
            }

            public Task<LocationModel> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

    }

    
}