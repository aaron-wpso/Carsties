using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer(IMapper mapper) : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper = mapper;

    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--> Consuming auction updated: " + context.Message.Id);

        var item = _mapper.Map<Item>(context.Message);

        var result = await DB.Update<Item>()
            .Match(a => a.ID == context.Message.Id)
            .ModifyOnly(x => new
            {
                x.Color,
                x.Make,
                x.Model,
                x.Year,
                x.Mileage
            }, item)
            .ExecuteAsync();

        if (!result.IsAcknowledged)
            throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb");

        // await DB.Update<Item>()
        //     .MatchID(context.Message.Id)
        //     .Modify(x => x.Make, context.Message.Make)
        //     .Modify(x => x.Model, context.Message.Model)
        //     .Modify(x => x.Year, context.Message.Year)
        //     .Modify(x => x.Color, context.Message.Color)
        //     .Modify(x => x.Mileage, context.Message.Mileage)
        //     .ExecuteAsync();
    }
}
