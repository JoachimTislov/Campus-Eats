using Microsoft.EntityFrameworkCore;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

[Owned]
public class Address
{
    public Address() { }

    public Address(string addressLine, string city, string postalCode)
    {
        AddressLine = addressLine;
        City = city;
        PostalCode = postalCode;
    }

    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}
