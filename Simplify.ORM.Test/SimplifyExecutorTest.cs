using Dapper;
using Moq;
using Simplify.ORM.Interfaces;
using Simplify.ORM.Test.Entities;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Simplify.ORM.Test
{
    public class SimplifyExecutorTests
    {
        /*
        [Fact]
        public async Task HydrateAsync_Should_HydrateObjectMember()
        {
            // Arrange
            var mockConnection = new Mock<IDbConnection>();
            var mockQueryBuilder = new Mock<ISimplifyQueryBuilder>();

            var executor = new SimplifyExecutor(mockConnection.Object, mockQueryBuilder.Object);

            var mockEntity = new MockRelatedEntity { MockRelatedEntityId = 1, RelatedEntityId = 10 };
            Expression<Func<MockRelatedEntity, object>> fkExpression = e => e.RelatedEntityId;
            Expression<Func<MockRelatedEntity, object>> memberToHydrateExpression = e => e.RelatedEntity;
            Expression<Func<RelatedEntity, object>> newFkExpression = r => r.Id;

            var relatedEntities = new List<RelatedEntity>
            {
                new RelatedEntity { Id = 10, Name = "Related Entity 1" }
            };

            // Simulate query builder returning the appropriate query
            mockQueryBuilder
                .Setup(q => q.BuildQuery())
                .Returns("SELECT * FROM RelatedEntity WHERE Id = @Id");

            // Setup the connection to return the expected result when executing the query
            //mockConnection
            //    .Setup(c => c.QueryAsync<RelatedEntity>(It.IsAny<string>(), null, null, null, null))
            //    .ReturnsAsync(relatedEntities);

            // Act
            await executor.HydrateAsync(mockEntity, fkExpression, memberToHydrateExpression, newFkExpression);

            // Assert
            Assert.NotNull(mockEntity.RelatedEntity);
            Assert.Equal(10, mockEntity.RelatedEntity.Id);
            Assert.Equal("Related Entity 1", mockEntity.RelatedEntity.Name);
        }

        */
        
        #region GetPropertyInfo

        [Fact]
        public void GetPropertyInfo_ShouldReturnPropertyInfoForMemberExpression()
        {
            // Arrange
            Expression<Func<MockRelatedEntity, int>> expression = e => e.MockRelatedEntityId;

            // Act
            var propertyInfo = SimplifyExecutor.GetPropertyInfo(expression);

            // Assert
            Assert.NotNull(propertyInfo);
            Assert.Equal("MockRelatedEntityId", propertyInfo.Name);
            Assert.Equal(typeof(int), propertyInfo.PropertyType);
        }

        [Fact]
        public void GetPropertyInfo_ShouldReturnPropertyInfoForUnaryExpression()
        {
            // Arrange
            Expression<Func<MockRelatedEntity, int>> expression = e => ((MockRelatedEntity)e).MockRelatedEntityId;

            // Act
            var propertyInfo = SimplifyExecutor.GetPropertyInfo(expression);

            // Assert
            Assert.NotNull(propertyInfo);
            Assert.Equal("MockRelatedEntityId", propertyInfo.Name);
            Assert.Equal(typeof(int), propertyInfo.PropertyType);
        }

        [Fact]
        public void GetPropertyInfo_ShouldThrowExceptionForInvalidExpression()
        {
            // Arrange
            Expression<Func<MockRelatedEntity, bool>> expression = e => e.MockRelatedEntityId > 5;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => SimplifyExecutor.GetPropertyInfo(expression));
        }

        [Fact]
        public void GetPropertyInfo_ShouldThrowExceptionForExpressionThatIsNotAMemberExpression()
        {
            // Arrange
            Expression<Func<MockRelatedEntity, bool>> expression = e => e.RelatedEntity.Name.Length > 5;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => SimplifyExecutor.GetPropertyInfo(expression));
        }

        #endregion

        #region SetObjectMemberValue

        [Fact]
        public void SetObjectMemberValue_ShouldAddItemsToICollection()
        {
            // Arrange
            var entity = new TestEntity();
            var relatedEntities = new List<RelatedEntity> { new RelatedEntity { Id = 1, Name = "Test" } };
            var propertyInfo = typeof(TestEntity).GetProperty(nameof(TestEntity.RelatedEntities));

            // Act
            SimplifyExecutor.SetObjectMemberValue(entity, propertyInfo, relatedEntities);

            // Assert
            Assert.Single(entity.RelatedEntities);
            Assert.Equal(1, entity.RelatedEntities.First().Id);
        }

        [Fact]
        public void SetObjectMemberValue_ShouldAddItemsToList()
        {
            // Arrange
            var entity = new TestEntity();
            var relatedEntities = new List<RelatedEntity> { new RelatedEntity { Id = 1, Name = "Test" } };
            var propertyInfo = typeof(TestEntity).GetProperty(nameof(TestEntity.RelatedEntitiesList));

            // Act
            SimplifyExecutor.SetObjectMemberValue(entity, propertyInfo, relatedEntities);

            // Assert
            Assert.Single(entity.RelatedEntitiesList);
            Assert.Equal(1, entity.RelatedEntitiesList.First().Id);
        }

        [Fact]
        public void SetObjectMemberValue_ShouldSetSingleItem()
        {
            // Arrange
            var entity = new TestEntity();
            var relatedEntities = new List<RelatedEntity> { new RelatedEntity { Id = 1, Name = "Test" } };
            var propertyInfo = typeof(TestEntity).GetProperty(nameof(TestEntity.SingleRelatedEntity));

            // Act
            SimplifyExecutor.SetObjectMemberValue(entity, propertyInfo, relatedEntities);

            // Assert
            Assert.NotNull(entity.SingleRelatedEntity);
            Assert.Equal(1, entity.SingleRelatedEntity.Id);
        }

        [Fact]
        public void SetObjectMemberValue_ShouldSetNullableItem()
        {
            // Arrange
            var entity = new TestEntity();
            var relatedEntities = new List<RelatedEntity> { new RelatedEntity { Id = 1, Name = "Test" } };
            var propertyInfo = typeof(TestEntity).GetProperty(nameof(TestEntity.NullableRelatedEntity));

            // Act
            SimplifyExecutor.SetObjectMemberValue(entity, propertyInfo, relatedEntities);

            // Assert
            Assert.NotNull(entity.NullableRelatedEntity);
            Assert.Equal(1, entity.NullableRelatedEntity.Id);
        }

        [Fact]
        public void SetObjectMemberValue_ShouldThrowExceptionForUnsupportedType()
        {
            // Arrange
            var entity = new TestEntity();
            var unsupportedProperty = typeof(TestEntity).GetProperty(nameof(TestEntity.IsPersisted));
            var relatedEntities = new List<RelatedEntity> { new RelatedEntity { Id = 1, Name = "Test" } };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => SimplifyExecutor.SetObjectMemberValue(entity, unsupportedProperty, relatedEntities));
        }

        [Fact]
        public void SetObjectMemberValue_MultipleEntities_ShouldSetRelatedItemsBasedOnForeignKey()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Id = 1 },
                new TestEntity { Id = 2 }
            };

            var relatedEntities = new List<RelatedEntity>
            {
                new RelatedEntity { Id = 1, Name = "Related 1" },
                new RelatedEntity { Id = 2, Name = "Related 2" },
                new RelatedEntity { Id = 1, Name = "Related 3" } // Mesmo FK que a primeira entidade
            };

            var objectMember = typeof(TestEntity).GetProperty(nameof(TestEntity.RelatedEntities));
            var objectFK = typeof(TestEntity).GetProperty(nameof(TestEntity.Id));
            var newObjectFK = typeof(RelatedEntity).GetProperty(nameof(RelatedEntity.Id));

            // Act
            SimplifyExecutor.SetObjectMemberValue(
                entities,       
                objectMember,   
                relatedEntities,
                objectFK,       
                newObjectFK
            );

            // Assert
            Assert.Equal(2, entities[0].RelatedEntities.Count);
            Assert.Single(entities[1].RelatedEntities);
        }


        [Fact]
        public void SetObjectMemberValue_ShouldThrowExceptionForUnsupportedType_MultipleEntities()
        {
            // Arrange
            var entities = new List<TestEntity>
            {
                new TestEntity { Id = 1 }
            };

            var relatedEntities = new List<RelatedEntity>
            {
                new RelatedEntity { Id = 1, Name = "Related 1" }
            };

            var unsupportedProperty = typeof(TestEntity).GetProperty(nameof(TestEntity.IsPersisted)); // Propriedade não suportada
            var objectFK = typeof(TestEntity).GetProperty(nameof(TestEntity.Id));
            var newObjectFK = typeof(RelatedEntity).GetProperty(nameof(RelatedEntity.Id));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                SimplifyExecutor.SetObjectMemberValue(entities, unsupportedProperty, relatedEntities, objectFK, newObjectFK));
        }


        #endregion
    }
}
