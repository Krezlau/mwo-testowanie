using AutoMapper;
using mwo_testowanie.Models;
using mwo_testowanie.Models.DTOs;
using mwo_testowanie.Repository;

namespace mwo_testowanie.Services;

public class ClientService : IClientService
{
    private readonly IRepository<Client> _clientRepo;
    private readonly IMapper _mapper;

    public ClientService(IRepository<Client> clientRepo, IMapper mapper)
    {
        _clientRepo = clientRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClientDTO>> GetAllClientsAsync()
    {
        var clients = await _clientRepo.GetAllAsync();
        return _mapper.Map<IEnumerable<ClientDTO>>(clients);
    }

    public async Task<ClientDTO> GetClientAsync(Guid id)
    {
        return _mapper.Map<ClientDTO>(await _clientRepo.GetAsync(c => c.Id == id));
    }

    public async Task<Guid> CreateClientAsync(ClientCreateDTO client)
    {
        var clientEntity = _mapper.Map<Client>(client);
        await _clientRepo.CreateAsync(clientEntity);
        return clientEntity.Id;
    }

    public async Task UpdateClientAsync(Guid id, ClientCreateDTO client)
    {
        var clientInDb = await _clientRepo.GetAsync(c => c.Id == id);
        if (clientInDb is null) throw new Exception("Client not found");
        _mapper.Map(client, clientInDb);
        await _clientRepo.UpdateAsync(clientInDb);
    }

    public async Task DeleteClientAsync(Guid id)
    {
        var clientInDb = await _clientRepo.GetAsync(c => c.Id == id);
        if (clientInDb is null) throw new Exception("Client not found");
        await _clientRepo.DeleteAsync(clientInDb);
    }
}