using System;

namespace M2SA.AppGenome.Data
{
	/// <summary>
	/// Description of IEntity.
	/// </summary>
	public interface IEntity<TId>
	{
        /// <summary>
        /// 
        /// </summary>
		TId Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        PersistentState PersistentState { get; set; }
	}
}
