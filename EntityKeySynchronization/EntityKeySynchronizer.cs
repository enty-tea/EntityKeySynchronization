using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityKeySynchronization
{
    /// <summary>
    /// Synchronizes an entity with its key value.
    /// </summary>
    /// <typeparam name="TEntity">the type of the entity</typeparam>
    /// <typeparam name="TKey">the type of the entity's key</typeparam>
    public class EntityKeySynchronizer<TEntity, TKey> : EntityKeySynchronizerBase<TEntity, TKey>
        where TEntity : class
    {
        private readonly Func<TEntity, TKey> entityKeyExtractor;
        private readonly IEqualityComparer<TKey> keyEqualityComparer;

        /// <summary>
        /// Constructs a synchronizer with the specified key extractor.
        /// </summary>
        /// <param name="entityKeyExtractor">extracts the key value from an entity</param>
        public EntityKeySynchronizer(Func<TEntity, TKey> entityKeyExtractor)
            : this(entityKeyExtractor, keyEqualityComparer: null) { }

        /// <summary>
        /// Constructs a synchronizer with the specified key extractor and comparer.
        /// </summary>
        /// <param name="entityKeyExtractor">extracts the key value from an entity</param>
        /// <param name="keyEqualityComparer">compares two keys for equality</param>
        public EntityKeySynchronizer(Func<TEntity, TKey> entityKeyExtractor, IEqualityComparer<TKey> keyEqualityComparer)
        {
            if (entityKeyExtractor == null) throw new ArgumentNullException("entityKeyExtractor");
            this.entityKeyExtractor = entityKeyExtractor;
            this.keyEqualityComparer = keyEqualityComparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// Extracts the key from the specified entity.
        /// </summary>
        /// <param name="entity">the entity from which to extract the key</param>
        /// <returns>the key for the entity</returns>
        /// <exception cref="ArgumentNullException">if the entity is null</exception>
        protected override TKey GetKeyForEntity(TEntity entity)
        {
            return entityKeyExtractor(entity);
        }

        /// <summary>
        /// Determines whether the specified keys are equal.
        /// </summary>
        /// <param name="a">the first key to compare</param>
        /// <param name="b">the second key to compare</param>
        /// <returns>true if the specified keys are equal</returns>
        protected override bool KeyEquals(TKey a, TKey b)
        {
            return keyEqualityComparer.Equals(a, b);
        }
    }

    /// <summary>
    /// The base class for synchronizing an entity with its key value.
    /// </summary>
    /// <typeparam name="TEntity">the type of the entity</typeparam>
    /// <typeparam name="TKey">the type of the entity's key</typeparam>
    [Serializable]
    public abstract class EntityKeySynchronizerBase<TEntity, TKey>
        where TEntity : class
    {
        /// <summary>
        /// The entity that owns the key value, or null if the entity is not loaded or known.
        /// </summary>
        public TEntity Entity
        {
            get { return entity; }
            set
            {
                if (!object.ReferenceEquals(entity, value))
                {
                    var oldEntity = entity;
                    entity = value;
                    if (entity != null)
                    {
                        key = default(TKey);
                    }
                    else if (oldEntity != null)
                    {
                        key = GetKeyForEntity(oldEntity);
                    }
                }
            }
        }
        private TEntity entity;

        /// <summary>
        /// The key value of the entity.
        /// </summary>
        public TKey Key
        {
            get
            {
                return entity != null ? GetKeyForEntity(entity) : key;
            }
            set
            {
                if (entity != null && !KeyEquals(GetKeyForEntity(entity), value))
                {
                    entity = null;
                }
                key = value;   
            }
        }
        private TKey key;

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified entity.
        /// </summary>
        /// <param name="entity">the entity from which to extract the key</param>
        /// <returns>the key for the entity</returns>
        /// <exception cref="ArgumentNullException">if the entity is null</exception>
        protected abstract TKey GetKeyForEntity(TEntity entity);

        /// <summary>
        /// Determines whether the specified keys are equal.
        /// </summary>
        /// <param name="a">the first key to compare</param>
        /// <param name="b">the second key to compare</param>
        /// <returns>true if the specified keys are equal</returns>
        protected virtual bool KeyEquals(TKey a, TKey b)
        {
            return object.Equals(a, b);
        }
    }
}
