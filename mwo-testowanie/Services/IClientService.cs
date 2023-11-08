using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;

namespace mwo_testowanie.Services;

public interface IClientService
{
    Task<IEnumerable<ClientDTO>> GetAllClientsAsync();
    Task<ClientDTO> GetClientAsync(Guid id);
    Task<Guid> CreateClientAsync(ClientCreateDTO client);
    Task UpdateClientAsync(Guid id, ClientCreateDTO client);
    Task DeleteClientAsync(Guid id);
}