using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Trevental.Common.Model.Location;
using Trevental.DAL.Repository;

namespace Trevental.DAL.Location.Service.Query
{
    public class SearchLocationByIdListQuery:IRequest<IEnumerable<LocationModel>>
    {
        /// <param name="query">1,2,3,4,5,</param>
        public string query { get; set; }

        public class SearchLocationByIdListQueryHandler : IRequestHandler<SearchLocationByIdListQuery, IEnumerable<LocationModel>>
        {
            private readonly ILocationRepository _locationRepository;
            public SearchLocationByIdListQueryHandler(ILocationRepository _Repository)
            {
                _locationRepository = _Repository;
            }            
            public Task<IEnumerable<LocationModel>> Handle(SearchLocationByIdListQuery request, CancellationToken cancellationToken)
            {
                return _locationRepository.SearchInIdList(request.query,cancellationToken);

                //throw new System.NotImplementedException();
            }
        }
    }
}