using System;

namespace M2SA.AppGenome.Data.Tests.Mocks
{
	/// <summary>
	/// Description of TestEntity.
	/// </summary>
	public class TestEntity : IEntity<int>
	{
	    [NonSerializedProperty]
        public int Id 
        {
            get { return this.TestId; }
            set { this.TestId = value; }
        }

        public int TestId { get; set; }

        public PersistentState PersistentState { get; set; }
		
		public string Name { get; set; }
		
		public DateTime UpdateDate { get; set; }
		
		public TestEntity()
		{
			this.UpdateDate = DateTime.Now;
		}
	}
}
