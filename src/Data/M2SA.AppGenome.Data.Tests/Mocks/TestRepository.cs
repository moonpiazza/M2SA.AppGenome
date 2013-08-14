using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using M2SA.AppGenome.Data;

namespace M2SA.AppGenome.Data.Tests.Mocks
{
	public partial interface ITestRepository : IRepository<TestEntity,int>
	{
	}
		
    /// <summary>
    /// 
    /// </summary>
    public class TestRepository : SimpleRepositoryBase<TestEntity, int>, ITestRepository
    {
		
    }
}
