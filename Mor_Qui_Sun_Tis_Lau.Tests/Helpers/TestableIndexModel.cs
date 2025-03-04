using MediatR;
using Microsoft.Extensions.Logging;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Pages.Home;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;


namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

public class TestableIndexModel(IMediator mediator, IUserService userService, IUserRepository userRepository, ILogger<IndexModel> logger, IAdminService adminService, IProductRepository productRepository) : IndexModel(mediator, userService, userRepository, logger, adminService, productRepository)
{
    public override void ClearModelStateAndValidateViewModel(object ViewModel) { }
}
