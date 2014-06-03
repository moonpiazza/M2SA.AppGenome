using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using M2SA.AppGenome.Data;

namespace M2SA.AppGenome.Data.Tests.Mocks
{
	public partial interface ITestRepository : IRepository<TestEntity,int>
	{
        IList<TestEntity> LoadForPagination(DateTime updateDate, Pagination pagination);
	}
		
    /// <summary>
    /// 
    /// </summary>
    public class TestRepository : SimpleRepositoryBase<TestEntity, int>, ITestRepository
    {
        public IList<TestEntity> LoadForPagination(DateTime updateDate, Pagination pagination)
        {
            var sqlName = this.FormatSqlName("SelectForPagination");
            var pValues = new Dictionary<string, object>(1);
            pValues.Add("UpdateDate", updateDate);

            var dataTable = SqlHelper.ExecutePaginationTable(sqlName, pValues, pagination);

            var list = dataTable.Convert<TestEntity>();
            return list;
        }
    }
}
