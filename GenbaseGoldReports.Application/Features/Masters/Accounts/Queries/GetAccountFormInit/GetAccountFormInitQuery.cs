using GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Queries.GetAccountFormInit
{
    public record GetAccountFormInitQuery(string? ParentCode, bool IsGroup) : IRequest<AccountFormInitDto>;
}
