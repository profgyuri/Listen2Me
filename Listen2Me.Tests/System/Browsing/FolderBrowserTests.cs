using Listen2Me.MVVM.System.Browsing;
using Moq;

namespace Listen2Me.Tests.System.Browsing;

[ TestClass]
public class FolderBrowserTests
{
    [ TestMethod]
    public void GetSubFolders_ReturnsSubFolders()
    {
        var directory = new Mock<DirectoryAccess>();
        directory.Setup(x => x.EnumerateDirectories(@"C:\Music\House")).Returns(["House", "House2"]);
        directory.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        var sut = new FolderBrowser(directory.Object);
        sut.CurrentPath = @"C:\Music";
        
        sut.NavigateToChild("House");
        
        Assert.AreEqual(@"C:\Music\House", sut.CurrentPath);
        Assert.AreEqual("House", sut.GetSubFolders().Result[0]);
        Assert.AreEqual("House2", sut.GetSubFolders().Result[1]);
        Assert.HasCount(2, sut.GetSubFolders().Result);
    }
}