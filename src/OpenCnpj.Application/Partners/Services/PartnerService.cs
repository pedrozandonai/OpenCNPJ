using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OpenCnpj.Application.Companies.Repositories;
using OpenCnpj.Application.Phones.Domain;
using OpenCnpj.Application.RawRecords;
using OpenCnpj.Application.Simples.Services;
using OpenCnpj.Core.Configurations;
using OpenCnpj.Core.Database.Factory.Interfaces;
using OpenCnpj.Core.Helpers;
using Serilog;

namespace OpenCnpj.Application.Partners.Services;
public class PartnerService(IMongoDatabaseFactory mongoDatabaseFactory, IServiceProvider serviceProvider, TweakSettings tweakSettings, ILogger logger)
{
    private readonly IdGenerator _phoneIdGen = new();
    private readonly IdGenerator _contactIdGen = new();
    private readonly ILogger _logger = logger.ForContext<SimpleService>();
    public async Task<Result> CreateSimples(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        try
        {
            var partnersCollection = mongoDatabaseFactory
                .Database
                .GetCollection<PartnerRawRecord>("PartnersRaw");

            int pageSize = tweakSettings.FormatRawDataSettings.RecordsBatchAmount;
            var page = 0;

            while (true)
            {
                _logger.Information("Current page: {0}", page);

                var data = await partnersCollection
                    .Find(Builders<PartnerRawRecord>.Filter.Empty)
                    .Skip(page * pageSize)
                    .Limit(pageSize)
                    .ToListAsync(cancellationToken);

                if (data.Count == 0)
                    break;

                List<Task> tasks = [];
                foreach (var simpleDataRawRecord in data)
                {
                    //tasks.Add(ProcessSimples(simpleDataRawRecord, cancellationToken));
                }

                await Task.WhenAll(tasks);

                var dbFactory = scope.ServiceProvider.GetRequiredService<IDatabaseFactory>();

                await dbFactory.BeginAsync();

                //if (!_meisToInsert.IsEmpty)
                //{
                //    await meiRepository.CopyToTable(_meisToInsert, cancellationToken);

                //    _meisToInsert.Clear();
                //}

                //if (!_simplesToInsert.IsEmpty)
                //{
                //    await simpleRepository.CopyToTable(_simplesToInsert, cancellationToken);

                //    _simplesToInsert.Clear();
                //}

                await dbFactory.CommitAsync();

                page++;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An exception occurred while trying to create the SIMPLES data.");
            return Result.Failure("An exception occurred while trying to create the SIMPLES data.");
        }

        return Result.Success();
    }

    private async Task ProcesssPartners(PartnerRawRecord partnerRawRecord, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var companyRepository = serviceProvider.GetRequiredService<ICompanyRepository>();

        var company = await companyRepository.GetByBasicCnpj(partnerRawRecord.BasicCnpj, cancellationToken);
        if (company == null)
        {
            _logger.Warning("Unable to retreive company by basic cnpj.");
            return;
        }

        //var phoneOne = TryToCreatePhone();

        //Phone? phoneTwo = null;
        //if (int.TryParse(establishmentRawRecord.Contact.PhoneAreaCode2, out int phoneAreaCode2) &&
        //    int.TryParse(establishmentRawRecord.Contact.PhoneNumber2, out int phoneNumber2))
        //    phoneTwo = Phone.Create(_phoneIdGen.NextId(), phoneAreaCode2, phoneNumber2);

        //int? faxAreaCode = null;
        //if (int.TryParse(establishmentRawRecord.Contact.FaxAreaCode, out var parsedFaxAreaCode))
        //    faxAreaCode = parsedFaxAreaCode;

        //int? faxNumber = null;
        //if (int.TryParse(establishmentRawRecord.Contact.FaxNumber, out int parsedFaxNumber))
        //    faxNumber = parsedFaxNumber;

        //List<Phone> phones = [];
        //List<Contact> contacts = [];
        //if (phoneOne != null)
        //{
        //    phones.Add(phoneOne);
        //    contacts.Add(Contact.Create(_contactIdGen.NextId(), phoneOne.ID, faxAreaCode, faxNumber, establishmentRawRecord.Contact.Email));
        //}
        //if (phoneTwo != null)
        //{
        //    phones.Add(phoneTwo);
        //    contacts.Add(Contact.Create(_contactIdGen.NextId(), phoneTwo.ID, faxAreaCode, faxNumber, establishmentRawRecord.Contact.Email));
        //}

    }

    //private IEnumerable<Phone> CreatePhonesByContact()
    //{

    //}

    private Phone? TryToCreatePhone(string phoneAreaCode, string phoneNumber)
    {
        if (int.TryParse(phoneAreaCode, out int parsedPhoneAreaCode) &&
            int.TryParse(phoneNumber, out int parsedPhoneNumber))
            return Phone.Create(_phoneIdGen.NextId(), parsedPhoneAreaCode, parsedPhoneNumber);

        return null;
    }
} 
