using PgDeployDiff.Core;

namespace PgDeployDiff.Tests;

public class ColumnDifferenceTests
{
    [Fact]
    public void ColumnDifference_Added_HasCorrectProperties()
    {
        var diff = new ColumnDifference
        {
            ColumnName = "email",
            ChangeType = DiffChangeType.Added,
            NewDefinition = "varchar(255) NOT NULL"
        };

        Assert.Equal("email", diff.ColumnName);
        Assert.Equal(DiffChangeType.Added, diff.ChangeType);
        Assert.Null(diff.OldDefinition);
        Assert.Equal("varchar(255) NOT NULL", diff.NewDefinition);
    }
}
