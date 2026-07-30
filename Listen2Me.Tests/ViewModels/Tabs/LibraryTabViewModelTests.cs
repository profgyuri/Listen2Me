using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings.Library;
using Listen2Me.MVVM.ViewModels.Tabs.Settings;
using Moq;
using Serilog;

namespace Listen2Me.Tests.ViewModels.Tabs;

[TestClass]
public class LibraryTabViewModelTests
{
    private LibraryTabViewModel _sut;
    private Mock<LibrarySettings> _settings;

    [TestInitialize]
    public async Task Setup()
    {
        var errorHandler = new Mock<IErrorHandler>();
        errorHandler.Setup(x => x.HandleAsync(It.IsAny<Exception>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        var logger = new Mock<ILogger>();
        var messenger = new Mock<IMessenger>();
        _settings = new Mock<LibrarySettings>();
        _settings.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _settings.Setup(x => x.LoadAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var dialogManager = new Mock<IDialogManager>();
        
        _sut = new LibraryTabViewModel(errorHandler.Object, logger.Object, messenger.Object, _settings.Object, dialogManager.Object);
        await _sut.EnsureInitializedAsync().ConfigureAwait(false);
        _sut.MusicFolders = new ObservableCollection<MusicFolder>();
    }
    
    [TestMethod]
    public void AddingNewMusicFolder_AlsoPopulatesCollectionInSettings()
    {
        _sut.MusicFolders.Add(new MusicFolder(){Id = Guid.NewGuid(), Path = @"C:\Music"});
        
        _settings.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.HasCount(1, _sut.MusicFolders);
        Assert.AreEqual(@"C:\Music", _sut.MusicFolders[0].Path);
        Assert.HasCount(_sut.MusicFolders.Count, _settings.Object.MusicFolders);
        Assert.AreEqual(_sut.MusicFolders[0], _settings.Object.MusicFolders.First());
    }
}