using Simplify.ORM.Builders;
using Simplify.ORM.Test.Entities;
using Xunit;

namespace Simplify.ORM.Test.Builders
{
    public class SimplifyResponseBuilderTest
    {
        [Fact]
        public void ShouldBuildResponse_WithSelectedFields()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity
            {
                MockRelatedEntityId = 1,
                RelatedEntityId = 2,
                RelatedEntity = new RelatedEntity
                {
                    Id = 10,
                    Name = "Test Entity"
                }
            };

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>();

            // Act
            var response = builder
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Build(mockEntity);

            // Assert
            Assert.Equal(1, response.MockRelatedEntityId);
            Assert.Equal(2, response.RelatedEntityId);
        }

        [Fact]
        public void ShouldNotInclude_NavigationPropertyInResponse()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity
            {
                MockRelatedEntityId = 1,
                RelatedEntityId = 2,
                RelatedEntity = new RelatedEntity
                {
                    Id = 10,
                    Name = "Test Entity"
                }
            };

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>();

            // Act
            var response = builder
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Build(mockEntity);

            // Assert
            Assert.Equal(1, response.MockRelatedEntityId);
            Assert.Equal(2, response.RelatedEntityId);

            // Check if the RelatedEntity is not present
            var responseDict = (IDictionary<string, object>)response;
            Assert.False(responseDict.ContainsKey("RelatedEntity"));
        }

        [Fact]
        public void ShouldHandleEmptyEntity()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity();

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>();

            // Act
            var response = builder
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Build(mockEntity);

            // Assert
            Assert.Equal(0, response.MockRelatedEntityId); // Default value for int
            Assert.Equal(0, response.RelatedEntityId); // Default value for int
        }

        [Fact]
        public void ShouldBuildResponse_WithSelectedFieldsAndList()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity
            {
                MockRelatedEntityId = 1,
                RelatedEntityId = 2,
                RelatedEntity = new RelatedEntity
                {
                    Id = 10,
                    Name = "Related Entity"
                },
                RelatedEntities = new List<RelatedEntity>
                {
                    new RelatedEntity { Id = 20, Name = "Entity 1" },
                    new RelatedEntity { Id = 30, Name = "Entity 2" }
                }
            };

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>()
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Entity(x => x.RelatedEntity)
                .List(x => x.RelatedEntities, e => e
                    .Field(y => y.Id)
                    .Field(y => y.Name)
                );

            // Act
            var response = builder.Build(mockEntity);

            // Assert
            Assert.Equal(mockEntity.MockRelatedEntityId, response.MockRelatedEntityId);
            Assert.Equal(mockEntity.RelatedEntityId, response.RelatedEntityId);

            // Assert for RelatedEntity
            Assert.NotNull(response.RelatedEntity);
            Assert.Equal(mockEntity.RelatedEntity.Id, response.RelatedEntity.Id);
            Assert.Equal(mockEntity.RelatedEntity.Name, response.RelatedEntity.Name);

            // Assert for RelatedEntities list
            var relatedEntities = response.RelatedEntities as IEnumerable<dynamic>;
            Assert.NotNull(relatedEntities);
            Assert.Equal(2, relatedEntities.Count());

            var firstEntity = relatedEntities.First();
            Assert.Equal(20, firstEntity.Id);
            Assert.Equal("Entity 1", firstEntity.Name);

            var secondEntity = relatedEntities.Skip(1).First();
            Assert.Equal(30, secondEntity.Id);
            Assert.Equal("Entity 2", secondEntity.Name);
        }

        [Fact]
        public void ShouldBuildResponse_WithSelectedFieldsAndDefaultListMapping()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity
            {
                MockRelatedEntityId = 1,
                RelatedEntityId = 2,
                RelatedEntity = new RelatedEntity
                {
                    Id = 10,
                    Name = "Test Entity"
                },
                RelatedEntities = new List<RelatedEntity>
                {
                    new RelatedEntity { Id = 20, Name = "Entity 1" },
                    new RelatedEntity { Id = 30, Name = "Entity 2" }
                }
            };

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>()
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Entity(x => x.RelatedEntity)
                .List(x => x.RelatedEntities, listBuilder =>
                {
                    listBuilder.Field(x => x.Id);
                    listBuilder.Field(x => x.Name);
                });

            // Act
            var response = builder.Build(mockEntity);

            // Assert
            Assert.Equal(1, response.MockRelatedEntityId);
            Assert.Equal(2, response.RelatedEntityId);
            Assert.Equal(10, response.RelatedEntity.Id);
            Assert.Equal("Test Entity", response.RelatedEntity.Name);

            var relatedEntities = response.RelatedEntities as IEnumerable<IDictionary<string, object>>;
            Assert.NotNull(relatedEntities);
            Assert.Equal(2, relatedEntities.Count());
            Assert.Equal(20, relatedEntities.First()["Id"]);
            Assert.Equal("Entity 1", relatedEntities.First()["Name"]);
        }

        [Fact]
        public void ShouldBuildResponse_WithSelectedFieldsAndSpecificEntityFields()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity
            {
                MockRelatedEntityId = 1,
                RelatedEntityId = 2,
                RelatedEntity = new RelatedEntity
                {
                    Id = 10,
                    Name = "Test Entity"
                },
                RelatedEntities = new List<RelatedEntity>
                {
                    new RelatedEntity { Id = 20, Name = "Entity 1" },
                    new RelatedEntity { Id = 30, Name = "Entity 2" }
                }
            };

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>()
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Entity(x => x.RelatedEntity)
                .List(x => x.RelatedEntities);

            // Act
            var response = builder.Build(mockEntity);

            // Assert
            Assert.Equal(1, response.MockRelatedEntityId);
            Assert.Equal(2, response.RelatedEntityId);

            // Assert that only selected fields of the RelatedEntity are included
            var relatedEntity = (IDictionary<string, object>)response.RelatedEntity;
            Assert.Contains("Id", relatedEntity.Keys);
            Assert.Contains("Name", relatedEntity.Keys);
            Assert.DoesNotContain("AnotherProperty", relatedEntity.Keys); // Example of a non-existing property
        }


        [Fact]
        public void ShouldBuildResponse_WithSelectedFieldsAndEntityWithAllProperties()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity
            {
                MockRelatedEntityId = 1,
                RelatedEntityId = 2,
                RelatedEntity = new RelatedEntity
                {
                    Id = 10,
                    Name = "Test Entity" // Example of an additional property
                },
                RelatedEntities = new List<RelatedEntity>
                {
                    new RelatedEntity { Id = 20, Name = "Entity 1" },
                    new RelatedEntity { Id = 30, Name = "Entity 2" }
                }
            };

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>()
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Entity(x => x.RelatedEntity) // Using the method that includes all properties
                .List(x => x.RelatedEntities);

            // Act
            var response = builder.Build(mockEntity);

            // Assert
            Assert.Equal(1, response.MockRelatedEntityId);
            Assert.Equal(2, response.RelatedEntityId);

            // Assert that all properties of the RelatedEntity are included
            var relatedEntity = (IDictionary<string, object>)response.RelatedEntity;
            Assert.Contains("Id", relatedEntity.Keys);
            Assert.Contains("Name", relatedEntity.Keys);
        }

        [Fact]
        public void ShouldBuildResponse_WithEntityIncludingAllProperties()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity
            {
                MockRelatedEntityId = 1,
                RelatedEntityId = 2,
                RelatedEntity = new RelatedEntity
                {
                    Id = 10,
                    Name = "Test Entity"
                },
                RelatedEntities = new List<RelatedEntity>
                {
                    new RelatedEntity { Id = 20, Name = "Entity 1" },
                    new RelatedEntity { Id = 30, Name = "Entity 2" }
                }
            };

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>()
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Entity(x => x.RelatedEntity)
                .List(x => x.RelatedEntities);

            // Act
            var response = builder.Build(mockEntity);
            var responseDict = (IDictionary<string, object>)response;

            // Assert
            Assert.Equal(1, responseDict["MockRelatedEntityId"]);
            Assert.Equal(2, responseDict["RelatedEntityId"]);

            Assert.True(responseDict.ContainsKey("RelatedEntity"), "Field 'RelatedEntity' should be present.");
            var relatedEntity = responseDict["RelatedEntity"] as IDictionary<string, object>;
            Assert.NotNull(relatedEntity);
            Assert.Equal(10, relatedEntity["Id"]);
            Assert.Equal("Test Entity", relatedEntity["Name"]);

            Assert.True(responseDict.ContainsKey("RelatedEntities"), "Field 'RelatedEntities' should be present.");
            var relatedEntities = responseDict["RelatedEntities"] as IEnumerable<IDictionary<string, object>>;
            Assert.NotNull(relatedEntities);
            Assert.Equal(2, relatedEntities.Count());
        }

        [Fact]
        public void ShouldBuildResponse_WithEntity_CustomFieldMapping()
        {
            // Arrange
            var mockEntity = new MockRelatedEntity
            {
                MockRelatedEntityId = 1,
                RelatedEntityId = 2,
                RelatedEntity = new RelatedEntity
                {
                    Id = 10,
                    Name = "Test Entity"
                },
                RelatedEntities = new List<RelatedEntity>
                {
                    new RelatedEntity { Id = 20, Name = "Entity 1" },
                    new RelatedEntity { Id = 30, Name = "Entity 2" }
                }
            };

            var builder = new SimplifyResponseBuilder<MockRelatedEntity>()
                .Field(x => x.MockRelatedEntityId)
                .Field(x => x.RelatedEntityId)
                .Entity(x => x.RelatedEntity, entityBuilder =>
                {
                    entityBuilder.Field(e => e.Id); // Include only Id field
                });

            // Act
            var response = builder.Build(mockEntity);
            var responseDict = (IDictionary<string, object>)response;

            // Assert
            Assert.Equal(1, responseDict["MockRelatedEntityId"]);
            Assert.Equal(2, responseDict["RelatedEntityId"]);

            Assert.True(responseDict.ContainsKey("RelatedEntity"), "Field 'RelatedEntity' should be present.");
            var relatedEntity = responseDict["RelatedEntity"] as IDictionary<string, object>;
            Assert.NotNull(relatedEntity);
            Assert.False(relatedEntity.ContainsKey("Name"), "Field 'Name' should not be included.");
            Assert.True(relatedEntity.ContainsKey("Id"), "Field 'Name' should be included.");
            Assert.Equal(10, relatedEntity["Id"]);

            Assert.False(responseDict.ContainsKey("RelatedEntities"), "Field 'RelatedEntities' should not be included.");
        }



    }
}
