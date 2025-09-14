using FoxTales.Application.Helpers;

namespace FoxTales.Application.Tests.Helpers;

public class RoomCodeGeneratorTests
{
    [Fact]
    public void GenerateUniqueCode_ShouldReturnNonEmptyCode_WhenUnique()
    {
        // Given
        static bool isUnique(string _) => true;

        // When
        var code = RoomCodeGenerator.GenerateUniqueCode(isUnique);

        // Then
        Assert.False(string.IsNullOrWhiteSpace(code));
    }

    [Fact]
    public void GenerateUniqueCode_ShouldReturnUniqueCode()
    {
        // Given
        var usedCodes = new HashSet<string> { "CODE1", "CODE2" };
        bool isUnique(string code) => !usedCodes.Contains(code);

        // When
        var code = RoomCodeGenerator.GenerateUniqueCode(isUnique);

        // Then
        Assert.DoesNotContain(code, usedCodes);
    }

    [Fact]
    public void GenerateUniqueCode_ShouldThrow_WhenNoUniqueCodeFound()
    {
        // Given
        static bool isUnique(string _) => false;

        // When
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RoomCodeGenerator.GenerateUniqueCode(isUnique));

        // Then
        Assert.Equal("Could not generate a unique code.", ex.Message);
    }

    [Fact]
    public void GenerateMultipleUniqueCodes_ShouldBeDifferent()
    {
        // Given
        var generated = new HashSet<string>();
        bool isUnique(string code) => !generated.Contains(code);

        // When
        for (int i = 0; i < 10; i++)
        {
            var code = RoomCodeGenerator.GenerateUniqueCode(isUnique);
            generated.Add(code);
        }

        // Then
        Assert.Equal(10, generated.Count);
    }
}
