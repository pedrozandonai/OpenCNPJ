namespace OpenCnpj.Application.Contacts.Models.Dtos;
public record ContactDto(IEnumerable<PhoneDto> Phones, string FaxAreaCode, string FaxNumber, string Email);
