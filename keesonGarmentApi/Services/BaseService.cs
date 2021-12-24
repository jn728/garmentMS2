using AutoMapper;
using keesonGarmentApi.Common;
using keesonGarmentApi.Entities;
using keesonGarmentApi.Services.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace keesonGarmentApi.Services
{
    public class BaseService
    {
        protected readonly IMemoryCache Cache;
        protected readonly GarmentContext GarmentContext;
        protected readonly IMapper Mapper;

        private const string Connection = "Server=192.168.1.236;Database=AuthPlatform;User Id=sa;Password=Wms123456";
        private static readonly IFreeSql FreeSql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(global::FreeSql.DataType.SqlServer, Connection)
            .UseGenerateCommandParameterWithLambda(true).UseNoneCommandParameter(false)
            .UseLazyLoading(true).Build();

        protected BaseService(GarmentContext garmentContext, IMemoryCache cache, IMapper mapper)
        {
            Cache = cache;
            Mapper = mapper;
            GarmentContext = garmentContext;
            var user = HttpContextHelp.Current?.User;
            if (user != null)
            {
                //UserId = user.GetUserId();
                UserId = "admin";
            }
        }

        protected string UserId { get; }

        protected List<string> GetRoleNames()
        {
            var list = new List<string>
            {
                "IT运营部",
                "人事文员"
            };
            return list;
        }
            
        protected string CoventPosition(string pos)
        {
            var managements = new List<string>()
            {
                "组长","主管"
            };
            
            if (managements.Contains(pos))
            {
                return "管理";
            }
            else
            {
                return "员工";
            }
        }
        
        //protected List<string> GetRoleNames() =>
        //    FreeSql.Ado.Query<string>(@"Select Postion from AspNetRoles
        //                             where Id in ( select RoleId from AspNetUserRoles where UserId = @uid )", new { uid = UserId });
    }
}
