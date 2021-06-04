using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Trevental.Common.Model.Location;
using Trevental.DAL.Repository;

namespace Trevental.DAL.Location.Service.Query
{
    public class SearchLocationByQuery:IRequest<IEnumerable<LocationModel>>
    {
        public string query { get; set; }

        public class SearchLocationByQueryHandler : IRequestHandler<SearchLocationByQuery, IEnumerable<LocationModel>>
        {
            private readonly ILocationRepository _locationRepository;
            public SearchLocationByQueryHandler(ILocationRepository _Repository)
            {
                _locationRepository = _Repository;
            }            
            public Task<IEnumerable<LocationModel>> Handle(SearchLocationByQuery request, CancellationToken cancellationToken)
            {
                return _locationRepository.Search(request.query,cancellationToken);

                //throw new System.NotImplementedException();
            }
        }
    }
}