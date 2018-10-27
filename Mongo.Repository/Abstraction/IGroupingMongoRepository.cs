using Common.Mongo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Mongo.Respository.Abstraction
{
    public interface IGroupingMongoRepository
    {
        #region Grouping

        /// <summary>
        /// Groups a collection of documents given a grouping criteria, 
        /// and returns a list of projected documents.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TGroupKey">The type of the grouping criteria.</typeparam>
        /// <typeparam name="TProjection">The type of the projected group.</typeparam>
        /// <param name="groupingCriteria">The grouping criteria.</param>
        /// <param name="groupProjection">The projected group result.</param>
        /// <param name="partitionKey">The partition key of your document, if any.</param>
        List<TProjection> GroupBy<TDocument, TGroupKey, TProjection>(
            Expression<Func<TDocument, TGroupKey>> groupingCriteria,
            Expression<Func<IGrouping<TGroupKey, TDocument>, TProjection>> groupProjection,
            string partitionKey = null)
            where TDocument : IBaseModel
            where TProjection : class, new();

        /// <summary>
        /// Groups filtered a collection of documents given a grouping criteria, 
        /// and returns a dictionary of listed document groups with keys having the different values of the grouping criteria.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TGroupKey">The type of the grouping criteria.</typeparam>
        /// <typeparam name="TProjection">The type of the projected group.</typeparam>
        /// <param name="filter"></param>
        /// <param name="selector">The grouping criteria.</param>
        /// <param name="projection">The projected group result.</param>
        /// <param name="partitionKey">The partition key of your document, if any.</param>
        List<TProjection> GroupBy<TDocument, TGroupKey, TProjection>(Expression<Func<TDocument, bool>> filter,
                                                       Expression<Func<TDocument, TGroupKey>> selector,
                                                       Expression<Func<IGrouping<TGroupKey, TDocument>, TProjection>> projection,
                                                       string partitionKey = null)
                                                       where TDocument : IBaseModel
                                                       where TProjection : class, new();

        #endregion
    }
}
