using DeliveryHouse.Common.Responses;
using System.Threading.Tasks;

namespace DeliveryHouse.Common.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(string urlBase, string servicePrefix, string controller);
    }
}
