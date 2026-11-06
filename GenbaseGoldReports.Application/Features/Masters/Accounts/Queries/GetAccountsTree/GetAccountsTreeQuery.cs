using GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Queries.GetAccountsTree
{
    public record GetAccountsTreeQuery(string? Search = null) : IRequest<List<AccountTreeItemDto>>;
}
