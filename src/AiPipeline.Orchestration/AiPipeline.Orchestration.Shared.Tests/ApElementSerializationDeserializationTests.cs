using System.Text.Json;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;

namespace AiPipeline.Orchestration.Contracts.Tests;

public class ApElementSerializationDeserializationTests
{
    [Fact]
    public void ApObject_ValidApObject_SerializesCorrectly()
    {
        // Arrange
        IApElement original = new ApObject(new()
        {
            { "FullName", new ApString("John Doe") },
            { "Age", new ApInt(30) },
            {
                "CreditCard", new ApObject(new()
                {
                    { "Number", new ApString("12344567890/1111") },
                    { "OwnerName", new ApString("John Doe") },
                    {
                        "ActiveAt", new ApList([
                            new ApDateTime(DateTime.UtcNow),
                            new ApDateTime(DateTime.UtcNow)
                        ])
                    }
                })
            }
        }, nonRequiredProperties: ["CreditCard"]);

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<IApElement>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.IsType<ApObject>(result);
        var apObject = (ApObject)result;
        Assert.Equal(3, apObject.Properties.Count);
        Assert.Contains(apObject.Properties, e => e.Value is ApString { Value: "John Doe" });
        Assert.Contains(apObject.Properties, e => e.Value is ApInt { Value: 30 });
        Assert.Contains(apObject.Properties, e => e.Value is ApObject { Properties.Count: 3 });
    }

    [Fact]
    public void ApList_ValidApList_SerializesCorrectly()
    {
        // Arrange
        IApElement original = new ApList([
            new ApString("John Doe"),
            new ApString("Jane Dale"),
        ]);

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<IApElement>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.IsType<ApList>(result);
        var apObject = (ApList)result;
        Assert.Equal(2, apObject.Items.Count);
        Assert.Contains(apObject.Items, e => e is ApString { Value: "John Doe" });
        Assert.Contains(apObject.Items, e => e is ApString { Value: "Jane Dale" });
    }

    [Fact]
    public void ApList_EmptyApList_SerializesCorrectly()
    {
        // Arrange
        IApElement original = new ApList([]);

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<IApElement>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.IsType<ApList>(result);
        var apObject = (ApList)result;
        Assert.Empty(apObject.Items);
    }

    [Fact]
    public void ApList_DifferingApListItemTypes_ThrowsArgumentException()
    {
        // Assert
        Assert.Throws<ArgumentException>(
            () =>
            {
                IApElement test = new ApList([
                    new ApString("John Doe"),
                    new ApInt(30),
                    new ApObject(new() { { "Test", new ApInt(10) } })
                ]);
            });
    }

    [Fact]
    public void ApString_ValidApString_SerializesCorrectly()
    {
        // Arrange
        IApElement original = new ApString("John Doe");

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<ApString>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.Equal("John Doe", result.Value);
    }

    [Fact]
    public void ApBool_ValidApBool_SerializesCorrectly()
    {
        // Arrange
        IApElement original = new ApBool(false);

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<ApBool>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public void ApDateTime_ValidApDateTime_SerializesCorrectly()
    {
        // Arrange
        var currentDateTime = DateTime.UtcNow;
        IApElement original = new ApDateTime(currentDateTime);

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<ApDateTime>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.Equal(currentDateTime, result.Value);
    }

    [Fact]
    public void ApDecimal_ValidApDecimal_SerializesCorrectly()
    {
        // Arrange
        var number = 31.23m;
        IApElement original = new ApDecimal(number);

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<ApDecimal>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.Equal(number, result.Value);
    }

    [Fact]
    public void ApFile_ValidApFile_SerializesCorrectly()
    {
        // Arrange
        var filename = "test.json";
        var url = "https://test.com/test";
        var contentType = "application/json";
        IApElement original = new ApFile(fileName: filename, contentType: contentType, fileUrl: url);

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<ApFile>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.Equal(filename, result.FileName);
        Assert.Equal(url, result.FileUrl);
        Assert.Equal(contentType, result.ContentType);
    }

    [Fact]
    public void ApInt_ValidApInt_SerializesCorrectly()
    {
        // Arrange
        var number = 3;
        IApElement original = new ApInt(number);

        // Act
        var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
        var result = JsonSerializer.Deserialize<ApInt>(json, TestUtils.ApElementSerializerOptions)!;

        // Assert
        Assert.Equal(number, result.Value);
    }

    //
    // [Fact]
    // public void ApList_ValidApList_SerializesCorrectly()
    // {
    //     // Arrange
    //     IApElement original = new ApList("Nicknames", true, [
    //         new ApString("Nick1", true, "Johnny"),
    //         new ApString("Nick2", true, "Smithy")
    //     ]);
    //
    //     // Act
    //     var json = JsonSerializer.Serialize(original, TestUtils.ApElementSerializerOptions);
    //     var result = JsonSerializer.Deserialize<IApElement>(json, TestUtils.ApElementSerializerOptions)!;
    //
    //     // Assert
    //     Assert.IsType<ApList>(result);
    //     var list = (ApList)result;
    //     Assert.Equal("Nicknames", list.Name);
    //     Assert.Equal(2, list.Items.Count);
    //     Assert.All(list.Items, i => Assert.IsType<ApString>(i));
    // }
}