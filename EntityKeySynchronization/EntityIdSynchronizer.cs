using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityKeySynchronization
{
    /// <summary>
    /// Synchronizes an entity with its id.
    /// </summary>
    /// <typeparam name="TEntity">the type of the entity</typeparam>
    /// <typeparam name="TId">the type of the entity's id</typeparam>
    public class EntityIdSynchronizer<TEntity, TId> : EntityIdSynchronizerBase<TEntity, TId>
        where TEntity : class
        where TId : struct
    {
        private readonly Func<TEntity, TId> entityIdExtractor;
        private readonly IEqualityComparer<TId> idEqualityComparer;

        /// <summary>
        /// Constructs a synchronizer with the specified id extractor.
        /// </summary>
        /// <param name="entityIdExtractor">extracts the id value from an entity</param>
        public EntityIdSynchronizer(Func<TEntity, TId> entityIdExtractor)
        {
            if (entityIdExtractor == null) throw new ArgumentNullException("entityIdExtractor");
            this.entityIdExtractor = entityIdExtractor;
        }

        /// <summary>
        /// Extracts the id from the specified entity.
        /// </summary>
        /// <param name="entity">the entity from which to extract the id</param>
        /// <returns>the id for the entity</returns>
        /// <exception cref="ArgumentNullException">if the entity is null</exception>
        protected override TId GetKeyForEntity(TEntity entity)
        {
            return entityIdExtractor(entity);
        }

        /// <summary>
        /// Determines whether the specified ids are equal.
        /// </summary>
        /// <param name="a">the first id to compare</param>
        /// <param name="b">the second id to compare</param>
        /// <returns>true if the specified ids are equal</returns>
        protected override bool KeyEquals(TId a, TId b)
        {
            return object.Equals(a, b);
        }
    }

    /// <summary>
    /// The base class for synchronizing an entity with its id.
    /// </summary>
    /// <typeparam name="TEntity">the type of the entity</typeparam>
    /// <typeparam name="TId">the type of the entity's id</typeparam>
    [Serializable]
    public abstract class EntityIdSynchronizerBase<TEntity, TId> : EntityKeySynchronizerBase<TEntity, TId>
        where TEntity : class
        where TId : struct
    {
        /// <summary>
        /// The id of the entity, or the id sentinel value if the id is not known.
        /// </summary>
        public TId Id
        {
            get { return Key; }
            set { Key = value; }
        }

        /// <summary>
        /// The id of the entity, or null if the id is not known.
        /// </summary>
        public TId? IdOrNull
        {
            get
            {
                var key = Key;
                return !KeyEquals(key, IdSentinel) ? key : default(TId?);
            }
            set { Key = value ?? IdSentinel; }
        }

        /// <summary>
        /// An invalid ID value that is used to indicate that an entity has not id value.  
        /// Defaults to default(TId).
        /// </summary>
        protected virtual TId IdSentinel
        {
            get { return default(TId); }
        }
    }
}
