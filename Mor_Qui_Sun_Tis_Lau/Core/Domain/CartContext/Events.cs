using System.Security.Claims;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;

public record CartCheckedOut(List<CartItem> CartItems, ClaimsPrincipal UserClaimsPrincipal, Action<Guid> OnOrderCreated) : BaseDomainEvent;

