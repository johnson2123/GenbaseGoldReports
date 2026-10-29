using Dapper;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Models;
using MediatR;
using System.Text;

namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySalesLedger
{
    public class GetJewellerySalesLedgerQueryHandler : IRequestHandler<GetJewellerySalesLedgerQuery, JewellerySalesLedgerResponse>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetJewellerySalesLedgerQueryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<JewellerySalesLedgerResponse> Handle(GetJewellerySalesLedgerQuery request, CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var whereClause = new StringBuilder("WHERE doctype IN ('MSJ', 'MSL')");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(request.Brncode))
            {
                whereClause.Append(" AND brncode = @Brncode");
                parameters.Add("Brncode", request.Brncode);
            }

            if (!string.IsNullOrWhiteSpace(request.Divcode))
            {
                whereClause.Append(" AND divcode = @Divcode");
                parameters.Add("Divcode", request.Divcode);
            }

            if (request.FromDate.HasValue)
            {
                whereClause.Append(" AND docdate >= @FromDate");
                parameters.Add("FromDate", request.FromDate.Value.Date);
            }

            if (request.ToDate.HasValue)
            {
                whereClause.Append(" AND docdate <= @ToDate");
                parameters.Add("ToDate", request.ToDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            if (!string.IsNullOrWhiteSpace(request.FixUnfix) && request.FixUnfix.ToUpper() != "ALL")
            {
                whereClause.Append(" AND fixunfix = @FixUnfix");
                parameters.Add("FixUnfix", request.FixUnfix.ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                whereClause.Append(" AND (partyname LIKE @Search OR partycode LIKE @Search OR refdocno LIKE @Search OR CAST(DOCNO AS VARCHAR) LIKE @Search)");
                parameters.Add("Search", $"%{request.SearchTerm}%");
            }

            parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
            parameters.Add("PageSize", request.PageSize);

            var sql = $@"
            -- 1. KPI Summary Aggregations
            SELECT 
                COUNT(1) AS TotalInvoicesCount,
                ISNULL(SUM(totpuqty), 0.0) AS TotalDispatchedPureWeight,
                ISNULL(SUM(totmkgamt), 0.0) AS TotalMakingReceivable,
                ISNULL(SUM(CASE WHEN fixunfix = 'UNFIX' THEN totpuqty ELSE 0.0 END), 0.0) AS TotalUnfixedGoldExposure
            FROM msal {whereClause};

            -- 2. Total Count for Pagination
            SELECT COUNT(1) FROM msal {whereClause};

            -- 3. Paged Records
            SELECT 
                brncode AS Brncode,
                divcode AS Divcode,
                doctype AS Doctype,
                DOCNO AS Docno,
                docdate AS Docdate,
                partycode AS Partycode,
                partyname AS Partyname,
                fixunfix AS Fixunfix,
                totgrqty AS Totgrqty,
                totpuqty AS Totpuqty,
                totmkgamt AS Totmkgamt,
                tottaxamt AS Tottaxamt,
                partyamount AS Partyamount,
                curr AS Curr,
                refdocno AS Refdocno
            FROM msal
            {whereClause}
            ORDER BY docdate DESC, DOCNO DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var kpiSummary = await multi.ReadFirstOrDefaultAsync<JewellerySalesSummaryKpiDto>()
                ?? new JewellerySalesSummaryKpiDto(0, 0, 0, 0);

            var totalCount = await multi.ReadFirstAsync<int>();

            var items = (await multi.ReadAsync<JewellerySaleLedgerItemDto>()).AsList();

            var pagedResult = new PagedResult<JewellerySaleLedgerItemDto>(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize
            );

            return new JewellerySalesLedgerResponse(pagedResult, kpiSummary);
        }
    }
}
