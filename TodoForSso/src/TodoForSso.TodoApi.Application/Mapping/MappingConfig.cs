using Mapster;
using TodoForSso.TodoApi.Application.Dtos;
using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Application.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TodoItem, TodoDto>();
    }
}
