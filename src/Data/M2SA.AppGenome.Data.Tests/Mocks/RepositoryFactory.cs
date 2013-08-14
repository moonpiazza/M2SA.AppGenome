using System;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Data.Tests.Mocks
{
	public class RepositoryFactory : IRepositoryFactory
	{
		static RepositoryFactory()
		{
			AppInstance.RegisterTypeAlias<TestRepository>("TestRepository");
		}
		
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TRepository GetRepository<TRepository>()
        {
        	var type = typeof(TRepository).GetMapType();

            object obj = Activator.CreateInstance(type);
        	return (TRepository)obj;
        }
	}
}
