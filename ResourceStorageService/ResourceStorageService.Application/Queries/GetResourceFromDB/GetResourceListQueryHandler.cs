using MediatR;
using MongoDB.Driver;
using ResourceStorageService.Application.Models;
using ResourceStorageService.Infrastructure.Database;

namespace ResourceStorageService.Application.Queries.GetResourceFromDB
{
    public class GetResourceListQueryHandler : IRequestHandler<GetResourceListQuery, FilterResourceDto>
    {
        private readonly IMongoConnection _mongoConnectionFactory;

        public GetResourceListQueryHandler(IMongoConnection mongoConnectionFactory)
        {
            _mongoConnectionFactory = mongoConnectionFactory;
        }

        public async Task<FilterResourceDto> Handle(GetResourceListQuery request, CancellationToken cancellationToken)
        {
            var collection = _mongoConnectionFactory.GetConnection().GetCollection<ResourceDto>("ResourceStorage");
            var filter = Builders<ResourceDto>.Filter.Empty;
            List<FilterDefinition<ResourceDto>> orFilters = new();

            //orfilters for search name and search path
            //if (!string.IsNullOrEmpty(request.SearchFileName))
            //{
            //    request.SearchFileName = request.SearchFileName.ToLower();
            //    orFilters.Add(Builders<ResourceDto>.Filter.Where(x => x.Name.ToLower().Contains(request.SearchFileName)));
            //}
            //if (!string.IsNullOrEmpty(request.SearchFilePath))
            //{
            //    request.SearchFilePath = request.SearchFilePath.ToLower();
            //    orFilters.Add(Builders<ResourceDto>.Filter.Where(x => x.Path.ToLower().Contains(request.SearchFilePath)));
            //}


            //andfilters for search name and search path
            if (!string.IsNullOrEmpty(request.SearchResourceName))
            {
                request.SearchResourceName = request.SearchResourceName.ToLower();
                filter &= Builders<ResourceDto>.Filter.Where(x => x.Name.ToLower().Contains(request.SearchResourceName));
            }
            if (!string.IsNullOrEmpty(request.SearchResourcePath))
            {
                request.SearchResourcePath = request.SearchResourcePath.ToLower();
                filter &= Builders<ResourceDto>.Filter.Where(x => x.Path.ToLower().Contains(request.SearchResourcePath));
            }

            if (request.ResourceIds != null && request.ResourceIds.Any(x => x != Guid.Empty))
            {
                request.ResourceIds = request.ResourceIds.FindAll(x => x != Guid.Empty);
                orFilters.Add(Builders<ResourceDto>.Filter.Where(x => request.ResourceIds.Contains(x.Id)));
            }
            if (request.ResourcePaths != null && request.ResourcePaths.Any(x => !string.IsNullOrEmpty(x)))
            {
                request.ResourcePaths = request.ResourcePaths.FindAll(x => !string.IsNullOrEmpty(x));
                orFilters.Add(Builders<ResourceDto>.Filter.Where(x => request.ResourcePaths.Contains(x.Path)));
            }
            if (orFilters.Any())
                filter &= Builders<ResourceDto>.Filter.Or(orFilters);

            var data = await collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);

            request.FromDate = request.FromDate.Date;
            request.ToDate = request.ToDate.Date;
            if (request.FromDate != DateTime.MinValue && request.ToDate != DateTime.MinValue)
            {
                data = data.FindAll(x => x.CreatedAt.Date >= request.FromDate && x.CreatedAt.Date <= request.ToDate);
            }
            else if (request.FromDate != DateTime.MinValue)
            {
                data = data.FindAll(x => x.CreatedAt.Date >= request.FromDate);
            }
            else if (request.ToDate != DateTime.MinValue)
            {
                data = data.FindAll(x => x.CreatedAt.Date <= request.ToDate);
            }

            var total = data.Count;
            if (request.Limit > total || request.Limit < 1)
            {
                request.Offset = 1;
                request.Limit = total;
                if (request.Limit == 0)
                {
                    request.Limit = 1;
                }
            }

            if (total > 0)
            {
                data = data.OrderByDescending(x => x.CreatedAt).Skip((request.Offset - 1) * request.Limit).Take(request.Limit).ToList();
            }
            return new() { Offset = request.Offset, Limit = request.Limit, TotalPages = (int)Math.Ceiling((double)total/request.Limit), TotalCount = total, Resources = data};
        }
    }
}
