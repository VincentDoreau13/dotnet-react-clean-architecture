using MediatR;

namespace ShopApi.Domain.Orders.Events;

public record OrderDeletedDomainEvent(Order Order) : INotification;
