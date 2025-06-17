using Domain.Tasks;

namespace UnitTests.Domain;

public class TaskAttachmentTests
{
    [Theory]
    [InlineData("application/pdf", AttachmentType.Document)]
    [InlineData("application/json", AttachmentType.Document)]
    [InlineData("text/plain", AttachmentType.Document)]
    [InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document", AttachmentType.Document)]
    [InlineData("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", AttachmentType.Document)]
    [InlineData("application/vnd.openxmlformats-officedocument.presentationml.presentation", AttachmentType.Document)]
    [InlineData("text/csv", AttachmentType.Document)]
    [InlineData("image/jpeg", AttachmentType.Image)]
    [InlineData("image/png", AttachmentType.Image)]
    [InlineData("image/gif", AttachmentType.Image)]
    public void Matches_Content_Type_To_Attachment_Type(string contentType, AttachmentType expected)
    {
        var result = TaskAttachment.GetAttachmentType(contentType);

        result.Should().Be(expected);
    }

    [Fact]
    public void Throws_Exception_For_Unsupported_Content_Type()
    {
        var unsupportedContentType = "abc";

        Action act = () => TaskAttachment.GetAttachmentType(unsupportedContentType);

        act.Should().Throw<ArgumentException>();
    }
}
