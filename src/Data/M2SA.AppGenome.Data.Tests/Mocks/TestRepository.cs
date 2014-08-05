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

        IList<TestEntity> LoadForPaginationWithKey(DateTime updateDate, Pagination pagination);

        IList<TestEntity> FindByList(IList<int> idList);

        void RaisError();
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

        public IList<TestEntity> LoadForPaginationWithKey(DateTime updateDate, Pagination pagination)
        {
            var sqlName = this.FormatSqlName("SelectForPaginationWithKey");
            var pValues = new Dictionary<string, object>(1);
            pValues.Add("UpdateDate", updateDate);
            var dataTable = SqlHelper.ExecutePaginationTable(sqlName, pValues, pagination);

            var list = dataTable.Convert<TestEntity>();
            return list;
        }

        public IList<TestEntity> FindByList(IList<int> idList)
        {
            var sqlName = this.FormatSqlName("SelectByList");
            var pValues = new Dictionary<string, object>(2);
            pValues.Add("UpdateDate", DateTime.Now);
            pValues.Add("IdList", idList);
            var dataSet = SqlHelper.ExecuteDataSet(sqlName, pValues);

            var list = dataSet.Tables[0].Convert<TestEntity>();
            return list;
        }

        public void RaisError()
        {
            var sqlName = this.FormatSqlName("RaisError");
            var pValues = new Dictionary<string, object>(2);
            pValues.Add("Now", DateTime.Now);
            var dataSet = SqlHelper.ExecuteDataSet(sqlName, pValues);
        }
    }
}
