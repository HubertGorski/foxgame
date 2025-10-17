namespace FoxTales.Infrastructure.Data.Seeders;

public interface IClearableSeeder : ISeeder
{
    Task ClearAsync();
}
