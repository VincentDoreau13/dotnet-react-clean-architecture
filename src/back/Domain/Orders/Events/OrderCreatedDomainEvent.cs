using MediatR;

namespace ShopApi.Domain.Orders.Events;

public record OrderCreatedDomainEvent(Order Order) : INotification;
